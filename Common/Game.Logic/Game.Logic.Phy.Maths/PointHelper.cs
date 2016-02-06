using System;
using System.Drawing;
namespace Game.Logic.Phy.Maths
{
	public static class PointHelper
	{
		public static Point Normalize(this Point point, int len)
		{
			double i = point.Length();
			return new Point((int)((double)(point.X * len) / i), (int)((double)(point.Y * len) / i));
		}
		public static double Length(this Point point)
		{
			return Math.Sqrt((double)(point.X * point.X + point.Y * point.Y));
		}
		public static double Distance(this Point point, Point target)
		{
			int dx = point.X - target.X;
			int dy = point.Y - target.Y;
			return Math.Sqrt((double)(dx * dx + dy * dy));
		}
		public static double Distance(this Point point, int tx, int ty)
		{
			int dx = point.X - tx;
			int dy = point.Y - ty;
			return Math.Sqrt((double)(dx * dx + dy * dy));
		}
		public static PointF Normalize(this PointF point, float len)
		{
			double i = Math.Sqrt((double)(point.X * point.X + point.Y * point.Y));
			return new PointF((float)((double)(point.X * len) / i), (float)((double)(point.Y * len) / i));
		}
		public static double Length(this PointF point)
		{
			return Math.Sqrt((double)(point.X * point.X + point.Y * point.Y));
		}
	}
}
