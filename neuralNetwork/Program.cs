using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuralNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string inputPath = "input.txt";
            string outputPath = "output.txt";

            if (!File.Exists(inputPath) || !File.Exists(outputPath))
            {
                Console.WriteLine("nie ma takiego pliku");
                Console.ReadKey();
                return;
            }
            StreamReader inputStreamReader = File.OpenText(inputPath);
            StreamReader outputStreamReader = File.OpenText(outputPath);

            string[] values = inputStreamReader.ReadToEnd().Split('\t', '\n');


            Console.WriteLine("Hello world");
            //Console.ReadKey();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            double[][] inputs = new double[][]
            {
                new double[]{0.3,0.2},
                new double[]{0.3,0.4},
                new double[]{0.5,0.6},
                new double[]{0.7,0.8}
            };
            double[][] target = new double[][]
            {
                new double[]{0.2},
                new double[]{0.5},
                new double[]{0.7},
                new double[]{0.9}
            };

            int epoch = 60000;
            double errorGoal = Math.Pow(0.5, 2.0 ) / inputs.Length;


            Network net = new Network(new int[] { 2, 4, 1 });


            double MSE = 1; //< błąd kwadratowy
            for (int i = 0; i < epoch && MSE >errorGoal; i++)
            {
                MSE = 0.0;
                for (int j = 0; j < inputs.Length; j++)
                {
                    //Array.ForEach(net.Feedforward(inputs[j]), Console.WriteLine); 
                    double[] outputVal = net.Feedforward(inputs[j]);
                    net.BackPropagateError(target[j]);
                    net.GradientDescent(0.01);
                    MSE += (target[j][0] - outputVal[0]) * (target[j][0] - outputVal[0]);
                    Console.WriteLine("-" + MSE);
                }
                MSE = MSE / inputs.Length;


            }

            double SE=0.0;
            for (int j = 0; j < inputs.Length; j++)
            {
                double[] outputVal = net.Feedforward(inputs[j]);
                Console.WriteLine("      ");
                Console.WriteLine("wyjścia: ");
                Array.ForEach(outputVal, Console.WriteLine);
                Console.WriteLine("Poprawna odpowiedz:");
                Array.ForEach(target[j], Console.WriteLine);
                SE += (target[j][0] - outputVal[0]) * (target[j][0] - outputVal[0]);
                Console.WriteLine("błąd: " + (target[j][0] - outputVal[0]) * (target[j][0] - outputVal[0]));
                //Console.WriteLine("błąd: " + (1.0/(inputs.Length*2))* (target[j][0] - outputVal[0]) * (target[j][0] - outputVal[0]));
                Console.WriteLine("      ");
            }
            // średni błąd kwadratowy
            Console.WriteLine("błąd całkowity: " + (1.0/(inputs.Length))* SE);

            stopwatch.Stop();
            TimeSpan timeSpan = stopwatch.Elapsed;
            Console.WriteLine("Czas trwania uczenia: {0:00}:{1:00}:{2:00}.{3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);

            Console.ReadKey();
        }
    }
}
