using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Drawing;
using System.Drawing.Imaging;

namespace neuralNetwork
{
    class Program
    {
        static void Main(string[] args)
        {

            string inputPath = "../../input.txt";
            string targetPath = "../../output.txt";

            int variableInInput = 27;
            int variableInTarget = 7;

            int numberOfInputData = CalculateNumberOfData(inputPath) / variableInInput;
            int numberOfTargetData = CalculateNumberOfData(targetPath) / variableInTarget;

            double[][] inputs = new double[numberOfInputData][];
            double[][] target = new double[numberOfTargetData][];

            for (int i = 0; i < numberOfInputData; i++)
            {
                inputs[i] = new double[variableInInput];
            }
            for (int i = 0; i < numberOfTargetData; i++)
            {
                target[i] = new double[variableInTarget];
            }

            ImpotrData(inputs, inputPath);
            ImpotrData(target, targetPath);


            Console.WriteLine("Hello world");
            //Console.ReadKey();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //double[][] inputs = new double[][]
            //{
            //    new double[]{0.2,0.5},
            //    new double[]{0.5,0.7},
            //    //new double[]{0.5,0.6},
            //    //new double[]{0.7,0.8}
            //};
            //double[][] target = new double[][]
            //{
            //    new double[]{0.5},
            //    new double[]{0.9},
            //    //new double[]{0.7},
            //    //new double[]{0.9}
            //};

            int epoch = 10000;
            double errorGoal = Math.Pow(0.5, 2.0 ) / inputs.Length;

            double[] bledyWykres = new double[epoch];
            Network net = new Network(new int[] { variableInInput, 40,20, variableInTarget });
            //Network net = new Network(new int[] { 2, 2, 1});

            double MSE = 1; //< błąd kwadratowy
            for (int i = 0; i < epoch && MSE > errorGoal; i++) /// 
            {
                MSE = 0.0;
                for (int j = 0; j < inputs.Length; j++)
                {
                    //Array.ForEach(net.Feedforward(inputs[j]), Console.WriteLine); 
                    double[] outputVal = net.Feedforward(inputs[j]);
                    net.BackPropagateError(target[j]);
                    net.GradientDescent(0.01,0.9);
                    double SSE = 0.0;
                    for (int n = 0; n < outputVal.Length; n++)
                    {
                        SSE += (target[j][n] - outputVal[n]) * (target[j][n] - outputVal[n]);
                    }
                    SSE = SSE / outputVal.Length;
                    MSE += SSE;
                    //MSE += (target[j][0] - outputVal[0]) * (target[j][0] - outputVal[0]);
                    //Console.WriteLine("-" + MSE);
                }

                MSE = MSE / inputs.Length;
                bledyWykres[i] = MSE;
                Console.WriteLine(MSE);

            }

            //////////////////////////////////
            string savePath = "save.txt";
            StreamWriter sw;
            if (!(File.Exists(savePath)))
            {
                sw = File.CreateText(savePath);
                Console.WriteLine("Stworzono plik");
            }
            else
            {
                sw = new StreamWriter(savePath, false);
            }
            //////////////////////////////////

            double SE=0.0;
            for (int j = 0; j < inputs.Length; j++)
            {
                double[] outputVal = net.Feedforward(inputs[j]);
                Console.WriteLine("      ");
                Console.WriteLine("wejscia: ");
                Array.ForEach(inputs[j], Console.WriteLine);
                Console.WriteLine("wyjścia: ");
                Array.ForEach(outputVal, Console.WriteLine);
                Console.WriteLine("Poprawna odpowiedz:");
                Array.ForEach(target[j], Console.WriteLine);
                SE += (target[j][0] - outputVal[0]) * (target[j][0] - outputVal[0]);
                Console.WriteLine("błąd: " + (target[j][0] - outputVal[0]) * (target[j][0] - outputVal[0]));
                //Console.WriteLine("błąd: " + (1.0/(inputs.Length*2))* (target[j][0] - outputVal[0]) * (target[j][0] - outputVal[0]));
                Console.WriteLine("      ");

                /////////////////////////////////////////////////
                sw.WriteLine("wyjscia: ");
                Array.ForEach(outputVal, sw.WriteLine);

                sw.WriteLine("\nPoprawne odpowiedzi: ");
                Array.ForEach(target[j], sw.WriteLine);

                sw.WriteLine("błąd : " + (target[j][0] - outputVal[0]) * (target[j][0] - outputVal[0]));
                SE += (target[j][0] - outputVal[0]) * (target[j][0] - outputVal[0]);
                //////////////////////////////////////////////////////////////////////
            }
            // średni błąd kwadratowy
            Console.WriteLine("błąd całkowity: " + (SE / inputs.Length ));

            

            stopwatch.Stop();
            TimeSpan timeSpan = stopwatch.Elapsed;
            Console.WriteLine("Czas trwania uczenia: {0:00}:{1:00}:{2:00}.{3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);


            //////////////////////////////////////////////////////
            sw.WriteLine("błąd całkowity: " + (SE / inputs.Length));
            sw.WriteLine("Czas trwania uczenia: {0:00}:{1:00}:{2:00}.{3} ", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
            sw.Close();
            /////////////////////////////////////////////////////

            double[] tmp = new double[epoch];
            for (int i = 0; i < epoch; i++)
            {
                tmp[i] = i;
            }
            /////////////////////////////////////////////////// graf
            var pane = new ZedGraph.GraphPane();
            var curve1 = pane.AddCurve(
                label: "demo",
                x: tmp,
                y: bledyWykres,
                color: Color.Blue);
            curve1.Line.IsAntiAlias = true;
            curve1.Symbol.IsVisible = false;
            pane.AxisChange();
            Bitmap bmp = pane.GetImage(1000, 800, dpi: 1000, isAntiAlias: true);
            bmp.Save("zedgraph-console-quickstart.png", ImageFormat.Png);
            ////////////////////////////////////////////////////


            Console.ReadKey();
        }

        private static double[][] ImpotrData(double[][] values, string path)
        {
            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("pl-PL");

            if (!File.Exists(path))
            {
                Console.WriteLine("nie ma takiego pliku");
                Console.ReadKey();
                return values;
            }
            StreamReader streamReader = File.OpenText(path);

            string[] stringValues = streamReader.ReadToEnd().Split('\t');
            int n = 0;

            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values[i].Length; j++)
                {
                    double.TryParse(stringValues[n], NumberStyles.Float, cultureInfo, out values[i][j]);
                    n++;
                }
            }
            return values;
        }
        private static int CalculateNumberOfData(string path) 
        {

            if (!File.Exists(path))
            {
                Console.WriteLine("nie ma takiego pliku");
                Console.ReadKey();
                return -1;
            }
            StreamReader streamReader = File.OpenText(path);
            int x= streamReader.ReadToEnd().Split('\t').Length;
            streamReader.Close();
            return x;
        }

    }
    
}
