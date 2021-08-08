using SkiaSharp;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Engine;
using System.Text;

namespace GUI
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent, new KeyEventHandler(Window_KeyPressed), true);
			this.Loaded += this.MainWindow_Loaded;
			this.Closed += this.MainWindow_Closed;
		}

		public GameMap GameMap { get; set; }
		public Showcase Showcase { get; set; }
		public bool Lost { get; set; }

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
			MainGrid.Margin = new(MainGrid.Margin.Left / DpiScaler.Scale.Value);

			Restart();
			TryLoadFromSave();
			ResetSettings();

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

			Redraw();
		}

		private void Restart()
		{
			DraggingView.Visibility = Visibility.Hidden;
			MouseLocation = new();
			ShowcaseClickOffset = new();
			Lost = false;
			GameMap = new(10);
			Showcase = new();
			Engine.Engine.Score = 0;
			Title = "Счёт: " + Engine.Engine.Score + " / " + Properties.Settings.Default.BestScore;

			if (Left < 0)
				this.Left = 0;
			if (Top < 0)
				this.Top = 0;

			Paint = new SKPaint() { Color = SKColors.Black };
		}

		private void Redraw()
		{
			MapView.InvalidateVisual();
			Showcase1View.InvalidateVisual();
			Showcase2View.InvalidateVisual();
			Showcase3View.InvalidateVisual();
			DraggingView.InvalidateVisual();
		}

		private void TryLoadFromSave()
		{
			if (Properties.Settings.Default.IsSaved)
			{
				Engine.Engine.Score = Properties.Settings.Default.SavedScore;
				Title = "Счёт: " + Engine.Engine.Score + " / " + Properties.Settings.Default.BestScore;

				if (Properties.Settings.Default.SavedShowcase0 != -1)
				{
					var figure = new Figure((FigureShape)Properties.Settings.Default.SavedShowcase0);
					Showcase.Pick(Showcase.Maps[0].Figure);
					figure.TryPutOnMap(Showcase.Maps[0], new Location(0, 0));
				}
				else
				{
					Showcase.Maps[0].Figure = null;
				}

				if (Properties.Settings.Default.SavedShowcase1 != -1)
				{
					var figure = new Figure((FigureShape)Properties.Settings.Default.SavedShowcase1);
					Showcase.Pick(Showcase.Maps[1].Figure);
					figure.TryPutOnMap(Showcase.Maps[1], new Location(0, 0));
				}
				else
				{
					Showcase.Maps[1].Figure = null;
				}

				if (Properties.Settings.Default.SavedShowcase2 != -1)
				{
					var figure = new Figure((FigureShape)Properties.Settings.Default.SavedShowcase2);
					Showcase.Pick(Showcase.Maps[2].Figure);
					figure.TryPutOnMap(Showcase.Maps[2], new Location(0, 0));
				}
				else
				{
					Showcase.Maps[2].Figure = null;
				}

				var savedMap = Properties.Settings.Default.SavedMap;
				for (int x = 0; x < GameMap.Size; ++x)
				{
					for (int y = 0; y < GameMap.Size; ++y)
					{
						string tileStr = savedMap[x * 10 + y].ToString();
						if (tileStr != "-")
						{
							FigureShape shape = (FigureShape)IntStringBaseConverter.BaseToLong(tileStr);
							Tile tile = new Tile(new Location(x, y), new Figure(shape));
							GameMap.SetTile(x, y, tile);
						}
					}
				}
			}
		}

		private void ResetSettings()
		{
			Properties.Settings.Default.SavedScore = 0;
			Properties.Settings.Default.SavedShowcase0 = -1;
			Properties.Settings.Default.SavedShowcase1 = -1;
			Properties.Settings.Default.SavedShowcase2 = -1;
			Properties.Settings.Default.SavedMap = string.Empty;
			Properties.Settings.Default.IsSaved = false;
			Properties.Settings.Default.Save();
		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			if (!Lost)
			{
				StringBuilder mapBuilder = new StringBuilder();
				for (int x = 0; x < GameMap.Size; ++x)
				{
					for (int y = 0; y < GameMap.Size; ++y)
					{
						var tile = GameMap.GetTile(x, y);
						string tileStr = tile is null ? "-" : IntStringBaseConverter.LongToBase((int)tile.Parent.Shape);
						mapBuilder.Append(tileStr);
					}
				}

				Properties.Settings.Default.SavedScore = Engine.Engine.Score;
				var figure0 = Showcase.Maps[0].Figure;
				var figure1 = Showcase.Maps[1].Figure;
				var figure2 = Showcase.Maps[2].Figure;
				Properties.Settings.Default.SavedShowcase0 = figure0 is not null ? (int)figure0.Shape : -1;
				Properties.Settings.Default.SavedShowcase1 = figure1 is not null ? (int)figure1.Shape : -1;
				Properties.Settings.Default.SavedShowcase2 = figure2 is not null ? (int)figure2.Shape : -1;
				Properties.Settings.Default.SavedMap = mapBuilder.ToString();
				Properties.Settings.Default.IsSaved = true;
				Properties.Settings.Default.Save();
			}
		}

		private void Window_KeyPressed(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.R)
			{
				Restart();
				Redraw();
			}
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
			Captured = new(figure);

			DraggingView.Visibility = Visibility.Visible;
			DraggingView.InvalidateVisual();

			ShowcaseClickOffset = Point.Subtract(position, new Point());
			DraggingView_MouseMove(sender, e);
		}

		private void DraggingView_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (Captured is null || Lost)
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
					Engine.Engine.Score += Captured.Figure.GetTiles().Count();
					if (Engine.Engine.Score > Properties.Settings.Default.BestScore)
					{
						Properties.Settings.Default.BestScore = Engine.Engine.Score;
						Properties.Settings.Default.Save();
					}
					Title = "Счёт: " + Engine.Engine.Score + " / " + Properties.Settings.Default.BestScore;

					if (DidLose())
					{
						Lost = true;
						Captured = null;
						MouseLocation = new();
						MapView.InvalidateVisual();
						DraggingView.InvalidateVisual();
						Showcase1View.InvalidateVisual();
						Showcase2View.InvalidateVisual();
						Showcase3View.InvalidateVisual();
						return;
					}
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
			MouseLocation = new();
			DraggingView.Visibility = Visibility.Hidden;
			MapView.InvalidateVisual();
			DraggingView.InvalidateVisual();
			Showcase1View.InvalidateVisual();
			Showcase2View.InvalidateVisual();
			Showcase3View.InvalidateVisual();
		}

		private bool DidLose()
		{
			for (int i = 0; i < Showcase.Maps.Length; ++i)
			{
				var map = Showcase.Maps[i];
				var figure = map.Figure;
				if (figure is not null && GameMap.CanFit(figure))
					return false;
			}

			return true;
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
						Paint.Color = new(0xffd6d6d6);
					}
					else
					{
						Paint.Color = new(tile.Color);
					}

					canvas.DrawRect(skRect, Paint);
				}
			}

			// debug
			if (Captured is not null && !Lost)
			{
				Paint.Color = new(0x44ffffff);

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
						Paint.Color = new(tile.Color);
						canvas.DrawRect(skRect, Paint);
					}
				}
			}
		}

		private void DraggingView_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
		{
			var canvas = e.Surface.Canvas;

			if (!Lost)
			{
				canvas.Clear(SKColors.Transparent);

				var tiles = Captured.Figure.GetTiles();
				if (tiles is null)
					return;

				foreach (var tile in tiles)
				{
					Paint.Color = new(tile.Color);
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
			else
			{
				canvas.Clear(new SKColor(0x88ffffff));

				Paint.Color = SKColors.Black;
				using SKPaint paint = new SKPaint { Color = SKColors.Black, TextSize = 30, IsStroke = false, TextAlign = SKTextAlign.Center };
				canvas.DrawText($"Вы проиграли! Счёт: {Engine.Engine.Score}/{Properties.Settings.Default.BestScore}", (float)DraggingView.ActualWidth / 2, (float)DraggingView.ActualHeight / 2, paint);
			}			
		}
	}
}
