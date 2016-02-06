
using System;
using System.Reflection;
using System.Threading;
namespace Game.Base.Packets
{
	public class GSPacketIn : PacketIn
	{

		public static readonly ushort HDR_SIZE = 20;
		public static readonly short HEADER = 29099;
		protected short m_checksum;
		protected short m_code;
		protected int m_cliendId;
		protected int m_parameter1;
		protected int m_parameter2;
		public short CheckSum
		{
			get
			{
				return this.m_checksum;
			}
		}
		public short Code
		{
			get
			{
				return this.m_code;
			}
			set
			{
				this.m_code = value;
			}
		}
		public int ClientID
		{
			get
			{
				return this.m_cliendId;
			}
			set
			{
				if (value != this.m_cliendId)
				{
					this.m_cliendId = value;
					this.ClearChecksum();
				}
			}
		}
		public int Parameter1
		{
			get
			{
				return this.m_parameter1;
			}
			set
			{
				if (value != this.m_parameter1)
				{
					this.m_parameter1 = value;
					this.ClearChecksum();
				}
			}
		}
		public int Parameter2
		{
			get
			{
				return this.m_parameter2;
			}
			set
			{
				if (value != this.m_parameter2)
				{
					this.m_parameter2 = value;
					this.ClearChecksum();
				}
			}
		}
		public GSPacketIn(byte[] buf, int len) : base(buf, len)
		{
		}
		public GSPacketIn(short code, int clientId, int size) : base(new byte[size], (int)GSPacketIn.HDR_SIZE)
		{
			this.m_code = code;
			this.m_cliendId = clientId;
			this.m_offset = (int)GSPacketIn.HDR_SIZE;
		}
		public GSPacketIn(short code) : this(code, 0, 2048)
		{
		}
		public GSPacketIn(short code, int clientId) : this(code, clientId, 2048)
		{
		}
		public void ReadHeader()
		{
			Monitor.Enter(this);
			try
			{
				this.m_offset = 0;
				this.ReadShort();
				this.m_length = (int)this.ReadShort();
				this.m_checksum = this.ReadShort();
				this.m_code = this.ReadShort();
				this.m_cliendId = this.ReadInt();
				this.m_parameter1 = this.ReadInt();
				this.m_parameter2 = this.ReadInt();
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		public void WriteHeader()
		{
			Monitor.Enter(this);
			try
			{
				int old = this.m_offset;
				this.m_offset = 0;
				base.WriteShort(GSPacketIn.HEADER);
				base.WriteShort((short)this.m_length);
				this.m_offset = 6;
				base.WriteShort(this.m_code);
				base.WriteInt(this.m_cliendId);
				base.WriteInt(this.m_parameter1);
				base.WriteInt(this.m_parameter2);
				if (this.m_checksum == 0)
				{
					this.m_checksum = this.CalculateChecksum();
				}
				this.m_offset = 4;
				base.WriteShort(this.m_checksum);
				this.m_offset = old;
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		public void ClearChecksum()
		{
			this.m_checksum = 0;
		}
		public short CalculateChecksum()
		{
			byte[] pak = this.m_buffer;
			int val = 119;
			int i = 6;
			int len = this.m_length;
			while (i < len)
			{
				val += (int)pak[i++];
			}
			return (short)(val & 32639);
		}
		public void WritePacket(GSPacketIn content)
		{
			content.WriteHeader();
			this.WriteShort((short)content.Length);
			this.WriteShort(0);
			this.Write(content.Buffer, 2, content.Length - 2);
		}
		public GSPacketIn ReadPacket()
		{
			int length = (int)this.ReadShort();
			byte[] data = this.ReadBytes(length);
			data[0] = 113;
			data[1] = 171;
			GSPacketIn content = new GSPacketIn(data, length);
			content.ReadHeader();
			if (content.Length != length)
			{
				throw new Exception(string.Format("Error packet in ReadPacket,data length didn't equal packet length, data:{0}, packet:{1}", length, content.Length));
			}
			return content;
		}
		public void Compress()
		{
			byte[] temp = Marshal.Compress(this.m_buffer, (int)GSPacketIn.HDR_SIZE, base.Length - (int)GSPacketIn.HDR_SIZE);
			this.m_offset = (int)GSPacketIn.HDR_SIZE;
			this.Write(temp);
			this.m_length = temp.Length + (int)GSPacketIn.HDR_SIZE;
		}
		public void UnCompress()
		{
		}
		public void ClearContext()
		{
			this.m_offset = (int)GSPacketIn.HDR_SIZE;
			this.m_length = (int)GSPacketIn.HDR_SIZE;
			this.m_checksum = 0;
		}
		public GSPacketIn Clone()
		{
			GSPacketIn pkg = new GSPacketIn(this.m_buffer, this.m_length);
			pkg.ReadHeader();
			pkg.Offset = this.m_length;
			pkg.ClearChecksum();
			return pkg;
		}
	}
}
