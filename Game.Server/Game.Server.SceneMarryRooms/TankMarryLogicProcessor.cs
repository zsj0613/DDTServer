using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.SceneMarryRooms.TankHandle;
using Lsj.Util.Logs;
using System;
using System.Reflection;
namespace Game.Server.SceneMarryRooms
{
	[MarryProcessor(9, "礼堂逻辑")]
	public class TankMarryLogicProcessor : AbstractMarryProcessor
	{
		private MarryCommandMgr _commandMgr;
		public readonly int TIMEOUT = 60000;
		private static LogProvider log => LogProvider.Default;
		public TankMarryLogicProcessor()
		{
			this._commandMgr = new MarryCommandMgr();
		}
		public override void OnTick(MarryRoom room)
		{
			try
			{
				if (room != null)
				{
					room.KickAllPlayer();
					using (PlayerBussiness db = new PlayerBussiness())
					{
						db.DisposeMarryRoomInfo(room.Info.ID);
					}
					GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(room.Info.GroomID);
					GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(room.Info.BrideID);
					GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(room.Info.GroomID, false, room.Info);
					GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(room.Info.BrideID, false, room.Info);
					MarryRoomMgr.RemoveMarryRoom(room);
					GSPacketIn pkg = new GSPacketIn(254);
					pkg.WriteInt(room.Info.ID);
					WorldMgr.MarryScene.SendToALL(pkg);
					room.StopTimer();
				}
			}
			catch (Exception ex)
			{
				//if (TankMarryLogicProcessor.log.IsErrorEnabled)
				{
					TankMarryLogicProcessor.log.Error("OnTick", ex);
				}
			}
		}
		public override void OnGameData(MarryRoom room, GamePlayer player, GSPacketIn packet)
		{
			MarryCmdType type = (MarryCmdType)packet.ReadByte();
			try
			{
				IMarryCommandHandler handleCommand = this._commandMgr.LoadCommandHandler((int)type);
				if (handleCommand != null)
				{
					handleCommand.HandleCommand(this, player, packet);
				}
				else
				{
					TankMarryLogicProcessor.log.Error(string.Format("IP: {0}", player.Client.TcpEndpoint));
				}
			}
			catch (Exception e)
			{
				TankMarryLogicProcessor.log.Error(string.Format("IP:{1}, OnGameData is Error: {0}", e.ToString(), player.Client.TcpEndpoint));
			}
		}
	}
}
