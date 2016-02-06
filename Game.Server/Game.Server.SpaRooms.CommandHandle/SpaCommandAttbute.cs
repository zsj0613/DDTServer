using System;
namespace Game.Server.SpaRooms.CommandHandle
{
	public class SpaCommandAttbute : Attribute
	{
		public byte Code
		{
			get;
			private set;
		}
		public SpaCommandAttbute(byte code)
		{
			this.Code = code;
		}
	}
}
