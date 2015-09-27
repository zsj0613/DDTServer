using System;
using System.Drawing;
using System.IO;
namespace Game.Logic.Phy.Maps
{
	public class Tile
	{
		private byte[] _data;
		private int _width;
		private int _height;
		private Rectangle _rect;
		private int _bw = 0;
		private int _bh = 0;
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
			set
			{
				this._data = value;
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
			GC.AddMemoryPressure((long)data.Length);
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
					byte flag = (bitmap.GetPixel(j, i).A <= 100) ? (byte)0 : (byte)1;
					byte[] expr_A4_cp_0 = this._data;
					int expr_A4_cp_1 = i * this._bw + j / 8;
					expr_A4_cp_0[expr_A4_cp_1] |= (byte)(flag << 7 - j % 8);
				}
			}
			this._rect = new Rectangle(0, 0, this._width, this._height);
			GC.AddMemoryPressure((long)this._data.Length);
		}
		public Tile(string file, bool digable)
		{
			FileStream fs = File.Open(file, FileMode.Open);
			BinaryReader reader = new BinaryReader(fs);
			this._width = reader.ReadInt32();
			this._height = reader.ReadInt32();
			this._bw = this._width / 8 + 1;
			this._bh = this._height;
			this._data = reader.ReadBytes(this._bw * this._bh);
			this._digable = digable;
			this._rect = new Rectangle(0, 0, this._width, this._height);
			reader.Close();
			GC.AddMemoryPressure((long)this._data.Length);
		}
		~Tile()
		{
			GC.RemoveMemoryPressure((long)this._data.Length);
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
		}
		protected void Remove(int x, int y, Tile tile)
		{
			byte[] addData = tile._data;
			Rectangle rect = tile.Bound;
			rect.Offset(x, y);
			rect.Intersect(this._rect);
			if (rect.Width != 0 && rect.Height != 0)
			{
				rect.Offset(-x, -y);
				int cx = rect.X / 8;
				int cx2 = (rect.X + x) / 8;
				int cy = rect.Y;
				int cw = rect.Width / 8 + 1;
				int ch = rect.Height;
				if (rect.X == 0)
				{
					if (cw + cx2 < this._bw)
					{
						cw++;
						cw = ((cw > tile._bw) ? tile._bw : cw);
					}
					int b_offset = (rect.X + x) % 8;
					for (int i = 0; i < ch; i++)
					{
						int l_bits = 0;
						for (int j = 0; j < cw; j++)
						{
							int self_offset = (i + y + cy) * this._bw + j + cx2;
							int tile_offset = (i + cy) * tile._bw + j + cx;
							int src = (int)addData[tile_offset];
							int r_bits = src >> b_offset;
							int target = (int)this._data[self_offset];
							target &= ~(target & r_bits);
							if (l_bits != 0)
							{
								target &= ~(target & l_bits);
							}
							this._data[self_offset] = (byte)target;
							l_bits = src << 8 - b_offset;
						}
					}
				}
				else
				{
					int b_offset = rect.X % 8;
					for (int i = 0; i < ch; i++)
					{
						for (int j = 0; j < cw; j++)
						{
							int self_offset = (i + y + cy) * this._bw + j + cx2;
							int tile_offset = (i + cy) * tile._bw + j + cx;
							int src = (int)addData[tile_offset];
							int l_bits = src << b_offset;
							int r_bits;
							if (j < cw - 1)
							{
								src = (int)addData[tile_offset + 1];
								r_bits = src >> 8 - b_offset;
							}
							else
							{
								r_bits = 0;
							}
							int target = (int)this._data[self_offset];
							target &= ~(target & l_bits);
							if (r_bits != 0)
							{
								target &= ~(target & r_bits);
							}
							this._data[self_offset] = (byte)target;
						}
					}
				}
			}
		}
		public bool IsEmpty(int x, int y)
		{
			bool result;
			if (x >= 0 && x < this._width && y >= 0 && y < this._height)
			{
				byte flag = (byte)(1 << 7 - x % 8);
				result = ((this._data[y * this._bw + x / 8] & flag) == 0);
			}
			else
			{
				result = true;
			}
			return result;
		}
		public bool IsYLineEmtpy(int x, int y, int h)
		{
			bool result;
			if (x >= 0 && x < this._width)
			{
				y = ((y < 0) ? 0 : y);
				h = ((y + h > this._height) ? (this._height - y) : h);
				for (int i = 0; i < h; i++)
				{
					if (!this.IsEmpty(x, y + i))
					{
						result = false;
						return result;
					}
				}
				result = true;
			}
			else
			{
				result = true;
			}
			return result;
		}
		public bool IsRectangleEmptyQuick(Rectangle rect)
		{
			rect.Intersect(this._rect);
			return this.IsEmpty(rect.Right, rect.Bottom) && this.IsEmpty(rect.Left, rect.Bottom) && this.IsEmpty(rect.Right, rect.Top) && this.IsEmpty(rect.Left, rect.Top);
		}
		public Point FindNotEmptyPoint(int x, int y, int h)
		{
			Point result;
			if (x >= 0 && x < this._width)
			{
				y = ((y < 0) ? 0 : y);
				h = ((y + h > this._height) ? (this._height - y) : h);
				for (int i = 0; i < h; i++)
				{
					if (!this.IsEmpty(x, y + i))
					{
						result = new Point(x, y + i);
						return result;
					}
				}
				result = new Point(-1, -1);
			}
			else
			{
				result = new Point(-1, -1);
			}
			return result;
		}
		public Bitmap ToBitmap()
		{
			Bitmap bitmap = new Bitmap(this._width, this._height);
			for (int i = 0; i < this._height; i++)
			{
				for (int j = 0; j < this._width; j++)
				{
					if (this.IsEmpty(j, i))
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
		public bool CopyData(Tile source)
		{
			bool result;
			if (source == null)
			{
				result = false;
			}
			else
			{
				if (source._data.Length != this._data.Length)
				{
					result = false;
				}
				else
				{
					Array.Copy(source._data, this._data, this._data.Length);
					result = true;
				}
			}
			return result;
		}
	}
}
