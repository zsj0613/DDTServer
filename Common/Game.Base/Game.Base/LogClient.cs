
using System;
using System.Reflection;
namespace Game.Base
{
	public class LogClient : BaseClient
	{

		public LogClient() : base(null, null)
		{
		}
		public override void DisplayMessage(string msg)
		{
			LogClient.log.Info(msg);
		}
	}
}
