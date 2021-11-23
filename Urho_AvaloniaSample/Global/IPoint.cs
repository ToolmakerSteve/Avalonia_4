using System;
using System.Collections.Generic;
using System.Text;
using Global;

namespace Global
{
    public interface IPoint
    {
        public Type ValueType { get; }

        public Distance X { get; }
        public Distance Y { get; }

    }
}
