using Game.Base;
using Game.Logic;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;
namespace Game.Server.Commands.Admin
{
	[Cmd("&map", ePrivLevel.Player, "   地图信息相关查询.", new string[]
	{
		"       /map -list            : 列举所有 map 对象池信息 ",
		"       /map -new             : 列举所有 新手玩家 使用地图",
		"       /map -old             : 列举所有 老手玩家 使用地图",
		"       /map -mapid [mapid]   : 根据 mapid 查询对应的对象池信息",
		"       /map -mapobj [mapid]  : 根据 mapid 查询该地图信息"
	})]
	public class MapManagerCommand : AbstractCommandHandler, ICommandHandler
	{
		private BaseClient client = null;
		public bool OnCommand(BaseClient client, string[] args)
		{
			this.client = client;
			if (args.Length > 1)
			{
				string text = args[1];
				switch (text)
				{
				case "-list":
					this.ListAllMap();
					goto IL_148;
				case "-new":
				{
					string newMapStr = MapMgr.ListServerMap(MapMgr.SimpleMapList);
					this.DisplayMessage(client, newMapStr);
					goto IL_148;
				}
				case "-old":
				{
					string oldMapStr = MapMgr.ListServerMap(MapMgr.NormalMapList);
					this.DisplayMessage(client, oldMapStr);
					goto IL_148;
				}
				case "-mapid":
					if (args.Length <= 2)
					{
						this.DisplaySyntax(client);
						goto IL_148;
					}
					this.ListMapById(args[2]);
					goto IL_148;
				case "-mapobj":
					if (args.Length <= 2)
					{
						this.DisplaySyntax(client);
						goto IL_148;
					}
					this.ListMapObjects(args[2]);
					goto IL_148;
				case "-clear":
					Console.Clear();
					goto IL_148;
				}
				this.DisplaySyntax(client);
				IL_148:;
			}
			else
			{
				this.DisplaySyntax(client);
			}
			return true;
		}
		private void ListAllMap()
		{
			foreach (int key in MapMgr.MapInstancePool.Keys)
			{
				int count = MapMgr.MapInstancePool[key].Count;
				this.DisplayMessage(this.client, "  MapId: {0},Count: {1} ", new object[]
				{
					key,
					count
				});
			}
			this.DisplayMessage(this.client, "---------------------------");
			this.DisplayMessage(this.client, " TotalMapCount:{0}", new object[]
			{
				MapMgr.MapInstancePool.Count
			});
		}
		private void ListMapById(string mapId)
		{
			int mapIdKey = 0;
			try
			{
				mapIdKey = int.Parse(mapId);
			}
			catch
			{
				this.DisplayMessage(this.client, "You Enter MapId Type Error！");
				return;
			}
			if (!MapMgr.MapInstancePool.ContainsKey(mapIdKey))
			{
				this.DisplayMessage(this.client, "This MapId Not Exist！");
			}
			else
			{
				Queue<Map> map = MapMgr.MapInstancePool[mapIdKey];
				this.DisplayMessage(this.client, "  MapId: {0} Count: {1}", new object[]
				{
					mapId,
					map.Count
				});
			}
		}
		private void ListMapObjects(string mapId)
		{
			int mapIdKey = 0;
			try
			{
				mapIdKey = int.Parse(mapId);
			}
			catch
			{
				this.DisplayMessage(this.client, "You Enter MapId Error！");
				return;
			}
			if (!MapMgr.MapInstancePool.ContainsKey(mapIdKey))
			{
				this.DisplayMessage(this.client, "This MapId Not Exist！");
			}
			else
			{
				foreach (Map map in MapMgr.MapInstancePool[mapIdKey])
				{
					this.DisplayMessage(this.client, " ---------------------------------------------------");
					foreach (Physics phy in map.Ojbects)
					{
						this.DisplayMessage(this.client, "Id:{0} X:{1} Y:{2}", new object[]
						{
							phy.Id,
							phy.X,
							phy.Y
						});
					}
					this.DisplayMessage(this.client, " ---------------------------------------------------");
				}
			}
		}
	}
}
