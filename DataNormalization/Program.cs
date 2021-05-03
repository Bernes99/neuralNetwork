using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataNormalization
{
    class Program
    {
        static int numberOfInputData = 27;
        static int numberOfOutputData = 7;
        static void Main(string[] args)
        {
            CultureInfo cultureInfo = CultureInfo.InvariantCulture;

            string path = @"../../Faults.NNA";
            string saveInputPath = @"../../../neuralNetwork/input.txt";
            string saveOutputPath = @"../../../neuralNetwork/output.txt";
            
            double[][] input;
            double[][] output;
            

            if (!File.Exists(path))
            {
                Console.WriteLine("nie ma takiego pliku");
                return;
            }
            StreamReader sr = File.OpenText(path);
            



            string[] values = sr.ReadToEnd().Split('\t','\n');
            
           
            int numberOfValues = values.Length/(numberOfInputData+ numberOfOutputData);

            input = new double[numberOfValues][];
            output = new double[numberOfValues][];
            for (int value = 0; value < numberOfValues; value++)
            {
                input[value] = new double[numberOfInputData];
                output[value] = new double[numberOfOutputData];
            }




            int i = 0, j = 0, k = 0, indexOfValue = 0;
            for (int n=0;n< values.Length -1;n++)
            {
                if (values[n].Contains("\r"))
                {
                    values[n] = values[n].Remove(values[n].IndexOf("\r"));
                }
                if (i < numberOfInputData)
                {
                    double.TryParse(values[n],NumberStyles.Float, cultureInfo, out input[indexOfValue][j]);
                    k = 0;
                    j++;
                }
                else
                {
                    double.TryParse(values[n], out output[indexOfValue][k]);
                    j = 0;
                    k++;
                }

                i++;

                if (i>= numberOfInputData+ numberOfOutputData)
                {
                    i = 0;
                    indexOfValue++;
                }
                
                //Console.WriteLine(item);
            }

            input = Normalization(input);

            sr.Close();

            SaveFile(input, saveInputPath);
            SaveFile(output, saveOutputPath);
            Console.WriteLine("zakonczono");
            Console.ReadKey();
        }

        private static double[][] Normalization(double[][] values)
        {
            double[] max = new double[values[0].Length];
            double[] min = new double[values[0].Length];
            for (int i = 0; i < values[0].Length; i++)
            {
                min[i] = double.MaxValue;
                max[i] = double.MinValue;
            }

            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values[i].Length; j++)
                {
                    if (values[i][j] > max[j])
                    {
                        max[j] = values[i][j];
                    }
                    if (values[i][j] < min[j])
                    {
                        min[j] = values[i][j];
                    } 
                }
            }
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values[i].Length; j++)
                {
                    values[i][j] = (values[i][j] - min[j]) / (max[j] - min[j]);
                    //values[i][j] = 2 * (values[i][j] - min[j]) / (max[j] - min[j]) - 1;
                }
            }
            return values;
        }


        private static void SaveFile(double[][] values, string path)
        {
            StreamWriter sw;
            if (!(File.Exists(path)))
            {
                sw = File.CreateText(path);
                Console.WriteLine("Stworzono plik");
            }
            else
            {
                sw = new StreamWriter(path, false);
            }

            for (int i = 0; i < values.Length; i++)
            {
                for (int j = 0; j < values[i].Length; j++)
                {
                    sw.Write(values[i][j]+"\t");
                }
                //sw.Write('\n');
            }
            sw.Close();

        }
    }
}
