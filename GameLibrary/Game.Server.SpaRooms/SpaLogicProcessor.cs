using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.SpaRooms.CommandHandle;
using log4net;
using System;
using System.Reflection;
namespace Game.Server.SpaRooms
{
	[SpaProcessor(10, "温泉逻辑")]
	public class SpaLogicProcessor : AbstractSpaProcessor
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private SpaCommandMgr _commandMgr;
		public SpaLogicProcessor()
		{
			this._commandMgr = new SpaCommandMgr();
		}
		public override void OnTick(SpaRoom room)
		{
			try
			{
				if (room != null)
				{
					room.KickAllPlayer();
					using (PlayerBussiness db = new PlayerBussiness())
					{
						db.DisposeSpaRoomInfo(room.Spa_Room_Info.RoomID);
					}
					SpaRoomMgr.RemoveSpaRoom(room);
					GSPacketIn pkg = new GSPacketIn(174);
					pkg.WriteInt(room.Spa_Room_Info.RoomID);
					WorldMgr.SpaScene.SendToALL(pkg);
					room.StopTimer();
				}
			}
			catch (Exception ex)
			{
				if (SpaLogicProcessor.log.IsErrorEnabled)
				{
					SpaLogicProcessor.log.Error("OnTick", ex);
				}
			}
		}
		public override void OnGameData(SpaRoom room, GamePlayer player, GSPacketIn packet)
		{
			SpaCmdType type = (SpaCmdType)packet.ReadByte();
			try
			{
				ISpaCommandHandler handleCommand = this._commandMgr.LoadCommandHandler((int)type);
				if (handleCommand != null)
				{
					handleCommand.HandleCommand(player, packet);
				}
				else
				{
					SpaLogicProcessor.log.Error(string.Format("IP: {0}", player.Client.TcpEndpoint));
				}
			}
			catch (Exception e)
			{
				SpaLogicProcessor.log.Error(string.Format("IP:{1}, OnGameData is Error: {0}", e.ToString(), player.Client.TcpEndpoint));
			}
		}
	}
}
