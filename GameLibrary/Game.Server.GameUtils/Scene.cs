using Game.Base.Packets;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Game.Server.GameUtils
{
	public class Scene
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		protected ReaderWriterLock _locker = new ReaderWriterLock();
		protected Dictionary<int, GamePlayer> _players = new Dictionary<int, GamePlayer>();
		public Scene(ServerInfo info)
		{
		}
		public bool AddPlayer(GamePlayer player)
		{
			this._locker.AcquireWriterLock(-1);
			bool result;
			try
			{
				if (this._players.ContainsKey(player.PlayerCharacter.ID))
				{
					this._players[player.PlayerCharacter.ID] = player;
					result = true;
				}
				else
				{
					this._players.Add(player.PlayerCharacter.ID, player);
					result = true;
				}
			}
			finally
			{
				this._locker.ReleaseWriterLock();
			}
			return result;
		}
		public void RemovePlayer(GamePlayer player)
		{
			this._locker.AcquireWriterLock(-1);
			try
			{
				if (this._players.ContainsKey(player.PlayerCharacter.ID))
				{
					this._players.Remove(player.PlayerCharacter.ID);
				}
			}
			finally
			{
				this._locker.ReleaseWriterLock();
			}
			GamePlayer[] list = this.GetAllPlayer();
			GSPacketIn pkg = null;
			GamePlayer[] array = list;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				if (pkg == null)
				{
					pkg = p.Out.SendSceneRemovePlayer(player);
				}
				else
				{
					p.Out.SendTCP(pkg);
				}
			}
		}
		public GamePlayer[] GetAllPlayer()
		{
			GamePlayer[] list = null;
			this._locker.AcquireReaderLock(-1);
			try
			{
				list = this._players.Values.ToArray<GamePlayer>();
			}
			finally
			{
				this._locker.ReleaseReaderLock();
			}
			return (list == null) ? new GamePlayer[0] : list;
		}
		public GamePlayer GetClientFromID(int id)
		{
			GamePlayer result;
			try
			{
				if (this._players.Keys.Contains(id))
				{
					result = this._players[id];
					return result;
				}
			}
			finally
			{
			}
			result = null;
			return result;
		}
		public void SendToALL(GSPacketIn pkg)
		{
			this.SendToALL(pkg, null);
		}
		public void SendToALL(GSPacketIn pkg, GamePlayer except)
		{
			GamePlayer[] list = this.GetAllPlayer();
			GamePlayer[] array = list;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				if (p != except)
				{
					p.Out.SendTCP(pkg);
				}
			}
		}
	}
}
