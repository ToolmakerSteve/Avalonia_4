using System;
using System.Collections.Generic;
using System.Text;
using WTF.Numerics;

namespace WTF.Units
{
    public partial struct TorqueAxis
    {
        public float Nm; // Newton-meter torque
        public Vector3 Axis; // Axis around which this torque is applied, clockwise
        public float Magnitude => Nm;
    }
}
