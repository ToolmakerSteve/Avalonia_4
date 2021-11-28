using System;
using System.Collections.Generic;
using System.Text;

namespace OU
{
	public partial struct TorqueAxis
	{
		public float Nm; // Newton-meter torque
		public Vec3 Axis; // Axis around which this torque is applied, clockwise
		public float Magnitude => Nm;
	}
}
