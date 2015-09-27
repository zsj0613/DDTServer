using Game.Base.Packets;
using Game.Server.Battle;
using Game.Server.GameObjects;
using System;
using System.Collections.Generic;
using System.Threading;
namespace Game.Server.Managers
{
	public class MessageMgr
	{
		private static Dictionary<string, int> m_MesageList = new Dictionary<string, int>();
		public static Guid Guid
		{
			get
			{
				return Guid.NewGuid();
			}
		}
		public static int GetGuidToInt(Guid id)
		{
			byte[] bytes = id.ToByteArray();
			return BitConverter.ToInt32(bytes, 0);
		}
		public static bool AddMessageRecord(string guid, int date)
		{
			Dictionary<string, int> mesageList;
			Monitor.Enter(mesageList = MessageMgr.m_MesageList);
			bool result;
			try
			{
				if (!MessageMgr.m_MesageList.ContainsKey(guid))
				{
					MessageMgr.m_MesageList.Add(guid, date);
					result = true;
					return result;
				}
			}
			finally
			{
				Monitor.Exit(mesageList);
			}
			result = false;
			return result;
		}
		public static void SendMessages(GSPacketIn pkg)
		{
			GSPacketIn packetClone = pkg.Clone();
			string guid = pkg.ReadString();
			if (MessageMgr.AddMessageRecord(guid, pkg.Parameter1))
			{
				GSPacketIn clentPacket = pkg.ReadPacket();
				GamePlayer[] players = WorldMgr.GetAllPlayers();
				GamePlayer[] array = players;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer player = array[i];
					player.Out.SendTCP(clentPacket);
				}
				GameServer.Instance.LoginServer.SendTCP(packetClone);
				List<BattleServer> servers = BattleMgr.GetAllBattles();
				foreach (BattleServer server in servers)
				{
					server.Connector.SendTCP(packetClone);
				}
			}
		}
		public static void RemoveMessageRecord()
		{
			Dictionary<string, int> mesageList;
			Monitor.Enter(mesageList = MessageMgr.m_MesageList);
			try
			{
				List<string> removeList = new List<string>();
				foreach (string key in MessageMgr.m_MesageList.Keys)
				{
					if (MessageMgr.m_MesageList[key] + 1 < DateTime.Today.DayOfYear || MessageMgr.m_MesageList[key] + 1 >= 366)
					{
						removeList.Add(key);
					}
				}
				if (removeList != null)
				{
					foreach (string key in removeList)
					{
						MessageMgr.m_MesageList.Remove(key);
					}
				}
			}
			finally
			{
				Monitor.Exit(mesageList);
			}
		}
	}
}
