using System;
using System.Collections.Generic;
using System.Text;
using Urho;

namespace Global
{
	static public class BoundingBoxExts
	{
		static public BoundingBox Union(BoundingBox a, BoundingBox b)
		{
			BoundingBox c = a;   // Clone struct by copy.
			c.Merge(b);
			return c;
		}
	}
}
