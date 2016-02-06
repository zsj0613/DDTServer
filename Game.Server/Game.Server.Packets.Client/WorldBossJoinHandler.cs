using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Rooms;
using Game.Logic;
using Bussiness;


namespace Game.Server.Packets.Client
{
    [PacketHandler(27, "参加世界Boss")]
    public class WorldBossJoinHandler :AbstractPlayerPacketHandler
    {
        public override int HandlePacket(GamePlayer player, GSPacketIn packet)
        {
            int a = packet.ReadInt();
            if (!WorldBossMgr.CanJoin)
            {
                player.Out.SendMessage(eMessageType.Normal, "世界Boss尚未开放");
                //player.Out.SendMessage(eMessageType.Normal, "世界Boss尚未开放，世界boss只于每天10点、14点、20点开放，并于击杀后结束");
            }
            else if (player.MainWeapon == null)
            {
                player.Out.SendMessage(eMessageType.Normal,LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip", new object[0]));
            }
            else
            {
                RoomMgr.CreateRoom(player, "WorldBoss", "", eRoomType.WorldBoss, 2);
                BaseRoom room = player.CurrentRoom;
                if (room != null)
                {
                    RoomMgr.UpdateRoomGameType(room, null, eRoomType.WorldBoss, 2, eHardLevel.Simple, 0, 200, false);
                    RoomMgr.StartGame(room);
                }
            }
            return 0;
        }
    }
}
