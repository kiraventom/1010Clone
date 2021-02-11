namespace Engine__F
{
	public class ShowcaseMap : Map
	{
		public ShowcaseMap(uint size) : base(size)
		{
		}

		private Figure _figure;
		public Figure Figure 
		{
			get => _figure;
			set // fix
			{
				if (_figure is not null)
				{
					foreach (var tile in _figure.GetTiles())
					{
						Tiles[tile.X, tile.Y] = null;
					}
				}

				_figure = value;
				if (_figure is not null)
				{
					foreach (var tile in _figure.GetTiles())
					{
						Tiles[tile.X, tile.Y] = tile;
					}
				}
			} 
		}
	}
}
