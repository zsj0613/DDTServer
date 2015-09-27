using Game.Base.Packets;
using log4net;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
namespace Game.Base
{
	public class BaseClient
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected Socket m_sock;
		protected byte[] m_readBuffer;
		protected int m_readBufEnd;
		private SocketAsyncEventArgs rc_event;
		private int m_isConnected;
		protected byte[] m_sendBuffer;
		private bool m_encryted;
		private bool m_strict;
		private bool m_asyncPostSend;
		protected ePrivLevel m_privLevel = ePrivLevel.Player;
		protected IStreamProcessor m_processor;
        public event ClientEventHandle Connected;
		public event ClientEventHandle Disconnected;
		public Socket Socket
		{
			get
			{
				return this.m_sock;
			}
			set
			{
				this.m_sock = value;
			}
		}
		public byte[] PacketBuf
		{
			get
			{
				return this.m_readBuffer;
			}
		}
		public bool IsConnected
		{
			get
			{
				return this.m_isConnected == 1;
			}
		}
		public int PacketBufSize
		{
			get
			{
				return this.m_readBufEnd;
			}
			set
			{
				this.m_readBufEnd = value;
			}
		}
		public string TcpEndpoint
		{
			get
			{
				Socket s = this.m_sock;
				string result;
				if (s != null && s.Connected && s.RemoteEndPoint != null)
				{
					result = s.RemoteEndPoint.ToString();
				}
				else
				{
					result = "not connected";
				}
				return result;
			}
		}
		public byte[] SendBuffer
		{
			get
			{
				return this.m_sendBuffer;
			}
		}
		public bool Encryted
		{
			get
			{
				return this.m_encryted;
			}
			set
			{
				this.m_encryted = value;
			}
		}
		public bool Strict
		{
			get
			{
				return this.m_strict;
			}
			set
			{
				this.m_strict = value;
			}
		}
		public bool AsyncPostSend
		{
			get
			{
				return this.m_asyncPostSend;
			}
			set
			{
				this.m_asyncPostSend = value;
			}
		}
		public ePrivLevel PrivLevel
		{
			get
			{
				return this.m_privLevel;
			}
		}
		public virtual void OnRecv(int num_bytes)
		{
			this.m_processor.ReceiveBytes(num_bytes);
		}
		public virtual void OnRecvPacket(GSPacketIn pkg)
		{
		}
		protected virtual void OnConnect()
		{
			try
			{
				if (((IPEndPoint)this.Socket.RemoteEndPoint).Address == ((IPEndPoint)this.Socket.LocalEndPoint).Address)
				{
					this.m_privLevel = ePrivLevel.Admin;
				}
				else
				{
					this.m_privLevel = ePrivLevel.Player;
				}
			}
			catch (Exception ex)
			{
				BaseClient.log.Error(ex.Message, ex);
			}
			if (Interlocked.Exchange(ref this.m_isConnected, 1) == 0 && this.Connected != null)
			{
				this.Connected(this);
			}
		}
		protected virtual void OnDisconnect()
		{
			if (this.Disconnected != null)
			{
				this.Disconnected(this);
			}
		}
		public BaseClient(byte[] readBuffer, byte[] sendBuffer)
		{
			this.m_readBuffer = readBuffer;
			this.m_sendBuffer = sendBuffer;
			this.m_readBufEnd = 0;
			this.rc_event = new SocketAsyncEventArgs();
			this.rc_event.Completed += new EventHandler<SocketAsyncEventArgs>(this.RecvEventCallback);
			this.m_processor = this.CreateStreamProcessor();
			this.m_encryted = false;
			this.m_strict = true;
		}
		protected virtual IStreamProcessor CreateStreamProcessor()
		{
			return new StreamProcessor(this);
		}
		public void SetFsm(int adder, int muliter)
		{
			this.m_processor.SetFsm(adder, muliter);
		}
		public void ReceiveAsync()
		{
			this.ReceiveAsyncImp(this.rc_event);
		}
		private void ReceiveAsyncImp(SocketAsyncEventArgs e)
		{
			if (this.m_sock != null && this.m_sock.Connected)
			{
				int bufSize = this.m_readBuffer.Length;
				if (this.m_readBufEnd >= bufSize)
				{
					if (BaseClient.log.IsErrorEnabled)
					{
						BaseClient.log.Error(this.TcpEndpoint + " disconnected because of buffer overflow!");
						BaseClient.log.Error(string.Concat(new object[]
						{
							"m_pBufEnd=",
							this.m_readBufEnd,
							"; buf size=",
							bufSize
						}));
						BaseClient.log.Error(Marshal.ToHexDump("receive_buffer:", this.m_readBuffer));
					}
					this.Disconnect();
				}
				else
				{
					e.SetBuffer(this.m_readBuffer, this.m_readBufEnd, bufSize - this.m_readBufEnd);
					if (!this.m_sock.ReceiveAsync(e))
					{
						this.RecvEventCallback(this.m_sock, e);
					}
				}
			}
			else
			{
				this.Disconnect();
			}
		}
		private void RecvEventCallback(object sender, SocketAsyncEventArgs e)
		{
			try
			{
				int num_bytes = e.BytesTransferred;
				if (num_bytes > 0)
				{
					this.OnRecv(num_bytes);
					this.ReceiveAsyncImp(e);
				}
				else
				{
					BaseClient.log.DebugFormat("Disconnecting client ({0}), received bytes={1}", this.TcpEndpoint, num_bytes);
					this.Disconnect();
				}
			}
			catch (Exception ex)
			{
				BaseClient.log.ErrorFormat("{0} RecvCallback:{1}", this.TcpEndpoint, ex);
				this.Disconnect();
			}
		}
		public virtual void SendTCP(GSPacketIn pkg)
		{
			this.m_processor.SendTCP(pkg);
		}
		protected void CloseConnections()
		{
			if (this.m_sock != null)
			{
				try
				{
					this.m_sock.Shutdown(SocketShutdown.Both);
				}
				catch
				{
				}
				try
				{
					this.m_sock.Close();
				}
				catch
				{
				}
			}
		}
		public virtual bool Connect(Socket connectedSocket)
		{
			this.m_sock = connectedSocket;
			bool result;
			if (this.m_sock.Connected)
			{
				int connected = Interlocked.Exchange(ref this.m_isConnected, 1);
				if (connected == 0)
				{
					this.OnConnect();
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public virtual void Disconnect()
		{
			try
			{
				int connected = Interlocked.Exchange(ref this.m_isConnected, 0);
				if (connected == 1)
				{
					this.CloseConnections();
					this.OnDisconnect();
					this.rc_event.Dispose();
					this.m_processor.Dispose();
				}
			}
			catch (Exception e)
			{
				if (BaseClient.log.IsErrorEnabled)
				{
					BaseClient.log.Error("Exception", e);
				}
			}
		}
		public virtual void DisplayMessage(string msg)
		{
		}
	}
}