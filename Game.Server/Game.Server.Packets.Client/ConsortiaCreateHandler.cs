using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Text;

namespace Game.Server.Packets.Client
{
	[PacketHandler(130, "创建公会")]
	public class ConsortiaCreateHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result2;
			if (player.PlayerCharacter.ConsortiaID != 0)
			{
				result2 = 0;
			}
			else
			{
				ConsortiaLevelInfo levelInfo = ConsortiaLevelMgr.FindConsortiaLevelInfo(1);
				string name = packet.ReadString();
				if (string.IsNullOrEmpty(name) || Encoding.Default.GetByteCount(name) > 12)
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaCreateHandler.Long", new object[0]));
					result2 = 1;
				}
				else
				{
					bool result = false;
					int id = 0;
					//int mustGold = levelInfo.NeedGold;
					//int mustLevel = 5;
					string msg = "ConsortiaCreateHandler.Failed";
					ConsortiaDutyInfo dutyInfo = new ConsortiaDutyInfo();
					if (!string.IsNullOrEmpty(name) && player.PlayerCharacter.Money >= 1000)
					{
						using (ConsortiaBussiness db = new ConsortiaBussiness())
						{
							ConsortiaInfo info = new ConsortiaInfo();
							info.BuildDate = DateTime.Now;
							info.CelebCount = 0;
							info.ChairmanID = player.PlayerCharacter.ID;
							info.ChairmanName = player.PlayerCharacter.NickName;
							info.ConsortiaName = name;
							info.CreatorID = info.ChairmanID;
							info.CreatorName = info.ChairmanName;
							info.Description = "";
							info.Honor = 0;
							info.IP = "";
							info.IsExist = true;
							info.Level = levelInfo.Level;
							info.MaxCount = levelInfo.Count;
							info.Riches = levelInfo.Riches;
							info.Placard = "";
							info.Port = 0;
							info.Repute = 0;
							info.Count = 1;
							if (db.AddConsortia(info, ref msg, ref dutyInfo))
							{
								player.PlayerCharacter.ConsortiaID = info.ConsortiaID;
								player.PlayerCharacter.ConsortiaName = info.ConsortiaName;
								player.PlayerCharacter.DutyLevel = dutyInfo.Level;
								player.PlayerCharacter.DutyName = dutyInfo.DutyName;
								player.PlayerCharacter.Right = dutyInfo.Right;
								player.PlayerCharacter.ConsortiaLevel = levelInfo.Level;
								player.RemoveGold(1000);
								msg = "ConsortiaCreateHandler.Success";
								result = true;
								id = info.ConsortiaID;
								GameServer.Instance.LoginServer.SendConsortiaCreate(id, player.PlayerCharacter.Offer, info.ChairmanName);
							}
						}
					}
					packet.WriteBoolean(result);
					packet.WriteInt(id);
                    packet.WriteString("cName");//UnKnown
					packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
					packet.WriteInt(dutyInfo.Level);
					packet.WriteString((dutyInfo.DutyName == null) ? "" : dutyInfo.DutyName);
					packet.WriteInt(dutyInfo.Right);
					player.Out.SendTCP(packet);
					result2 = 0;
				}
			}
			return result2;
		}
	}
}
