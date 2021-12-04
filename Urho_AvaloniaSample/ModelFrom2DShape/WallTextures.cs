using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
	internal enum WallTexture
	{
		StoneWall4,
		BrickWall19
	}

	static public partial class Utils
	{
		public const float StoneWall4_TextureScale = 1.0f / 4;
		public const float BrickWall19_TextureScale = 1.0f / 8;

		static internal string MaterialNameFor(WallTexture texture)
		{
			switch (texture) {
				case WallTexture.StoneWall4:
					//return "Materials/Stone.xml";
					return "Materials/StoneWall4.xml";

					//return "Parallax/Materials/ParallaxStonesDemoVer.xml";
					//return "Materials/StonesParallaxOffset.xml";
					//return "Materials/StonesParallaxOcclusion.xml";
					//return "Materials/StoneWall4ParallaxOffset.xml";
					//return "Materials/StoneWall4ParallaxOcclusion.xml";

				case WallTexture.BrickWall19:
					//return "Materials/BricksNormal.xml";
					//return "Materials/BricksParallaxOffset.xml";
					//return "Materials/BricksParallaxOcclusion.xml";

					return "Materials/BrickWall19.xml";
					//return "Materials/BrickWall19ParallaxOcclusion.xml";
				default:
					throw new NotImplementedException("TextureScaleFor - unknown WallTexture");
			}
		}

		static internal float TextureScaleFor(WallTexture texture)
		{
			switch (texture) {
				case WallTexture.StoneWall4:
					return StoneWall4_TextureScale;
				case WallTexture.BrickWall19:
					return BrickWall19_TextureScale;
				default:
					throw new NotImplementedException("TextureScaleFor - unknown WallTexture");
			}
		}
	}
}
