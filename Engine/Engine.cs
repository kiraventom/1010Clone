using System;
using System.Windows;
using System.Numerics;

namespace Engine
{
	public static class Engine
	{
		public static Random RND { get; } = new();
		public static int Score { get; set; } = 0;
	}
}
