using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
	public class Figure
	{
		public Figure(FigureType type, Location? location = null) : base()
		{
			Type = type;
			Location = location;
		}

		public enum FigureType { Angle, Stick };
		public FigureType Type { get; }
		public Location? Location { get; private set; }
		private IEnumerable<Tile> Tiles { get; set; }

		private static bool X => true;
		private static bool O => false;

		public bool TryPutOnMap(Map map, Location location)
		{
			Location = location;
			var tiles = GetTiles();
			//                     out of bounds                    intersection
			if (tiles.Any(t => !map.Contains(t.Coords) ||  map.GetTile(t.Coords) is not null)) 
			{
				Location = null;
				return false;
			}

			map.AddFigure(this);
			return true;
		}

		public IEnumerable<Tile> GetTiles()
		{
			if (Location is null)
			{
				Tiles = Enumerable.Empty<Tile>();
			}
			else
			{
				var safeLoc = Location.Value;
				List<Tile> tiles = new();
				var tileMap = GetTileMap(this.Type);
				for (int x = 0; x < 3; ++x)
				{
					for (int y = 0; y < 3; ++y)
					{
						if (tileMap[x, y])
						{
							var tileLoc = new Location(safeLoc.X + x, safeLoc.Y + y);
							var tile = new Tile(tileLoc, this);
							tiles.Add(tile);
						}
					}
				}
				Tiles = tiles;
			}

			return Tiles;
		}

		private static bool[,] GetTileMap(FigureType type)
		{
			return type switch
			{
				FigureType.Angle => AngleMap,
				FigureType.Stick => StickMap,

				_ => throw new NotImplementedException(),
			};
		}

		private static readonly bool[,] AngleMap = new bool[,] { { X, X, O }, { X, O, O }, { O, O, O } };
		private static readonly bool[,] StickMap = new bool[,] { { X, O, O }, { X, O, O }, { X, O, O } };
	}
}
