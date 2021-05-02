using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuralNetwork
{
    class Layer
    {
        public double[] values;
        public double[] valuesDerivative;
        public double[][] weights;
        public double[] error;
        public double[] errorDerivative;
        public double[] bias;
        public int layerSize;

        Random random = new Random();

        public Layer(int layerSize,int numberNeuronsInPrevLayer)
        {
            this.layerSize = layerSize;

            values = new double[layerSize];
            //weights = new double[numberOfvalues, numberOfInputs];
            error = new double[layerSize];
            errorDerivative = new double[layerSize];
            valuesDerivative = new double[layerSize];
            // zasiewanie pierwotne
            weights = new double[layerSize][];
            bias = new double[layerSize];
            for (int i = 0; i < layerSize; i++)
            {
                bias[i] = random.NextDouble() * Math.Sqrt(2.0 / bias.Length);
            }
            for (int i = 0; i < layerSize; i++)
            {
                weights[i] = new double[numberNeuronsInPrevLayer];
                for (int j = 0; j < numberNeuronsInPrevLayer; j++)
                {
                    weights[i][j] = random.NextDouble() * Math.Sqrt(2.0 / weights[i].Length);
                }
            }


        }
        public Layer(int layerSize)
        {
            this.layerSize = layerSize;

            values = new double[layerSize];
            //weights = new double[numberOfvalues, numberOfInputs];
            error = new double[layerSize];
            errorDerivative = new double[layerSize];
            valuesDerivative = new double[layerSize];
            // zasiewanie pierwotne

        }



        public static double Sum(IEnumerable<double> values, IList<double> weights)
        {
            return values.Select((v,i) => v * weights[i]).Sum();
        }

        public static double Sigmoid(double x)
        {
            return 1.0 / (1 + Math.Exp(-x));
        }
        public static double DerivativeSigmoid(double x)
        {
            return x * (1 - x);
        }

    }
}
