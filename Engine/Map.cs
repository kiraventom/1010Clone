namespace Engine
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
		
		public Tile GetTile(int x, int y) => Tiles[x, y];
		public Tile GetTile(Location loc) => Tiles[loc.X, loc.Y];
		public bool Contains(Location loc) => loc.X >= 0 && loc.X < Size && loc.Y >= 0 && loc.Y < Size;
	}
}
