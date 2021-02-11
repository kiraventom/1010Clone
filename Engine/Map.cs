using System.Linq;

namespace Engine__F
{
	public abstract class Map
	{
		protected Map(uint size)
		{
			Size = size;
			Tiles = new Tile[Size, Size];
		}

		public uint Size { get; }
		protected Tile[,] Tiles { get; }

		public bool CanFit(Figure figure)
		{
			for (int x = 0; x < Size; ++x)
			{
				for (int y = 0; y < Size; ++y)
				{
					figure.Location = new(x, y);
					var tiles = figure.GetTiles();
					//                     out of bounds                    intersection
					if (tiles.All(t => this.Contains(t.Coords) && this.GetTile(t.Coords) is null))
					{
						figure.Location = new(0, 0);
						return true;
					}
				}
			}

			figure.Location = new(0, 0);
			return false;
		}
		
		public Tile GetTile(int x, int y) => Tiles[x, y];
		public Tile GetTile(Location loc) => Tiles[loc.X, loc.Y];
		public Tile SetTile(int x, int y, Tile tile) => Tiles[x, y] = tile;
		public Tile SetTile(Location loc, Tile tile) => Tiles[loc.X, loc.Y] = tile;
		public bool Contains(Location loc) => loc.X >= 0 && loc.X < Size && loc.Y >= 0 && loc.Y < Size;
	}
}
