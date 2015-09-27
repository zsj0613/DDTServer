using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Bussiness.Managers
{
	public class QuestMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, QuestInfo> m_questinfo = new Dictionary<int, QuestInfo>();
		private static Dictionary<int, List<QuestConditionInfo>> m_questcondiction = new Dictionary<int, List<QuestConditionInfo>>();
		private static Dictionary<int, List<QuestAwardInfo>> m_questgoods = new Dictionary<int, List<QuestAwardInfo>>();
		public static bool Init()
		{
			return QuestMgr.ReLoad();
		}
		public static bool ReLoad()
		{
			bool result;
			try
			{
				Dictionary<int, QuestInfo> tempQuestInfo = QuestMgr.LoadQuestInfoDb();
				Dictionary<int, List<QuestConditionInfo>> tempQuestCondiction = QuestMgr.LoadQuestCondictionDb(tempQuestInfo);
				Dictionary<int, List<QuestAwardInfo>> tempQuestGoods = QuestMgr.LoadQuestGoodDb(tempQuestInfo);
				if (tempQuestInfo.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, QuestInfo>>(ref QuestMgr.m_questinfo, tempQuestInfo);
					Interlocked.Exchange<Dictionary<int, List<QuestConditionInfo>>>(ref QuestMgr.m_questcondiction, tempQuestCondiction);
					Interlocked.Exchange<Dictionary<int, List<QuestAwardInfo>>>(ref QuestMgr.m_questgoods, tempQuestGoods);
				}
				result = true;
				return result;
			}
			catch (Exception e)
			{
				QuestMgr.log.Error("QuestMgr", e);
			}
			result = false;
			return result;
		}
		public static Dictionary<int, QuestInfo> LoadQuestInfoDb()
		{
			Dictionary<int, QuestInfo> list = new Dictionary<int, QuestInfo>();
			using (ProduceBussiness db = new ProduceBussiness())
			{
				QuestInfo[] infos = db.GetALlQuest();
				QuestInfo[] array = infos;
				for (int i = 0; i < array.Length; i++)
				{
					QuestInfo info = array[i];
					if (!list.ContainsKey(info.ID))
					{
						list.Add(info.ID, info);
					}
				}
			}
			return list;
		}
		public static Dictionary<int, List<QuestConditionInfo>> LoadQuestCondictionDb(Dictionary<int, QuestInfo> quests)
		{
			Dictionary<int, List<QuestConditionInfo>> list = new Dictionary<int, List<QuestConditionInfo>>();
			using (ProduceBussiness db = new ProduceBussiness())
			{
				QuestConditionInfo[] infos = db.GetAllQuestCondiction();
				foreach (QuestInfo quest in quests.Values)
				{
					IEnumerable<QuestConditionInfo> temp = 
						from s in infos
						where s.QuestID == quest.ID
						select s;
					list.Add(quest.ID, temp.ToList<QuestConditionInfo>());
				}
			}
			return list;
		}
		public static Dictionary<int, List<QuestAwardInfo>> LoadQuestGoodDb(Dictionary<int, QuestInfo> quests)
		{
			Dictionary<int, List<QuestAwardInfo>> list = new Dictionary<int, List<QuestAwardInfo>>();
			using (ProduceBussiness db = new ProduceBussiness())
			{
				QuestAwardInfo[] infos = db.GetAllQuestGoods();
				foreach (QuestInfo quest in quests.Values)
				{
					IEnumerable<QuestAwardInfo> temp = 
						from s in infos
						where s.QuestID == quest.ID
						select s;
					list.Add(quest.ID, temp.ToList<QuestAwardInfo>());
				}
			}
			return list;
		}
		public static QuestInfo GetSingleQuest(int id)
		{
			QuestInfo result;
			if (QuestMgr.m_questinfo.ContainsKey(id))
			{
				result = QuestMgr.m_questinfo[id];
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static List<QuestAwardInfo> GetQuestGoods(QuestInfo info)
		{
			List<QuestAwardInfo> result;
			if (QuestMgr.m_questgoods.ContainsKey(info.ID))
			{
				List<QuestAwardInfo> items = QuestMgr.m_questgoods[info.ID];
				result = items;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static List<QuestConditionInfo> GetQuestCondiction(QuestInfo info)
		{
			List<QuestConditionInfo> result;
			if (QuestMgr.m_questcondiction.ContainsKey(info.ID))
			{
				List<QuestConditionInfo> items = QuestMgr.m_questcondiction[info.ID];
				result = items;
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
