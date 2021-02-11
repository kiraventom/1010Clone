namespace Engine
{
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
