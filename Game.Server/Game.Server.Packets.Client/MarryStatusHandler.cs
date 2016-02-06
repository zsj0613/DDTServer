using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(246, "请求结婚状态")]
	internal class MarryStatusHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int UserID = packet.ReadInt();
			GamePlayer spouse = WorldMgr.GetPlayerById(UserID);
			if (spouse != null)
			{
				player.Out.SendPlayerMarryStatus(player, spouse.PlayerCharacter.ID, spouse.PlayerCharacter.IsMarried);
			}
			else
			{
				using (PlayerBussiness db = new PlayerBussiness())
				{
					PlayerInfo tempSpouse = db.GetUserSingleByUserID(UserID);
					player.Out.SendPlayerMarryStatus(player, tempSpouse.ID, tempSpouse.IsMarried);
				}
			}
			return 0;
		}
	}
}
