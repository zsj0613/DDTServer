using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(200, "验证码")]
	public class CheckCodeHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (string.IsNullOrEmpty(player.PlayerCharacter.CheckCode))
			{
				result = 1;
			}
			else
			{
				string check = packet.ReadString();
				if (player.PlayerCharacter.CheckCode.ToLower() == check.ToLower())
				{
					int GP = LevelMgr.GetGP(player.PlayerCharacter.Grade);
					if (player.PlayerCharacter.ChatCount != 5)
					{
						player.AddGP(LevelMgr.IncreaseGP(player.PlayerCharacter.Grade, player.PlayerCharacter.GP));
						player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CheckCodeHandler.Msg1", new object[]
						{
							player.PlayerCharacter.Grade * 12
						}));
					}
					player.PlayerCharacter.CheckCount = 0;
					player.PlayerCharacter.ChatCount = 0;
					packet.ClearContext();
					packet.WriteByte(1);
					packet.WriteBoolean(false);
					player.Out.SendTCP(packet);
				}
				else
				{
					if (player.PlayerCharacter.CheckError < 9)
					{
						player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("CheckCodeHandler.Msg3", new object[0]));
						player.PlayerCharacter.CheckError++;
						player.Out.SendCheckCode();
					}
					else
					{
						player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CheckCodeHandler.Msg3", new object[0]));
						player.Disconnect();
					}
				}
				result = 0;
			}
			return result;
		}
	}
}
