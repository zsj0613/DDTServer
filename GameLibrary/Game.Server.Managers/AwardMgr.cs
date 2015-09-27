using Bussiness.Managers;
using Game.Logic.LogEnum;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using Game.Language;
using Bussiness;
namespace Game.Server.Managers
{
	public class AwardMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, DailyAwardInfo> _dailyAward;
		private static bool _dailyAwardState;
		private static ReaderWriterLock m_lock;
		public static bool DailyAwardState
		{
			get
			{
				return AwardMgr._dailyAwardState;
			}
			set
			{
				AwardMgr._dailyAwardState = value;
			}
		}
		public static bool ReLoad()
		{
			bool result;
			try
			{
				Dictionary<int, DailyAwardInfo> tempDaily = new Dictionary<int, DailyAwardInfo>();
				if (AwardMgr.LoadDailyAward(tempDaily))
				{
					AwardMgr.m_lock.AcquireWriterLock(-1);
					try
					{
						AwardMgr._dailyAward = tempDaily;
						result = true;
						return result;
					}
					catch
					{
					}
					finally
					{
						AwardMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception e)
			{
				if (AwardMgr.log.IsErrorEnabled)
				{
					AwardMgr.log.Error("AwardMgr", e);
				}
			}
			result = false;
			return result;
		}
		public static bool Init()
		{
			bool result;
			try
			{
				AwardMgr.m_lock = new ReaderWriterLock();
				AwardMgr._dailyAward = new Dictionary<int, DailyAwardInfo>();
				AwardMgr._dailyAwardState = false;
				result = AwardMgr.LoadDailyAward(AwardMgr._dailyAward);
			}
			catch (Exception e)
			{
				if (AwardMgr.log.IsErrorEnabled)
				{
					AwardMgr.log.Error("AwardMgr", e);
				}
				result = false;
			}
			return result;
		}
		private static bool LoadDailyAward(Dictionary<int, DailyAwardInfo> awards)
		{
			using (ProduceBussiness db = new ProduceBussiness())
			{
				DailyAwardInfo[] infos = db.GetAllDailyAward();
				DailyAwardInfo[] array = infos;
				for (int i = 0; i < array.Length; i++)
				{
					DailyAwardInfo info = array[i];
					if (!awards.ContainsKey(info.ID))
					{
						awards.Add(info.ID, info);
					}
				}
			}
			return true;
		}
		public static DailyAwardInfo[] GetAwardInfo(int getWay)
		{
			List<DailyAwardInfo> infos = new List<DailyAwardInfo>();
			AwardMgr.m_lock.AcquireReaderLock(-1);
			try
			{
				foreach (DailyAwardInfo info in AwardMgr._dailyAward.Values)
				{
					if (info.GetWay == getWay)
					{
						infos.Add(info);
					}
				}
			}
			catch
			{
			}
			finally
			{
				AwardMgr.m_lock.ReleaseReaderLock();
			}
			return infos.ToArray();
		}
		public static bool AddLoginAward(GamePlayer player)
		{
			bool result;
			if (DateTime.Now.Date == player.PlayerCharacter.LastAward.Date)
			{
				result = false;
			}
			else
			{
				player.PlayerCharacter.LastAward = DateTime.Now;
				result = AwardMgr.AddDailyAward(player, 0);
			}
			return result;
		}
		public static bool AddAuncherAward(GamePlayer player)
		{
			bool result;
			if (DateTime.Now.Date == player.PlayerCharacter.LastAuncherAward.Date || player.ClientType != eClientType.Auncher)
			{
				result = false;
			}
			else
			{
				player.PlayerCharacter.LastAuncherAward = DateTime.Now;
				result = AwardMgr.AddDailyAward(player, 1);
			}
			return result;
		}
		private static bool AddDailyAward(GamePlayer player, int getWay)
		{
            ItemInfo item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(100 + player.VIPLevel), 1, 0);
            item.IsBinds = true;
            if (player.AddItem(item))
            {
                player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("GameUserDailyAward.Success", new object[0]));
            }
            else
            {
                using (PlayerBussiness db = new PlayerBussiness())
                {
                    item.UserID = 0;
                    db.AddGoods(item);
                    MailInfo message = new MailInfo();
                    message.Annex1 = item.ItemID.ToString();
                    message.Content = Language.LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Content", new object[]
                    {
                                   item.Template.Name
                    });
                    message.Gold = 0;
                    message.Money = 0;
                    message.GiftToken = 0;
                    message.Receiver = player.PlayerCharacter.NickName;
                    message.ReceiverID = player.PlayerCharacter.ID;
                    message.Sender = message.Receiver;
                    message.SenderID = message.ReceiverID;
                    message.Title = Language.LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Title", new object[]
                    {
                                            item.Template.Name
                    });
                    message.Type = 15;
                    db.SendMail(message);
                    string full = Language.LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Mail", new object[0]);
                    player.Out.SendMessage(eMessageType.Normal, full);
                    player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
                }
                
            }

			return true;
		}
		private static eGetWay GetGetWay(int getWay)
		{
			eGetWay result;
			switch (getWay)
			{
			case 0:
				result = eGetWay.WEB;
				break;
			case 1:
				result = eGetWay.AUNCHER;
				break;
			default:
				result = eGetWay.WEB;
				break;
			}
			return result;
		}
	}
}
