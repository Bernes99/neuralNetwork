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

            string inputPath = "../../input.txt"; //<ścieżka do pliku z danymi wejściowymi
            string targetPath = "../../output.txt"; //<ścieżka do pliku z danymi wyjściowymi

            int variableInInput = 27; //<ilość neuronów na wejściu
            int variableInTarget = 7; // ilość meuronów na wyjściu

            int numberOfInputData = CalculateNumberOfData(inputPath) / variableInInput; //<liczba danych wa wejściu
            int numberOfTargetData = CalculateNumberOfData(targetPath) / variableInTarget; //< liczba danych na wyjściu

            double[][] inputs = new double[numberOfInputData][]; //<tablica danych wejściowych
            double[][] target = new double[numberOfTargetData][]; //<tablica danych wyjściowych


            //inicjalizacja tablic dla danych wejściowych i wyjściowych
            for (int i = 0; i < numberOfInputData; i++)
            {
                inputs[i] = new double[variableInInput];
            }
            for (int i = 0; i < numberOfTargetData; i++)
            {
                target[i] = new double[variableInTarget];
            }

            // pobieram dane z plików i zapisauje je do talbic
            ImpotrData(inputs, inputPath);
            ImpotrData(target, targetPath);

            // scalenie danych z tablic do jednej listy obiektów zawierającej odpowiadające sobie dane wejściowe i wyjściowe
            List<InputTarget> inputTargets = new List<InputTarget>();
            for (int i = 0; i < inputs.Length; i++)
            {
                inputTargets.Add(new InputTarget(inputs[i], target[i], i));
            }


            // pomieszanie kolejności danych 
            Helper.Shuffle(inputTargets);


            //rozdzielenie danych na dane treningowe oraz dane testowe
            List<InputTarget> trainData = new List<InputTarget>();
            List<InputTarget> testData = new List<InputTarget>();

            for (int i = 0; i < (int)(0.8 * inputTargets.Count()); i++)
            {
                trainData.Add(inputTargets[i]);
            }
            for (int i = (int)(0.8 * inputTargets.Count()); i < inputTargets.Count(); i++)
            {
                testData.Add(inputTargets[i]);
            }

            //sortowanie danych w danych treningowych oraz testowych
            trainData.Sort();
            testData.Sort();

            // start odliczaia czasu pracy sieci
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int epoch = 10000;  //< liczba epok nauki
            int S1 = 40; //< ilość neuronów w 2 warstwie (pierwszej ukrytej)
            int S2 = 20; //< ilość neuronów w 3 warstwie (drugiej ukrytej)
            double MSE = 1; //< błąd średnio kwadratowy
            double learningRate = 0.01; //< współczynnik uczenia sieci
            double momentum = 0.9; //<współczynnik momentum
            double errorGoal = Math.Pow(0.5, 2.0) / trainData.Count();  //< minimalny błąd sieci po którym przerywanie jest uczenie

            double[] errorGraph = new double[epoch];  //< tablica zbierająca MSE z poszczególnych epok

            //konstuowanie sieci neuronowej
            Network net = new Network(new int[] { variableInInput, 40, 20, variableInTarget });

            // ucznie sieci 
            for (int i = 0; i < epoch && MSE > errorGoal; i++) /// 
            {
                MSE = 0.0;
                // pętla przechodzi przez każdą daną uczącą 
                for (int j = 0; j < trainData.Count(); j++)
                {
                    // przechodzenie przez sieć
                    double[] outputVal = net.Feedforward(trainData[j].intput);
                    //błędu dla warstw
                    net.BackPropagateError(trainData[j].target);
                    // aktualizowanie wartości wag oraz bisaów 
                    net.GradientDescent(learningRate, momentum);

                    double SSE = 0.0;//< suma kwadratów błędu
                    for (int n = 0; n < outputVal.Length; n++)
                    {
                        SSE += (trainData[j].target[n] - outputVal[n]) * (trainData[j].target[n] - outputVal[n]);
                    }
                    SSE = SSE / outputVal.Length;
                    MSE += SSE;
                }
                // obiczanie, zapisywanie i wyświetlanie MSE dla każdej epoki
                MSE = MSE / trainData.Count();
                errorGraph[i] = MSE;
                Console.WriteLine(MSE);

            }

            //////////////////////////////////
            // otwarcie pliku do zapisu danych z procesu uczenia
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
            int correctValues = 0; //< ilość poprawnych odpowiedzi sieci
            double[] outputValGraph = new double[testData.Count()]; //< tablica przechowująca wartości jakie siec podała na wyjście
            //petla sprawdzajaca odpowiedzi sieci na danych testowych
            for (int j = 0; j < testData.Count(); j++)
            {
                // generowanie odpowiedzi cieci dla danej testowej
                double[] outputVal = net.Feedforward(testData[j].intput);

                //wypisywanie danych na oknoo konsoli 
                Console.WriteLine("      ");
                Console.WriteLine("wejscia: ");
                Array.ForEach(testData[j].intput, Console.WriteLine);
                Console.WriteLine("wyjścia: ");
                Array.ForEach(outputVal, Console.WriteLine);
                Console.WriteLine("Poprawna odpowiedz:");
                Array.ForEach(testData[j].target, Console.WriteLine);

                double SSE = 0.0; //< suma kwadratów błędu
                for (int n = 0; n < outputVal.Length; n++)
                {
                    SSE += (testData[j].target[n] - outputVal[n]) * (testData[j].target[n] - outputVal[n]);
                }
                SSE = SSE / outputVal.Length;
                MSE += SSE;

                Console.WriteLine("błąd: " + SSE);
                Console.WriteLine("      ");

                /////////////////////////////////////////////////
                // zapisywanie do pliku odpowiedzi dla każdej danej testowej
                sw.WriteLine("wyjscia: ");
                Array.ForEach(outputVal, sw.WriteLine);

                sw.WriteLine("\nPoprawne odpowiedzi: ");
                Array.ForEach(testData[j].target, sw.WriteLine);

                sw.WriteLine("błąd : " + SSE);
                //////////////////////////////////////////////////////////////////////

                double taregetMaxValue = testData[j].target.Max(); //< maksymalna wartość jakla jest na wyjściu w danych testujących (dane są w postaci 1 z N)
                double outputMaxValue = outputVal.Max(); //< maksymalna wartość jaka jest na wyjściu sieci

                //sprawdzam czy indeks największej wartości w obu danych jest taki sam, jeśli tak uznaje to za poprawną odpowiedz sieci
                if (testData[j].target.ToList().IndexOf(taregetMaxValue) == outputVal.ToList().IndexOf(outputMaxValue))
                {
                    correctValues++;

                }

                // przypisuje klase do jakiej wyjście sieci je przydzieliło
                testData[j].outputClass = outputVal.ToList().IndexOf(outputMaxValue);
                outputValGraph[j] = testData[j].outputClass;

            }
            // wyświetlam na konsole całego zbioru testowego średni błąd kwadratowy
            Console.WriteLine("błąd całkowity: " + (MSE / testData.Count()));
            Console.WriteLine("Poprawność: " + ((double)correctValues / testData.Count()));


            // zatrzymuje pomiar czasu działania sieci oraz wyświetlam go na konsole
            stopwatch.Stop();
            TimeSpan timeSpan = stopwatch.Elapsed;
            Console.WriteLine("Czas trwania uczenia: {0:00}:{1:00}:{2:00}.{3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);


            //////////////////////////////////////////////////////
            // zapisuje czas działania sieci oraz średni błąd kwadratowy do pliku
            sw.WriteLine("błąd całkowity: " + (MSE / testData.Count()));
            sw.WriteLine("Czas trwania uczenia: {0:00}:{1:00}:{2:00}.{3} ", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
            sw.WriteLine("Poprawność: " + ((double)correctValues / testData.Count()));
            sw.Close();
            /////////////////////////////////////////////////////

            //generuje 2 wykresy , wykres błędu, wykres przynależności
            double[] tmp = new double[epoch];
            for (int i = 0; i < epoch; i++)
            {
                tmp[i] = i;
            }

            // wykres błędu
            var pane = new ZedGraph.GraphPane();
            var curve1 = pane.AddCurve(
                label: "wykres błędu podczas uczenia",
                x: tmp,
                y: errorGraph,
                color: Color.Blue);
            curve1.Line.IsAntiAlias = true;
            curve1.Symbol.IsVisible = false;
            pane.YAxis.Title.Text = "MSE";
            pane.XAxis.Title.Text = "Epoki";
            pane.AxisChange();
            Bitmap bmp = pane.GetImage(1000, 800, dpi: 1000, isAntiAlias: true);
            bmp.Save("wykres_bledu.png", ImageFormat.Png);

            ////////////////////////////////////////////////////////////////////////////
            // wykres przynależności  
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
            pane.YAxis.Title.Text = "Klasy uszkodzeń";
            pane.XAxis.Title.Text = "Dane wyjściowe sieci";
            bmp = pane.GetImage(1000, 800, dpi: 1000, isAntiAlias: true);
            bmp.Save("wykres_przynaleznosci.png", ImageFormat.Png);

            ////////////////////////////////////////////////////


            Console.ReadKey();
        }

        /// <summary>
        /// Metoda importuje dane z pliku i przypisuje je do tablic
        /// </summary>
        /// <param name="values">tablica tablic wartości do zaimportowania</param>
        /// <param name="path">ścieżka do pliku z którego dane są pobierane</param>
        /// <returns>Zwraca tablice tablic z przypisanymi danymi</returns>
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

        /// <summary>
        /// metoda oblicza ilość wierszy w pliku
        /// </summary>
        /// <param name="path"> ścieżka do pliku </param>
        /// <returns>Zwraca liczbę wierszy w pliku </returns>
        private static int CalculateNumberOfData(string path)
        {

            if (!File.Exists(path))
            {
                Console.WriteLine("nie ma takiego pliku");
                Console.ReadKey();
                return -1;
            }
            StreamReader streamReader = File.OpenText(path);
            int x = streamReader.ReadToEnd().Split('\t').Length;
            streamReader.Close();
            return x;
        }



    }

}