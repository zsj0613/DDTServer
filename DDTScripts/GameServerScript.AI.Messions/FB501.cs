using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class FB501 : AMissionControl
	{
		private List<SimpleNpc> someNpc = new List<SimpleNpc>();

		private int dieRedCount;

		private int[] npcIDs = new int[]
		{
			1000041,
			1000042
		};

		private int[] birthX = new int[]
		{
			52,
			115,
			183,
			253,
			320,
			1206,
			1275,
			1342,
			1410,
			1475
		};

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 1870)
			{
				return 3;
			}
			if (score > 1825)
			{
				return 2;
			}
			if (score > 1780)
			{
				return 1;
			}
			return 0;
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] npcIds = new int[]
			{
				this.npcIDs[0],
				this.npcIDs[1]
			};
			int[] npcIds2 = new int[]
			{
				this.npcIDs[1],
				this.npcIDs[0],
				this.npcIDs[0],
				this.npcIDs[0]
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(11003);
		}

		public override void OnPrepareStartGame()
		{
			base.OnPrepareStartGame();
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
		}

		public override void OnPrepareNewGame()
		{
			base.OnPrepareNewGame();
			int num = base.Game.Random.Next(0, this.npcIDs.Length);
			this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], 52, 506, 1));
			num = base.Game.Random.Next(0, this.npcIDs.Length);
			this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], 100, 507, 1));
			num = base.Game.Random.Next(0, this.npcIDs.Length);
			this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], 155, 508, 1));
			num = base.Game.Random.Next(0, this.npcIDs.Length);
			this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], 210, 507, 1));
			num = base.Game.Random.Next(0, this.npcIDs.Length);
			this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], 253, 507, 1));
			num = base.Game.Random.Next(0, this.npcIDs.Length);
			this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], 1275, 508, 1));
			num = base.Game.Random.Next(0, this.npcIDs.Length);
			this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], 1325, 506, 1));
			num = base.Game.Random.Next(0, this.npcIDs.Length);
			this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], 1360, 508, 1));
			num = base.Game.Random.Next(0, this.npcIDs.Length);
			this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], 1410, 506, 1));
			num = base.Game.Random.Next(0, this.npcIDs.Length);
			this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[num], 1475, 508, 1));
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
			PVEGame game = base.Game;
			int key = game.FindTurnNpcRank();
			if (base.Game.GetLivedLivings().Count == 0)
			{
				game.NpcTurnQueue[key] = 0;
			}
			if (base.Game.TurnIndex > 1 && base.Game.CurrentPlayer.Delay > game.NpcTurnQueue[key] && base.Game.GetLivedLivings().Count < 10)
			{
				for (int i = 0; i < 10 - base.Game.GetLivedLivings().Count; i++)
				{
					if (this.someNpc.Count == base.Game.MissionInfo.TotalCount)
					{
						return;
					}
					int num = base.Game.Random.Next(0, this.birthX.Length);
					int num2 = this.birthX[num];
					int direction = -1;
					if (num2 <= 320)
					{
						direction = 1;
					}
					num = base.Game.Random.Next(0, this.npcIDs.Length);
					if (num == 1 && this.GetNpcCountByID(this.npcIDs[1]) < 10)
					{
						this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[1], num2, 506, 1, direction));
					}
					else
					{
						this.someNpc.Add(base.Game.CreateNpc(this.npcIDs[0], num2, 506, 1, direction));
					}
				}
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override bool CanGameOver()
		{
			bool flag = true;
			base.CanGameOver();
			this.dieRedCount = 0;
			foreach (SimpleNpc current in this.someNpc)
			{
				if (current.IsLiving)
				{
					flag = false;
				}
				else
				{
					this.dieRedCount++;
				}
			}
			if (flag && this.dieRedCount == base.Game.MissionInfo.TotalCount)
			{
				base.Game.IsWin = true;
				return true;
			}
			return false;
		}

		public override int UpdateUIData()
		{
			return base.Game.TotalKillCount;
		}

		public override void OnPrepareGameOver()
		{
			base.OnPrepareGameOver();
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (base.Game.GetLivedLivings().Count == 0)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
			List<LoadingFileInfo> list = new List<LoadingFileInfo>();
			list.Add(new LoadingFileInfo(2, "image/map/2/show2", ""));
			base.Game.SendLoadResource(list);
		}

		protected int GetNpcCountByID(int Id)
		{
			int num = 0;
			foreach (SimpleNpc current in this.someNpc)
			{
				if (current.NpcInfo.ID == Id)
				{
					num++;
				}
			}
			return num;
		}
	}
}
