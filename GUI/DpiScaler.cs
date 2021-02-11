using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace GUI
{
	static class DpiScaler
	{
		[DllImport("User32.dll")]
		private static extern uint GetDpiForWindow(IntPtr hwnd);

		public static double? Scale { get; private set; }

		public static void Initialize(Window window)
		{
			var wih = new WindowInteropHelper(window);
			IntPtr hWnd = wih.Handle;
			uint dpi = GetDpiForWindow(hWnd);
			Scale = dpi switch
			{
				96 => 1.0,
				120 => 1.25,
				144 => 1.50,
				168 => 1.75,
				0 => throw new Exception("GetDpiForWindow returned 0"),

				_ => 1.0
			};
		}

		public static Point ScaleUp(this Point point) => new Point(point.X * Scale.Value, point.Y * Scale.Value);
		public static Point ScaleDown(this Point point) => new Point(point.X / Scale.Value, point.Y / Scale.Value);
		public static Size ScaleUp(this Size size) => new Size(size.Width * Scale.Value, size.Height * Scale.Value);
		public static Size ScaleDown(this Size size) => new Size(size.Width / Scale.Value, size.Height / Scale.Value);
		public static Vector ScaleUp(this Vector vec) => new Vector(vec.X * Scale.Value, vec.Y * Scale.Value);
		public static Vector ScaleDown(this Vector vec) => new Vector(vec.X / Scale.Value, vec.Y / Scale.Value);
	}
}
