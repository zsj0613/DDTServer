using System;
namespace Game.Base.Packets
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class PacketLibAttribute : Attribute
	{
		private int m_rawVersion;
		public int RawVersion
		{
			get
			{
				return this.m_rawVersion;
			}
		}
		public PacketLibAttribute(int rawVersion)
		{
			this.m_rawVersion = rawVersion;
		}
	}
}
