using System;
using System.Collections.Generic;
using System.Drawing;
namespace Game.Logic
{
	public class MapPoint
	{
		private List<Point> posX;
		private List<Point> posX1;
		private List<Point> posX2;
		public List<Point> PosX
		{
			get
			{
				return this.posX;
			}
			set
			{
				this.posX = value;
			}
		}
		public List<Point> PosX1
		{
			get
			{
				return this.posX1;
			}
			set
			{
				this.posX1 = value;
			}
		}
		public List<Point> PosX2
		{
			get
			{
				return this.posX2;
			}
			set
			{
				this.posX2 = value;
			}
		}
		public MapPoint()
		{
			this.posX = new List<Point>();
			this.posX1 = new List<Point>();
			this.posX2 = new List<Point>();
		}
	}
}
