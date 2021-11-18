using System;
using System.Collections.Generic;
using System.Text;
using Global;

namespace Geo
{
    public interface IGeoPoint
    {
        public IGeoContext Context { get; }

        public Type ValueType { get; }

        public double X { get; }
        public double Y { get; }

    }
}
