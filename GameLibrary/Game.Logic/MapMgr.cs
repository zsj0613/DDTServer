using Bussiness;
using Game.Logic.Phy.Maps;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace Game.Logic
{
	public class MapMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static Dictionary<int, MapPoint> _maps;
		private static Dictionary<int, Map> _mapInfos;
		private static ThreadSafeRandom random = new ThreadSafeRandom();
		private static ReaderWriterLock m_lock;
		private static Dictionary<int, Queue<Map>> _mapInstancePool;
		private static Dictionary<int, List<int>> _serverMap;
		private static Dictionary<int, List<int>> simpleMapList;
		private static Dictionary<int, List<int>> normalMapList;
		private static Dictionary<int, List<int>> difficultMapList;
		private static Dictionary<int, List<int>> professionalMapList;
		public static Dictionary<int, Queue<Map>> MapInstancePool
		{
			get
			{
				return MapMgr._mapInstancePool;
			}
		}
		public static Dictionary<int, List<int>> ServerMap
		{
			get
			{
				return MapMgr._serverMap;
			}
		}
		public static Dictionary<int, List<int>> SimpleMapList
		{
			get
			{
				return MapMgr.simpleMapList;
			}
		}
		public static Dictionary<int, List<int>> NormalMapList
		{
			get
			{
				return MapMgr.normalMapList;
			}
		}
		public static Dictionary<int, List<int>> DifficultMapList
		{
			get
			{
				return MapMgr.difficultMapList;
			}
		}
		public static Dictionary<int, List<int>> ProfessionalMapList
		{
			get
			{
				return MapMgr.professionalMapList;
			}
		}
		public static int GetWeekDay
		{
			get
			{
				int day = Convert.ToInt32(DateTime.Now.DayOfWeek);
				return (day == 0) ? 7 : day;
			}
		}
		public static bool Init()
		{
			bool result;
			try
			{
				MapMgr.m_lock = new ReaderWriterLock();
				MapMgr._maps = new Dictionary<int, MapPoint>();
				MapMgr._mapInfos = new Dictionary<int, Map>();
				MapMgr._mapInstancePool = new Dictionary<int, Queue<Map>>();
				if (!MapMgr.LoadMap(MapMgr._maps, MapMgr._mapInfos))
				{
					result = false;
					return result;
				}
				MapMgr._serverMap = new Dictionary<int, List<int>>();
				MapMgr.simpleMapList = new Dictionary<int, List<int>>();
				MapMgr.normalMapList = new Dictionary<int, List<int>>();
				MapMgr.difficultMapList = new Dictionary<int, List<int>>();
				MapMgr.professionalMapList = new Dictionary<int, List<int>>();
				if (!MapMgr.InitServerMap(MapMgr._serverMap, MapMgr.simpleMapList, MapMgr.normalMapList, MapMgr.difficultMapList, MapMgr.professionalMapList))
				{
					result = false;
					return result;
				}
			}
			catch (Exception e)
			{
				if (MapMgr.log.IsErrorEnabled)
				{
					MapMgr.log.Error("MapMgr", e);
				}
				result = false;
				return result;
			}
			result = true;
			return result;
		}
		public static bool LoadMap(Dictionary<int, MapPoint> maps, Dictionary<int, Map> mapInfos)
		{
			bool result;
			try
			{
				using (MapBussiness db = new MapBussiness())
				{
					MapInfo[] query = db.GetAllMap();
					MapInfo[] array = query;
					for (int j = 0; j < array.Length; j++)
					{
						MapInfo i = array[j];
						if (!string.IsNullOrEmpty(i.PosX))
						{
							MapMgr._mapInstancePool.Add(i.ID, new Queue<Map>());
							if (!maps.Keys.Contains(i.ID))
							{
								string[] tmp = i.PosX.Split(new char[]
								{
									'|'
								});
								string[] tmp2 = i.PosX1.Split(new char[]
								{
									'|'
								});
								string[] tmp3 = i.PosX2.Split(new char[]
								{
									'|'
								});
								MapPoint pos = new MapPoint();
								string[] array2 = tmp;
								for (int k = 0; k < array2.Length; k++)
								{
									string s = array2[k];
									if (!string.IsNullOrEmpty(s.Trim()))
									{
										string[] xy = s.Split(new char[]
										{
											','
										});
										pos.PosX.Add(new Point(int.Parse(xy[0]), int.Parse(xy[1])));
									}
								}
								array2 = tmp2;
								for (int k = 0; k < array2.Length; k++)
								{
									string s = array2[k];
									if (!string.IsNullOrEmpty(s.Trim()))
									{
										string[] xy = s.Split(new char[]
										{
											','
										});
										pos.PosX1.Add(new Point(int.Parse(xy[0]), int.Parse(xy[1])));
									}
								}
								array2 = tmp3;
								for (int k = 0; k < array2.Length; k++)
								{
									string s = array2[k];
									if (!string.IsNullOrEmpty(s.Trim()))
									{
										string[] xy = s.Split(new char[]
										{
											','
										});
										pos.PosX2.Add(new Point(int.Parse(xy[0]), int.Parse(xy[1])));
									}
								}
								maps.Add(i.ID, pos);
							}
							if (!mapInfos.ContainsKey(i.ID))
							{
								Tile force = null;
								string file = string.Format("map\\{0}\\fore.map", i.ID);
								if (File.Exists(file))
								{
									force = new Tile(file, true);
								}
								Tile dead = null;
								file = string.Format("map\\{0}\\dead.map", i.ID);
								if (File.Exists(file))
								{
									dead = new Tile(file, false);
								}
								if (force == null && dead == null)
								{
									if (MapMgr.log.IsErrorEnabled)
									{
										MapMgr.log.Error("地图"+i.ID.ToString()+"不存在!");
									}
									result = false;
									return result;
								}
								mapInfos.Add(i.ID, new Map(i, force, dead));
							}
						}
					}
					if (maps.Count == 0 || mapInfos.Count == 0)
					{
						if (MapMgr.log.IsErrorEnabled)
						{
							MapMgr.log.Error(string.Concat(new object[]
							{
								"maps:",
								maps.Count,
								",mapInfos:",
								mapInfos.Count
							}));
						}
						result = false;
						return result;
					}
				}
			}
			catch (Exception e)
			{
				if (MapMgr.log.IsErrorEnabled)
				{
					MapMgr.log.Error("MapMgr", e);
				}
				result = false;
				return result;
			}
			result = true;
			return result;
		}
		public static bool ReLoadMap()
		{
			bool result;
			try
			{
				MapMgr._mapInstancePool.Clear();
				Dictionary<int, MapPoint> tempMaps = new Dictionary<int, MapPoint>();
				Dictionary<int, Map> tempMapInfos = new Dictionary<int, Map>();
				if (MapMgr.LoadMap(tempMaps, tempMapInfos))
				{
					MapMgr.m_lock.AcquireWriterLock(-1);
					try
					{
						MapMgr._maps = tempMaps;
						MapMgr._mapInfos = tempMapInfos;
						result = true;
						return result;
					}
					catch
					{
					}
					finally
					{
						MapMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception e)
			{
				if (MapMgr.log.IsErrorEnabled)
				{
					MapMgr.log.Error("ReLoadMap", e);
				}
			}
			result = false;
			return result;
		}
		public static bool InitServerMap(Dictionary<int, List<int>> _serverMap, Dictionary<int, List<int>> simpleMapList, Dictionary<int, List<int>> normalMapList, Dictionary<int, List<int>> difficultMapList, Dictionary<int, List<int>> professionalMapList)
		{
			bool result;
			using (MapBussiness db = new MapBussiness())
			{
				ServerMapInfo[] serverMapList = db.GetAllServerMap();
				try
				{
					ServerMapInfo[] array = serverMapList;
					for (int i = 0; i < array.Length; i++)
					{
						ServerMapInfo serverMap = array[i];
						if (!_serverMap.Keys.Contains(serverMap.ServerID))
						{
							string[] mapStr = serverMap.OpenMap.Split(new char[]
							{
								'|'
							});
							if (mapStr.Length < 4)
							{
								MapMgr.log.Error(serverMap.ServerID + " 号服务器新手地图加载失败,数据格式错误！");
							}
							else
							{
								string[] simpleMap = mapStr[0].Split(new char[]
								{
									','
								});
								string[] normalMap = mapStr[1].Split(new char[]
								{
									','
								});
								string[] difficultMap = mapStr[2].Split(new char[]
								{
									','
								});
								string[] professionalMap = mapStr[3].Split(new char[]
								{
									','
								});
								List<int> smaps = new List<int>();
								List<int> nmaps = new List<int>();
								List<int> dmaps = new List<int>();
								List<int> pmaps = new List<int>();
								List<int> allmaps = new List<int>();
								string[] array2 = simpleMap;
								for (int j = 0; j < array2.Length; j++)
								{
									string map = array2[j];
									if (!string.IsNullOrEmpty(map))
									{
										int mapId = int.Parse(map);
										allmaps.Add(mapId);
										smaps.Add(mapId);
									}
								}
								simpleMapList.Add(serverMap.ServerID, smaps);
								array2 = normalMap;
								for (int j = 0; j < array2.Length; j++)
								{
									string map = array2[j];
									if (!string.IsNullOrEmpty(map))
									{
										int mapId = int.Parse(map);
										allmaps.Add(mapId);
										nmaps.Add(mapId);
									}
								}
								normalMapList.Add(serverMap.ServerID, nmaps);
								array2 = difficultMap;
								for (int j = 0; j < array2.Length; j++)
								{
									string map = array2[j];
									if (!string.IsNullOrEmpty(map))
									{
										int mapId = int.Parse(map);
										allmaps.Add(mapId);
										dmaps.Add(mapId);
									}
								}
								difficultMapList.Add(serverMap.ServerID, dmaps);
								array2 = professionalMap;
								for (int j = 0; j < array2.Length; j++)
								{
									string map = array2[j];
									if (!string.IsNullOrEmpty(map))
									{
										int mapId = int.Parse(map);
										allmaps.Add(mapId);
										pmaps.Add(mapId);
									}
								}
								professionalMapList.Add(serverMap.ServerID, pmaps);
								_serverMap.Add(serverMap.ServerID, allmaps);
							}
						}
					}
				}
				catch (Exception ex)
				{
					MapMgr.log.Error(ex.ToString());
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}
		public static bool ReLoadServerMap()
		{
			bool result;
			try
			{
				Dictionary<int, List<int>> temServerMapList = new Dictionary<int, List<int>>();
				Dictionary<int, List<int>> temSimpleMapList = new Dictionary<int, List<int>>();
				Dictionary<int, List<int>> temNormalMapList = new Dictionary<int, List<int>>();
				Dictionary<int, List<int>> temDifficultMapList = new Dictionary<int, List<int>>();
				Dictionary<int, List<int>> temProfessionalMapList = new Dictionary<int, List<int>>();
				if (MapMgr.InitServerMap(temServerMapList, temSimpleMapList, temNormalMapList, temDifficultMapList, temProfessionalMapList))
				{
					MapMgr.m_lock.AcquireWriterLock(-1);
					try
					{
						MapMgr._serverMap = temServerMapList;
						MapMgr.simpleMapList = temSimpleMapList;
						MapMgr.normalMapList = temNormalMapList;
						MapMgr.difficultMapList = temDifficultMapList;
						MapMgr.professionalMapList = temProfessionalMapList;
						result = true;
						return result;
					}
					catch
					{
					}
					finally
					{
						MapMgr.m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception e)
			{
				if (MapMgr.log.IsErrorEnabled)
				{
					MapMgr.log.Error("ReLoadMapWeek", e);
				}
			}
			result = false;
			return result;
		}
		private static Map CloneMap(int index)
		{
			Map result;
			if (MapMgr._mapInfos.ContainsKey(index))
			{
				result = MapMgr._mapInfos[index].Clone();
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static MapInfo FindMapInfo(int index)
		{
			MapInfo result;
			if (MapMgr._mapInfos.ContainsKey(index))
			{
				result = MapMgr._mapInfos[index].Info;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static Map FindMap(int index)
		{
			Map result;
			if (MapMgr._mapInfos.ContainsKey(index))
			{
				result = MapMgr._mapInfos[index];
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static int GetMapIndex(int index, byte type, int serverId)
		{
			if (index != 0 && !MapMgr._maps.Keys.Contains(index))
			{
				index = 0;
			}
			int result;
			if (index == 0)
			{
				List<int> tempIndex = new List<int>();
				foreach (int id in MapMgr._serverMap[serverId])
				{
					MapInfo tempInfo = MapMgr.FindMapInfo(id);
					if ((type & tempInfo.Type) != 0)
					{
						tempIndex.Add(id);
					}
				}
				if (tempIndex.Count == 0)
				{
					int count = MapMgr._serverMap[serverId].Count;
					result = MapMgr._serverMap[serverId][MapMgr.random.Next(count)];
				}
				else
				{
					int count = tempIndex.Count;
					result = tempIndex[MapMgr.random.Next(count)];
				}
			}
			else
			{
				result = index;
			}
			return result;
		}
		public static int GetMapIndex(int level, int serverId)
		{
			Dictionary<int, List<int>> temp;
			if (level <= 10)
			{
				temp = MapMgr.simpleMapList;
			}
			else
			{
				if (level <= 15)
				{
					temp = MapMgr.normalMapList;
				}
				else
				{
					if (level <= 20)
					{
						temp = MapMgr.difficultMapList;
					}
					else
					{
						temp = MapMgr.professionalMapList;
					}
				}
			}
			int result2;
			if (temp.Keys.Contains(serverId))
			{
				List<int> result = temp[serverId];
				result2 = result[MapMgr.random.Next(0, result.Count)];
			}
			else
			{
				result2 = 1015;
			}
			return result2;
		}
		public static MapPoint GetMapRandomPos(int index)
		{
			MapPoint pos = new MapPoint();
			if (index != 0 && !MapMgr._maps.Keys.Contains(index))
			{
				index = 0;
			}
			MapPoint temp;
			if (index == 0)
			{
				int[] map = MapMgr._maps.Keys.ToArray<int>();
				temp = MapMgr._maps[map[MapMgr.random.Next(map.Length)]];
			}
			else
			{
				temp = MapMgr._maps[index];
			}
			if (MapMgr.random.Next(2) == 1)
			{
				pos.PosX.AddRange(temp.PosX);
				pos.PosX1.AddRange(temp.PosX1);
			}
			else
			{
				pos.PosX.AddRange(temp.PosX1);
				pos.PosX1.AddRange(temp.PosX);
			}
			pos.PosX2.AddRange(temp.PosX2);
			return pos;
		}
		public static MapPoint GetPVEMapRandomPos(int index)
		{
			MapPoint pos = new MapPoint();
			if (index != 0 && !MapMgr._maps.Keys.Contains(index))
			{
				index = 0;
			}
			MapPoint temp;
			if (index == 0)
			{
				int[] map = MapMgr._maps.Keys.ToArray<int>();
				temp = MapMgr._maps[map[MapMgr.random.Next(map.Length)]];
			}
			else
			{
				temp = MapMgr._maps[index];
			}
			pos.PosX.AddRange(temp.PosX);
			pos.PosX1.AddRange(temp.PosX1);
			return pos;
		}
		public static Map AllocateMapInstance(int mapId)
		{
			Map result;
			if (MapMgr._mapInstancePool.ContainsKey(mapId))
			{
				Map source = MapMgr.FindMap(mapId);
				Map map = null;
				MapMgr.m_lock.AcquireWriterLock(-1);
				try
				{
					if (MapMgr._mapInstancePool[mapId].Count > 0)
					{
						map = MapMgr._mapInstancePool[mapId].Dequeue();
					}
				}
				catch
				{
				}
				finally
				{
					MapMgr.m_lock.ReleaseWriterLock();
				}
				if (map == null)
				{
					result = source.Clone();
				}
				else
				{
					map.CopyData(source);
					result = map;
				}
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static void ReleaseMapInstance(Map map)
		{
			if (map != null && map.Info != null)
			{
				int mapId = map.Info.ID;
				map.Dispose();
				MapMgr.m_lock.AcquireWriterLock(-1);
				try
				{
					if (MapMgr._mapInstancePool.ContainsKey(mapId))
					{
						MapMgr._mapInstancePool[mapId].Enqueue(map);
					}
				}
				catch
				{
				}
				finally
				{
					MapMgr.m_lock.ReleaseWriterLock();
				}
			}
		}
		public static string ListServerMap(Dictionary<int, List<int>> mapLists)
		{
			string result;
			using (Dictionary<int, List<int>>.KeyCollection.Enumerator enumerator = mapLists.Keys.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					int key = enumerator.Current;
					string mapIds = "";
					foreach (int str in mapLists[key])
					{
						mapIds = mapIds + str + ",";
					}
					result = string.Format(" ServerID:{0}\n MapListID:{1}\n 总共:{2} 张", key, mapIds, mapLists[key].Count);
					return result;
				}
			}
			result = "No data";
			return result;
		}
	}
}
