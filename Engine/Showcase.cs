using System.Collections.Generic;
using System.Linq;

namespace Engine
{
	public class Showcase
	{
		public Showcase()
		{
			Maps = new ShowcaseMap[3]
			{
				new ShowcaseMap(3),
				new ShowcaseMap(3),
				new ShowcaseMap(3),
			};

			Update();
		}

		public void Pick(Figure figure)
		{
			for (int i = 0; i < Maps.Length; ++i)
			{
				if (Maps[i].Figure == figure)
				{
					Maps[i].Figure = null;
					return;
				}
			}
		}

		public void Return(Figure figure)
		{
			for (int i = 0; i < Maps.Length; ++i)
			{
				if (Maps[i].Figure is null)
				{
					Maps[i].Figure = figure;
					return;
				}
			}
		}

		public void Update()
		{
			for (int i = 0; i < Maps.Length; ++i)
			{
				if (Maps[i].Figure is not null)
				{
					return;
				}
			}

			for (int i = 0; i < Maps.Length; ++i)
			{
				Figure figure = new Figure(FigureShapes.GetRandom());
				figure.TryPutOnMap(Maps[i], new Location(0, 0));
			}
		}

		public ShowcaseMap[] Maps { get; }
	}
}
