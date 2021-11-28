namespace OU
{
	public static partial class Geo
	{
		public interface IContext
		{
			public string StandardName { get; }

			public Dist3D ToWGS84(Dist3D pt);
			public Dist2D ToWGS84(Dist2D pt);
			/// <summary>
			/// Result is in meters, on the earth's surface at "altitude zero", within some "local" orthographic coordinate system.
			/// </summary>
			/// <param name="geoPt"></param>
			/// <returns></returns>
			public Dist3D FromWGS84(Dist3D geoPt);
			public Dist2D FromWGS84(Dist2D geoPt);

			// Projection+Zone.
		}
	}
}
