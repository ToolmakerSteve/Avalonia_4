﻿using System;
using System.Collections.Generic;
using System.Text;
using Global;

namespace Geo
{
    public interface IGeoPoint
    {
        public IGeoContext Context { get; set; }

        public Type ValueType { get; }

        public IPoint IValue { get; set; }

        public double X { get; }
        public double Y { get; }

    }
}
