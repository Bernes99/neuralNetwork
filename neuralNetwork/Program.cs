using System;
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
            Console.WriteLine("Hello world");
            //Console.ReadKey();

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


            Network net = new Network(new int[] { 2, 4, 1 });



            for (int i = 0; i < epoch; i++)
            {
                for (int j = 0; j < inputs.Length; j++)
                {
                    Array.ForEach(net.Feedforward(inputs[j]), Console.WriteLine); 
                    net.BackPropagateError(target[j]);
                    net.GradientDescent(0.01);
                    Console.WriteLine("-");
                }
                
            }

            for (int j = 0; j < inputs.Length; j++)
            {
                double[] outputVal = net.Feedforward(inputs[j]);
                Console.WriteLine("      ");
                Console.WriteLine("wyjścia: ");
                Array.ForEach(outputVal, Console.WriteLine);
                Console.WriteLine("Poprawna odpowiedz:");
                Array.ForEach(target[j], Console.WriteLine);
                Console.WriteLine("błąd: " + 0.5 * (target[j][0] - outputVal[0]) * (target[j][0] - outputVal[0]));
                Console.WriteLine("      ");
            }

            Console.ReadKey();
        }
    }
}
