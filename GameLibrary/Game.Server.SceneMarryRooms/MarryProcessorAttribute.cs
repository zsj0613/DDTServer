using System;
namespace Game.Server.SceneMarryRooms
{
	public class MarryProcessorAttribute : Attribute
	{
		private byte _code;
		private string _descript;
		public byte Code
		{
			get
			{
				return this._code;
			}
		}
		public string Description
		{
			get
			{
				return this._descript;
			}
		}
		public MarryProcessorAttribute(byte code, string description)
		{
			this._code = code;
			this._descript = description;
		}
	}
}
