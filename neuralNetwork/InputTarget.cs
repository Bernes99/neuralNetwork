using System;


namespace neuralNetwork
{
    class InputTarget : IEquatable<InputTarget>, IComparable<InputTarget>
    {


        public double[] intput; //<dane wejściowe
        public double[] target; //< dane pożądane na wyjściu
        public int id; //< id danych
        public int outputClass; //< klasa do jakiej zakwalifikowano dane

        public InputTarget(double[] intput, double[] target, int id)
        {
            this.intput = intput;
            this.target = target;
            this.id = id;
        }

        public int CompareTo(InputTarget comparePart) =>
                comparePart == null ? 1 : id.CompareTo(comparePart.id);


        public bool Equals(InputTarget other) =>
            other is null ? false : id.Equals(other.id);
    }
}