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
using System.Collections;

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

            List<InputTarget> inputTargets = new List<InputTarget>();
            for (int i = 0; i < inputs.Length; i++)
            {
                inputTargets.Add(new InputTarget(inputs[i], target[i], i));
            }


            Helper.Shuffle(inputTargets);

            List<InputTarget> trainData = new List<InputTarget>();
            List<InputTarget> testData = new List<InputTarget>();

            for (int i = 0; i < (int)(0.8 * inputTargets.Count()); i++) {
                trainData.Add(inputTargets[i]);
            }
            for (int i = (int)(0.8 * inputTargets.Count()); i < inputTargets.Count(); i++)
            {
                testData.Add(inputTargets[i]);
            }
            trainData.Sort();
            testData.Sort();


            Console.WriteLine("Start");
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
            double errorGoal = Math.Pow(0.5, 2.0) / trainData.Count();

            double[] bledyWykres = new double[epoch];
            Network net = new Network(new int[] { variableInInput, 40, 20, variableInTarget });
            //Network net = new Network(new int[] { 2, 2, 1});

            double MSE = 1; //< błąd kwadratowy
            for (int i = 0; i < epoch && MSE > errorGoal; i++) /// 
            {
                MSE = 0.0;
                for (int j = 0; j < trainData.Count(); j++)
                {
                    //Array.ForEach(net.Feedforward(inputs[j]), Console.WriteLine); 
                    double[] outputVal = net.Feedforward(trainData[j].intput);
                    net.BackPropagateError(trainData[j].target);
                    net.GradientDescent(0.01, 0.9);
                    double SSE = 0.0;
                    for (int n = 0; n < outputVal.Length; n++)
                    {
                        SSE += (trainData[j].target[n] - outputVal[n]) * (trainData[j].target[n] - outputVal[n]);
                    }
                    SSE = SSE / outputVal.Length;
                    MSE += SSE;
                    //MSE += (target[j][0] - outputVal[0]) * (target[j][0] - outputVal[0]);
                    //Console.WriteLine("-" + MSE);
                }

                MSE = MSE / trainData.Count();
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

            MSE = 0;
            int correctValues = 0;
            double[] outputValGraph = new double[testData.Count()];
            for (int j = 0; j < testData.Count(); j++)
            {
                double[] outputVal = net.Feedforward(testData[j].intput);
                Console.WriteLine("      ");
                Console.WriteLine("wejscia: ");
                Array.ForEach(testData[j].intput, Console.WriteLine);
                Console.WriteLine("wyjścia: ");
                Array.ForEach(outputVal, Console.WriteLine);
                Console.WriteLine("Poprawna odpowiedz:");
                Array.ForEach(testData[j].target, Console.WriteLine);

                double SSE = 0.0;
                for (int n = 0; n < outputVal.Length; n++)
                {
                    SSE += (testData[j].target[n] - outputVal[n]) * (testData[j].target[n] - outputVal[n]);
                }
                SSE = SSE / outputVal.Length;
                MSE += SSE;

                Console.WriteLine("błąd: " + SSE);
                //Console.WriteLine("błąd: " + (1.0/(inputs.Length*2))* (target[j][0] - outputVal[0]) * (target[j][0] - outputVal[0]));
                Console.WriteLine("      ");

                /////////////////////////////////////////////////
                sw.WriteLine("wyjscia: ");
                Array.ForEach(outputVal, sw.WriteLine);

                sw.WriteLine("\nPoprawne odpowiedzi: ");
                Array.ForEach(testData[j].target, sw.WriteLine);

                sw.WriteLine("błąd : " + SSE);
                //////////////////////////////////////////////////////////////////////
                double taregetMaxValue = testData[j].target.Max();
                double outputMaxValue = outputVal.Max();
                if (testData[j].target.ToList().IndexOf(taregetMaxValue) == outputVal.ToList().IndexOf(outputMaxValue))
                {
                    correctValues++;

                }

                testData[j].outputClass = outputVal.ToList().IndexOf(outputMaxValue);
                outputValGraph[j] = testData[j].outputClass;

            }
            // średni błąd kwadratowy
            Console.WriteLine("błąd całkowity: " + (MSE / testData.Count()));
            Console.WriteLine("Poprawność: " + ((double)correctValues / testData.Count()));



            stopwatch.Stop();
            TimeSpan timeSpan = stopwatch.Elapsed;
            Console.WriteLine("Czas trwania uczenia: {0:00}:{1:00}:{2:00}.{3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);


            //////////////////////////////////////////////////////
            sw.WriteLine("błąd całkowity: " + (MSE / testData.Count()));
            sw.WriteLine("Czas trwania uczenia: {0:00}:{1:00}:{2:00}.{3} ", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
            sw.WriteLine("Poprawność: " + ((double)correctValues / testData.Count()));
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
                label: "wykres błędu podczas uczenia",
                x: tmp,
                y: bledyWykres,
                color: Color.Blue);
            curve1.Line.IsAntiAlias = true;
            curve1.Symbol.IsVisible = false;
            pane.AxisChange();
            Bitmap bmp = pane.GetImage(1000, 800, dpi: 1000, isAntiAlias: true);
            bmp.Save("wykres_bledu.png", ImageFormat.Png);

            tmp = new double[testData.Count()];
            for (int i = 0; i < testData.Count(); i++)
            {
                tmp[i] = i;
            }
            double[] graphClassY = new double[testData.Count()];
            for (int i = 0; i < testData.Count(); i++)
            {
                graphClassY[i] = testData[i].target.ToList().IndexOf(testData[i].target.Max());
            }

            pane = new ZedGraph.GraphPane();
            curve1 = pane.AddCurve(
                label: "klasy uszkodzeń blach",
                x: tmp,
                y: graphClassY,
                color: Color.Red);
            curve1.Line.IsAntiAlias = true;
            curve1.Symbol.IsVisible = false;
            var curve2 = pane.AddCurve(
                label: "klasy uszkodzeń blach\n podane przez sieć",
                x: tmp,
                y: outputValGraph,
                color: Color.Green);
            curve2.Line.IsAntiAlias = true;
            curve2.Symbol.IsVisible = true;
            curve2.Symbol.Fill.Color = Color.Green;
            curve2.Symbol.Type = ZedGraph.SymbolType.Circle;
            pane.AxisChange();
            bmp = pane.GetImage(1000, 800, dpi: 1000, isAntiAlias: true);
            bmp.Save("wykres_przynaleznosci.png", ImageFormat.Png);

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
