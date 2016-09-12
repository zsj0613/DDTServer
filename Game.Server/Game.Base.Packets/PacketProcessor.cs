using Game.Base.Events;
using Game.Server;
using Game.Server.Packets.Client;
using Lsj.Util.Logs;
using System;
using System.Reflection;
using System.Threading;
namespace Game.Base.Packets
{
	public class PacketProcessor
	{
		private static LogProvider log => LogProvider.Default;
		protected IPacketHandler m_activePacketHandler;
		protected int m_handlerThreadID = 0;
		protected GameClient m_client;
		protected static readonly IPacketHandler[] m_packetHandlers = new IPacketHandler[256];
		public PacketProcessor(GameClient client)
		{
			this.m_client = client;
		}
		public void HandlePacket(GSPacketIn packet)
		{
            int code = (int)packet.Code;
			Statistics.BytesIn += (long)packet.Length;
			Statistics.PacketsIn += 1L;
			IPacketHandler packetHandler = null;
            if (code < PacketProcessor.m_packetHandlers.Length)
			{
				packetHandler = PacketProcessor.m_packetHandlers[code];
			}
			else
			{
				{
					PacketProcessor.log.ErrorFormat("Received packet code is outside of m_packetHandlers array bounds! " + this.m_client.ToString(), new object[0]);
					PacketProcessor.log.Error(Marshal.ToHexDump(string.Format("===> <{2}> Packet 0x{0:X2} (0x{1:X2}) length: {3} (ThreadId={4})", new object[]
					{
						code,
						code ^ 168,
						this.m_client.TcpEndpoint,
						packet.Length,
						Thread.CurrentThread.ManagedThreadId
					}), packet.Buffer));
				}
			}

            if (packetHandler != null)
			{
				long start = (long)Environment.TickCount;
				try
				{
         
                    packetHandler.HandlePacket(this.m_client, packet);
				}
				catch (Exception e)
				{
		
					{
						string client = this.m_client.TcpEndpoint;
						PacketProcessor.log.Error(string.Concat(new string[]
						{
							"Error while processing packet (handler=",
							packetHandler.GetType().FullName,
							"  client: ",
							client,
							")"
						}), e);
						PacketProcessor.log.Error(Marshal.ToHexDump("Package Buffer:", packet.Buffer, 0, packet.Length));
					}
				}
				long timeUsed = (long)Environment.TickCount - start;
				this.m_activePacketHandler = null;
				if (timeUsed > 1000L)
				{
					string source = this.m_client.TcpEndpoint;

						PacketProcessor.log.Warn(string.Concat(new object[]
						{
							"(",
							source,
							") Handle packet Thread ",
							Thread.CurrentThread.ManagedThreadId,
							" ",
							packetHandler,
							" took ",
							timeUsed,
							"ms!"
						}));

				}
			}
		}
		[ScriptLoadedEvent]
		public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
		{
			Array.Clear(PacketProcessor.m_packetHandlers, 0, PacketProcessor.m_packetHandlers.Length);
			int count = PacketProcessor.SearchPacketHandlers("v168", Assembly.GetAssembly(typeof(GameServer)));

				PacketProcessor.log.Info("PacketProcessor: Loaded " + count + " handlers from GameServer Assembly!");

		}
		public static void RegisterPacketHandler(int packetCode, IPacketHandler handler)
		{
			PacketProcessor.m_packetHandlers[packetCode] = handler;
		}
		protected static int SearchPacketHandlers(string version, Assembly assembly)
		{
			int count = 0;
			Type[] types = assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (type.IsClass)
				{
					if (type.GetInterface("Game.Server.Packets.Client.IPacketHandler") != null)
					{
						PacketHandlerAttribute[] packethandlerattribs = (PacketHandlerAttribute[])type.GetCustomAttributes(typeof(PacketHandlerAttribute), true);
						if (packethandlerattribs.Length > 0)
						{
							count++;
							PacketProcessor.RegisterPacketHandler(packethandlerattribs[0].Code, (IPacketHandler)Activator.CreateInstance(type));
						}
					}
				}
			}
			return count;
		}
	}
}
