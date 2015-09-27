using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(25, "二级密码")]
	public class PassWordTwoHandle : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			string msg = "";
			bool result = false;
			int re_Type = 0;
			bool addInfo = false;
			int Count = 0;
			string PasswordTwo = packet.ReadString();
			string PasswordTwo_new = packet.ReadString();
			int Type = packet.ReadInt();
			string PasswordQuestion = packet.ReadString();
			string PasswordAnswer = packet.ReadString();
			string PasswordQuestion2 = packet.ReadString();
			string PasswordAnswer2 = packet.ReadString();
			switch (Type)
			{
			case 1:
				re_Type = 1;
				if (string.IsNullOrEmpty(player.PlayerCharacter.PasswordTwo))
				{
					using (PlayerBussiness db = new PlayerBussiness())
					{
						if (PasswordTwo != "")
						{
							if (db.UpdatePasswordTwo(player.PlayerCharacter.ID, PasswordTwo))
							{
								player.PlayerCharacter.PasswordTwo = PasswordTwo;
								player.PlayerCharacter.IsLocked = false;
								msg = "SetPassword.success";
							}
						}
						if (PasswordQuestion != "" && PasswordAnswer != "" && PasswordQuestion2 != "" && PasswordAnswer2 != "")
						{
							if (db.UpdatePasswordInfo(player.PlayerCharacter.ID, PasswordQuestion, PasswordAnswer, PasswordQuestion2, PasswordAnswer2, 5))
							{
								result = true;
								addInfo = false;
								msg = "UpdatePasswordInfo.Success";
							}
							else
							{
								result = false;
							}
						}
						else
						{
							result = true;
							addInfo = true;
						}
					}
				}
				else
				{
					msg = "SetPassword.Fail";
					result = false;
					addInfo = false;
				}
				break;
			case 2:
				re_Type = 2;
				if (PasswordTwo == player.PlayerCharacter.PasswordTwo)
				{
					player.PlayerCharacter.IsLocked = false;
					msg = "BagUnlock.success";
					result = true;
				}
				else
				{
					msg = "PasswordTwo.error";
					result = false;
					addInfo = false;
				}
				break;
			case 3:
				re_Type = 3;
				using (PlayerBussiness db = new PlayerBussiness())
				{
					db.GetPasswordInfo(player.PlayerCharacter.ID, ref PasswordQuestion, ref PasswordAnswer, ref PasswordQuestion2, ref PasswordAnswer2, ref Count);
					Count--;
					if (Count >= 0)
					{
						db.UpdatePasswordInfo(player.PlayerCharacter.ID, PasswordQuestion, PasswordAnswer, PasswordQuestion2, PasswordAnswer2, Count);
					}
					if (PasswordTwo == player.PlayerCharacter.PasswordTwo)
					{
						if (db.UpdatePasswordTwo(player.PlayerCharacter.ID, PasswordTwo_new))
						{
							player.PlayerCharacter.IsLocked = false;
							player.PlayerCharacter.PasswordTwo = PasswordTwo_new;
							msg = "UpdatePasswordTwo.Success";
							result = true;
							addInfo = false;
						}
						else
						{
							msg = "UpdatePasswordTwo.Fail";
							result = false;
							addInfo = false;
						}
					}
					else
					{
						msg = "UpdatePasswordTwo.Fail";
						result = false;
						addInfo = false;
					}
				}
				break;
			case 4:
			{
				re_Type = 4;
				string db_PasswordAnswer = "";
				string PassWordTwo = "";
				string db_PasswordAnswer2 = "";
				using (PlayerBussiness db = new PlayerBussiness())
				{
					db.GetPasswordInfo(player.PlayerCharacter.ID, ref PasswordQuestion, ref db_PasswordAnswer, ref PasswordQuestion2, ref db_PasswordAnswer2, ref Count);
					Count--;
					if (Count >= 0)
					{
						db.UpdatePasswordInfo(player.PlayerCharacter.ID, PasswordQuestion, db_PasswordAnswer, PasswordQuestion2, db_PasswordAnswer2, Count);
					}
					if (db_PasswordAnswer == PasswordAnswer && db_PasswordAnswer2 == PasswordAnswer2 && db_PasswordAnswer != "" && db_PasswordAnswer2 != "")
					{
						if (db.UpdatePasswordTwo(player.PlayerCharacter.ID, PassWordTwo))
						{
							player.PlayerCharacter.PasswordTwo = PassWordTwo;
							player.PlayerCharacter.IsLocked = false;
							msg = "DeletePassword.success";
							result = true;
							addInfo = false;
						}
						else
						{
							msg = "DeletePassword.Fail";
							result = false;
						}
					}
					else
					{
						if (PasswordTwo == player.PlayerCharacter.PasswordTwo)
						{
							if (db.UpdatePasswordTwo(player.PlayerCharacter.ID, PassWordTwo))
							{
								player.PlayerCharacter.PasswordTwo = PassWordTwo;
								player.PlayerCharacter.IsLocked = false;
								msg = "DeletePassword.success";
								result = true;
								addInfo = false;
							}
						}
						else
						{
							msg = "DeletePassword.Fail";
							result = false;
						}
					}
				}
				break;
			}
			case 5:
				re_Type = 5;
				if (player.PlayerCharacter.PasswordTwo != null)
				{
					if (PasswordQuestion != "" && PasswordAnswer != "" && PasswordQuestion2 != "" && PasswordAnswer2 != "")
					{
						using (PlayerBussiness db = new PlayerBussiness())
						{
							if (db.UpdatePasswordInfo(player.PlayerCharacter.ID, PasswordQuestion, PasswordAnswer, PasswordQuestion2, PasswordAnswer2, 5))
							{
								result = true;
								addInfo = false;
								msg = "UpdatePasswordInfo.Success";
							}
							else
							{
								result = false;
								msg = "";
							}
						}
					}
				}
				break;
			}
			GSPacketIn pkg = packet.Clone();
			pkg.ClearContext();
			pkg.WriteInt(player.PlayerCharacter.ID);
			pkg.WriteInt(re_Type);
			pkg.WriteBoolean(result);
			pkg.WriteBoolean(addInfo);
			pkg.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
			pkg.WriteInt(Count);
			pkg.WriteString(PasswordQuestion);
			pkg.WriteString(PasswordQuestion2);
			player.Out.SendTCP(pkg);
			return 0;
		}
	}
}
