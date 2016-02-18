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

    [PacketHandler((int)ePackageType.WORLDBOSS_JOIN, "参加世界Boss")]
    public class WorldBossJoinHandler :AbstractPlayerPacketHandler
    {
        public override int HandlePacket(GamePlayer player, GSPacketIn packet)
        {
            int a = packet.ReadInt();
            //just for debug
           // if (false)
            if (!WorldBossMgr.CanJoin)
            {

                player.Out.SendMessage(eMessageType.Normal, "世界Boss尚未开放");
               // player.Out.SendMessage(eMessageType.Normal, "世界Boss尚未开放，世界boss只于每天10点、14点、20点开放，并于击杀后结束");
            }
            else
            {
                WorldBossMgr.AddPlayer(player);
                player.Out.SendEnterWorldBossRoom(player);
            }
            return 0;
        }
    }
}
