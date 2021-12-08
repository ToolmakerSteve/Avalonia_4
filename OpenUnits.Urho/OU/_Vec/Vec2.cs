// Changes - Copyright ToolmakerSteve and najak3d.
// Based on https://referencesource.microsoft.com/#System.Numerics/System/Numerics/Vector2.cs
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;

namespace OU
{
	// SAME AS Numerics - Vector2
	/// <summary>Double-precision two-dimensional (XY) unitless vector value.   Same as a Vector2, but with double-precision.</summary>
	public partial struct Vec2
	{
        #region --- class Array ----------------------------------------
        public class Array : List<Vec2> // NOTE: this <Vector2> would be auto-gen'ed to the specified preferred library -- like Urho... or defaults to Numerics.Vector2
		{
		}
        #endregion

        #region --- data (See Vec2.Intrinsics.cs) ----------------------------------------
        #endregion

        #region --- new (See Vec2.Intrinsics.cs) ----------------------------------------
        #endregion

        #region --- implicit operators ----------------------------------------
        static public implicit operator Urho.Vector2(Vec2 v2)
        {
            return new Urho.Vector2(v2.X, v2.Y);
        }

        static public implicit operator Vec2(Urho.Vector2 v2)
        {
            return new Vec2(v2.X, v2.Y);
        }
        #endregion
    }
}
