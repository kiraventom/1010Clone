using SkiaSharp;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Engine;

namespace GUI
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			GameMap = new GameMap(10);
			Showcase = new Showcase();
			this.Loaded += this.MainWindow_Loaded;
		}

		private GameMap GameMap { get; }
		private Showcase Showcase { get; }

		private Point MouseLocation { get; set; }
		private Vector ShowcaseClickOffset { get; set; }
		private SKPaint Paint { get; set; }
		private Size TileSize => new Size(MapView.ActualWidth / GameMap.Size - TileMargin, MapView.ActualHeight / GameMap.Size - TileMargin);
		private Draggable Captured { get; set; }
		private const uint TileMargin = 5;

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			DpiScaler.Initialize(this);

			Paint = new SKPaint() { Color = SKColors.Black };

			this.MouseLeave += this.Window_MouseLeave;

			MapView.PaintSurface += this.MainView_PaintSurface;

			ShowcaseView.MouseDown += this.ShowcaseView_MouseDown;
			ShowcaseView.PaintSurface += this.ShowcaseView_PaintSurface;

			DraggingView.MouseDown += this.DraggingView_MouseDown;
			DraggingView.MouseMove += this.DraggingView_MouseMove;
			DraggingView.PaintSurface += this.DraggingView_PaintSurface;

			MapView.InvalidateVisual();
			ShowcaseView.InvalidateVisual();
		}

		private Location? GetHoveredLocation(Map map, Point point, bool isMouseLocation)
		{
			double x_actual = point.X / TileSize.Width;
			int x = (int)(isMouseLocation ? Math.Floor(x_actual) : (int)Math.Round(x_actual));

			double y_actual = point.Y / TileSize.Height;
			int y = (int)(isMouseLocation ? Math.Floor(y_actual) : (int)Math.Round(y_actual));

			if (x < 0 || x >= map.Size || y < 0 || y >= map.Size)
			{
				return null;
			}

			return new Location(x, y);
		}

		private void DraggingView_MouseMove(object sender, MouseEventArgs e)
		{
			var prevLoc = MouseLocation;
			MouseLocation = e.GetPosition(DraggingView);
			MouseLocation = Point.Add(MouseLocation, - ShowcaseClickOffset);

			if (Captured is not null)
			{
				Captured.Move(Point.Subtract(MouseLocation, prevLoc));
				this.DraggingView.InvalidateVisual();

				// debug
				this.MapView.InvalidateVisual();
			}
		}

		private void ShowcaseView_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var hoveredLoc = GetHoveredLocation(Showcase.Map, e.GetPosition(ShowcaseView), true);
			if (hoveredLoc is null)
			{
				return;
			}

			var safeHoveredLoc = hoveredLoc.Value;
			var hoveredTile = Showcase.Map.GetTile(safeHoveredLoc);
			var figure = hoveredTile?.Parent;
			if (figure is null)
				return;

			Showcase.Pick(figure);
			ShowcaseView.InvalidateVisual();
			Captured = new Draggable(figure);

			DraggingView.Visibility = Visibility.Visible;
			DraggingView.InvalidateVisual();

			ShowcaseClickOffset = Point.Subtract(e.GetPosition(ShowcaseView), new Point());
			DraggingView_MouseMove(sender, e);
		}

		private void DraggingView_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (Captured is null)
			{
				return;
			}

			var draggableLoc = GetHoveredLocation(GameMap, Captured.Point, false);
			if (draggableLoc is not null)
			{
				var safeDraggableLoc = draggableLoc.Value;
				if (Captured.Figure.TryPutOnMap(GameMap, safeDraggableLoc))
				{
					Showcase.Update();
				}
				else
				{
					Showcase.Return(Captured.Figure);
				}
			}
			else
			{
				Showcase.Return(Captured.Figure);
			}

			Captured = null;
			MouseLocation = new Point();
			DraggingView.Visibility = Visibility.Hidden;
			MapView.InvalidateVisual();
			DraggingView.InvalidateVisual();
			ShowcaseView.InvalidateVisual();
		}

		private void Window_MouseLeave(object sender, MouseEventArgs e)
		{
			//DraggingView_MouseDown(sender, null);
		}

		private void MainView_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
		{
			var canvas = e.Surface.Canvas;

			canvas.Clear(SKColors.White);

			for (int x = 0; x < GameMap.Size; ++x)
			{
				for (int y = 0; y < GameMap.Size; ++y)
				{
					var skRect = new SKRect(
						x * (float)TileSize.Width + TileMargin, 
						y * (float)TileSize.Height + TileMargin, 
						(x + 1) * (float)TileSize.Width, 
						(y + 1) * (float)TileSize.Height);

					var tile = GameMap.GetTile(x, y);
					if (tile is null)
					{
						Paint.Color = new SKColor(0xffd6d6d6);
					}
					else
					{
						Paint.Color = new SKColor(tile.Color);
					}

					canvas.DrawRect(skRect, Paint);
				}
			}

			// debug
			if (Captured is not null)
			{
				Paint.Color = new SKColor(0x44ffffff);

				var draggableLoc = GetHoveredLocation(GameMap, Captured.Point, false);
				if (draggableLoc is not null)
				{
					foreach (var tile in Captured.Figure.GetTiles())
					{
						var safeDraggableLoc = draggableLoc.Value;
						var skRect = new SKRect(
							(safeDraggableLoc.X + tile.X) * (float)TileSize.Width + TileMargin,
							(safeDraggableLoc.Y + tile.Y) * (float)TileSize.Height + TileMargin,
							(safeDraggableLoc.X + tile.X + 1) * (float)TileSize.Width,
							(safeDraggableLoc.Y + tile.Y + 1) * (float)TileSize.Height);

						canvas.DrawRect(skRect, Paint);
					}
				}
			}
		}

		private void ShowcaseView_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
		{
			var canvas = e.Surface.Canvas;

			canvas.Clear(SKColors.White);

			for (int x = 0; x < Showcase.Map.Size; ++x)
			{
				for (int y = 0; y < Showcase.Map.Size; ++y)
				{
					var skRect = new SKRect(
						x * (float)TileSize.Width + TileMargin,
						y * (float)TileSize.Height + TileMargin,
						(x + 1) * (float)TileSize.Width,
						(y + 1) * (float)TileSize.Height);

					var tile = Showcase.Map.GetTile(x, y);
					if (tile is not null)
					{
						Paint.Color = new SKColor(tile.Color);
						canvas.DrawRect(skRect, Paint);
					}
				}
			}
		}

		private void DraggingView_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
		{
			var canvas = e.Surface.Canvas;

			canvas.Clear(SKColors.Transparent);

			var tiles = Captured.Figure.GetTiles();
			if (tiles is null)
				return;

			foreach (var tile in tiles)
			{
				Paint.Color = new SKColor(tile.Color);
				int x = tile.X;
				int y = tile.Y;
				var skRect = new SKRect(
						(float)Captured.Point.X + x * (float)TileSize.Width + TileMargin,
						(float)Captured.Point.Y + y * (float)TileSize.Height + TileMargin,
						(float)Captured.Point.X + (x + 1) * (float)TileSize.Width,
						(float)Captured.Point.Y + (y + 1) * (float)TileSize.Height);

				canvas.DrawRect(skRect, Paint);
			}
		}
	}
}
