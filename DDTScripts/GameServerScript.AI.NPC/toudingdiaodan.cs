using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Drawing;

namespace GameServerScript.AI.NPC
{
	public class toudingdiaodan : ABrain
	{
		private int m_attackTurn;

		private int isSay;

		private Point[] brithPoint = new Point[]
		{
			new Point(500, 50)
		};

		private static string[] AllAttackChat = new string[]
		{
			"看见我的人都的死！"
		};

		private static string[] ShootChat = new string[]
		{
			"吃我一招！"
		};

		private static string[] KillPlayerChat = new string[]
		{
			"去死吧！"
		};

		private static string[] CallChat = new string[]
		{
			"DDT万岁！"
		};

		private static string[] JumpChat = new string[]
		{
			"经验一族永不灭！"
		};

		private static string[] KillAttackChat = new string[]
		{
			"来吧。成为我经验一格吧！"
		};

		private static string[] ShootedChat = new string[]
		{
			"这怎么可能。。。"
		};

		private static string[] DiedChat = new string[]
		{
			"我还会再回来的！"
		};

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
			this.isSay = 0;
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			bool flag = false;
			int num = 0;
			foreach (Player current in base.Game.GetAllFightPlayers())
			{
				if (current.IsLiving && current.X > 1169 && current.X < base.Game.Map.Info.ForegroundWidth + 1)
				{
					int num2 = (int)base.Body.Distance(current.X, current.Y);
					if (num2 > num)
					{
						num = num2;
					}
					flag = true;
				}
			}
			if (flag)
			{
				this.KillAttack(1169, base.Game.Map.Info.ForegroundWidth + 1);
				return;
			}
			if (this.m_attackTurn == 0)
			{
				if (((PVEGame)base.Game).GetLivedLivings().Count == 9)
				{
					this.PersonalAttack();
				}
				this.m_attackTurn++;
				return;
			}
			this.PersonalAttack();
			this.m_attackTurn = 0;
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		private void KillAttack(int fx, int tx)
		{
			int num = base.Game.Random.Next(0, toudingdiaodan.KillAttackChat.Length);
			base.Body.Say(toudingdiaodan.KillAttackChat[num], 1, 1000);
			base.Body.CurrentDamagePlus = 10f;
			base.Body.PlayMovie("beatA", 3000, 0);
			base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
		}

		private void PersonalAttack()
		{
			Player player = base.Game.FindRandomPlayer();
			if (player != null)
			{
				base.Body.CurrentDamagePlus = 0.8f;
				int num = base.Game.Random.Next(0, toudingdiaodan.ShootChat.Length);
				base.Body.Say(toudingdiaodan.ShootChat[num], 1, 0);
				base.Game.Random.Next(670, 880);
				base.Game.Random.Next(player.X - 10, player.X + 10);
				if (base.Body.ShootPoint(player.X, player.Y - 100, 4, 1200, 700, 2, 3f, 2550))
				{
					base.Body.PlayMovie("beatA", 1700, 0);
				}
			}
		}

		public override void OnKillPlayerSay()
		{
			base.OnKillPlayerSay();
			int num = base.Game.Random.Next(0, toudingdiaodan.KillPlayerChat.Length);
			base.Body.Say(toudingdiaodan.KillPlayerChat[num], 1, 0, 2000);
		}

		public override void OnDiedSay()
		{
		}

		private void CreateChild()
		{
		}

		public override void OnShootedSay(int delay)
		{
			int num = base.Game.Random.Next(0, toudingdiaodan.ShootedChat.Length);
			if (this.isSay == 0 && base.Body.IsLiving)
			{
				base.Body.Say(toudingdiaodan.ShootedChat[num], 1, delay, 0);
				this.isSay = 1;
			}
			if (!base.Body.IsLiving)
			{
				num = base.Game.Random.Next(0, toudingdiaodan.DiedChat.Length);
				base.Body.Say(toudingdiaodan.DiedChat[num], 1, delay - 800, 2000);
			}
		}
	}
}
