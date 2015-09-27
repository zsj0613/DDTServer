using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(235, "获取征婚信息")]
	internal class MarryInfoGetHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (player.PlayerCharacter.MarryInfoID == 0)
			{
				result = 1;
			}
			else
			{
				int id = packet.ReadInt();
				using (PlayerBussiness db = new PlayerBussiness())
				{
					MarryInfo info = db.GetMarryInfoSingle(id);
					if (info != null)
					{
						player.Out.SendMarryInfo(player, info);
						result = 0;
						return result;
					}
				}
				result = 1;
			}
			return result;
		}
	}
}
