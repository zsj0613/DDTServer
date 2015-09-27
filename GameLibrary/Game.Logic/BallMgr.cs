using Bussiness;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public class BallMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, BallInfo> m_infos;
		private static Dictionary<int, Tile> m_tiles;
		public static bool Init()
		{
			return BallMgr.ReLoad();
		}
		public static bool ReLoad()
		{
			bool result;
			try
			{
				Dictionary<int, BallInfo> tempBalls = BallMgr.LoadFromDatabase();
				Dictionary<int, Tile> tempBallTile = BallMgr.LoadFromFiles(tempBalls);
				if (tempBalls.Values.Count > 0 && tempBallTile.Values.Count > 0)
				{
					Interlocked.Exchange<Dictionary<int, BallInfo>>(ref BallMgr.m_infos, tempBalls);
					Interlocked.Exchange<Dictionary<int, Tile>>(ref BallMgr.m_tiles, tempBallTile);
					result = true;
					return result;
				}
			}
			catch (Exception ex)
			{
				BallMgr.log.Error("Ball Mgr init error:", ex);
			}
			result = false;
			return result;
		}
		private static Dictionary<int, BallInfo> LoadFromDatabase()
		{
			Dictionary<int, BallInfo> list = new Dictionary<int, BallInfo>();
			using (ProduceBussiness db = new ProduceBussiness())
			{
				BallInfo[] ballInfos = db.GetAllBall();
				BallInfo[] array = ballInfos;
				for (int i = 0; i < array.Length; i++)
				{
					BallInfo b = array[i];
					if (!list.ContainsKey(b.ID))
					{
						list.Add(b.ID, b);
					}
				}
			}
			return list;
		}
		private static Dictionary<int, Tile> LoadFromFiles(Dictionary<int, BallInfo> list)
		{
			Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();
			foreach (BallInfo info in list.Values)
			{
				if (info.HasTunnel)
				{
					string file = string.Format("bomb\\{0}.bomb", info.ID);
					Tile shape = null;
					if (File.Exists(file))
					{
						shape = new Tile(file, false);
					}
					tiles.Add(info.ID, shape);
					if (shape == null && info.ID != 1 && info.ID != 2 && info.ID != 3)
					{
						BallMgr.log.ErrorFormat("can't find bomb file:{0}", file);
					}
				}
			}
			return tiles;
		}
		public static bool IsExist(int id)
		{
			return BallMgr.m_infos.ContainsKey(id);
		}
		public static BallInfo FindBall(int id)
		{
			BallInfo result;
			if (BallMgr.m_infos.ContainsKey(id))
			{
				result = BallMgr.m_infos[id];
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static Tile FindTile(int id)
		{
			Tile result;
			if (BallMgr.m_tiles.ContainsKey(id))
			{
				result = BallMgr.m_tiles[id];
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static BombType GetBallType(int ballId)
		{
			BombType result;
			switch (ballId)
			{
			case 1:
				result = BombType.FORZEN;
				return result;
			case 2:
				break;
			case 3:
				result = BombType.TRANFORM;
				return result;
			default:
				if (ballId == 64)
				{
					result = BombType.CURE;
					return result;
				}
				break;
			}
			result = BombType.Normal;
			return result;
		}
	}
}
