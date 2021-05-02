using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuralNetwork
{
    class Network
    {

        int numberOfLayers;
        double[] deisredValues;
        Layer[] layers;

        public Network(int[] layer)
        {
            //this.deisredValues = deisredValues;
            numberOfLayers = layer.Length;
            //this.layer = new int[layer.Length];
            //for (int i = 0; i < layer.Length; i++)
            //{
            //    this.layer[i] = layer[i];
            //}
            layers = new Layer[numberOfLayers];

            for (int i = 0; i < layers.Length ; i++)
            {
                if (i >0)
                {
                    layers[i] = new Layer(layer[i], layer[i -1]);
                }
                else
                    layers[i] = new Layer(layer[i]);

            }
        }

        public double[] SetInputs(double[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                layers[0].values[i] = inputs[i];
            }
            
            return layers[0].values;
        }

        /// <summary>
        /// feedforward
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public double[] Feedforward(double[] inputs)
        {
            SetInputs(inputs) ;
            for (int i = 1; i < numberOfLayers; i++)
            {
                for (int j = 0; j < layers[i].layerSize; j++)
                {
                    layers[i].values[j] = Layer.Sigmoid(Layer.Sum(layers[i - 1].values, layers[i].weights[j]) + layers[i].bias[j]);
                    layers[i].valuesDerivative[j] = Layer.DerivativeSigmoid(layers[i].values[j]);
                    //double sum = 0;
                    //for (int k = 0; k < layers[i-1].layerSize; k++)
                    //{
                    //    sum += layers[i].weights[j][k] * layers[i - 1].values[k];
                    //}
                    //sum += layers[i].bias[j];
                    //layers[i].values[j] = Layer.Sigmoid(sum);
                    //layers[i].valuesDerivative[j] = Layer.DerivativeSigmoid(layers[i].values[j]);
                }
            }

            return layers[numberOfLayers - 1].values;
        }

        private void OutputError(double[] deisredValues)
        {
            for (int i = 0; i < layers[numberOfLayers-1].layerSize; i++)
            {
                layers[numberOfLayers - 1].error[i] = (layers[numberOfLayers - 1].values[i] - deisredValues[i]) * layers[numberOfLayers - 1].valuesDerivative[i];
            }
        }

        public void BackPropagateError(double[] deisredValues)
        {
            OutputError(deisredValues);

            for (int i = numberOfLayers -2; i > 0; i--)
            {
                for (int j = 0; j < layers[i].layerSize; j++)
                {
                    layers[i].error[j] = 0;
                    for (int k = 0; k < layers[i+1].layerSize; k++)
                    {
                        layers[i].error[j] += layers[i + 1].weights[k][j] * layers[i + 1].error[k] * layers[i].valuesDerivative[j];
                    }
                    
                }
                
            }
        }

        public void GradientDescent(double learningRate)
        {
            for (int i = 1; i < numberOfLayers; i++)
            {
                for (int j = 0; j < layers[i].layerSize; j++)
                {
                    for (int k = 0; k < layers[i-1].layerSize; k++)
                    {
                        layers[i].weights[j][k] -= learningRate * layers[i - 1].values[k] * layers[i].error[j];
                    }
                    layers[i].bias[j] -= learningRate * layers[i].error[j];
                }
            }

           
        }            
            
    }
}
