﻿namespace Engine
{
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
}
