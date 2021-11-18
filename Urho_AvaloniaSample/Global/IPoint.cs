using System;
using System.Collections.Generic;
using System.Text;
using Global;

namespace Global
{
    public interface IPoint
    {
        public Type ValueType { get; }

        public double X { get; }
        public double Y { get; }

    }
}
