using System;
using System.Drawing;
using System.IO;

namespace DDTTools
{
	public class Tile
	{
		private byte[] _data;

		private int _width;

		private int _height;

		private Rectangle _rect;

		private int _bw;

		private int _bh;

		private bool _digable;

		public Rectangle Bound
		{
			get
			{
				return this._rect;
			}
		}

		public byte[] Data
		{
			get
			{
				return this._data;
			}
		}

		public int Width
		{
			get
			{
				return this._width;
			}
		}

		public int Height
		{
			get
			{
				return this._height;
			}
		}

		public Tile(byte[] data, int width, int height, bool digable)
		{
			this._data = data;
			this._width = width;
			this._height = height;
			this._digable = digable;
			this._bw = this._width / 8 + 1;
			this._bh = this._height;
			this._rect = new Rectangle(0, 0, this._width, this._height);
		}

		public Tile(Bitmap bitmap, bool digable)
		{
			this._width = bitmap.Width;
			this._height = bitmap.Height;
			this._bw = this._width / 8 + 1;
			this._bh = this._height;
			this._data = new byte[this._bw * this._bh];
			this._digable = digable;
			for (int i = 0; i < bitmap.Height; i++)
			{
				for (int j = 0; j < bitmap.Width; j++)
				{
					byte b = (bitmap.GetPixel(j, i).A <= (byte)100) ? (byte)0 : (byte)1;
					byte[] expr_92_cp_0 = this._data;
					int expr_92_cp_1 = i * this._bw + j / 8;
					expr_92_cp_0[expr_92_cp_1] |= (byte)(b << 7 - j % 8);
				}
			}
			this._rect = new Rectangle(0, 0, this._width, this._height);
		}

		public Tile(string file, bool digable)
		{
			BinaryReader binaryReader = new BinaryReader(File.Open(file, FileMode.Open));
			this._width = binaryReader.ReadInt32();
			this._height = binaryReader.ReadInt32();
			this._bw = this._width / 8 + 1;
			this._bh = this._height;
			this._data = binaryReader.ReadBytes(this._bw * this._bh);
			this._digable = digable;
			this._rect = new Rectangle(0, 0, this._width, this._height);
			binaryReader.Close();
		}

		public void Dig(int cx, int cy, Tile surface, Tile border)
		{
			if (this._digable && surface != null)
			{
				int x = cx - surface.Width / 2;
				int y = cy - surface.Height / 2;
				this.Remove(x, y, surface);
				if (border != null)
				{
					x = cx - border.Width / 2;
					y = cy - border.Height / 2;
					this.Add(x, y, surface);
				}
			}
		}

		protected void Add(int x, int y, Tile tile)
		{
			byte[] data = tile._data;
			Rectangle bound = tile.Bound;
			bound.Offset(x, y);
			bound.Intersect(this._rect);
			if (bound.Width != 0 && bound.Height != 0)
			{
				bound.Offset(-x, -y);
				int num = bound.X / 8;
				int num2 = (bound.X + x) / 8;
				int y2 = bound.Y;
				int num3 = (int)Math.Floor((double)bound.Width / 8.0);
				int height = bound.Height;
				int num4 = bound.X % 8;
				for (int i = 0; i < height; i++)
				{
					int num5 = 0;
					for (int j = 0; j < num3; j++)
					{
						int num6 = (i + y + y2) * this._bw + j + num2;
						int num7 = (i + y2) * tile._bw + j + num;
						byte expr_DF = data[num7];
						int num8 = expr_DF >> num4;
						byte[] expr_F5_cp_0 = this._data;
						int expr_F5_cp_1 = num6;
						expr_F5_cp_0[expr_F5_cp_1] |= (byte)num8;
						byte[] expr_10A_cp_0 = this._data;
						int expr_10A_cp_1 = num6;
						expr_10A_cp_0[expr_10A_cp_1] |= (byte)num5;
						num5 = (int)expr_DF << 8 - num4;
					}
				}
			}
		}

		protected void Remove(int x, int y, Tile tile)
		{
			byte[] data = tile._data;
			Rectangle bound = tile.Bound;
			bound.Offset(x, y);
			bound.Intersect(this._rect);
			if (bound.Width != 0 && bound.Height != 0)
			{
				bound.Offset(-x, -y);
				int num = bound.X / 8;
				int num2 = (bound.X + x) / 8;
				int y2 = bound.Y;
				int num3 = (int)Math.Floor((double)bound.Width / 8.0);
				int height = bound.Height;
				int num4 = bound.X % 8;
				for (int i = 0; i < height; i++)
				{
					int num5 = 0;
					for (int j = 0; j < num3; j++)
					{
						int num6 = (i + y + y2) * this._bw + j + num2;
						int num7 = (i + y2) * tile._bw + j + num;
						byte expr_E2 = data[num7];
						int num8 = expr_E2 >> num4;
						int num9 = (int)this._data[num6];
						int expr_F8 = num9;
						num9 = (expr_F8 & ~(expr_F8 & num8));
						if (num5 != 0)
						{
							int expr_106 = num9;
							num9 = (expr_106 & ~(expr_106 & num5));
						}
						this._data[num6] = (byte)num9;
						num5 = (int)expr_E2 << 8 - num4;
					}
				}
			}
		}

		public bool IsEmpty(int x, int y)
		{
			if (x >= 0 && x < this._width && y >= 0 && y < this._height)
			{
				byte b = (byte)(1 << 7 - x % 8);
				return (this._data[y * this._width + x / 8] & b) == 0;
			}
			return true;
		}

		public bool IsYLineEmtpy(int x, int y, int h)
		{
			if (x >= 0 && x < this._width)
			{
				y = ((y < 0) ? 0 : y);
				h = ((y + h > this._height) ? (this._height - y) : h);
				for (int i = 0; i < h; i++)
				{
					if (!this.IsEmpty(x, y + i))
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		public bool IsRectangleEmptyQuick(Rectangle rect)
		{
			rect.Intersect(this._rect);
			return this.IsEmpty(rect.Right, rect.Bottom) && this.IsEmpty(rect.Left, rect.Bottom) && this.IsEmpty(rect.Right, rect.Top) && this.IsEmpty(rect.Left, rect.Top);
		}

		public Point FindNotEmptyPoint(int x, int y, int h)
		{
			if (x >= 0 && x < this._width)
			{
				y = ((y < 0) ? 0 : y);
				h = ((y + h > this._height) ? (this._height - y) : h);
				for (int i = 0; i < h; i++)
				{
					if (!this.IsEmpty(x, y + i))
					{
						return new Point(x, y + i);
					}
				}
				return new Point(-1, -1);
			}
			return new Point(-1, -1);
		}

		public Bitmap ToBitmap()
		{
			Bitmap bitmap = new Bitmap(this._width, this._height);
			for (int i = 0; i < this._height; i++)
			{
				for (int j = 0; j < this._width; j++)
				{
					int num = j / 8;
					byte b = (byte)(1 << 7 - j % 8);
					if ((this._data[i * this._bw + num] & b) == 0)
					{
						bitmap.SetPixel(j, i, Color.FromArgb(0, 0, 0, 0));
					}
					else
					{
						bitmap.SetPixel(j, i, Color.FromArgb(255, 0, 0, 0));
					}
				}
			}
			return bitmap;
		}

		public Tile Clone()
		{
			return new Tile(this._data.Clone() as byte[], this._width, this._height, this._digable);
		}
	}
}
