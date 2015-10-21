
using Bussiness;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;

namespace GameServerScript.AI.Messions
{
	public class SF112 : AMissionControl
	{
		private SimpleBoss boss;

		private int npcID = 8888888;

		private int bossID = 2000012;

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
				this.npcID
			};
			int[] npcIds2 = new int[]
			{
				this.bossID
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.AddLoadingFile(1, "bombs/51.swf", "tank.resource.bombs.Bomb51");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.AntQueenAsset");
			base.Game.SetMap(11001);
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
			this.m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 1, 0);
			this.m_front = base.Game.Createlayer(1131, 150, "font", "game.asset.living.AntQueenAsset", "out", 1, 1, 0);
			this.boss = base.Game.CreateBoss(this.bossID, 912, 486, -1, 1);
			this.boss.SetRelateDemagemRect(-42, -200, 84, 194);
			this.boss.Say(LanguageMgr.GetTranslation("GameServerScript.AI.Messions.DCNM2102.msg1", new object[0]), 0, 200, 0);
			this.m_moive.PlayMovie("in", 6000, 0);
			this.m_front.PlayMovie("in", 6100, 0);
			this.m_moive.PlayMovie("out", 10000, 1000);
			this.m_front.PlayMovie("out", 9900, 0);
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
			if (this.boss != null && !this.boss.IsLiving)
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
			if (this.boss != null && !this.boss.IsLiving)
			{
				base.Game.IsWin = true;
				return;
			}
			base.Game.IsWin = false;
		}
	}
}
