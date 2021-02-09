using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Engine
{
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
}
