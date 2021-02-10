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
		private double SmallerSide => MapView.ActualWidth < MapView.ActualHeight ? MapView.ActualWidth : MapView.ActualHeight;
		private Size TileSize => new Size(SmallerSide / GameMap.Size - TileMargin, SmallerSide / GameMap.Size - TileMargin);
		private Draggable Captured { get; set; }
		private uint TileMargin => (uint)Math.Ceiling(SmallerSide / GameMap.Size / 10.0);

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			DpiScaler.Initialize(this);

			Width = this.Width / DpiScaler.Scale.Value;
			Height = this.Height / DpiScaler.Scale.Value;
			MainGrid.Margin = new Thickness(MainGrid.Margin.Left / DpiScaler.Scale.Value);

			if (Left < 0)
				this.Left = 0;
			if (Top < 0)
				this.Top = 0;

			Paint = new SKPaint() { Color = SKColors.Black };

			this.MouseLeave += this.Window_MouseLeave;

			MapView.PaintSurface += this.MainView_PaintSurface;

			Showcase1View.MouseDown += this.ShowcaseView_MouseDown;
			Showcase1View.PaintSurface += this.ShowcaseView_PaintSurface;
			Showcase2View.MouseDown += this.ShowcaseView_MouseDown;
			Showcase2View.PaintSurface += this.ShowcaseView_PaintSurface;
			Showcase3View.MouseDown += this.ShowcaseView_MouseDown;
			Showcase3View.PaintSurface += this.ShowcaseView_PaintSurface;

			DraggingView.MouseUp += this.DraggingView_MouseUp;
			DraggingView.MouseMove += this.DraggingView_MouseMove;
			DraggingView.PaintSurface += this.DraggingView_PaintSurface;

			MapView.InvalidateVisual();
			Showcase1View.InvalidateVisual();
			Showcase2View.InvalidateVisual();
			Showcase3View.InvalidateVisual();
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
			var position = DpiScaler.ScaleUp(e.GetPosition(DraggingView));
			MouseLocation = position;
			MouseLocation = Point.Add(position, -DpiScaler.ScaleUp(DpiScaler.ScaleUp(ShowcaseClickOffset))); // the fuck is this

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
			var showcaseView = sender as SkiaSharp.Views.WPF.SKElement;
			var position = DpiScaler.ScaleDown(e.GetPosition(showcaseView));
			var map = Showcase.Maps[int.Parse(showcaseView.Tag.ToString())];
			var hoveredLoc = GetHoveredLocation(map, position, true);
			if (hoveredLoc is null)
			{
				return;
			}

			var safeHoveredLoc = hoveredLoc.Value;
			var hoveredTile = map.GetTile(safeHoveredLoc);
			var figure = hoveredTile?.Parent;
			if (figure is null)
				return;

			Showcase.Pick(figure);
			showcaseView.InvalidateVisual();
			Captured = new Draggable(figure);

			DraggingView.Visibility = Visibility.Visible;
			DraggingView.InvalidateVisual();

			ShowcaseClickOffset = Point.Subtract(position, new Point());
			DraggingView_MouseMove(sender, e);
		}

		private void DraggingView_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (Captured is null)
			{
				return;
			}

			var draggableLoc = GetHoveredLocation(GameMap, DpiScaler.ScaleDown(Captured.Point), false);
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
			Showcase1View.InvalidateVisual();
			Showcase2View.InvalidateVisual();
			Showcase3View.InvalidateVisual();
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
						(x * (float)TileSize.Width + TileMargin) * (float)DpiScaler.Scale.Value, 
						(y * (float)TileSize.Height + TileMargin) * (float)DpiScaler.Scale.Value, 
						((x + 1) * (float)TileSize.Width) * (float)DpiScaler.Scale.Value, 
						((y + 1) * (float)TileSize.Height) * (float)DpiScaler.Scale.Value);

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

				var draggableLoc = GetHoveredLocation(GameMap, DpiScaler.ScaleDown(Captured.Point), false);
				if (draggableLoc is not null)
				{
					foreach (var tile in Captured.Figure.GetTiles())
					{
						var safeDraggableLoc = draggableLoc.Value;
						var skRect = new SKRect(
							((safeDraggableLoc.X + tile.X) * (float)TileSize.Width + TileMargin) * (float)DpiScaler.Scale.Value,
							((safeDraggableLoc.Y + tile.Y) * (float)TileSize.Height + TileMargin) * (float)DpiScaler.Scale.Value,
							((safeDraggableLoc.X + tile.X + 1) * (float)TileSize.Width) * (float)DpiScaler.Scale.Value,
							((safeDraggableLoc.Y + tile.Y + 1) * (float)TileSize.Height) * (float)DpiScaler.Scale.Value);

						canvas.DrawRect(skRect, Paint);
					}
				}
			}
		}

		private void ShowcaseView_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
		{
			var canvas = e.Surface.Canvas;
			canvas.Clear(SKColors.White);

			var showcaseView = sender as SkiaSharp.Views.WPF.SKElement;
			var map = Showcase.Maps[int.Parse(showcaseView.Tag.ToString())];

			for (int x = 0; x < map.Size; ++x)
			{
				for (int y = 0; y < map.Size; ++y)
				{
					var skRect = new SKRect(
						(x * (float)TileSize.Width + TileMargin) * (float)DpiScaler.Scale.Value,
						(y * (float)TileSize.Height + TileMargin) * (float)DpiScaler.Scale.Value,
						((x + 1) * (float)TileSize.Width) * (float)DpiScaler.Scale.Value,
						((y + 1) * (float)TileSize.Height) * (float)DpiScaler.Scale.Value);

					var tile = map.GetTile(x, y);
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
						((float)Captured.Point.X / (float)DpiScaler.Scale.Value + x * (float)TileSize.Width + TileMargin) * (float)DpiScaler.Scale.Value,
						((float)Captured.Point.Y / (float)DpiScaler.Scale.Value + y * (float)TileSize.Height + TileMargin) * (float)DpiScaler.Scale.Value,
						((float)Captured.Point.X / (float)DpiScaler.Scale.Value + (x + 1) * (float)TileSize.Width) * (float)DpiScaler.Scale.Value,
						((float)Captured.Point.Y / (float)DpiScaler.Scale.Value + (y + 1) * (float)TileSize.Height) * (float)DpiScaler.Scale.Value);

				canvas.DrawRect(skRect, Paint);
			}
		}
	}
}
