using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using System.Collections.Generic;
using System.Threading;
namespace Game.Server.Rooms
{
	public class BaseWaitingRoom
	{
		private Dictionary<int, GamePlayer> m_list;
		public BaseWaitingRoom()
		{
			this.m_list = new Dictionary<int, GamePlayer>();
		}
		public bool AddPlayer(GamePlayer player)
		{
			bool result = false;
			Dictionary<int, GamePlayer> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				if (!this.m_list.ContainsKey(player.PlayerId))
				{
					this.m_list.Add(player.PlayerId, player);
					result = true;
				}
			}
			finally
			{
				Monitor.Exit(list);
			}
			if (result)
			{
				GSPacketIn pkg = player.Out.SendSceneAddPlayer(player);
				this.SendToALL(pkg, player);
			}
			return result;
		}
		public bool RemovePlayer(GamePlayer player)
		{
			bool result2;
			if (player == null)
			{
				result2 = false;
			}
			else
			{
				bool result = false;
				Dictionary<int, GamePlayer> list;
				Monitor.Enter(list = this.m_list);
				try
				{
					result = this.m_list.Remove(player.PlayerId);
				}
				finally
				{
					Monitor.Exit(list);
				}
				if (result)
				{
					GSPacketIn pkg = player.Out.SendSceneRemovePlayer(player);
					this.SendToALL(pkg, player);
				}
				result2 = true;
			}
			return result2;
		}
		public void SendUpdateRoom(BaseRoom room)
		{
		}
		public void SendToALL(GSPacketIn packet)
		{
			this.SendToALL(packet, null);
		}
		public void SendToALL(GSPacketIn packet, GamePlayer except)
		{
			GamePlayer[] temp = null;
			Dictionary<int, GamePlayer> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				temp = new GamePlayer[this.m_list.Count];
				this.m_list.Values.CopyTo(temp, 0);
			}
			finally
			{
				Monitor.Exit(list);
			}
			if (temp != null)
			{
				GamePlayer[] array = temp;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer p = array[i];
					if (p != null && p != except)
					{
						p.Out.SendTCP(packet);
					}
				}
			}
		}
		public GamePlayer[] GetPlayersSafe()
		{
			GamePlayer[] temp = null;
			Dictionary<int, GamePlayer> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				temp = new GamePlayer[this.m_list.Count];
				this.m_list.Values.CopyTo(temp, 0);
			}
			finally
			{
				Monitor.Exit(list);
			}
			return (temp == null) ? new GamePlayer[0] : temp;
		}
	}
}
