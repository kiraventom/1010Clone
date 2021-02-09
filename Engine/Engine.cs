using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Collections.Specialized;
using System.Numerics;

namespace Engine
{
	internal static class Engine
	{
		internal static Random RND = new Random();
	}

	public class Showcase
	{
		public Showcase(/*int count*/)
		{
			Map = new ShowcaseMap(3);
			Update();
		}

		public void Pick(Figure figure)
		{
			Map.Figures.Remove(figure);
		}

		public void Return(Figure figure)
		{
			if (!Map.Figures.Any())
			{
				figure.TryPutOnMap(Map, new Location(0, 0));
			}
		}

		public void Update()
		{
			var type = Engine.RND.Next(0, 2);
			Figure figure = new Figure(type == 0 ? Figure.FigureType.Angle : Figure.FigureType.Stick);
			figure.TryPutOnMap(Map, new Location(0, 0));
		}

		public ShowcaseMap Map { get; }
	}

	public abstract class Map
	{
		protected Map(uint size)
		{
			Size = size;
			Tiles = new Tile[Size, Size];
		}

		public abstract void AddFigure(Figure figure);

		public uint Size { get; }
		protected Tile[,] Tiles { get; }
		
		public Tile GetTile(int x, int y) => Tiles[x, y];
		public Tile GetTile(Location loc) => Tiles[loc.X, loc.Y];
		public bool Contains(Location loc) => loc.X >= 0 && loc.X < Size && loc.Y >= 0 && loc.Y < Size;
	}

	public class ShowcaseMap : Map
	{
		public ShowcaseMap(uint size) : base(size)
		{
			Figures = new();
			Figures.CollectionChanged += this.Figures_CollectionChanged;
		}

		public override void AddFigure(Figure figure) => Figures.Add(figure);

		internal ObservableCollection<Figure> Figures { get; }

		private void Figures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (var tile in (e.NewItems[0] as Figure).GetTiles())
				{
					Tiles[tile.X, tile.Y] = tile;
				}
			}
			else
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (var tile in (e.OldItems[0] as Figure).GetTiles())
				{
					Tiles[tile.X, tile.Y] = null;
				}
			}
		}
	}

	public class GameMap : Map
	{
		public GameMap(uint size) : base(size)
		{
		}

		public override void AddFigure(Figure figure)
		{
			var tiles = figure.GetTiles();
			foreach (var tile in tiles)
			{
				tile.Parent = null;
				Tiles[tile.X, tile.Y] = tile;
			}

			CheckForLines(figure);
		}

		private void CheckForLines(Figure addedFigure)
		{
			var allX = addedFigure.GetTiles().Select(t => t.X).Distinct();
			var allY = addedFigure.GetTiles().Select(t => t.Y).Distinct();
			foreach (var x in allX)
			{
				if (IsThereLine(x, true))
				{
					ClearLine(x, true);
				}
			}

			foreach (var y in allY)
			{
				if (IsThereLine(y, false))
				{
					ClearLine(y, false);
				}
			}
		}

		private bool IsThereLine(int i, bool isHorizontal)
		{
			for (int j = 0; j < Size; ++j)
			{
				var tile = isHorizontal ? Tiles[i, j] : Tiles[j, i];
				if (tile is null)
					return false;
			}

			return true;
		}

		private void ClearLine(int i, bool isHorizontal)
		{
			for (int j = 0; j < Size; ++j)
			{
				var tile = isHorizontal ? Tiles[i, j] : Tiles[j, i];
				Tiles[tile.X, tile.Y] = null;
			}
		}
	}

	public class Tile
	{
		public Tile(Location coords, Figure parent)
		{
			Coords = coords;
			Parent = parent;
		}

		public Figure Parent { get; internal set; }
		public Location Coords { get; }
		public int X => Coords.X;
		public int Y => Coords.Y;
	}

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

	public struct Location
	{
		public Location(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public int X { get; }
		public int Y { get; }

		public bool Equals(Location loc) => this.X == loc.X && this.Y == loc.Y;
	}
}
