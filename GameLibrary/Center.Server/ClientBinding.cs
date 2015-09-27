using System;
using System.ServiceModel;
namespace Center.Server
{
	public class ClientBinding
	{
		public const string URI_STRING = "net.tcp://{0}:9230/service";
		private string _Address;
		public NetTcpBinding Binding
		{
			get;
			private set;
		}
		public EndpointAddress Uri
		{
			get;
			private set;
		}
		public string Address
		{
			get
			{
				return this._Address;
			}
			set
			{
				this.Uri = new EndpointAddress(string.Format("net.tcp://{0}:9230/service", value));
				this._Address = value;
			}
		}
		public ClientBinding()
		{
			this.Binding = new NetTcpBinding
			{
				MaxReceivedMessageSize = 65536L
			};
			this.Binding.ReaderQuotas.MaxArrayLength = 16384;
			this.Binding.CloseTimeout = new TimeSpan(0, 1, 0);
			this.Binding.OpenTimeout = new TimeSpan(0, 1, 0);
			this.Binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
			this.Binding.SendTimeout = new TimeSpan(0, 1, 0);
			this.Binding.Security.Mode = SecurityMode.None;
		}
	}
}
