using System.Linq;

namespace Engine
{
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
}
