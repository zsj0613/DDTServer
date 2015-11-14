
using Bussiness;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;

namespace GameServerScript.AI.Messions
{
	public class FB112 : AMissionControl
	{
		private SimpleBoss boss;

		private SimpleBoss boss2;

		private int bossID = 1000111;

		private int boss2ID = 1000112;

		private int kill;

		private PhysicalObj m_moive;

		private PhysicalObj m_front;

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 1750)
			{
				return 3;
			}
			if (score > 1675)
			{
				return 2;
			}
			if (score > 1600)
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
				this.bossID,
				this.boss2ID
			};
			int[] npcIds2 = new int[]
			{
				this.bossID,
				this.boss2ID
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.AddLoadingFile(2, "bombs/4.swf", "tank.resource.bombs.Bomb4");
			base.Game.SetMap(1214);
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
			this.boss = base.Game.CreateBoss(this.bossID, 1275, 444, -1, 1);
			this.boss.SetRelateDemagemRect(-42, -200, 84, 194);
			this.boss.Say(LanguageMgr.GetTranslation("我老大", new object[0]), 0, 200, 0);
			this.boss2 = base.Game.CreateBoss(this.boss2ID, 991, 972, -1, 1);
			this.boss2.SetRelateDemagemRect(-42, -200, 84, 194);
			this.boss2.Say(LanguageMgr.GetTranslation("我老二", new object[0]), 0, 200, 0);
		}

		public override void OnNewTurnStarted()
		{
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (base.Game.TurnIndex > 1)
			{
				if (this.m_moive != null)
				{
					base.Game.RemovePhysicalObj(this.m_moive, true);
					this.m_moive = null;
				}
				if (this.m_front != null)
				{
					base.Game.RemovePhysicalObj(this.m_front, true);
					this.m_front = null;
				}
			}
		}

		public override bool CanGameOver()
		{
			if (!this.boss.IsLiving && !this.boss2.IsLiving)
			{
				this.kill++;
				return true;
			}
			return false;
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return this.kill;
		}

		public override void OnPrepareGameOver()
		{
			base.OnPrepareGameOver();
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (!this.boss.IsLiving && !this.boss2.IsLiving)
			{
				base.Game.IsWin = true;
				return;
			}
			base.Game.IsWin = false;
		}
	}
}
