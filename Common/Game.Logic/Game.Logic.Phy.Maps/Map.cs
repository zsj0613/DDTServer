using Game.Logic.Phy.Object;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
namespace Game.Logic.Phy.Maps
{
	public class Map
	{
		private static LogProvider log => LogProvider.Default;
		private MapInfo _info;
		private float _wind = 0f;
		private HashSet<Physics> _objects;
		protected Tile _layer1;
		protected Tile _layer2;
		protected Rectangle _bound;
		public float wind
		{
			get
			{
				return this._wind;
			}
			set
			{
				this._wind = value;
			}
		}
		public float gravity
		{
			get
			{
				return (float)this._info.Weight;
			}
		}
		public float airResistance
		{
			get
			{
				return (float)this._info.DragIndex;
			}
		}
		public HashSet<Physics> Ojbects
		{
			get
			{
				return this._objects;
			}
		}
		public Tile Ground
		{
			get
			{
				return this._layer1;
			}
		}
		public MapInfo Info
		{
			get
			{
				return this._info;
			}
		}
		public Rectangle Bound
		{
			get
			{
				return this._bound;
			}
		}
		public Map(MapInfo info, Tile layer1, Tile layer2)
		{
			this._info = info;
			this._objects = new HashSet<Physics>();
			this._layer1 = layer1;
			this._layer2 = layer2;
			if (this._layer1 != null)
			{
				this._bound = new Rectangle(0, 0, this._layer1.Width, this._layer1.Height);
			}
			else
			{
				this._bound = new Rectangle(0, 0, this._layer2.Width, this._layer2.Height);
			}
		}
		public void Dig(int cx, int cy, Tile surface, Tile border)
		{
			if (this._layer1 != null)
			{
				this._layer1.Dig(cx, cy, surface, border);
			}
			if (this._layer2 != null)
			{
				this._layer2.Dig(cx, cy, surface, border);
			}
		}
		public bool IsEmpty(int x, int y)
		{
			return (this._layer1 == null || this._layer1.IsEmpty(x, y)) && (this._layer2 == null || this._layer2.IsEmpty(x, y));
		}
		public bool IsRectangleEmpty(Rectangle rect)
		{
			return (this._layer1 == null || this._layer1.IsRectangleEmptyQuick(rect)) && (this._layer2 == null || this._layer2.IsRectangleEmptyQuick(rect));
		}
		public Point FindYLineNotEmptyPoint(int x, int y, int h)
		{
			x = ((x < 0) ? 0 : ((x >= this._bound.Width) ? (this._bound.Width - 1) : x));
			y = ((y < 0) ? 0 : y);
			h = ((y + h >= this._bound.Height) ? (this._bound.Height - y - 1) : h);
			Point result;
			for (int i = 0; i < h; i++)
			{
				if (!this.IsEmpty(x - 1, y) || !this.IsEmpty(x + 1, y))
				{
					result = new Point(x, y);
					return result;
				}
				y++;
			}
			result = Point.Empty;
			return result;
		}
		public Point FindYLineNotEmptyPoint(int x, int y)
		{
			return this.FindYLineNotEmptyPoint(x, y, this._bound.Height);
		}
		public Point FindNextWalkPoint(int x, int y, int direction, int stepX, int stepY)
		{
			Point result;
			if (direction != 1 && direction != -1)
			{
				result = Point.Empty;
			}
			else
			{
				int tx = x + direction * stepX;
				if (tx < 0 || tx > this._bound.Width)
				{
					result = Point.Empty;
				}
				else
				{
					Point p = this.FindYLineNotEmptyPoint(tx, y - stepY - 1, stepY * 2 + 3);
					if (p != Point.Empty)
					{
						if (Math.Abs(p.Y - y) > stepY)
						{
							p = Point.Empty;
						}
					}
					result = p;
				}
			}
			return result;
		}
		public bool canMove(int x, int y)
		{
			return this.IsEmpty(x, y) && !this.IsOutMap(x, y);
		}
		public bool IsOutMap(int x, int y)
		{
			return x < 0 || x >= this._bound.Width || y >= this._bound.Height;
		}
		public void AddPhysical(Physics phy)
		{
			phy.SetMap(this);
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				this._objects.Add(phy);
			}
			finally
			{
				Monitor.Exit(objects);
			}
		}
		public void RemovePhysical(Physics phy)
		{
			phy.SetMap(null);
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				this._objects.Remove(phy);
			}
			finally
			{
				Monitor.Exit(objects);
			}
		}
		public List<Physics> GetAllPhysicalSafe()
		{
			List<Physics> list = new List<Physics>();
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics p in this._objects)
				{
					list.Add(p);
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return list;
		}
		public List<PhysicalObj> GetAllPhysicalObjSafe()
		{
			List<PhysicalObj> list = new List<PhysicalObj>();
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics p in this._objects)
				{
					if (p is PhysicalObj)
					{
						list.Add(p as PhysicalObj);
					}
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return list;
		}
		public Physics[] FindPhysicalObjects(Rectangle rect, Physics except)
		{
			List<Physics> list = new List<Physics>();
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics phy in this._objects)
				{
					if (phy.IsLiving && phy != except)
					{
						Rectangle t = phy.Bound;
						t.Offset(phy.X, phy.Y);
						if (t.IntersectsWith(rect))
						{
							list.Add(phy);
						}
					}
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return list.ToArray();
		}
		public List<Player> FindPlayers(int x, int y, int radius)
		{
			List<Player> list = new List<Player>();
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics phy in this._objects)
				{
					if (phy is Player && phy.IsLiving && phy.Distance(x, y) < (double)radius)
					{
						list.Add(phy as Player);
					}
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return list;
		}
		public List<Living> FindLivings(int x, int y, int radius)
		{
			List<Living> list = new List<Living>();
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics phy in this._objects)
				{
					if (phy is Living && phy.IsLiving && phy.Distance(x, y) < (double)radius)
					{
						list.Add(phy as Living);
					}
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return list;
		}
		public List<Living> FindLiving(int fx, int tx, List<Living> exceptLivings)
		{
			List<Living> list = new List<Living>();
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics phy in this._objects)
				{
					bool result = true;
					if (phy is Living && phy.IsLiving && phy.X > fx && phy.X < tx)
					{
						if (exceptLivings != null && exceptLivings.Count != 0)
						{
							foreach (Living living in exceptLivings)
							{
								if (((Living)phy).Id == living.Id)
								{
									result = false;
									break;
								}
							}
							if (result)
							{
								list.Add(phy as Living);
							}
						}
						else
						{
							list.Add(phy as Living);
						}
					}
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return list;
		}
		public List<Living> FindHitByHitPiont(Point p, int radius)
		{
			List<Living> list = new List<Living>();
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics phy in this._objects)
				{
					if (phy is Living && phy.IsLiving && (phy as Living).BoundDistance(p) < (double)radius)
					{
						list.Add(phy as Living);
					}
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return list;
		}
		public List<Living> FindLivings(Point p, int radius)
		{
			List<Living> list = new List<Living>();
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics phy in this._objects)
				{
					if (phy is Living && phy.IsLiving && (phy as Living).BoundDistance(p) < (double)radius)
					{
						list.Add((Living)phy);
					}
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return list;
		}
		public Living FindNearestEnemy(int x, int y, double maxdistance, Living except)
		{
			Living player = null;
			HashSet<Physics> objects;
			Monitor.Enter(objects = this._objects);
			try
			{
				foreach (Physics phy in this._objects)
				{
					if (phy is Living && phy != except && phy.IsLiving && ((Living)phy).Team != except.Team)
					{
						double dis = phy.Distance(x, y);
						if (dis < maxdistance)
						{
							player = (phy as Living);
							maxdistance = dis;
						}
					}
				}
			}
			finally
			{
				Monitor.Exit(objects);
			}
			return player;
		}
		public void Dispose()
		{
			HashSet<Physics> list = this._objects;
			this._objects = new HashSet<Physics>();
			foreach (Physics phy in list)
			{
				try
				{
					phy.Dispose();
				}
				catch (Exception ex)
				{
					Map.log.Error("Map Dispose exception", ex);
				}
			}
		}
		public Map Clone()
		{
			Tile layer = (this._layer1 != null) ? this._layer1.Clone() : null;
			Tile layer2 = (this._layer2 != null) ? this._layer2.Clone() : null;
			return new Map(this._info, layer, layer2);
		}
		public bool CopyData(Map source)
		{
			this._objects.Clear();
			return source != null && (this._layer1 == null || this._layer1.CopyData(source._layer1)) && (this._layer2 == null || this._layer2.CopyData(source._layer2));
		}
	}
}
