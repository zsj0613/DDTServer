using Bussiness;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
namespace Game.Server.Managers
{
	public class ConsortiaMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<string, int> _ally;
		private static ReaderWriterLock m_lock;
		private static Dictionary<int, ConsortiaInfo> _consortia;
		public static bool ReLoad()
		{
			bool result;
			try
			{
				Dictionary<string, int> tempAlly = new Dictionary<string, int>();
				Dictionary<int, ConsortiaInfo> tempConsortia = new Dictionary<int, ConsortiaInfo>();
				if (ConsortiaMgr.Load(tempAlly) && ConsortiaMgr.LoadConsortia(tempConsortia))
				{
					ConsortiaMgr.m_lock.AcquireWriterLock(-1);
					try
					{
						ConsortiaMgr._ally = tempAlly;
						ConsortiaMgr._consortia = tempConsortia;
						result = true;
						return result;
					}
					catch
					{
					}
					finally
					{
						ConsortiaMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception e)
			{
				if (ConsortiaMgr.log.IsErrorEnabled)
				{
					ConsortiaMgr.log.Error("ConsortiaMgr", e);
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
				ConsortiaMgr.m_lock = new ReaderWriterLock();
				ConsortiaMgr._ally = new Dictionary<string, int>();
				if (!ConsortiaMgr.Load(ConsortiaMgr._ally))
				{
					result = false;
				}
				else
				{
					ConsortiaMgr._consortia = new Dictionary<int, ConsortiaInfo>();
					if (!ConsortiaMgr.LoadConsortia(ConsortiaMgr._consortia))
					{
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
			catch (Exception e)
			{
				if (ConsortiaMgr.log.IsErrorEnabled)
				{
					ConsortiaMgr.log.Error("ConsortiaMgr", e);
				}
				result = false;
			}
			return result;
		}
		private static bool Load(Dictionary<string, int> ally)
		{
			using (ConsortiaBussiness db = new ConsortiaBussiness())
			{
				ConsortiaAllyInfo[] infos = db.GetConsortiaAllyAll();
				ConsortiaAllyInfo[] array = infos;
				for (int i = 0; i < array.Length; i++)
				{
					ConsortiaAllyInfo info = array[i];
					if (info.IsExist)
					{
						string key;
						if (info.Consortia1ID < info.Consortia2ID)
						{
							key = info.Consortia1ID + "&" + info.Consortia2ID;
						}
						else
						{
							key = info.Consortia2ID + "&" + info.Consortia1ID;
						}
						if (!ally.ContainsKey(key))
						{
							ally.Add(key, info.State);
						}
					}
				}
			}
			return true;
		}
		private static bool LoadConsortia(Dictionary<int, ConsortiaInfo> consortia)
		{
			using (ConsortiaBussiness db = new ConsortiaBussiness())
			{
				ConsortiaInfo[] infos = db.GetConsortiaAll();
				ConsortiaInfo[] array = infos;
				for (int i = 0; i < array.Length; i++)
				{
					ConsortiaInfo info = array[i];
					if (info.IsExist)
					{
						if (!consortia.ContainsKey(info.ConsortiaID))
						{
							consortia.Add(info.ConsortiaID, info);
						}
					}
				}
			}
			return true;
		}
		public static int UpdateConsortiaAlly(int cosortiaID1, int consortiaID2, int state)
		{
			string key;
			if (cosortiaID1 < consortiaID2)
			{
				key = cosortiaID1 + "&" + consortiaID2;
			}
			else
			{
				key = consortiaID2 + "&" + cosortiaID1;
			}
			ConsortiaMgr.m_lock.AcquireWriterLock(-1);
			try
			{
				if (!ConsortiaMgr._ally.ContainsKey(key))
				{
					ConsortiaMgr._ally.Add(key, state);
				}
				else
				{
					ConsortiaMgr._ally[key] = state;
				}
			}
			catch
			{
			}
			finally
			{
				ConsortiaMgr.m_lock.ReleaseWriterLock();
			}
			return 0;
		}
		public static bool ConsortiaUpGrade(int consortiaID, int consortiaLevel)
		{
			bool result = false;
			ConsortiaMgr.m_lock.AcquireWriterLock(-1);
			try
			{
				if (ConsortiaMgr._consortia.ContainsKey(consortiaID) && ConsortiaMgr._consortia[consortiaID].IsExist)
				{
					ConsortiaMgr._consortia[consortiaID].Level = consortiaLevel;
				}
				else
				{
					ConsortiaInfo info = new ConsortiaInfo();
					info.BuildDate = DateTime.Now;
					info.Level = consortiaLevel;
					info.IsExist = true;
					ConsortiaMgr._consortia.Add(consortiaID, info);
				}
			}
			catch (Exception ex)
			{
				ConsortiaMgr.log.Error("ConsortiaUpGrade", ex);
			}
			finally
			{
				ConsortiaMgr.m_lock.ReleaseWriterLock();
			}
			return result;
		}
		public static bool ConsortiaStoreUpGrade(int consortiaID, int storeLevel)
		{
			bool result = false;
			ConsortiaMgr.m_lock.AcquireWriterLock(-1);
			try
			{
				if (ConsortiaMgr._consortia.ContainsKey(consortiaID) && ConsortiaMgr._consortia[consortiaID].IsExist)
				{
					ConsortiaMgr._consortia[consortiaID].StoreLevel = storeLevel;
				}
			}
			catch (Exception ex)
			{
				ConsortiaMgr.log.Error("ConsortiaUpGrade", ex);
			}
			finally
			{
				ConsortiaMgr.m_lock.ReleaseWriterLock();
			}
			return result;
		}
		public static bool ConsortiaShopUpGrade(int consortiaID, int shopLevel)
		{
			bool result = false;
			ConsortiaMgr.m_lock.AcquireWriterLock(-1);
			try
			{
				if (ConsortiaMgr._consortia.ContainsKey(consortiaID) && ConsortiaMgr._consortia[consortiaID].IsExist)
				{
					ConsortiaMgr._consortia[consortiaID].ShopLevel = shopLevel;
				}
			}
			catch (Exception ex)
			{
				ConsortiaMgr.log.Error("ConsortiaUpGrade", ex);
			}
			finally
			{
				ConsortiaMgr.m_lock.ReleaseWriterLock();
			}
			return result;
		}
		public static bool ConsortiaSmithUpGrade(int consortiaID, int smithLevel)
		{
			bool result = false;
			ConsortiaMgr.m_lock.AcquireWriterLock(-1);
			try
			{
				if (ConsortiaMgr._consortia.ContainsKey(consortiaID) && ConsortiaMgr._consortia[consortiaID].IsExist)
				{
					ConsortiaMgr._consortia[consortiaID].SmithLevel = smithLevel;
				}
			}
			catch (Exception ex)
			{
				ConsortiaMgr.log.Error("ConsortiaUpGrade", ex);
			}
			finally
			{
				ConsortiaMgr.m_lock.ReleaseWriterLock();
			}
			return result;
		}
		public static bool AddConsortia(int consortiaID, string chairmanName)
		{
			bool result = false;
			ConsortiaMgr.m_lock.AcquireWriterLock(-1);
			try
			{
				if (!ConsortiaMgr._consortia.ContainsKey(consortiaID))
				{
					ConsortiaInfo info = new ConsortiaInfo();
					info.BuildDate = DateTime.Now;
					info.Level = 1;
					info.IsExist = true;
					info.ChairmanName = chairmanName;
					info.ConsortiaID = consortiaID;
					ConsortiaMgr._consortia.Add(consortiaID, info);
				}
			}
			catch (Exception ex)
			{
				ConsortiaMgr.log.Error("ConsortiaUpGrade", ex);
			}
			finally
			{
				ConsortiaMgr.m_lock.ReleaseWriterLock();
			}
			return result;
		}
		public static ConsortiaInfo FindConsortiaInfo(int consortiaID)
		{
			ConsortiaMgr.m_lock.AcquireReaderLock(-1);
			ConsortiaInfo result;
			try
			{
				if (ConsortiaMgr._consortia.ContainsKey(consortiaID))
				{
					result = ConsortiaMgr._consortia[consortiaID];
					return result;
				}
			}
			catch
			{
			}
			finally
			{
				ConsortiaMgr.m_lock.ReleaseReaderLock();
			}
			result = null;
			return result;
		}
		public static int CanConsortiaFight(int consortiaID1, int consortiaID2)
		{
			int result;
			if (consortiaID1 == 0 || consortiaID2 == 0 || consortiaID1 == consortiaID2)
			{
				result = -1;
			}
			else
			{
				ConsortiaInfo consortia = ConsortiaMgr.FindConsortiaInfo(consortiaID1);
				ConsortiaInfo consortia2 = ConsortiaMgr.FindConsortiaInfo(consortiaID2);
				if (consortia == null || consortia2 == null || consortia.Level < 3 || consortia2.Level < 3)
				{
					result = -1;
				}
				else
				{
					result = ConsortiaMgr.FindConsortiaAlly(consortiaID1, consortiaID2);
				}
			}
			return result;
		}
		public static int FindConsortiaAlly(int cosortiaID1, int consortiaID2)
		{
			int result;
			if (cosortiaID1 == 0 || consortiaID2 == 0 || cosortiaID1 == consortiaID2)
			{
				result = -1;
			}
			else
			{
				string key;
				if (cosortiaID1 < consortiaID2)
				{
					key = cosortiaID1 + "&" + consortiaID2;
				}
				else
				{
					key = consortiaID2 + "&" + cosortiaID1;
				}
				ConsortiaMgr.m_lock.AcquireReaderLock(-1);
				try
				{
					if (ConsortiaMgr._ally.ContainsKey(key))
					{
						result = ConsortiaMgr._ally[key];
						return result;
					}
				}
				catch
				{
				}
				finally
				{
					ConsortiaMgr.m_lock.ReleaseReaderLock();
				}
				result = 0;
			}
			return result;
		}
		public static int GetOffer(int cosortiaID1, int consortiaID2, eGameType gameType)
		{
			return ConsortiaMgr.GetOffer(ConsortiaMgr.FindConsortiaAlly(cosortiaID1, consortiaID2), gameType);
		}
		private static int GetOffer(int state, eGameType gameType)
		{
			int result;
			switch (gameType)
			{
			case eGameType.Free:
				switch (state)
				{
				case 0:
					result = 1;
					return result;
				case 1:
					result = 0;
					return result;
				case 2:
					result = 3;
					return result;
				}
				break;
			case eGameType.Guild:
				switch (state)
				{
				case 0:
					result = 5;
					return result;
				case 1:
					result = 0;
					return result;
				case 2:
					result = 10;
					return result;
				}
				break;
			}
			result = 0;
			return result;
		}
		public static int KillPlayer(GamePlayer win, GamePlayer lose, Dictionary<GamePlayer, Player> players, eRoomType roomType, eGameType gameClass)
		{
			int result;
			if (roomType != eRoomType.Match)
			{
				result = -1;
			}
			else
			{
				int state = ConsortiaMgr.FindConsortiaAlly(win.PlayerCharacter.ConsortiaID, lose.PlayerCharacter.ConsortiaID);
				if (state == -1)
				{
					result = state;
				}
				else
				{
					int offer = ConsortiaMgr.GetOffer(state, gameClass);
					if (lose.PlayerCharacter.Offer < offer)
					{
						offer = lose.PlayerCharacter.Offer;
					}
					if (offer != 0)
					{
						players[win].GainOffer = offer;
						players[lose].GainOffer = -offer;
					}
					result = state;
				}
			}
			return result;
		}
		public static int ConsortiaFight(GamePlayer player, int consortiaWin, int consortiaLose, eRoomType roomType, eGameType gameClass, int totalKillHealth, int playercount)
		{
			int result;
			if (roomType != eRoomType.Match)
			{
				result = 0;
			}
			else
			{
				int playerCount = playercount / 2;
				int riches = 0;
				int state = 2;
				float richesRate = player.GMRichesRate * player.AuncherRichesRate;
				using (ConsortiaBussiness db = new ConsortiaBussiness())
				{
					if (gameClass == eGameType.Free)
					{
						playerCount = 0;
					}
					else
					{
						db.ConsortiaFight(consortiaWin, consortiaLose, playerCount, out riches, state, totalKillHealth, richesRate);
					}
				}
				if (riches > 10000)
				{
					ConsortiaMgr.log.Error(string.Format("player.nickname : {0}, gameClass : {1}, GMRichesRate : {2}, auncherRichesRate : {3}, consortiaWin : {4}, consortiaLose : {5}, playerCount : {6}, totalKillHealth : {7}", new object[]
					{
						player.PlayerCharacter.NickName,
						gameClass,
						player.GMRichesRate,
						player.AuncherRichesRate,
						consortiaWin,
						consortiaLose,
						playerCount,
						totalKillHealth
					}));
				}
				result = riches;
			}
			return result;
		}
		public static void ConsortiaChangChairman(int consortiaID, string NewChairmanName)
		{
			ConsortiaMgr.m_lock.AcquireReaderLock(-1);
			try
			{
				if (ConsortiaMgr._consortia.ContainsKey(consortiaID))
				{
					ConsortiaMgr._consortia[consortiaID].ChairmanName = NewChairmanName;
				}
			}
			catch
			{
			}
			finally
			{
				ConsortiaMgr.m_lock.ReleaseReaderLock();
			}
		}
	}
}
