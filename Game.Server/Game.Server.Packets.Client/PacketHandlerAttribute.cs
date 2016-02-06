using System;
namespace Game.Server.Packets.Client
{
	[AttributeUsage(AttributeTargets.Class)]
	public class PacketHandlerAttribute : Attribute
	{
		protected int m_code;
		protected string m_desc;
		protected ePacketLevel m_level;
		public int Code
		{
			get
			{
				return this.m_code;
			}
		}
		public ePacketLevel Level
		{
			get
			{
				return this.m_level;
			}
		}
		public string Description
		{
			get
			{
				return this.m_desc;
			}
		}
		public PacketHandlerAttribute(int code, string desc)
		{
			this.m_code = code;
			this.m_desc = desc;
		}
	}
}
