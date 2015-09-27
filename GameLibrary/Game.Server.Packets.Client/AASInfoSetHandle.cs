using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using System;
using System.Text.RegularExpressions;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(224, "设置防沉迷系统信息")]
	internal class AASInfoSetHandle : AbstractPlayerPacketHandler
	{
		private static Regex _objRegex1 = new Regex("/^[1-9]\\d{7}((0\\d)|(1[0-2]))(([0|1|2]\\d)|3[0-1])\\d{3}$/");
		private static Regex _objRegex2 = new Regex("/^[1-9]\\d{5}[1-9]\\d{3}((0\\d)|(1[0-2]))(([0|1|2]\\d)|3[0-1])\\d{4}$/");
		private static Regex _objRegex = new Regex("\\d{18}|\\d{15}");
		private static string[] cities = new string[]
		{
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			"北京",
			"天津",
			"河北",
			"山西",
			"内蒙古",
			null,
			null,
			null,
			null,
			null,
			"辽宁",
			"吉林",
			"黑龙江",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			"上海",
			"江苏",
			"浙江",
			"安微",
			"福建",
			"江西",
			"山东",
			null,
			null,
			null,
			"河南",
			"湖北",
			"湖南",
			"广东",
			"广西",
			"海南",
			null,
			null,
			null,
			"重庆",
			"四川",
			"贵州",
			"云南",
			"西藏",
			null,
			null,
			null,
			null,
			null,
			null,
			"陕西",
			"甘肃",
			"青海",
			"宁夏",
			"新疆",
			null,
			null,
			null,
			null,
			null,
			"台湾",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			"香港",
			"澳门",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			"国外"
		};
		private static int[] WI = new int[]
		{
			7,
			9,
			10,
			5,
			8,
			4,
			2,
			1,
			6,
			3,
			7,
			9,
			10,
			5,
			8,
			4,
			2
		};
		private static char[] checkCode = new char[]
		{
			'1',
			'0',
			'X',
			'9',
			'8',
			'7',
			'6',
			'5',
			'4',
			'3',
			'2'
		};
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			AASInfo info = new AASInfo();
			info.UserID = player.PlayerCharacter.ID;
			bool rlt = false;
			bool isclosed = packet.ReadBoolean();
			bool result;
			if (isclosed)
			{
				info.Name = "";
				info.IDNumber = "";
				info.State = 0;
				result = true;
			}
			else
			{
				info.Name = packet.ReadString();
				info.IDNumber = packet.ReadString();
				result = this.CheckIDNumber(info.IDNumber);
				if (info.IDNumber != "")
				{
					player.IsAASInfo = true;
					int Age = Convert.ToInt32(info.IDNumber.Substring(6, 4));
					int month = Convert.ToInt32(info.IDNumber.Substring(10, 2));
					if (DateTime.Now.Year.CompareTo(Age + 18) > 0 || (DateTime.Now.Year.CompareTo(Age + 18) == 0 && DateTime.Now.Month.CompareTo(month) >= 0))
					{
						player.IsMinor = false;
					}
				}
				if (info.Name != "" && result)
				{
					info.State = 1;
				}
				else
				{
					info.State = 0;
				}
			}
			if (result)
			{
				player.Out.SendAASState(false);
				player.Out.SendAASControl(false, player.IsAASInfo, player.IsMinor);
				using (ProduceBussiness db = new ProduceBussiness())
				{
					rlt = db.AddAASInfo(info);
				}
			}
			else
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("AASInfoSetHandle.error", new object[0]));
			}
			if (rlt && info.State == 1)
			{
				ItemTemplateInfo rewardItem = ItemMgr.FindItemTemplate(11019);
				if (rewardItem != null)
				{
					ItemInfo item = ItemInfo.CreateFromTemplate(rewardItem, 1, 107);
					if (item != null)
					{
						item.IsBinds = true;
						AbstractInventory bg = player.GetItemInventory(item.Template);
						if (bg.AddItem(item))
						{
							player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ASSInfoSetHandle.Success", new object[]
							{
								item.Template.Name
							}));
						}
						else
						{
							player.SendItemToMail(item, LanguageMgr.GetTranslation("ASSInfoSetHandle.Content", new object[]
							{
								item.Template.Name
							}), LanguageMgr.GetTranslation("ASSInfoSetHandle.Title", new object[]
							{
								item.Template.Name
							}), eMailType.Common);
							player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
							player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("ASSInfoSetHandle.NoPlace", new object[0]));
						}
					}
				}
			}
			return 0;
		}
		private bool CheckIDNumber(string IDNum)
		{
			bool result = false;
			bool result2;
			if (!AASInfoSetHandle._objRegex.IsMatch(IDNum))
			{
				result2 = false;
			}
			else
			{
				int province = int.Parse(IDNum.Substring(0, 2));
				if (AASInfoSetHandle.cities[province] == null)
				{
					result2 = false;
				}
				else
				{
					if (IDNum.Length == 18)
					{
						int sum = 0;
						for (int i = 0; i < 17; i++)
						{
							sum += int.Parse(IDNum[i].ToString()) * AASInfoSetHandle.WI[i];
						}
						int y = sum % 11;
						if (IDNum[17] == AASInfoSetHandle.checkCode[y])
						{
							result = true;
						}
					}
					result2 = result;
				}
			}
			return result2;
		}
	}
}
