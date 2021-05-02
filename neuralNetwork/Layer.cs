using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuralNetwork
{
    class Layer
    {
        public int numberNeuronsInNextLayer;
        public double[] outputs;
        double[][] weights;
        double[] error;
        double[] errorDerivative;
        double[] bias;

        Random random = new Random();

        public Layer(int layerSize)
        {

            outputs = new double[layerSize];
            //weights = new double[numberOfOutputs, numberOfInputs];
            error = new double[layerSize];
            errorDerivative = new double[layerSize];

            weights = new double[numberNeuronsInNextLayer][];
            for (int i = 0; i < layerSize; i++)
            {
                weights[i] = new double[layerSize];
                for (int j = 0; j < weights[i].Length; j++)
                {
                    weights[i][j] = random.NextDouble() * Math.Sqrt(2.0 / weights[i].Length);
                }
            }


        }

        public double[] FeedForward(double[] inputs)
        {

            
            return outputs;
        }
    }
}
