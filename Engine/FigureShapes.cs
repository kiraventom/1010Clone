using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine__F
{
	internal enum BasicShape
	{
		Angle = 0,
		SmallStick,
		Stick,
		LongStick,
		BigAngle,
		Square,
		BigSquare,
		Dot
	};

	public enum FigureShape 
	{ 
		AngleNW, AngleNE, AngleSW, AngleSE,
		SmallStickWE, SmallStickNS,
		StickWE, StickNS,
		LongStickWE, LongStickNS,
		BigAngleNW, BigAngleNE, BigAngleSW, BigAngleSE,
		Square,
		BigSquare,
		Dot
	};

	internal static class FigureShapes
	{
		static FigureShapes()
		{
			FigureParams = new()
			{
				{ FigureShape.AngleNW, new FigureParams { TileMap = AngleNWMap, Color = 0x38c787 } },
				{ FigureShape.AngleNE, new FigureParams { TileMap = AngleNEMap, Color = 0x38c787 } },
				{ FigureShape.AngleSE, new FigureParams { TileMap = AngleSEMap, Color = 0x38c787 } },
				{ FigureShape.AngleSW, new FigureParams { TileMap = AngleSWMap, Color = 0x38c787 } },

				{ FigureShape.BigAngleNW, new FigureParams { TileMap = BigAngleNWMap, Color = 0x3fb6e0 } },
				{ FigureShape.BigAngleNE, new FigureParams { TileMap = BigAngleNEMap, Color = 0x3fb6e0 } },
				{ FigureShape.BigAngleSE, new FigureParams { TileMap = BigAngleSEMap, Color = 0x3fb6e0 } },
				{ FigureShape.BigAngleSW, new FigureParams { TileMap = BigAngleSWMap, Color = 0x3fb6e0 } },

				{ FigureShape.SmallStickWE, new FigureParams { TileMap = SmallStickWEMap, Color = 0xe39c3d } },
				{ FigureShape.SmallStickNS, new FigureParams { TileMap = SmallStickNSMap, Color = 0xe39c3d } },

				{ FigureShape.StickWE, new FigureParams { TileMap = StickWEMap, Color = 0xf06297 } },
				{ FigureShape.StickNS, new FigureParams { TileMap = StickNSMap, Color = 0xf06297 } },

				{ FigureShape.LongStickWE, new FigureParams { TileMap = LongStickWEMap, Color = 0xe34b4b } },
				{ FigureShape.LongStickNS, new FigureParams { TileMap = LongStickNSMap, Color = 0xe34b4b } },

				{ FigureShape.Square, new FigureParams { TileMap = Square, Color = 0x59de5b } },

				{ FigureShape.BigSquare, new FigureParams { TileMap = BigSquare, Color = 0x7b66e3 } },

				{ FigureShape.Dot, new FigureParams { TileMap = Dot, Color = 0x4759de } },
			};
		}

		public static Dictionary<FigureShape, FigureParams> FigureParams { get; }

		private static readonly FigureShape[] Angles = new[] { FigureShape.AngleNW, FigureShape.AngleNE, FigureShape.AngleSE, FigureShape.AngleSW };
		private static readonly FigureShape[] BigAngles = new[] { FigureShape.BigAngleNW, FigureShape.BigAngleNE, FigureShape.BigAngleSE, FigureShape.BigAngleSW };
		private static readonly FigureShape[] SmallSticks = new[] { FigureShape.SmallStickWE, FigureShape.SmallStickNS };
		private static readonly FigureShape[] Sticks = new[] { FigureShape.StickWE, FigureShape.StickNS };
		private static readonly FigureShape[] LongSticks = new[] { FigureShape.LongStickWE, FigureShape.LongStickNS };
		private static readonly FigureShape[] Squares = new[] { FigureShape.Square };
		private static readonly FigureShape[] BigSquares = new[] { FigureShape.BigSquare };
		private static readonly FigureShape[] Dots = new[] { FigureShape.Dot };

		private static bool X => true;
		private static bool O => false;

		///////////////////

		private static readonly bool[,] AngleNWMap = new bool[,] { 
			{ O, X, O, O }, 
			{ X, X, O, O }, 
			{ O, O, O, O }, 
			{ O, O, O, O } 
		};
		private static readonly bool[,] AngleNEMap = new bool[,] { 
			{ X, O, O, O }, 
			{ X, X, O, O }, 
			{ O, O, O, O }, 
			{ O, O, O, O } 
		};
		private static readonly bool[,] AngleSEMap = new bool[,] { 
			{ X, X, O, O }, 
			{ X, O, O, O }, 
			{ O, O, O, O }, 
			{ O, O, O, O } 
		};
		private static readonly bool[,] AngleSWMap = new bool[,] { 
			{ X, X, O, O }, 
			{ O, X, O, O }, 
			{ O, O, O, O }, 
			{ O, O, O, O } 
		};

		///////////////////

		private static readonly bool[,] SmallStickNSMap = new bool[,] { 
			{ X, X, O, O }, 
			{ O, O, O, O }, 
			{ O, O, O, O }, 
			{ O, O, O, O } 
		};
		private static readonly bool[,] SmallStickWEMap = new bool[,] { 
			{ X, O, O, O }, 
			{ X, O, O, O }, 
			{ O, O, O, O }, 
			{ O, O, O, O } 
		};

		///////////////////
		
		private static readonly bool[,] StickNSMap = new bool[,] { 
			{ X, O, O, O }, 
			{ X, O, O, O }, 
			{ X, O, O, O }, 
			{ O, O, O, O } 
		};
		private static readonly bool[,] StickWEMap = new bool[,] { 
			{ X, X, X, O }, 
			{ O, O, O, O }, 
			{ O, O, O, O }, 
			{ O, O, O, O } 
		};

		///////////////////

		private static readonly bool[,] LongStickNSMap = new bool[,] { 
			{ X, O, O, O }, 
			{ X, O, O, O }, 
			{ X, O, O, O },
			{ X, O, O, O } 
		};
		private static readonly bool[,] LongStickWEMap = new bool[,] { 
			{ X, X, X, X }, 
			{ O, O, O, O }, 
			{ O, O, O, O }, 
			{ O, O, O, O } 
		};

		///////////////////

		private static readonly bool[,] BigAngleNWMap = new bool[,] { 
			{ O, O, X, O }, 
			{ O, O, X, O }, 
			{ X, X, X, O }, 
			{ O, O, O, O } 
		};
		private static readonly bool[,] BigAngleNEMap = new bool[,] { 
			{ X, O, O, O }, 
			{ X, O, O, O }, 
			{ X, X, X, O }, 
			{ O, O, O, O } 
		};
		private static readonly bool[,] BigAngleSEMap = new bool[,] { 
			{ X, X, X, O }, 
			{ X, O, O, O }, 
			{ X, O, O, O }, 
			{ O, O, O, O } 
		};
		private static readonly bool[,] BigAngleSWMap = new bool[,] { 
			{ X, X, X, O }, 
			{ O, O, X, O }, 
			{ O, O, X, O }, 
			{ O, O, O, O } 
		};

		///////////////////

		private static readonly bool[,] Square = new bool[,] { 
			{ X, X, O, O }, 
			{ X, X, O, O }, 
			{ O, O, O, O }, 
			{ O, O, O, O } 
		};

		///////////////////

		private static readonly bool[,] BigSquare = new bool[,] { 
			{ X, X, X, O }, 
			{ X, X, X, O }, 
			{ X, X, X, O }, 
			{ O, O, O, O } 
		};

		///////////////////

		private static readonly bool[,] Dot = new bool[,] { 
			{ X, O, O, O }, 
			{ O, O, O, O }, 
			{ O, O, O, O }, 
			{ O, O, O, O } 
		};

		///////////////////

		public static FigureShape GetRandom()
		{
			var allBasics = Enum.GetValues(typeof(BasicShape));
			var basicShape = (BasicShape)allBasics.GetValue(Engine.RND.Next(allBasics.Length));
			return GetRandom(basicShape);
		}

		public static FigureShape GetRandom(BasicShape basic)
		{
			var rotations = GetRotations(basic);
			return rotations.ElementAt(Engine.RND.Next(rotations.Length));
		}

		private static FigureShape[] GetRotations(BasicShape basic)
		{
			return basic switch
			{
				BasicShape.Angle => Angles,
				BasicShape.SmallStick => SmallSticks,
				BasicShape.Stick => Sticks,
				BasicShape.LongStick => LongSticks,
				BasicShape.BigAngle => BigAngles,
				BasicShape.Square => Squares,
				BasicShape.BigSquare => BigSquares,
				BasicShape.Dot => Dots,

				_ => throw new NotImplementedException()
			};
		}
	}

	internal record FigureParams
	{
		public bool[,] TileMap { get; init; }
		public uint Color { get; init; }
	}
}
