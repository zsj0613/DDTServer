using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.Effects;
using Game.Logic.LogEnum;
using Game.Logic.Phy.Maths;
using Game.Logic.Spells;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Game.Logic.Phy.Object
{
	public class Player : TurnedLiving
	{
		private static LogProvider log => LogProvider.Default;
		private IGamePlayer m_player;
		private ItemTemplateInfo m_weapon;
		private int m_mainBallId;
		private int m_spBallId;
		private BallInfo m_currentBall;
		private int m_energy;
		private int m_ghostEnergy;
		private bool m_isActive;
		public Point TargetPoint;
		public int GainGP;
		public int GainOffer;
		public bool LockDirection = false;
		public int TotalCure;
		private bool m_canGetProp;
		public int TotalAllHurt;
		public int TotalAllHitTargetCount;
		public int TotalAllShootCount;
		public int TotalAllKill;
		public int TotalAllExperience;
		public int TotalAllScore;
		public int TotalAllCure;
		public int CanTakeOut;
		public bool FinishTakeCard;
		public bool HasPaymentTakeCard;
		public bool Ready;
		public bool DefenceInformation;
		public bool AttackInformation;
		private int m_killedPunishmentOffer;
		private int m_loadingProcess;
		private int m_shootCount;
		private int m_ballCount;
		private ArrayList m_tempBoxes = new ArrayList();
		private static readonly int FLY_COOLDOWN = 2;
		private static readonly int CARRY_TEMPLATE_ID = 10016;
		private int m_flyCoolDown = 0;
		private int m_secondWeapon = 0;
		private int angelCount = 0;
		public event PlayerEventHandle BeforeBomb;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.BeforeBomb = (PlayerEventHandle)Delegate.Combine(this.BeforeBomb, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.BeforeBomb = (PlayerEventHandle)Delegate.Remove(this.BeforeBomb, value);
        //    }
        //}
		public event PlayerEventHandle LoadingCompleted;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.LoadingCompleted = (PlayerEventHandle)Delegate.Combine(this.LoadingCompleted, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.LoadingCompleted = (PlayerEventHandle)Delegate.Remove(this.LoadingCompleted, value);
        //    }
        //}
		public event PlayerShootEventHandle BeforePlayerShoot;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.BeforePlayerShoot = (PlayerShootEventHandle)Delegate.Combine(this.BeforePlayerShoot, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.BeforePlayerShoot = (PlayerShootEventHandle)Delegate.Remove(this.BeforePlayerShoot, value);
        //    }
        //}
		public event PlayerEventHandle AfterPlayerShooted;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.AfterPlayerShooted = (PlayerEventHandle)Delegate.Combine(this.AfterPlayerShooted, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.AfterPlayerShooted = (PlayerEventHandle)Delegate.Remove(this.AfterPlayerShooted, value);
        //    }
        //}
		public event PlayerEventHandle CollidByObject;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.CollidByObject = (PlayerEventHandle)Delegate.Combine(this.CollidByObject, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.CollidByObject = (PlayerEventHandle)Delegate.Remove(this.CollidByObject, value);
        //    }
        //}
		public event PlayerMissionEventHandle MissionEventHandle;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.MissionEventHandle = (PlayerMissionEventHandle)Delegate.Combine(this.MissionEventHandle, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.MissionEventHandle = (PlayerMissionEventHandle)Delegate.Remove(this.MissionEventHandle, value);
        //    }
        //}
		public event PlayerUserPropEventHandle PlayerUseProp;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.PlayerUseProp = (PlayerUserPropEventHandle)Delegate.Combine(this.PlayerUseProp, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.PlayerUseProp = (PlayerUserPropEventHandle)Delegate.Remove(this.PlayerUseProp, value);
        //    }
        //}
		public event PlayerUseSpecialSkillEventHandle PlayerUseSpecialSkill;
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    add
        //    {
        //        this.PlayerUseSpecialSkill = (PlayerUseSpecialSkillEventHandle)Delegate.Combine(this.PlayerUseSpecialSkill, value);
        //    }
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    remove
        //    {
        //        this.PlayerUseSpecialSkill = (PlayerUseSpecialSkillEventHandle)Delegate.Remove(this.PlayerUseSpecialSkill, value);
        //    }
        //}
		public IGamePlayer PlayerDetail
		{
			get
			{
				return this.m_player;
			}
		}
		public ItemTemplateInfo Weapon
		{
			get
			{
				return this.m_weapon;
			}
		}
		public bool IsActive
		{
			get
			{
				return this.m_isActive;
			}
		}
		public bool CanGetProp
		{
			get
			{
				return this.m_canGetProp;
			}
			set
			{
				if (this.m_canGetProp != value)
				{
					this.m_canGetProp = value;
				}
			}
		}
		public int KilledPunishmentOffer
		{
			get
			{
				return this.m_killedPunishmentOffer;
			}
			set
			{
				this.m_killedPunishmentOffer = value;
			}
		}
		public int LoadingProcess
		{
			get
			{
				return this.m_loadingProcess;
			}
			set
			{
				if (this.m_loadingProcess != value)
				{
					this.m_loadingProcess = value;
					if (this.m_loadingProcess >= 100)
					{
						this.OnLoadingCompleted();
					}
				}
			}
		}
		public int LevelPlusBlood
		{
			get
			{
				int plusblood = 0;
				for (int i = 10; i <= 80; i += 10)
				{
					if (this.PlayerDetail.PlayerCharacter.Grade - i > 0)
					{
						plusblood += (this.PlayerDetail.PlayerCharacter.Grade - i) * (i + 20);
					}
				}
				return plusblood;
			}
		}
		public int GhostEnergy
		{
			get
			{
				return this.m_ghostEnergy;
			}
			set
			{
				this.m_ghostEnergy = value;
			}
		}
		public int Energy
		{
			get
			{
				return this.m_energy;
			}
			set
			{
				this.m_energy = value;
			}
		}
		public BallInfo CurrentBall
		{
			get
			{
				return this.m_currentBall;
			}
			set
			{
				this.m_currentBall = value;
			}
		}
		public bool IsSpecialSkill
		{
			get
			{
				return this.m_currentBall.ID == this.m_spBallId;
			}
		}
		public int ShootCount
		{
			get
			{
				return this.m_shootCount;
			}
			set
			{
				if (this.m_shootCount != value)
				{
					this.m_shootCount = value;
					this.m_game.SendGameUpdateShootCount(this);
				}
			}
		}
		public int BallCount
		{
			get
			{
				return this.m_ballCount;
			}
			set
			{
				if (this.m_ballCount != value)
				{
					this.m_ballCount = value;
				}
			}
		}
		public int AngelCount
		{
			get
			{
				return this.angelCount;
			}
			set
			{
				this.angelCount = value;
			}
		}
		public Player(IGamePlayer player, int id, BaseGame game, int team) : base(id, game, team, "", "", 1000, 0, 1, 0)
		{
			this.m_rect = new Rectangle(-15, -20, 30, 30);
			this.m_player = player;
			this.m_player.GamePlayerId = id;
			this.m_isActive = true;
			this.m_canGetProp = true;
			this.Grade = player.PlayerCharacter.Grade;
			this.TotalAllHurt = 0;
			this.TotalAllHitTargetCount = 0;
			this.TotalAllShootCount = 0;
			this.TotalAllKill = 0;
			this.TotalAllExperience = 0;
			this.TotalAllScore = 0;
			this.TotalAllCure = 0;
			this.m_weapon = this.m_player.MainWeapon;
			if (this.m_weapon != null)
			{
				this.m_mainBallId = this.m_weapon.Property1;
				this.m_spBallId = this.m_weapon.Property2;
			}
			this.m_loadingProcess = 0;
			this.InitBuffer(this.m_player.EquipEffect);
			this.m_energy = this.m_player.PlayerCharacter.Agility / 30 + 240;
			this.m_maxBlood = (int)((double)(950 + this.m_player.PlayerCharacter.Grade * 50 + this.LevelPlusBlood + this.m_player.PlayerCharacter.Defence / 10) * this.m_player.GetBaseBlood());
		}
		public override void Reset()
		{
			this.m_maxBlood = (int)((double)(950 + this.m_player.PlayerCharacter.Grade * 50 + this.LevelPlusBlood + this.m_player.PlayerCharacter.Defence / 10) * this.m_player.GetBaseBlood());
			this.HasPaymentTakeCard = false;
			base.Dander = 0;
			this.m_energy = this.m_player.PlayerCharacter.Agility / 30 + 240;
			this.m_ghostEnergy = this.m_energy;
			base.IsLiving = true;
			this.FinishTakeCard = false;
			this.m_weapon = this.m_player.MainWeapon;
			this.m_mainBallId = this.m_weapon.Property1;
			this.m_spBallId = this.m_weapon.Property2;
			this.BaseDamage = this.m_player.GetBaseAttack();
			this.BaseGuard = this.m_player.GetBaseDefence();
			this.Attack = (double)this.m_player.PlayerCharacter.Attack;
			this.Defence = (double)this.m_player.PlayerCharacter.Defence;
			this.Agility = (double)this.m_player.PlayerCharacter.Agility;
			this.Lucky = (double)this.m_player.PlayerCharacter.Luck;
			this.InitBuffer(this.m_player.EquipEffect);
			this.m_currentBall = BallMgr.FindBall(this.m_mainBallId);
			this.m_shootCount = 1;
			this.m_ballCount = 1;
			this.CurrentIsHitTarget = false;
			this.m_killedPunishmentOffer = 0;
			this.TotalCure = 0;
			this.TotalHitTargetCount = 0;
			this.TotalHurt = 0;
			this.TotalKill = 0;
			this.TotalShootCount = 0;
			this.LockDirection = false;
			this.GainGP = 0;
			this.GainOffer = 0;
			this.Ready = false;
			this.PlayerDetail.ClearTempBag();
			this.m_delay = this.GetInitDelay();
			this.TargetPoint = Point.Empty;
			this.m_flyCoolDown = 0;
			this.m_secondWeapon = 0;
			if (this.PlayerDetail.SecondWeapon != null)
			{
				this.AngelCount = this.PlayerDetail.SecondWeapon.StrengthenLevel + 1;
			}
			base.Reset();
		}
		public void InitBuffer(List<int> equpedEffect)
		{
			base.EffectList.StopAllEffect();
			for (int i = 0; i < equpedEffect.Count; i++)
			{
				ItemTemplateInfo item = ItemMgr.FindItemTemplate(equpedEffect[i]);
				switch (item.Property3)
				{
				case 1:
					new AddAttackEffect(item.Property4, item.Property5).Start(this);
					break;
				case 2:
					new AddDefenceEffect(item.Property4, item.Property5).Start(this);
					break;
				case 3:
					new AddAgilityEffect(item.Property4, item.Property5).Start(this);
					break;
				case 4:
					new AddLuckyEffect(item.Property4, item.Property5).Start(this);
					break;
				case 5:
					new AddDamageEffect(item.Property4, item.Property5).Start(this);
					break;
				case 6:
					new ReduceDamageEffect(item.Property4, item.Property5).Start(this);
					break;
				case 7:
					new AddBloodEffect(item.Property4, item.Property5).Start(this);
					break;
				case 8:
					new FatalEffect(item.Property4, item.Property5).Start(this);
					break;
				case 9:
					new IceFronzeEquipEffect(item.Property4, item.Property5).Start(this);
					break;
				case 10:
					new NoHoleEquipEffect(item.Property4, item.Property5).Start(this);
					break;
				case 11:
					new AtomBombEquipEffect(item.Property4, item.Property5).Start(this);
					break;
				case 12:
					new ArmorPiercerEquipEffect(item.Property4, item.Property5).Start(this);
					break;
				case 13:
					new AvoidDamageEffect(item.Property4, item.Property5).Start(this);
					break;
				case 14:
					new MakeCriticalEffect(item.Property4, item.Property5).Start(this);
					break;
				case 15:
					new AssimilateDamageEffect(item.Property4, item.Property5).Start(this);
					break;
				case 16:
					new AssimilateBloodEffect(item.Property4, item.Property5).Start(this);
					break;
				case 17:
					new SealEquipEffect(item.Property4, item.Property5).Start(this);
					break;
				case 18:
					new AddTurnEquipEffect(item.Property4, item.Property5, item.TemplateID).Start(this);
					break;
				case 19:
					new AddDanderEquipEffect(item.Property4, item.Property5).Start(this);
					break;
				case 20:
					new ReflexDamageEquipEffect(item.Property4, item.Property5).Start(this);
					break;
				case 21:
					new ReduceStrengthEquipEffect(item.Property4, item.Property5).Start(this);
					break;
				case 22:
					new ContinueReduceBloodEquipEffect(item.Property4, item.Property5).Start(this);
					break;
				case 23:
					new LockDirectionEquipEffect(item.Property4, item.Property5).Start(this);
					break;
				case 24:
					new AddBombEquipEffect(item.Property4, item.Property5).Start(this);
					break;
				case 25:
					new ContinueReduceBaseDamageEquipEffect(item.Property4, item.Property5).Start(this);
					break;
				case 26:
					new RecoverBloodEffect(item.Property4, item.Property5).Start(this);
					break;
				}
			}
		}
		public bool ReduceEnergy(int value)
		{
			bool result;
			if (value > this.m_energy)
			{
				result = false;
			}
			else
			{
				this.m_energy -= value;
				result = true;
			}
			return result;
		}
		private int GetInitDelay()
		{
			return 1600 - 1200 * this.PlayerDetail.PlayerCharacter.Agility / (this.PlayerDetail.PlayerCharacter.Agility + 1200);
		}
		private int GetTurnDelay()
		{
			return 1600 - 1200 * this.PlayerDetail.PlayerCharacter.Agility / (this.PlayerDetail.PlayerCharacter.Agility + 1200);
		}
		public override bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, int type, int delay)
		{
			if ((source == this || source.Team == base.Team) && damageAmount + criticalAmount >= this.m_blood)
			{
				damageAmount = this.m_blood - 1;
				criticalAmount = 0;
			}
			bool result = base.TakeDamage(source, ref damageAmount, ref criticalAmount, type, delay);
			if (base.IsLiving && result)
			{
				base.AddDander(damageAmount * 500 / base.MaxBlood);
			}
			return result;
		}
		public void UseSpecialSkill()
		{
			this.OnPlayerUseSpecialSkill(this);
			if (base.Dander >= 200)
			{
				base.SetDander(0);
				this.SetBall(this.m_spBallId, true);
			}
		}
		public void SetBall(int ballId)
		{
			this.SetBall(ballId, false);
		}
		public void SetBall(int ballId, bool special)
		{
			if (BallMgr.IsExist(ballId))
			{
				if (ballId != this.m_currentBall.ID)
				{
					this.m_currentBall = BallMgr.FindBall(ballId);
					if (ballId != 4)
					{
						this.BallCount = this.m_currentBall.Amount;
					}
					if (!special && ballId != 4)
					{
						this.ShootCount = 1;
					}
					this.m_game.SendGameUpdateBall(this, special);
				}
			}
		}
		public void SetCurrentWeapon(ItemTemplateInfo item)
		{
			if (item != null)
			{
				this.m_weapon = item;
				this.m_mainBallId = this.m_weapon.Property1;
				this.SetBall(this.m_mainBallId);
			}
		}
		public void StartGhostMoving()
		{
			if (!this.TargetPoint.IsEmpty)
			{
				Point pv = new Point(this.TargetPoint.X - this.X, this.TargetPoint.Y - this.Y);
				if (pv.Length() != 0.0)
				{
					Point target = this.TargetPoint;
					if (pv.Length() > (double)this.m_ghostEnergy)
					{
						pv = pv.Normalize(this.m_ghostEnergy);
					}
					Point p = new Point(this.X + pv.X, this.Y + pv.Y);
					this.m_game.AddAction(new GhostMoveAction(this, p));
				}
			}
		}
		public override void SetXY(int x, int y)
		{
			if (this.m_x != x || this.m_y != y)
			{
				this.m_x = x;
				this.m_y = y;
				if (base.IsLiving)
				{
					this.m_energy -= Math.Abs(this.m_x - x);
				}
				else
				{
					Rectangle rect = this.m_rect;
					rect.Offset(this.m_x, this.m_y);
					Physics[] phys = this.m_map.FindPhysicalObjects(rect, this);
					Physics[] array = phys;
					for (int i = 0; i < array.Length; i++)
					{
						Physics p = array[i];
						if (p is Box)
						{
							Box b = p as Box;
							this.PickBox(b);
							this.OpenBox(b.Id);
						}
					}
				}
			}
		}
		public override void Die()
		{
			if (base.IsLiving)
			{
				this.m_y -= 70;
				base.Die();
			}
		}
		public override void PickBox(Box box)
		{
			this.m_tempBoxes.Add(box);
			base.PickBox(box);
		}
		public void OpenBox(int boxId)
		{
			Box box = null;
			foreach (Box temp in this.m_tempBoxes)
			{
				if (temp.Id == boxId)
				{
					box = temp;
					break;
				}
			}
			if (box != null && box.Item != null)
			{
				ItemInfo item = box.Item;
				int templateID = item.TemplateID;
				if (templateID != -300)
				{
					if (templateID != -200)
					{
						if (templateID != -100)
						{
							if (item.Template.CategoryID == 10)
							{
								if (!this.m_player.AddTemplate(item, eBageType.FightBag, item.Count))
								{
								}
							}
							else
							{
								this.m_player.AddTemplate(item, eBageType.TempBag, item.Count);
							}
						}
						else
						{
							this.m_player.AddGold(item.Count);
							StringBuilder msg = new StringBuilder();
							msg.Append(LanguageMgr.GetTranslation("OpenUpArkHandler.Start", new object[0]));
							msg.Append(LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGold", new object[]
							{
								item.Count
							}));
							base.Game.SendMessage(this.PlayerDetail, msg.ToString(), null, 2);
						}
					}
					else
					{
						this.m_player.AddMoney(item.Count, LogMoneyType.Box, LogMoneyType.Box_Open);
					}
				}
				else
				{
					this.m_player.AddGiftToken(item.Count);
				}
				this.m_tempBoxes.Remove(box);
			}
		}
		public override void PrepareNewTurn()
		{
			if (this.CurrentIsHitTarget)
			{
				this.TotalHitTargetCount++;
			}
			this.m_energy = this.m_player.PlayerCharacter.Agility / 30 + 240;
			this.m_ghostEnergy = (int)((double)this.m_energy * 1.5);
			this.m_shootCount = 1;
			this.m_ballCount = 1;
			this.AttackInformation = true;
			this.DefenceInformation = true;
			this.AttackEffectTrigger = false;
			this.DefenceEffectTrigger = false;
			this.SetCurrentWeapon(this.PlayerDetail.MainWeapon);
			if (this.m_currentBall.ID != this.m_mainBallId)
			{
				this.m_currentBall = BallMgr.FindBall(this.m_mainBallId);
			}
			if (!base.IsLiving)
			{
				this.StartGhostMoving();
			}
			base.PrepareNewTurn();
		}
		public override void PrepareSelfTurn()
		{
			base.PrepareSelfTurn();
			this.DefaultDelay = this.m_delay;
			this.m_flyCoolDown--;
			this.m_secondWeapon--;
			if (base.IsFrost)
			{
				base.AddDelay(this.GetTurnDelay());
			}
		}
		public override void StartAttacking()
		{
			if (!base.IsAttacking)
			{
				base.AddDelay(this.GetTurnDelay());
				base.StartAttacking();
			}
		}
		public override void Skip(int spendTime)
		{
			if (base.IsAttacking)
			{
				base.Skip(spendTime);
				base.AddDelay(100);
				base.AddDander(40);
			}
		}
		public override void CollidedByObject(Physics phy, int delay)
		{
			base.CollidedByObject(phy, delay);
			if (phy is SimpleBomb)
			{
				this.OnCollidedByObject(delay);
			}
		}
		public void PrepareShoot(byte speedTime)
		{
			int turnWaitTime = this.m_game.GetTurnWaitTime();
			int time = ((int)speedTime > turnWaitTime) ? turnWaitTime : ((int)speedTime);
			base.AddDelay(time * 20);
			this.TotalShootCount++;
		}
		public bool Shoot(int x, int y, int force, int angle)
		{
			bool result;
			if (this.m_shootCount > 0)
			{
				if (this.CurrentBall.ID != 1 && this.CurrentBall.ID != 64 && this.CurrentBall.ID != 3)
				{
					this.OnBeforePlayerShoot(this.CurrentBall.ID);
				}
				if (base.ShootImp(this.m_currentBall.ID, x, y, force, angle, this.m_ballCount))
				{
					this.m_shootCount--;
					if (this.m_shootCount <= 0 || !base.IsLiving)
					{
						this.StopAttacking();
						base.AddDelay(this.m_currentBall.Delay + this.m_weapon.Property8);
						base.AddDander(20);
						if (this.CanGetProp)
						{
							int gold = 0;
							int money = 0;
							int giftToken = 0;
							List<ItemInfo> infos = null;
							if (DropInventory.FireDrop(this.m_game.RoomType, ref infos))
							{
								if (infos != null)
								{
									foreach (ItemInfo info in infos)
									{
										ItemInfo tempInfo = ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken);
										if (tempInfo != null)
										{
											int templateID = tempInfo.TemplateID;
											this.PlayerDetail.AddTemplate(tempInfo, eBageType.FightBag, tempInfo.Count);
										}
									}
									this.PlayerDetail.AddGold(gold);
									this.PlayerDetail.AddMoney(money, LogMoneyType.Game, LogMoneyType.Game_Shoot);
									this.PlayerDetail.AddGiftToken(giftToken);
								}
							}
						}
					}
					this.SendAttackInformation();
					this.OnAfterPlayerShoot();
					result = true;
					return result;
				}
				Player.log.Error(string.Format("Player Shoot method call ShootImpl renturn false. m_currentBall.ID : {0}, x : {1}, y : {2}, force : {3}, angle : {4}, m_ballCount : {5}", new object[]
				{
					this.m_currentBall.ID,
					x,
					y,
					force,
					angle,
					this.m_ballCount
				}));
			}
			else
			{
				Player.log.Error("Player Shoot method m_shootCount < 0");
			}
			result = false;
			return result;
		}
		public bool CanUseItem(ItemTemplateInfo item)
		{
			return item != null && this.m_game.CurrentLiving != null && this.m_energy >= item.Property4 && (base.IsAttacking || (!base.IsLiving && base.Team == this.m_game.CurrentLiving.Team));
		}
		public bool UseItem(ItemTemplateInfo item)
		{
			bool result;
			if (this.CanUseItem(item))
			{
				this.m_energy -= item.Property4;
				this.m_delay += item.Property5;
				this.m_game.SendPlayerUseProp(this, -2, -2, item.TemplateID);
				this.OnPlayerUseProp(this);
				SpellMgr.ExecuteSpell(this.m_game, this.m_game.CurrentLiving as Player, item);
				if (item.Property6 == 1 && base.IsAttacking)
				{
					this.StopAttacking();
					this.m_game.CheckState(0);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public void UseFlySkill()
		{
			if (this.m_flyCoolDown <= 0)
			{
				this.m_flyCoolDown = Player.FLY_COOLDOWN;
				this.m_game.SendPlayerUseProp(this, -2, -2, Player.CARRY_TEMPLATE_ID);
				this.SetBall(3);
			}
		}
		public void UseSecondWeapon()
		{
			if (this.PlayerDetail.SecondWeapon != null && this.m_energy - this.PlayerDetail.SecondWeapon.Template.Property4 >= 0)
			{
				if (this.m_secondWeapon <= 0)
				{
					if (this.PlayerDetail.SecondWeapon != null)
					{
						if (this.AngelCount <= 0)
						{
							this.PlayerDetail.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UnusableSecondWeapon", new object[]
							{
								this.PlayerDetail.SecondWeapon.Template.Name
							}));
						}
						else
						{
							this.m_secondWeapon = this.PlayerDetail.SecondWeapon.Template.Property6;
							this.m_energy -= this.PlayerDetail.SecondWeapon.Template.Property4;
							SpellMgr.ExecuteSpell(base.Game, this, this.PlayerDetail.SecondWeapon);
							this.m_game.SendPlayerUseProp(this, -2, -2, this.PlayerDetail.SecondWeapon.TemplateID);
							this.AngelCount--;
						}
					}
				}
			}
		}
		public void DeadLink()
		{
			this.m_isActive = false;
			if (base.IsLiving)
			{
				this.Die();
			}
		}
		public bool CheckShootPoint(int x, int y)
		{
			bool result;
			if (Math.Abs(this.X - x) > 100)
			{
				string username = this.m_player.PlayerCharacter.UserName;
				string nickname = this.m_player.PlayerCharacter.NickName;
				this.m_player.Disconnect();
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
		public void SendAttackInformation()
		{
			if (this.AttackEffectTrigger && this.AttackInformation)
			{
				base.Game.SendMessage(this.PlayerDetail, LanguageMgr.GetTranslation("PlayerEquipEffect.Success", new object[0]), LanguageMgr.GetTranslation("PlayerEquipEffect.Success1", new object[]
				{
					this.PlayerDetail.PlayerCharacter.NickName
				}), 3);
				this.AttackEffectTrigger = false;
				this.AttackInformation = false;
			}
		}
		public void OnBeforeBomb(int delay)
		{
			if (this.BeforeBomb != null)
			{
				this.BeforeBomb(this, delay);
			}
		}
		protected void OnLoadingCompleted()
		{
			if (this.LoadingCompleted != null)
			{
				this.LoadingCompleted(this, 0);
			}
		}
		protected void OnBeforePlayerShoot(int ball)
		{
			if (this.BeforePlayerShoot != null)
			{
				this.BeforePlayerShoot(this, ball);
			}
		}
		protected void OnAfterPlayerShoot()
		{
			if (this.AfterPlayerShooted != null)
			{
				this.AfterPlayerShooted(this, 0);
			}
		}
		protected void OnCollidedByObject(int delay)
		{
			if (this.CollidByObject != null)
			{
				this.CollidByObject(this, delay);
			}
		}
		public override void OnAfterKillingLiving(Living target, int damageAmount, int criticalAmount, int delay)
		{
			base.OnAfterKillingLiving(target, damageAmount, criticalAmount, delay);
			if (target is Player)
			{
				this.m_player.OnKillingLiving(this.m_game, 1, target.Id, target.IsLiving, damageAmount + criticalAmount, this.PlayerDetail.IsArea);
				this.CalculatePlayerOffer(target as Player);
			}
			else
			{
				int targetId = 0;
				if (target is SimpleBoss)
				{
					SimpleBoss tempBoss = target as SimpleBoss;
					targetId = tempBoss.NpcInfo.ID;
				}
				if (target is SimpleNpc)
				{
					SimpleNpc tempNpc = target as SimpleNpc;
					targetId = tempNpc.NpcInfo.ID;
				}
				this.m_player.OnKillingLiving(this.m_game, 2, targetId, target.IsLiving, damageAmount + criticalAmount, false);
			}
		}
		public void OnMissionEventHandle(GSPacketIn packet)
		{
			if (this.MissionEventHandle != null)
			{
				this.MissionEventHandle(packet);
			}
		}
		protected void OnPlayerUseProp(Player player)
		{
			if (this.PlayerUseProp != null)
			{
				this.PlayerUseProp(player);
			}
		}
		protected void OnPlayerUseSpecialSkill(Player player)
		{
			if (this.PlayerUseSpecialSkill != null)
			{
				this.PlayerUseSpecialSkill(player);
			}
		}
		public void CalculatePlayerOffer(Player player)
		{
			if (this.m_game.RoomType == eRoomType.Match && (this.m_game.GameType == eGameType.Guild || this.m_game.GameType == eGameType.Free))
			{
				if (!player.IsLiving)
				{
					int robOffer;
					if (base.Game.GameType == eGameType.Guild)
					{
						robOffer = 10;
					}
					else
					{
						if (this.PlayerDetail.PlayerCharacter.ConsortiaID != 0 && player.PlayerDetail.PlayerCharacter.ConsortiaID != 0)
						{
							robOffer = 3;
						}
						else
						{
							robOffer = 1;
						}
					}
					if (robOffer > player.PlayerDetail.PlayerCharacter.Offer)
					{
						robOffer = player.PlayerDetail.PlayerCharacter.Offer;
					}
					if (robOffer > 0)
					{
						this.GainOffer += robOffer;
						player.KilledPunishmentOffer = robOffer;
					}
				}
			}
		}
		public string GetFsPlayerInfo()
		{
			string fsPlayerInfo = string.Empty;
			foreach (string str in new List<string>
			{
				string.Concat(new object[]
				{
					"     playerID:",
					this.m_player.PlayerCharacter.ID,
					" NickName:",
					this.m_player.PlayerCharacter.NickName,
					" Money:",
					this.m_player.PlayerCharacter.Money,
					" Win:",
					this.m_player.PlayerCharacter.Win,
					" Total:",
					this.m_player.PlayerCharacter.Total,
					" Escape:",
					this.m_player.PlayerCharacter.Escape
				})
			})
			{
				fsPlayerInfo += str;
			}
			return fsPlayerInfo;
		}
	}
}
