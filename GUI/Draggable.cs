using System.Windows;
using Engine;

namespace GUI
{
	class Draggable
	{
		public Draggable(Figure figure)
		{
			Figure = figure;
		}

		public void Move(Vector shift)
		{
			var newPoint = Point.Add(this.Point, shift);
			Point = newPoint;
		}

		public Point Point { get; set; }
		public Figure Figure { get; }
	}
}
