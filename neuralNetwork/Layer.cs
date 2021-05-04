using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuralNetwork
{
    class Layer
    {
        public double[] values; //< talbica watrości neuronów (a) w warstwie
        public double[] valuesDerivative; //< tablica pochodnych wartości neuronów w warstwie
        public double[][] weights; //< tablica wag przejść pomędzy neuronami; pierwszy indeks to neuron z którego rzejscie sie zaczyna, drugi indeks to neuron do którego przejscie zmierza
        public double[] error; //< tablica błędów licznonych dla kazdego neuronu 
        public double[] bias; //< tablica biasów dla kazdego neurou 
        public double[][] velocity;
        public int layerSize; //< zmienna przechowująca rozmiar warstwy

        Random random = new Random();

        /// <summary>
        /// Konstruktor dla warstw innych niz wejsciowa
        /// </summary>
        /// <param name="layerSize"> rozmiar warstwy</param>
        /// <param name="numberNeuronsInPrevLayer">rozmiar poprzedniej warstwy</param>
        public Layer(int layerSize,int numberNeuronsInPrevLayer)
        {
            this.layerSize = layerSize;
            values = new double[layerSize];
            error = new double[layerSize];
            valuesDerivative = new double[layerSize];

            // zasiewanie pierwotne (ustawianie losowych wartości wag i biasów)
            weights = new double[layerSize][];
            velocity = new double[layerSize][];
            bias = new double[layerSize];
            for (int i = 0; i < layerSize; i++)
            {
                bias[i] = random.NextDouble() * Math.Sqrt(2.0 / bias.Length);
                //bias[i] = 0;
                //bias[i] = 0.3;
            }
            for (int i = 0; i < layerSize; i++)
            {
                weights[i] = new double[numberNeuronsInPrevLayer];
                velocity[i] = new double[numberNeuronsInPrevLayer];
                for (int j = 0; j < numberNeuronsInPrevLayer; j++)
                {
                    velocity[i][j] = 0.0;
                    weights[i][j] = random.NextDouble() * Math.Sqrt(2.0 / weights[i].Length); /// -0.1
                    //weights[i][j] = 0.3;
                }
            }

        }
        /// <summary>
        /// konstruktor dla warstwy wejściowej
        /// </summary>
        /// <param name="layerSize">rozmiar warstwy</param>
        public Layer(int layerSize)
        {
            this.layerSize = layerSize;
            values = new double[layerSize];
            error = new double[layerSize];
            valuesDerivative = new double[layerSize];
        }


        /// <summary>
        /// Metoda oblicza sumę iloczynów podanych wartości neuronów oraz wag 
        /// </summary>
        /// <param name="values">wartości neuronów</param>
        /// <param name="weights">wartości wag</param>
        /// <returns> Zwraca sumę iloczynów podanych wartości neuronów oraz wag </returns>
        public static double Sum(IEnumerable<double> values, IList<double> weights)
        {
            return values.Select((v,i) => v * weights[i]).Sum();
        }

        /// <summary>
        /// Metoda oblicza funkce sigmoidalną dla podanej watrości
        /// </summary>
        /// <param name="x">x</param>
        /// <returns> Zwraca wynik fukcji sigmoidalnej dla podanej watrości </returns>
        public static double Sigmoid(double x)
        {
            return 1.0 / (1 + Math.Exp(-x));
        }

        /// <summary>
        /// Metoda słyzy jako szablon do obiczania pochodnej funkcji sigmoidalnej 
        /// </summary>
        /// <param name="sigmoid"> wynik fukcji sigmoidalnej</param>
        /// <returns> Zwraca wynik pochodniej podanje funkcji sigmoidalnej </returns>
        public static double DerivativeSigmoid(double sigmoid)
        {
            return sigmoid * (1 - sigmoid);
        }

    }
}
