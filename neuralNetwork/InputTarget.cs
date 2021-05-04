using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuralNetwork
{
    class InputTarget : IEquatable<InputTarget>, IComparable<InputTarget>
    {
                

        public double[] intput;
        public double[] target;
        public int id;
        public int outputClass;

        public InputTarget(double[] intput, double[] target,int id)
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
