using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuralNetwork
{
    class Network
    {

        int numberOfLayers; //< liczba warstw w sieci
        Layer[] layers; // tablica warstw

        public Network(int[] layer)
        {

            numberOfLayers = layer.Length;
            layers = new Layer[numberOfLayers];

            for (int i = 0; i < layers.Length ; i++)
            {
                // uzywam różnych konstruktorów dla warstwy wejsciowej oraz dla reszty warstw
                if (i >0)
                {
                    layers[i] = new Layer(layer[i], layer[i -1]);
                }
                else
                    layers[i] = new Layer(layer[i]);

            }
        }


        /// <summary>
        /// Metoda ustawia wartości neuronów na wejściu
        /// </summary>
        /// <param name="inputs"> tablica zawierajaca wartości wejsciowe</param>
        /// <returns></returns>
        public double[] SetInputs(double[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                layers[0].values[i] = inputs[i];
            }
            
            return layers[0].values;
        }

        /// <summary>
        /// Metoda oblicza wyjście sieci
        /// </summary>
        /// <param name="inputs"> wejścowe wartości sieci</param>
        /// <returns> Zwraca tablicę wartości neuronów z warstwy wyjściowej</returns>
        public double[] Feedforward(double[] inputs)
        {
            // ustawiam wejscia sieci
            SetInputs(inputs);

            // iteruję po każdym neuronie w sieci
            for (int i = 1; i < numberOfLayers; i++)
            {
                for (int j = 0; j < layers[i].layerSize; j++)
                {
                    //obliczam wartość dla kazdego neuronu sieci
                    layers[i].values[j] = Layer.Sigmoid(Layer.Sum(layers[i - 1].values, layers[i].weights[j]) + layers[i].bias[j]);
                    // obliczam takze pochodną fukcji figmoidalnej obliczonej wyzej
                    layers[i].valuesDerivative[j] = Layer.DerivativeSigmoid(layers[i].values[j]);
                }
            }

            return layers[numberOfLayers - 1].values;
        }

        /// <summary>
        /// Metoda wyznacza błąd na warstwie wyjściowej
        /// </summary>
        /// <param name="deisredValues"> Wartości prawidłowe(prawidłoa odpowiedz systemu)</param>
        private void OutputError(double[] deisredValues)
        {
            for (int i = 0; i < layers[numberOfLayers-1].layerSize; i++)
            {
                layers[numberOfLayers - 1].error[i] = (layers[numberOfLayers - 1].values[i] - deisredValues[i]) * layers[numberOfLayers - 1].valuesDerivative[i];
            }
        }

        /// <summary>
        /// Metoda wyznacza błąd w warstwach ukrytych oraz wywołuję metodle dla warstwy wyjściowej
        /// </summary>
        /// <param name="deisredValues"> Wartości prawidłowe(prawidłoa odpowiedz systemu) </param>
        public void BackPropagateError(double[] deisredValues)
        {
            // wyznaczanie błędów dla warstwy wyjściowej
            OutputError(deisredValues);

            // wyznaczanie błędów dla warstw ukrytych
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

        /// <summary>
        /// Metoda aktualizuje watrości wag oraz biasów
        /// </summary>
        /// <param name="learningRate">stopień uczenia</param>
        public void GradientDescent(double learningRate)
        {
            for (int i = 1; i < numberOfLayers; i++)
            {
                for (int j = 0; j < layers[i].layerSize; j++)
                {
                    //aktualizowanie wag
                    for (int k = 0; k < layers[i-1].layerSize; k++)
                    {
                        layers[i].weights[j][k] -= learningRate * layers[i - 1].values[k] * layers[i].error[j];
                    }
                    // aktualizowanie biasu
                    layers[i].bias[j] -= learningRate * layers[i].error[j];
                }
            }

           
        }            
            
    }
}
