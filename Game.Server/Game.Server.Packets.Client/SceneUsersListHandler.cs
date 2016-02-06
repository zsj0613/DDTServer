using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(69, "用户列表")]
	public class SceneUsersListHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			GSPacketIn pkg = packet.Clone();
			pkg.ClearContext();
			byte page = packet.ReadByte();
			byte count = packet.ReadByte();
			GamePlayer[] players = WorldMgr.GetAllPlayersNoGame();
			int total = players.Length;
			byte length = (total > (int)count) ? count : ((byte)total);
			pkg.WriteByte(length);
			for (int i = (int)(page * count); i < (int)(page * count + length); i++)
			{
				PlayerInfo info = players[i % total].PlayerCharacter;
				pkg.WriteInt(info.ID);
				pkg.WriteString((info.NickName == null) ? "" : info.NickName);
				pkg.WriteBoolean(info.Sex);
				pkg.WriteInt(info.Grade);
				pkg.WriteInt(info.ConsortiaID);
				pkg.WriteString((info.ConsortiaName == null) ? "" : info.ConsortiaName);
				pkg.WriteInt(info.Offer);
				pkg.WriteInt(info.Win);
				pkg.WriteInt(info.Total);
				pkg.WriteInt(info.Escape);
				pkg.WriteInt(info.Repute);
				pkg.WriteInt(info.FightPower);
			}
			player.Out.SendTCP(pkg);
			return 0;
		}
	}
}
