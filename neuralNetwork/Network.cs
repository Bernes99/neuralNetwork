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
        Layer[] layers;

        public Network(int[] layer)
        {
            numberOfLayers = layer.Length;
            //this.layer = new int[layer.Length];
            //for (int i = 0; i < layer.Length; i++)
            //{
            //    this.layer[i] = layer[i];
            //}
            layers = new Layer[numberOfLayers];

            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = new Layer(layer[i]);
                if (i<layers.Length -1)
                {
                    layers[i].numberNeuronsInNextLayer = layer[i + 1];
                }
            }
        }

        public double[] SetOutputs(double[] inputs)
        {
            layers[0].outputs = inputs;
            return layers[layers.Length - 1].outputs;
        }

        public void FeedForward()
        {
            for (int i = 1; i < numberOfLayers-1; i++)
            {
                layers[i].FeedForward();
            }
        }
    }
}
