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
			var type = Engine.RND.Next(0, 2);
			Figure figure = new Figure(type == 0 ? Figure.FigureType.Angle : Figure.FigureType.Stick);
			figure.TryPutOnMap(Map, new Location(0, 0));
		}

		public ShowcaseMap Map { get; }
	}
}
