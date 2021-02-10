using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
	public class Figure
	{
		public Figure(FigureShape shape, Location? location = null) : base()
		{
			Shape = shape;
			Location = location;
			Color = FigureShapes.FigureParams[shape].Color + 0xff000000;
		}

		public FigureShape Shape { get; }
		public Location? Location { get; private set; }
		internal uint Color { get; }
		private IEnumerable<Tile> Tiles { get; set; }

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

			if (map is ShowcaseMap sm)
			{
				sm.Figure = this;
			}
			else
			if (map is GameMap gm)
			{
				gm.AddFigure(this);
			}

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
				var tileMap = FigureShapes.FigureParams[this.Shape].TileMap;
				for (int x = 0; x < 4; ++x)
				{
					for (int y = 0; y < 4; ++y)
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
	}
}
