using System.Collections.Generic;
using System.Linq;

namespace Engine
{
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
			Figure figure = new Figure(FigureShapes.Random);
			figure.TryPutOnMap(Map, new Location(0, 0));
		}

		public ShowcaseMap Map { get; }
	}
}
