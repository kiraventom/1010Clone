﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
	public enum FigureShape 
	{ 
		AngleNW, AngleNE, AngleSW, AngleSE, 
		StickWE, StickNS,
		BigAngleNW, BigAngleNE, BigAngleSW, BigAngleSE,
		Square,
		BigSquare,
	};

	internal static class FigureShapes
	{
		static FigureShapes()
		{
			FigureParams = new()
			{
				{ FigureShape.AngleNW, new FigureParams { TileMap = AngleNWMap, Color = 0x66e3ab } },
				{ FigureShape.AngleNE, new FigureParams { TileMap = AngleNEMap, Color = 0x66e3ab } },
				{ FigureShape.AngleSE, new FigureParams { TileMap = AngleSEMap, Color = 0x66e3ab } },
				{ FigureShape.AngleSW, new FigureParams { TileMap = AngleSWMap, Color = 0x66e3ab } },

				{ FigureShape.StickWE, new FigureParams { TileMap = StickWEMap, Color = 0xff75a8 } },
				{ FigureShape.StickNS, new FigureParams { TileMap = StickNSMap, Color = 0xff75a8 } },

				{ FigureShape.BigAngleNW, new FigureParams { TileMap = BigAngleNWMap, Color = 0x66c2e3 } },
				{ FigureShape.BigAngleNE, new FigureParams { TileMap = BigAngleNEMap, Color = 0x66c2e3 } },
				{ FigureShape.BigAngleSE, new FigureParams { TileMap = BigAngleSEMap, Color = 0x66c2e3 } },
				{ FigureShape.BigAngleSW, new FigureParams { TileMap = BigAngleSWMap, Color = 0x66c2e3 } },

				{ FigureShape.Square, new FigureParams { TileMap = Square, Color = 0x66e368 } },

				{ FigureShape.BigSquare, new FigureParams { TileMap = BigSquare, Color = 0x7b66e3 } },
			};
		}

		public static Dictionary<FigureShape, FigureParams> FigureParams { get; }
		public static FigureShape Random => FigureParams.ElementAt(Engine.RND.Next(FigureParams.Count)).Key;

		private static bool X => true;
		private static bool O => false;

		private static readonly bool[,] AngleNWMap = new bool[,] { { O, X, O }, { X, X, O }, { O, O, O } };
		private static readonly bool[,] AngleNEMap = new bool[,] { { X, O, O }, { X, X, O }, { O, O, O } };
		private static readonly bool[,] AngleSEMap = new bool[,] { { X, X, O }, { X, O, O }, { O, O, O } };
		private static readonly bool[,] AngleSWMap = new bool[,] { { X, X, O }, { O, X, O }, { O, O, O } };

		private static readonly bool[,] StickNSMap = new bool[,] { { X, O, O }, { X, O, O }, { X, O, O } };
		private static readonly bool[,] StickWEMap = new bool[,] { { X, X, X }, { O, O, O }, { O, O, O } };

		private static readonly bool[,] BigAngleNWMap = new bool[,] { { O, O, X }, { O, O, X }, { X, X, X } };
		private static readonly bool[,] BigAngleNEMap = new bool[,] { { X, O, O }, { X, O, O }, { X, X, X } };
		private static readonly bool[,] BigAngleSEMap = new bool[,] { { X, X, X }, { X, O, O }, { X, O, O } };
		private static readonly bool[,] BigAngleSWMap = new bool[,] { { X, X, X }, { O, O, X }, { O, O, X } };

		private static readonly bool[,] Square = new bool[,] { { X, X, O }, { X, X, O }, { O, O, O } };

		private static readonly bool[,] BigSquare = new bool[,] { { X, X, X }, { X, X, X }, { X, X, X } };
	}

	internal record FigureParams
	{
		public bool[,] TileMap { get; init; }
		public uint Color { get; init; }
	}
}
