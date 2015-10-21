using Bussiness;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.LogEnum;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Game.Logic
{
	public class PVPGame : BaseGame
	{
		private new static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private List<Player> m_redTeam;
		private float m_redAvgLevel;
		private List<Player> m_blueTeam;
		private float m_blueAvgLevel;
		private int m_npcID;
		private int beginPlayerCount;
		private string teamAStr;
		private string teamBStr;
		private DateTime beginTime;
		public Player CurrentPlayer
		{
			get
			{
				return this.m_currentLiving as Player;
			}
		}
		public PVPGame(int id, int roomId, List<IGamePlayer> red, List<IGamePlayer> blue, Map map, eRoomType roomType, eGameType gameType, int timeType, int npcID) : base(id, roomId, map, roomType, gameType, timeType)
		{
			this.m_redTeam = new List<Player>();
			this.m_blueTeam = new List<Player>();
			this.m_npcID = npcID;
			StringBuilder sbTeampA = new StringBuilder();
			this.m_redAvgLevel = 0f;
			foreach (IGamePlayer player in red)
			{
				Player fp = new Player(player, this.physicalId++, this, 1);
				sbTeampA.Append(player.PlayerCharacter.ID).Append(",");
				fp.Reset();
				fp.Direction = ((this.m_random.Next(0, 2) == 0) ? 1 : -1);
				base.AddGamePlayer(player, fp);
				this.m_redTeam.Add(fp);
				this.m_redAvgLevel += (float)player.PlayerCharacter.Grade;
			}
			this.m_redAvgLevel /= (float)this.m_redTeam.Count;
			this.teamAStr = sbTeampA.ToString();
			StringBuilder sbTeampB = new StringBuilder();
			this.m_blueAvgLevel = 0f;
			foreach (IGamePlayer player in blue)
			{
				Player fp = new Player(player, this.physicalId++, this, 2);
				sbTeampB.Append(player.PlayerCharacter.ID).Append(",");
				fp.Reset();
				fp.Direction = ((this.m_random.Next(0, 2) == 0) ? 1 : -1);
				base.AddGamePlayer(player, fp);
				this.m_blueTeam.Add(fp);
				this.m_blueAvgLevel += (float)player.PlayerCharacter.Grade;
			}
			this.m_blueAvgLevel /= (float)blue.Count;
			this.teamBStr = sbTeampB.ToString();
			this.beginPlayerCount = this.m_redTeam.Count + this.m_blueTeam.Count;
			this.beginTime = DateTime.Now;
		}
		public void Prepare()
		{
			if (base.GameState == eGameState.Inited)
			{
				base.SendCreateGame();
				this.m_gameState = eGameState.Prepared;
				this.CheckState(0);
			}
		}
		public void StartLoading()
		{
			if (base.GameState == eGameState.Prepared)
			{
				this.m_gameState = eGameState.Loading;
				base.ClearWaitTimer();
				if (this.m_npcID != 0 && base.RoomType == eRoomType.Match)
				{
					NpcInfo npc = NPCInfoMgr.GetNpcInfoById(this.m_npcID);
					base.AddLoadingFile(2, npc.ResourcesPath, npc.ModelID);
				}
				base.SendStartLoading(60);
				base.AddAction(new WaitPlayerLoadingAction(this, 65000));
			}
		}
		public void StartGame()
		{
			if (base.GameState == eGameState.Loading)
			{
				this.m_gameState = eGameState.Playing;
				base.ClearWaitTimer();
				base.SendSyncLifeTime();
				List<Player> list = base.GetAllFightPlayers();
				MapPoint mapPos = MapMgr.GetMapRandomPos(this.m_map.Info.ID);
				GSPacketIn pkg = new GSPacketIn(91);
				pkg.WriteByte(99);
				pkg.WriteInt(list.Count);
				foreach (Player p in list)
				{
					p.Reset();
					Point pos = base.GetPlayerPoint(mapPos, p.Team);
					p.SetXY(pos);
					this.AddPlayer(p);
					p.StartFalling(true);
					p.StartGame();
					pkg.WriteInt(p.Id);
					pkg.WriteInt(p.X);
					pkg.WriteInt(p.Y);
					pkg.WriteInt(p.Direction);
					pkg.WriteInt(p.Blood);
				}
				base.SendToAll(pkg);
				if (this.m_npcID != 0 && base.RoomType == eRoomType.Match)
				{
					SimpleNpc npc = new SimpleNpc(this.physicalId++, this, NPCInfoMgr.GetNpcInfoById(this.m_npcID), 1, -1, 0);
					Point npcPos = base.GetPlayerPoint(mapPos, 3);
					npc.Reset();
					npc.SetXY(npcPos);
					this.AddLiving(npc);
					npc.StartFalling(false);
					npc.IsNoHole = true;
				}
				base.WaitTime(list.Count * 1000);
				base.OnGameStarted();
			}
		}
		public void NextTurn()
		{
			if (base.GameState == eGameState.Playing)
			{
				base.ClearWaitTimer();
				base.ClearDiedPhysicals();
				base.CheckBox();
				base.m_turnIndex++;
				base.UpdateWind(base.GetNextWind(), false);
				string msg = string.Empty;
				List<Box> newBoxes = base.CreateBox();
				List<Physics> list = this.m_map.GetAllPhysicalSafe();
				foreach (Physics p in list)
				{
					p.PrepareNewTurn();
				}
				this.lastTurnLiving = this.m_currentLiving;
				this.m_currentLiving = base.FindNextTurnedLiving();
				this.MinusDelays(this.m_currentLiving.Delay);
				this.m_currentLiving.PrepareSelfTurn();
				if (this.m_currentLiving.IsLiving && !base.CurrentLiving.IsFrost)
				{
					this.m_currentLiving.StartAttacking();
					base.SendSyncLifeTime();
					base.SendGameNextTurn(this.m_currentLiving, this, newBoxes);
					if (this.m_currentLiving.IsAttacking)
					{
						base.AddAction(new WaitLivingAttackingAction(this.m_currentLiving, base.m_turnIndex, (base.GetTurnWaitTime() + 28) * 1000));
					}
				}
				base.OnBeginNewTurn();
			}
		}
		public override bool TakeCard(Player player, bool isSysTake)
		{
			int index = 0;
			for (int i = 0; i < this.Cards.Length; i++)
			{
				if (this.Cards[i] == 0)
				{
					index = i;
					break;
				}
			}
			return this.TakeCard(player, index, isSysTake);
		}
		public override bool TakeCard(Player player, int index, bool isSysTake)
		{
			bool result;
			if (player.CanTakeOut == 0)
			{
				result = false;
			}
			else
			{
				if (!player.IsActive || index < 0 || index > this.Cards.Length || player.FinishTakeCard || this.Cards[index] > 0)
				{
					result = false;
				}
				else
				{
					player.CanTakeOut--;
					int gold = 0;
					int money = 0;
					int giftToken = 0;
					int templateID = 0;
					List<ItemInfo> infos = null;
					if (DropInventory.CardDrop(base.RoomType, ref infos))
					{
						if (infos != null)
						{
							foreach (ItemInfo info in infos)
							{
								templateID = info.Template.TemplateID;
								ItemInfo tempInfo = ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken);
								if (tempInfo != null && templateID > 0)
								{
									player.PlayerDetail.AddTemplate(tempInfo, eBageType.TempBag, info.Count);
								}
							}
						}
					}
					player.FinishTakeCard = true;
					this.Cards[index] = 1;
					int count = 0;
					int num = templateID;
					if (num <= -200)
					{
						if (num != -300)
						{
							if (num == -200)
							{
								count = money;
							}
						}
						else
						{
							count = giftToken;
						}
					}
					else
					{
						if (num != -100)
						{
							if (num == 0)
							{
								templateID = -100;
								count = 500;
							}
						}
						else
						{
							count = gold;
						}
					}
					player.PlayerDetail.AddGold(gold);
					player.PlayerDetail.AddMoney(money, LogMoneyType.Award, LogMoneyType.Award_TakeCard);
					player.PlayerDetail.AddGiftToken(giftToken);
					base.SendGamePlayerTakeCard(player, index, templateID, count, isSysTake);
					result = true;
				}
			}
			return result;
		}
		public void GameOver()
		{
			if (base.GameState == eGameState.Playing)
			{
				this.m_gameState = eGameState.GameOver;
				base.ClearWaitTimer();
				this.currentTurnTotalDamage = 0;
				List<Player> players = base.GetAllFightPlayers();
				int winTeam = this.GetWinTeam(players);
				int canBlueTakeOut = 0;
				int canRedTakeOut = 0;
				this.Cards = new int[8];
				foreach (Player p in players)
				{
					if (p.TotalHurt > 0)
					{
						if (p.Team == 1)
						{
							canRedTakeOut = 1;
						}
						else
						{
							canBlueTakeOut = 1;
						}
					}
				}
				GSPacketIn pkg = new GSPacketIn(91);
				pkg.WriteByte(100);
				pkg.WriteInt(base.PlayerCount);
				foreach (Player p in players)
				{
					p.LoadingProcess = 0;
					if (p.CurrentIsHitTarget)
					{
						p.TotalHitTargetCount++;
					}
					int rewardGP = 0;
					int gp = this.CalculateExperience(p, winTeam, ref rewardGP);
					p.GainGP = p.PlayerDetail.AddGP(gp);
					int offer = this.CalculateOffer(p, winTeam);
					p.GainOffer = p.PlayerDetail.AddOffer(offer);
					p.CanTakeOut = ((p.Team == 1) ? canRedTakeOut : canBlueTakeOut);
					pkg.WriteInt(p.Id);
					pkg.WriteBoolean(p.Team == winTeam);
					pkg.WriteInt(p.PlayerDetail.PlayerCharacter.Grade);
					pkg.WriteInt(p.PlayerDetail.PlayerCharacter.GP);
					pkg.WriteInt(p.TotalKill);
					pkg.WriteInt(p.TotalHurt);
					pkg.WriteInt(p.TotalHitTargetCount);
					pkg.WriteInt(p.TotalShootCount);
					pkg.WriteInt(p.GainGP);
					pkg.WriteInt(rewardGP);
					pkg.WriteInt(p.GainOffer);
					pkg.WriteInt(p.CanTakeOut);
				}
				int riches = this.CalculateGuildMatchResult(players, winTeam);
				pkg.WriteInt(riches);
				pkg.WriteInt(this.m_redTeam.Count);
				base.SendToAll(pkg);
				foreach (Player p in players)
				{
					p.PlayerDetail.OnGameOver(this, p.Team == winTeam, p.GainGP, p.PlayerDetail.IsArea, base.CoupleFight(p));
				}
				base.OnGameOverLog(base.RoomId, base.RoomType, base.GameType, 0, this.beginTime, DateTime.Now, this.beginPlayerCount, base.Map.Info.ID, this.teamAStr, this.teamBStr, "", winTeam, this.bossWarField);
				base.AddAction(new BaseAction(20000));
				base.OnGameOverred();
			}
		}
		private int GetWinTeam(List<Player> players)
		{
			int winTeam = -1;
			foreach (Player p in players)
			{
				if (p.IsLiving)
				{
					winTeam = p.Team;
					break;
				}
			}
			if (winTeam == -1)
			{
				if (this.CurrentPlayer != null)
				{
					winTeam = this.CurrentPlayer.Team;
				}
				else
				{
					if (base.CurrentLiving != null)
					{
						winTeam = base.CurrentLiving.Team;
					}
				}
			}
			return winTeam;
		}
		private int CalculateExperience(Player player, int winTeam, ref int reward)
		{
			int result;
			if (this.m_roomType == eRoomType.Match)
			{
				float againstTeamLevel = (player.Team == 1) ? this.m_blueAvgLevel : this.m_redAvgLevel;
				float againstTeamCount = (float)((player.Team == 1) ? this.m_blueTeam.Count : this.m_redTeam.Count);
				float disLevel = Math.Abs(againstTeamLevel - (float)player.PlayerDetail.PlayerCharacter.Grade);
				if (player.TotalHurt == 0)
				{
					if (againstTeamLevel - (float)player.PlayerDetail.PlayerCharacter.Grade >= 5f && this.TotalHurt > 0)
					{
						base.SendMessage(player.PlayerDetail, LanguageMgr.GetTranslation("GetGPreward", new object[0]), null, 2);
						reward = 200;
						result = 201;
					}
					else
					{
						result = 1;
					}
				}
				else
				{
					float winPlus = (float)((player.Team == winTeam) ? 2 : 0);
					player.TotalShootCount = ((player.TotalShootCount == 0) ? 1 : player.TotalShootCount);
					if (player.TotalShootCount < player.TotalHitTargetCount)
					{
						player.TotalShootCount = player.TotalHitTargetCount;
					}
					int maxHurt = (int)((player.Team == 1) ? ((float)this.m_blueTeam.Count * this.m_blueAvgLevel * 300f) : (this.m_redAvgLevel * (float)this.m_redTeam.Count * 300f));
					int totalHurt = (player.TotalHurt > maxHurt) ? maxHurt : player.TotalHurt;
					int gp = (int)Math.Ceiling(((double)winPlus + (double)totalHurt * 0.001 + (double)player.TotalKill * 0.5 + (double)(player.TotalHitTargetCount / player.TotalShootCount * 2)) * (double)againstTeamLevel * (0.9 + (double)(againstTeamCount - 1f) * 0.3));
					if (againstTeamLevel - (float)player.PlayerDetail.PlayerCharacter.Grade >= 5f && this.TotalHurt > 0)
					{
						base.SendMessage(player.PlayerDetail, LanguageMgr.GetTranslation("GetGPreward", new object[0]), null, 2);
						reward = 200;
						gp += 200;
					}
					gp = base.GainCoupleGP(player, gp);
					if (gp > 100000)
					{
						PVPGame.log.Error(string.Format("pvpgame ====== player.nickname : {0}, add gp : {1} ======== gp > 10000", player.PlayerDetail.PlayerCharacter.NickName, gp));
						PVPGame.log.Error(string.Format("pvpgame ====== player.nickname : {0}, parameters winPlus: {1}, totalHurt : {2}, totalKill : {3}, totalHitTargetCount : {4}, totalShootCount : {5}, againstTeamLevel : {6}, againstTeamCount : {7}", new object[]
						{
							player.PlayerDetail.PlayerCharacter.NickName,
							winPlus,
							player.TotalHurt,
							player.TotalKill,
							player.TotalHitTargetCount,
							player.TotalShootCount,
							againstTeamLevel,
							againstTeamCount
						}));
					}
					result = ((gp < 1) ? 1 : gp);
				}
			}
			else
			{
				result = 0;
			}
			return result;
		}
		private int CalculateOffer(Player player, int winTeam)
		{
			int result;
			if (base.RoomType != eRoomType.Match)
			{
				result = 0;
			}
			else
			{
				int appendOffer = 0;
				if (base.GameType == eGameType.Guild)
				{
					int againstTeamCount = (player.Team == 1) ? this.m_blueTeam.Count : this.m_redTeam.Count;
					if (player.Team == winTeam)
					{
						appendOffer = againstTeamCount;
					}
					else
					{
						appendOffer = (int)((double)againstTeamCount * 0.5);
					}
				}
				int baseOffer = player.GainOffer;
				int offer = (int)((double)((float)baseOffer + (float)appendOffer * player.PlayerDetail.GMOfferRate) * player.PlayerDetail.OfferPlusRate);
				offer -= player.KilledPunishmentOffer;
				if (offer > 1000)
				{
					PVPGame.log.Error(string.Format("pvegame ====== player.nickname : {0}, add offer : {1} ======== offer > 1000", player.PlayerDetail.PlayerCharacter.NickName, offer));
					PVPGame.log.Error(string.Format("pvegame ====== player.nickname : {0}, parameters RoomType : {1}, baseOffer : {2}, appendOffer : {3}, GMOfferRate : {4}, OfferPlusRate : {5}, KilledPunishmentOffer : {6}", new object[]
					{
						player.PlayerDetail.PlayerCharacter.NickName,
						base.RoomType,
						baseOffer,
						appendOffer,
						player.PlayerDetail.GMOfferRate,
						player.PlayerDetail.OfferPlusRate,
						player.KilledPunishmentOffer
					}));
				}
				result = offer;
			}
			return result;
		}
		private int CalculateGuildMatchResult(List<Player> players, int winTeam)
		{
			int result;
			if (base.RoomType == eRoomType.Match && base.GameType == eGameType.Guild)
			{
				StringBuilder winStr = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5", new object[0]));
				IGamePlayer winPlayer = null;
				IGamePlayer losePlayer = null;
				int teamTotalHurt = 0;
				foreach (Player p in players)
				{
					if (p.Team == winTeam)
					{
						winStr.Append(string.Format("[{0}]", p.PlayerDetail.PlayerCharacter.NickName));
						winPlayer = p.PlayerDetail;
						int maxHurt = (int)((p.Team == 1) ? ((float)this.m_blueTeam.Count * this.m_blueAvgLevel * 300f) : (this.m_redAvgLevel * (float)this.m_redTeam.Count * 300f));
						int totalHurt = (p.TotalHurt > maxHurt) ? maxHurt : p.TotalHurt;
						teamTotalHurt += totalHurt;
					}
					else
					{
						losePlayer = p.PlayerDetail;
					}
				}
				if (losePlayer != null)
				{
					winStr.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg1", new object[0]) + losePlayer.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg2", new object[0]));
					winPlayer.ConsortiaFight(winPlayer.PlayerCharacter.ConsortiaID, losePlayer.PlayerCharacter.ConsortiaID, base.RoomType, base.GameType, teamTotalHurt, players.Count);
					int riches = 0;
					int count = (winTeam == 1) ? this.m_blueTeam.Count : this.m_redTeam.Count;
					riches = (int)((float)(count + teamTotalHurt / 2000) * winPlayer.GMRichesRate * winPlayer.AuncherRichesRate);
					winPlayer.SendConsortiaFight(winPlayer.PlayerCharacter.ConsortiaID, riches, winStr.ToString());
					if (riches > 100000)
					{
						PVPGame.log.Error(string.Format("pvpgame ======= riches : {0}, count : {1}, teamTotalHurt : {2}, GMRichesRate : {3}, AuncherRichesRate : {4}", new object[]
						{
							riches,
							count,
							teamTotalHurt,
							winPlayer.GMRichesRate,
							winPlayer.AuncherRichesRate
						}));
					}
					foreach (Player p in players)
					{
						if (p.Team == winTeam)
						{
							p.PlayerDetail.AddRobRiches(riches);
						}
					}
					result = riches;
					return result;
				}
			}
			result = 0;
			return result;
		}
		public override void Stop()
		{
			if (base.GameState == eGameState.GameOver)
			{
				this.m_gameState = eGameState.Stopped;
				List<Player> players = base.GetAllFightPlayers();
				foreach (Player p in players)
				{
					if (p.IsActive && !p.FinishTakeCard && p.CanTakeOut > 0)
					{
						this.TakeCard(p, true);
					}
				}
				Dictionary<int, Player> players2;
				Monitor.Enter(players2 = this.m_players);
				try
				{
					this.m_players.Clear();
				}
				finally
				{
					Monitor.Exit(players2);
				}
				MapMgr.ReleaseMapInstance(this.m_map);
				base.Stop();
			}
		}
		public bool CanGameOver()
		{
			bool red = true;
			bool blue = true;
			foreach (Player p in this.m_redTeam)
			{
			//	Player p;
				if (p.IsLiving)
				{
					red = false;
					break;
				}
			}
			foreach (Player p in this.m_blueTeam)
			{
				//Player p;
				if (p.IsLiving)
				{
					blue = false;
					break;
				}
			}
			bool result;
			if (base.TurnIndex >= 1500)
			{
				Player[] allPlayers = base.GetAllPlayers();
				for (int i = 0; i < allPlayers.Length; i++)
				{
					Player p = allPlayers[i];
					p.Die();
				}
				result = true;
			}
			else
			{
				result = (red || blue);
			}
			return result;
		}
		public override Player RemovePlayer(IGamePlayer gp, bool IsKick)
		{
			Player player = base.RemovePlayer(gp, IsKick);
			if (player != null && base.GameState != eGameState.Loading)
			{
				if (player.IsLiving)
				{
					if (base.GameState == eGameState.Playing)
					{
						string msg = null;
						string msg2 = null;
						if (base.RoomType == eRoomType.Match)
						{
							if (base.GameType == eGameType.Guild)
							{
								msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", new object[]
								{
									gp.PlayerCharacter.Grade * 12,
									15
								});
								msg2 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", new object[]
								{
									gp.PlayerCharacter.NickName,
									gp.PlayerCharacter.Grade * 12,
									15
								});
							}
							else
							{
								if (base.GameType == eGameType.Free)
								{
									msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", new object[]
									{
										gp.PlayerCharacter.Grade * 12,
										5
									});
									msg2 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", new object[]
									{
										gp.PlayerCharacter.NickName,
										gp.PlayerCharacter.Grade * 12,
										5
									});
								}
							}
						}
						else
						{
							gp.RemoveGP(gp.PlayerCharacter.Grade * 12);
							msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg4", new object[]
							{
								gp.PlayerCharacter.Grade * 12
							});
							msg2 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg5", new object[]
							{
								gp.PlayerCharacter.NickName,
								gp.PlayerCharacter.Grade * 12
							});
						}
						base.SendMessage(gp, msg, msg2, 3);
					}
					if (base.GetSameTeam())
					{
						if (this.CurrentPlayer != null && this.CurrentPlayer != player)
						{
							base.CurrentLiving.StopAttacking();
							this.CheckState(0);
						}
					}
				}
				else
				{
					if (base.GameState == eGameState.Playing)
					{
						string msg3 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg1", new object[]
						{
							gp.PlayerCharacter.NickName
						});
						string msg4 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg8", new object[0]);
						if (msg3 != null)
						{
							int type = 3;
							GSPacketIn pkg = new GSPacketIn(3);
							pkg.WriteInt(type);
							pkg.WriteString(msg3);
							this.SendToTeam(pkg, player.Team);
						}
						if (msg4 != null)
						{
							int type = 3;
							GSPacketIn pkg = new GSPacketIn(3);
							pkg.WriteInt(type);
							pkg.WriteString(msg4);
							player.PlayerDetail.SendTCP(pkg);
						}
					}
					if (base.GetSameTeam())
					{
						if (this.CurrentPlayer != null && this.CurrentPlayer != player)
						{
							base.CurrentLiving.StopAttacking();
							this.CheckState(0);
						}
					}
				}
			}
			return player;
		}
		public override void CheckState(int delay)
		{
			base.AddAction(new CheckPVPGameStateAction(delay));
		}
	}
}
