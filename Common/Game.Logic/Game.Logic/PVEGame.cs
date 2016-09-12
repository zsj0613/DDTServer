using Bussiness;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.AI.Game;
using Game.Logic.AI.Mission;
using Game.Logic.LogEnum;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

using Game.Base.Managers;
namespace Game.Logic
{
	public class PVEGame : BaseGame
	{
		private new static LogProvider log => LogProvider.Default;
		private APVEGameControl m_gameAI = null;
		private AMissionControl m_missionAI = null;
		public int TotalMissionCount;
		
		
		
		
		public int SessionId;
		public bool IsWin;
		
		public int TotalCount;
		public int TotalTurn;
		public int Param1;
		public int Param2;
		public int Param3;
		public int Param4;
		public int Param5;
		public int Param6;
		public int Param7;
		public int Param8;
		public Living ParamLiving;
		public int TotalKillCount;
		public double TotalNpcExperience;
		public double TotalNpcGrade;
		private int BeginPlayersCount;
		private PveInfo m_info;
		private List<string> m_gameOverResources;
		public Dictionary<int, MissionInfo> Missions;
		private MapPoint mapPos;
		public int WantTryAgain;
		private eHardLevel m_hardLevel;
		private DateTime beginTime;
		private string m_IsBossType;
		private bool m_isPassDrama;
		private Dictionary<int, int> m_NpcTurnQueue = new Dictionary<int, int>();
		private List<Point> PlayersXY = new List<Point>();
		private MissionInfo m_missionInfo;
		private List<int> m_mapHistoryIds;
		public int[] BossCards;
		public int CanTakeCard;
		private int m_pveGameDelay;
		public Dictionary<int, int> NpcTurnQueue
		{
			get
			{
				return this.m_NpcTurnQueue;
			}
		}
		public PveInfo PveInfo
		{
			get
			{
				return this.m_info;
			}
		}
		public MissionInfo MissionInfo
		{
			get
			{
				return this.m_missionInfo;
			}
			set
			{
				this.m_missionInfo = value;
			}
		}
		public Player CurrentPlayer
		{
			get
			{
				return this.m_currentLiving as Player;
			}
		}
		public List<int> MapHistoryIds
		{
			get
			{
				return this.m_mapHistoryIds;
			}
			set
			{
				this.m_mapHistoryIds = value;
			}
		}
		public eHardLevel HandLevel
		{
			get
			{
				return this.m_hardLevel;
			}
		}
		public MapPoint MapPos
		{
			get
			{
				return this.mapPos;
			}
		}
		public string IsBossWar
		{
			get
			{
				return this.m_IsBossType;
			}
			set
			{
				this.m_IsBossType = value;
			}
		}
		public bool IsPassDrama
		{
			get
			{
				return this.m_isPassDrama;
			}
			set
			{
				this.m_isPassDrama = value;
			}
		}
		public List<string> GameOverResources
		{
			get
			{
				return this.m_gameOverResources;
			}
		}
		public int PveGameDelay
		{
			get
			{
				return this.m_pveGameDelay;
			}
			set
			{
				this.m_pveGameDelay = value;
			}
		}


        public PVEGame(int id, int roomId, PveInfo info, List<IGamePlayer> players, Map map, eRoomType roomType, eGameType gameType, int timeType, eHardLevel hardLevel) : base(id, roomId, map, roomType, gameType, timeType)
		{
			foreach (IGamePlayer player in players)
			{
				base.AddGamePlayer(player, new Player(player, this.physicalId++, this, 1)
				{
					Direction = (this.m_random.Next(0, 1) == 0) ? 1 : -1
				});
			}
			this.m_isPassDrama = false;
			this.m_info = info;
			this.BeginPlayersCount = players.Count;
			this.TotalKillCount = 0;
			this.TotalNpcGrade = 0.0;
			this.TotalNpcExperience = 0.0;
			this.TotalHurt = 0;
			this.ParamLiving = null;
			this.m_IsBossType = "";
			this.WantTryAgain = 0;
			this.SessionId = 0;
			this.m_gameOverResources = new List<string>();
			this.Missions = new Dictionary<int, MissionInfo>();
			this.m_mapHistoryIds = new List<int>();
			this.m_hardLevel = hardLevel;
			string script = this.GetScript(info, hardLevel);
			this.m_gameAI = (ScriptMgr.CreateInstance(script) as APVEGameControl);
			if (this.m_gameAI == null)
			{
				PVEGame.log.ErrorFormat("Can't create game ai :{0}", script);
				this.m_gameAI = SimplePVEGameControl.Simple;
			}
			this.m_gameAI.Game = this;
			try
			{
				this.m_gameAI.OnCreated();
			}
			catch (Exception ex)
			{
				PVEGame.log.ErrorFormat("game ai script OnCreated error:{0}", ex);
			}
			this.m_missionAI = SimpleMissionControl.Simple;
			this.beginTime = DateTime.Now;
			foreach (Player p in this.m_players.Values)
			{
				p.MissionEventHandle += new PlayerMissionEventHandle(this.m_missionAI.OnMissionEvent);
			}
		}
		
		private string GetScript(PveInfo pveInfo, eHardLevel hardLevel)
		{
			string script = string.Empty;
			switch (hardLevel)
			{
			case eHardLevel.Simple:
				script = pveInfo.SimpleGameScript;
				break;
			case eHardLevel.Normal:
				script = pveInfo.NormalGameScript;
				break;
			case eHardLevel.Hard:
				script = pveInfo.HardGameScript;
				break;
			case eHardLevel.Terror:
				script = pveInfo.TerrorGameScript;
				break;
			default:
				script = pveInfo.SimpleGameScript;
				break;
			}
			return script;
		}
		public string GetMissionIdStr(string missionIds, int randomCount)
		{
			string result;
			if (string.IsNullOrEmpty(missionIds))
			{
				result = "";
			}
			else
			{
				string[] ids = missionIds.Split(new char[]
				{
					','
				});
				if (ids.Length < randomCount)
				{
					result = "";
				}
				else
				{
					List<string> idList = new List<string>();
					int seed = ids.Length;
					int i = 0;
					while (i < randomCount)
					{
						int rand = base.Random.Next(seed);
						string id = ids[rand];
						if (!idList.Contains(id))
						{
							idList.Add(id);
							i++;
						}
					}
					StringBuilder sb = new StringBuilder();
					foreach (string s in idList)
					{
						sb.Append(s).Append(",");
					}
					result = sb.Remove(sb.Length - 1, 1).ToString();
				}
			}
			return result;
		}
		public void SetupMissions(string missionIds)
		{
			if (!string.IsNullOrEmpty(missionIds))
			{
				int i = 0;
				string[] ids = missionIds.Split(new char[]
				{
					','
				});
				string[] array = ids;
				for (int j = 0; j < array.Length; j++)
				{
					string id = array[j];
					i++;
					MissionInfo mi = MissionInfoMgr.GetMissionInfo(int.Parse(id));
					this.Missions.Add(i, mi);
				}
			}
		}
		
		
		public SimpleNpc CreateNpc(int npcId, int x, int y, int type)
		{
			return this.CreateNpc(npcId, x, y, type, -1);
		}
		public SimpleNpc CreateNpc(int npcId, int x, int y, int type, int direction)
		{
			return this.CreateNpc(npcId, x, y, type, direction, 100, 0);
		}
		public SimpleNpc CreateNpc(int npcId, int x, int y, int type, int direction, int rank)
		{
			return this.CreateNpc(npcId, x, y, type, direction, 100, rank);
		}
		public SimpleNpc CreateNpc(int npcId, int x, int y, int type, int direction, int bloodInver, int rank)
		{
			NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
			SimpleNpc npc = new SimpleNpc(this.physicalId++, this, npcInfo, type, direction, rank);
			npc.Reset();
			npc.Blood = npc.Blood / 100 * bloodInver;
			npc.SetXY(x, y);
			if (rank != -1 && !this.m_NpcTurnQueue.ContainsKey(rank))
			{
				this.m_NpcTurnQueue.Add(rank, this.m_pveGameDelay + rank * 2);
			}
			this.AddLiving(npc);
			npc.StartFalling(false);
			return npc;
		}
		public SimpleNpc CreateNpc(int npcId, int type)
		{
			NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
			SimpleNpc npc = new SimpleNpc(this.physicalId++, this, npcInfo, type, -1, 0);
			Point pos = base.GetPlayerPoint(this.mapPos, npcInfo.Camp);
			npc.Reset();
			npc.SetXY(pos);
			if (!this.m_NpcTurnQueue.ContainsKey(0))
			{
				this.m_NpcTurnQueue.Add(0, this.m_pveGameDelay);
			}
			this.AddLiving(npc);
			npc.StartFalling(false);
			return npc;
		}
		private void SetNPCProperty(SimpleBoss boss)
		{
			if (base.PlayerCount > 1)
			{
				Player fightPowerMax = null;
				foreach (Player p in this.m_players.Values)
				{
					if (fightPowerMax == null)
					{
						fightPowerMax = p;
					}
					else
					{
						if (p.PlayerDetail.PlayerCharacter.FightPower > fightPowerMax.PlayerDetail.PlayerCharacter.FightPower)
						{
							fightPowerMax = p;
						}
					}
					boss.MaxBlood += p.Blood;
				}
				boss.Blood = boss.MaxBlood;
			}
			else
			{
				boss.MaxBlood = boss.MaxBlood * base.GetTeamFightPower() / 2;
				boss.Blood = boss.MaxBlood;
				boss.BaseDamage = (double)((int)(boss.BaseDamage * (double)base.GetTeamFightPower() / 10.0));
				boss.BaseGuard = (double)((int)(boss.BaseGuard * (double)base.GetTeamFightPower() / 10.0));
				boss.Defence = (double)((int)(boss.Defence * (double)base.GetTeamFightPower() / 2.0));
				boss.Lucky = (double)((int)(boss.Lucky * (double)base.GetTeamFightPower() / 200.0));
			}
		}
		public SimpleBoss CreateBoss(int npcId, int type)
		{
			NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
			SimpleBoss boss = new SimpleBoss(this.physicalId++, this, npcInfo, -1, type);
			boss.Reset();
			if (npcId == 5001 || npcId == 5002 || npcId == 5003 || npcId == 5004)
			{
				this.SetNPCProperty(boss);
			}
			Point pos = base.GetPlayerPoint(this.mapPos, npcInfo.Camp);
			boss.SetXY(pos.X, pos.Y);
			this.AddLiving(boss);
			boss.StartFalling(false);
			return boss;
		}
		public SimpleWingNpc CreateWingNpc(int npcId, int x, int y, int type)
		{
			return this.CreateWingNpc(npcId, x, y, type, -1);
		}
		public SimpleWingNpc CreateWingNpc(int npcId, int x, int y, int type, int direction)
		{
			return this.CreateWingNpc(npcId, x, y, type, direction, 100, 0);
		}
		public SimpleWingNpc CreateWingNpc(int npcId, int x, int y, int type, int direction, int rank)
		{
			return this.CreateWingNpc(npcId, x, y, type, direction, 100, rank);
		}
		public SimpleWingNpc CreateWingNpc(int npcId, int x, int y, int type, int direction, int bloodInver, int rank)
		{
			NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
			SimpleWingNpc npc = new SimpleWingNpc(this.physicalId++, this, npcInfo, type, direction, rank);
			npc.Reset();
			npc.Blood = npc.Blood / 100 * bloodInver;
			npc.SetXY(x, y);
			if (!this.m_NpcTurnQueue.ContainsKey(rank))
			{
				this.m_NpcTurnQueue.Add(rank, this.m_pveGameDelay + rank * 10);
			}
			this.AddLiving(npc);
			npc.StartFalling(false);
			return npc;
		}
		public SimpleBoss CreateBoss(int npcId, int x, int y, int direction, int type, string createAction)
		{
			return this.CreateBoss(npcId, x, y, direction, type, 100, createAction);
		}
		public SimpleBoss CreateBoss(int npcId, int x, int y, int direction, int type)
		{
			return this.CreateBoss(npcId, x, y, direction, type, 100, null);
		}
		public SimpleBoss CreateBoss(int npcId, int x, int y, int direction, int type, int bloodInver)
		{
			return this.CreateBoss(npcId, x, y, direction, type, bloodInver, null);
		}
		public SimpleBoss CreateBoss(int npcId, int x, int y, int direction, int type, int bloodInver, string createAction)
		{
			NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
			SimpleBoss boss = new SimpleBoss(this.physicalId++, this, npcInfo, direction, type);
			boss.Reset();
			boss.CaeateAction = createAction;
			boss.Blood = boss.Blood / 100 * bloodInver;
			boss.SetXY(x, y);
			this.AddLiving(boss);
			boss.StartFalling(false);
			return boss;
		}
		public SimpleWingBoss CreateWingBoss(int npcId, int x, int y, int direction, int type)
		{
			return this.CreateWingBoss(npcId, x, y, direction, type, 100);
		}
		public SimpleWingBoss CreateWingBoss(int npcId, int x, int y, int direction, int type, int bloodInver)
		{
			NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
			SimpleWingBoss boss = new SimpleWingBoss(this.physicalId++, this, npcInfo, direction, type);
			boss.Reset();
			boss.Blood = boss.Blood / 100 * bloodInver;
			boss.SetXY(x, y);
			this.AddLiving(boss);
			boss.StartFalling(false);
			return boss;
		}
		
        public Box CreateBox(int x, int y, string model, ItemInfo item)
		{
			Box box = new Box(this.physicalId++, model, item);
			box.SetXY(x, y);
			this.m_map.AddPhysical(box);
			base.AddBox(box, true);
			return box;
		}
		public PhysicalObj CreatePhysicalObj(int x, int y, string name, string model, string defaultAction, int scaleX, int scaleY, int rotation)
		{
			return this.CreatePhysicalObj(x, y, name, model, defaultAction, scaleX, scaleY, rotation, 0);
		}
		public PhysicalObj CreatePhysicalObj(int x, int y, string name, string model, string defaultAction, int scaleX, int scaleY, int rotation, int layer)
		{
			PhysicalObj obj = new PhysicalObj(this.physicalId++, name, model, defaultAction, scaleX, scaleY, rotation);
			obj.SetXY(x, y);
			this.AddPhysicalObj(obj, true, layer);
			return obj;
		}
		public Layer Createlayer(int x, int y, string name, string model, string defaultAction, int scaleX, int scaleY, int rotation)
		{
			Layer obj = new Layer(this.physicalId++, name, model, defaultAction, scaleX, scaleY, rotation);
			obj.SetXY(x, y);
			this.AddPhysicalObj(obj, true, 0);
			return obj;
		}
		public Layer CreateTip(int x, int y, string name, string model, string defaultAction, int scaleX, int scaleY, int rotation, int type)
		{
			Layer obj = new Layer(this.physicalId++, name, model, defaultAction, scaleX, scaleY, rotation, type);
			obj.SetXY(x, y);
			this.AddPhysicalTip(obj, true);
			return obj;
		}
		public void ClearMissionData()
		{
			foreach (Living living in this.m_livings)
			{
				living.Dispose();
			}
			this.m_livings.Clear();
			base.TurnQueue.Clear();
			this.m_decklivings.Clear();
			if (this.m_map != null)
			{
				foreach (PhysicalObj obj in this.m_map.GetAllPhysicalObjSafe())
				{
					obj.Dispose();
				}
			}
			this.m_tempBox.Clear();
		}
		public void ChangeMissionDelay(int rankID, int delay)
		{
			if (this.m_NpcTurnQueue.ContainsKey(rankID))
			{
				this.m_NpcTurnQueue[rankID] = delay;
			}
		}
		public override void AddLiving(Living living)
		{
			base.AddLiving(living);
			living.Died += new LivingEventHandle(this.living_Died);
		}
		public override void AddPlayer(Player player)
		{
			base.AddPlayer(player);
			player.Died += new LivingEventHandle(this.living_Died);
		}
		private void living_Died(Living living)
		{
			if (base.CurrentLiving != null && base.CurrentLiving is Player)
			{
				if (!(living is Player) && living != base.CurrentLiving)
				{
					this.TotalKillCount++;
					this.TotalNpcExperience += (double)living.Experience;
					this.TotalNpcGrade += (double)living.Grade;
				}
			}
			if (living is SimpleBoss)
			{
				((SimpleBoss)living).DiedEvent();
				((SimpleBoss)living).DiedSay();
			}
			if (living is SimpleNpc)
			{
				((SimpleNpc)living).DiedEvent();
				((SimpleNpc)living).DiedSay();
			}
			if (living is Player && base.CurrentLiving is SimpleBoss)
			{
				((SimpleBoss)base.CurrentLiving).KillPlayerSay();
			}
		}
		public override bool CanAddPlayer()
		{
			Dictionary<int, Player> players;
			Monitor.Enter(players = this.m_players);
			bool result;
			try
			{
				result = (base.GameState == eGameState.SessionPrepared && this.m_players.Count < 4);
			}
			finally
			{
				Monitor.Exit(players);
			}
			return result;
		}
		public override Player AddPlayer(IGamePlayer gp)
		{
			Player result;
			if (this.CanAddPlayer())
			{
				Player fp = new Player(gp, this.physicalId++, this, 1);
				fp.Direction = ((this.m_random.Next(0, 1) == 0) ? 1 : -1);
				base.AddGamePlayer(gp, fp);
				this.SendCreateGameToSingle(this, gp);
				this.SendPlayerInfoInGame(this, gp, fp);
				result = fp;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public override Player RemovePlayer(IGamePlayer gp, bool isKick)
		{
			Player player = base.GetPlayer(gp);
			if (player != null)
			{
				string msg = null;
				if (base.GameState == eGameState.GameStart || base.GameState == eGameState.Playing)
				{
					msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg8", new object[0]);
				}
				string msg2 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg1", new object[]
				{
					gp.PlayerCharacter.NickName
				});
				base.SendMessage(gp, msg, msg2, 3);
				base.RemovePlayer(gp, isKick);
			}
			return player;
		}
		public void LoadResources(int[] npcIds)
		{
			if (npcIds != null && npcIds.Length != 0)
			{
				for (int i = 0; i < npcIds.Length; i++)
				{
					int npcId = npcIds[i];
					NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
					if (npcInfo == null)
					{
						PVEGame.log.Error("LoadResources npcInfo resoure is not exits");
					}
					else
					{
						base.AddLoadingFile(2, npcInfo.ResourcesPath, npcInfo.ModelID);
					}
				}
			}
		}
		public void LoadNpcGameOverResources(int[] npcIds)
		{
			if (npcIds != null && npcIds.Length != 0)
			{
				for (int i = 0; i < npcIds.Length; i++)
				{
					int npcId = npcIds[i];
					NpcInfo npcInfo = NPCInfoMgr.GetNpcInfoById(npcId);
					if (npcInfo == null)
					{
						PVEGame.log.Error("LoadGameOverResources npcInfo resoure is not exits");
					}
					else
					{
						this.m_gameOverResources.Add(npcInfo.ModelID);
					}
				}
			}
		}
		public void Prepare()
		{
			if (base.GameState == eGameState.Inited)
			{
				this.m_gameState = eGameState.Prepared;
				base.SendCreateGame();
				this.CheckState(0);
				try
				{
					this.m_gameAI.OnPrepated();
				}
				catch (Exception ex)
				{
					PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
				}
			}
		}
		public void PrepareNewSession()
		{
			if (base.GameState == eGameState.Prepared || base.GameState == eGameState.GameOver || base.GameState == eGameState.ALLSessionStopped)
			{
				this.m_gameState = eGameState.SessionPrepared;
				this.SessionId++;
				base.ClearLoadingFiles();
				this.ClearMissionData();
				this.m_gameOverResources.Clear();
				this.WantTryAgain = 0;
				this.BossCards = new int[8];
                PVEGame.log.Debug("SessionID" + this.SessionId.ToString());
                PVEGame.log.Debug("MissionsCount" + this.Missions.Count.ToString());
                this.m_missionInfo = this.Missions[this.SessionId];
				this.m_pveGameDelay = this.m_missionInfo.Delay;
				this.TotalCount = this.m_missionInfo.TotalCount;
				this.TotalTurn = this.m_missionInfo.TotalTurn;
				this.Param1 = this.m_missionInfo.Param1;
				this.Param2 = this.m_missionInfo.Param2;
				this.Param3 = -1;
				this.Param4 = -1;
				foreach (Player p in this.m_players.Values)
				{
					p.MissionEventHandle -= new PlayerMissionEventHandle(this.m_missionAI.OnMissionEvent);
				}
				this.m_missionAI = (ScriptMgr.CreateInstance(this.m_missionInfo.Script) as AMissionControl);
				foreach (Player p in this.m_players.Values)
				{
					p.MissionEventHandle += new PlayerMissionEventHandle(this.m_missionAI.OnMissionEvent);
				}
				if (this.m_missionAI == null)
				{
					PVEGame.log.ErrorFormat("Can't create game mission ai :{0}", this.m_missionInfo.Script);
					this.m_missionAI = SimpleMissionControl.Simple;
				}
				this.IsBossWar = "";
				this.m_missionAI.Game = this;
				try
				{
					this.m_missionAI.OnPrepareNewSession();
				}
				catch (Exception ex)
				{
					PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
				}
			}
		}
		public bool CanStartNewSession()
		{
			return base.m_turnIndex == 0 || this.IsAllReady();
		}
		public bool IsAllReady()
		{
			bool result;
			foreach (Player p in base.Players.Values)
			{
				if (!p.Ready)
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}
		public void StartLoading()
		{
			if (base.GameState == eGameState.SessionPrepared)
			{
				this.m_gameState = eGameState.Loading;
				base.m_turnIndex = 0;
				this.SendMissionInfo();
				base.SendStartLoading(60);
				base.AddAction(new WaitPlayerLoadingAction(this, 65000));
			}
		}
		public void StartGame()
		{
			if (base.GameState == eGameState.Loading)
			{
				this.m_gameState = eGameState.GameStart;
				this.SendSyncLifeTime();
				this.TotalKillCount = 0;
				this.TotalNpcGrade = 0.0;
				this.TotalNpcExperience = 0.0;
				this.TotalHurt = 0;
				this.PlayersXY.Clear();
				List<Player> list = base.GetAllFightPlayers();
				this.mapPos = MapMgr.GetPVEMapRandomPos(this.m_map.Info.ID);
				GSPacketIn pkg = new GSPacketIn(91);
				pkg.WriteByte(99);
				pkg.WriteInt(list.Count);
				foreach (Player p in list)
				{
					p.Reset();
					Point pos = base.GetPlayerPoint(this.mapPos, p.Team);
					this.PlayersXY.Add(pos);
					p.SetXY(pos);
					if (pos.X < 600)
					{
						p.Direction = 1;
					}
					else
					{
						p.Direction = -1;
					}
				}
				try
				{
					this.m_missionAI.OnPrepareStartGame();
				}
				catch (Exception ex)
				{
					PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
				}
				foreach (Player p in list)
				{
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
				try
				{
					this.m_missionAI.OnStartGame();
				}
				catch (Exception ex)
				{
					PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
				}
				base.OnGameStarted();
				this.SendUpdateUiData();
				base.WaitTime(base.PlayerCount * 2500 + 1000);
			}
		}
		public void PrepareFightingLivings()
		{
			if (base.GameState == eGameState.GameStart)
			{
				this.m_gameState = eGameState.Playing;
				this.SendSyncLifeTime();
				base.WaitTime(base.PlayerCount * 1000);
				try
				{
					this.m_missionAI.OnPrepareNewGame();
				}
				catch (Exception ex)
				{
					PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
				}
			}
		}
		public void NextTurn()
		{
			if (base.GameState == eGameState.Playing)
			{
				this.IsPassDrama = false;
				base.ClearWaitTimer();
				base.ClearDiedPhysicals();
				base.CheckBox();
				this.ConfigLivingSayRule();
				string msg = string.Empty;
				List<Box> newBoxes = base.CreateBox();
				List<Physics> list = this.m_map.GetAllPhysicalSafe();
				foreach (Physics p in list)
				{
					p.PrepareNewTurn();
				}
				this.lastTurnLiving = this.m_currentLiving;
				this.m_currentLiving = base.FindNextTurnedLiving();
				try
				{
					this.m_missionAI.OnNewTurnStarted();
				}
				catch (Exception ex)
				{
					PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
				}
				if (this.m_currentLiving != null)
				{
					base.m_turnIndex++;
					this.SendUpdateUiData();
					List<Living> livings = base.GetLivedLivings();
					int turnNpcRank = this.FindTurnNpcRank();
					if (livings.Count > 0 && this.m_NpcTurnQueue.Count > 0 && this.m_currentLiving.Delay > this.m_NpcTurnQueue[turnNpcRank])
					{
						this.MinusDelays(this.m_NpcTurnQueue[turnNpcRank]);
						bool isSend = false;
						foreach (Living living in this.m_livings)
						{
							if (((SimpleNpc)living).Rank == turnNpcRank)
							{
								living.PrepareSelfTurn();
								if (!living.IsFrost)
								{
									living.StartAttacking();
								}
								if (!isSend)
								{
									base.SendGameNextTurn(living, this, newBoxes);
									isSend = true;
								}
							}
						}
						foreach (Living living in this.m_livings)
						{
							if (((SimpleNpc)living).Rank == turnNpcRank)
							{
								if (living.IsAttacking)
								{
									living.StopAttacking();
								}
							}
						}
						Dictionary<int, int> npcTurnQueue;
						int key;
						(npcTurnQueue = this.m_NpcTurnQueue)[key = turnNpcRank] = npcTurnQueue[key] + this.MissionInfo.IncrementDelay;
						this.CheckState(0);
					}
					else
					{
						this.MinusDelays(this.m_currentLiving.Delay);
						base.UpdateWind(base.GetNextWind(), false);
						this.m_currentLiving.PrepareSelfTurn();
						if (this.m_currentLiving.IsLiving && !this.m_currentLiving.IsFrost)
						{
							this.m_currentLiving.StartAttacking();
							this.SendSyncLifeTime();
							base.SendGameNextTurn(this.m_currentLiving, this, newBoxes);
							if (this.m_currentLiving.IsAttacking)
							{
								base.AddAction(new WaitLivingAttackingAction(this.m_currentLiving, base.m_turnIndex, (base.GetTurnWaitTime() + 28) * 1000));
							}
						}
					}
				}
				base.OnBeginNewTurn();
				try
				{
					this.m_missionAI.OnBeginNewTurn();
				}
				catch (Exception ex)
				{
					PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
				}
			}
		}
		public void ConfigLivingSayRule()
		{
			if (this.m_livings != null && this.m_livings.Count != 0)
			{
				int livCount = this.m_livings.Count;
				foreach (Living living in this.m_livings)
				{
					living.CanSay = false;
				}
				if (base.TurnIndex % 2 != 0)
				{
					int sayCount;
					if (livCount <= 5)
					{
						sayCount = base.Random.Next(0, 2);
					}
					else
					{
						if (livCount > 5 && livCount <= 10)
						{
							sayCount = base.Random.Next(1, 3);
						}
						else
						{
							sayCount = base.Random.Next(1, 4);
						}
					}
					if (sayCount > 0)
					{
						int[] sayIndexs = new int[sayCount];
						int i = 0;
						while (i < sayCount)
						{
							int index = base.Random.Next(0, livCount);
							if (!this.m_livings[index].CanSay)
							{
								this.m_livings[index].CanSay = true;
								i++;
							}
						}
					}
				}
			}
		}
		public bool CanCampAttack(Living beatLiving, Living killLiving)
		{
			bool isAttack = false;
			int beatCamp = this.GetLivingCamp(beatLiving);
			int killCamp = this.GetLivingCamp(killLiving);
			if (beatCamp == 0 || beatCamp == 1 || beatCamp == 3)
			{
				isAttack = (killCamp == 0 || killCamp == 1 || killCamp == 2);
			}
			if (beatCamp == 2)
			{
				isAttack = (killCamp != 2 && killCamp != 4);
			}
			return isAttack;
		}
		public bool CanAddBlood(Living addBloodLiving, Living byaddLiving)
		{
			bool isAddBlood = false;
			int addBlood = this.GetLivingCamp(addBloodLiving);
			int byaddBlood = this.GetLivingCamp(byaddLiving);
			if (addBlood == 0 || addBlood == 1 || addBlood == 3)
			{
				isAddBlood = (byaddBlood != 2);
			}
			if (addBlood == 2)
			{
				isAddBlood = (byaddBlood == 2 || byaddBlood == 4);
			}
			return isAddBlood;
		}
		public int GetLivingCamp(Living living)
		{
			int camp = 0;
			if (living is Player)
			{
				camp = 0;
			}
			if (living is SimpleNpc)
			{
				SimpleNpc npc = (SimpleNpc)living;
				camp = npc.NpcInfo.Camp;
			}
			if (living is SimpleWingNpc)
			{
				SimpleWingNpc npc2 = (SimpleWingNpc)living;
				camp = npc2.NpcInfo.Camp;
			}
			if (living is SimpleBoss)
			{
				SimpleBoss boss = (SimpleBoss)living;
				camp = boss.NpcInfo.Camp;
			}
			if (living is SimpleWingBoss)
			{
				SimpleWingBoss boss2 = (SimpleWingBoss)living;
				camp = boss2.NpcInfo.Camp;
			}
			return camp;
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
					int gold = 0;
					int money = 0;
					int giftToken = 0;
					int templateID = 0;
					List<ItemInfo> infos = null;
					if (DropInventory.CopyUserDrop(this.m_missionInfo.Id, ref infos))
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
					if (base.RoomType == eRoomType.Treasure || base.RoomType == eRoomType.Boss || base.RoomType == eRoomType.Training)
					{
						player.CanTakeOut--;
						if (player.CanTakeOut == 0)
						{
							player.FinishTakeCard = true;
						}
					}
					else
					{
						player.FinishTakeCard = true;
					}
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
		public bool CanGameOver()
		{
			bool result;
			if (base.PlayerCount == 0)
			{
				result = true;
			}
			else
			{
				if (base.TurnIndex > this.TotalTurn - 1)
				{
					this.IsWin = false;
					result = true;
				}
				else
				{
					if (base.GetDiedPlayerCount() == base.PlayerCount)
					{
						this.IsWin = false;
						result = true;
					}
					else
					{
						try
						{
							result = this.m_missionAI.CanGameOver();
							return result;
						}
						catch (Exception ex)
						{
							PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
						}
						result = true;
					}
				}
			}
			return result;
		}
		public void PrepareGameOver()
		{
			if (base.GameState == eGameState.Playing)
			{
				this.m_gameState = eGameState.PrepareGameOver;
				try
				{
					this.m_missionAI.OnPrepareGameOver();
				}
				catch (Exception ex)
				{
					PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
				}
			}
		}
		public void GameOver()
		{
			if (base.GameState == eGameState.PrepareGameOver)
			{
				this.m_gameState = eGameState.GameOver;
				this.SendSyncLifeTime();
				this.SendUpdateUiData();
				try
				{
					this.m_missionAI.OnGameOver();
				}
				catch (Exception ex)
				{
					PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
				}
				List<Player> players = base.GetAllFightPlayers();
				this.currentTurnTotalDamage = 0;
				bool canEnterFinall = false;
				bool hasNextSess = this.HasNextSession();
				if (this.IsWin)
				{
					if (this.MissionInfo.TakeCard == 2)
					{
						this.Cards = new int[21];
						if (hasNextSess)
						{
							base.AddAction(new GameShowCardAction(this, 25000, 1000));
						}
					}
					else
					{
						if (this.MissionInfo.TakeCard == 1)
						{
							this.Cards = new int[8];
						}
					}
					this.CanTakeCard = this.MissionInfo.TakeCard;
				}
				GSPacketIn pkg = new GSPacketIn(91);
				pkg.WriteByte(112);
				pkg.WriteInt(this.CanTakeCard);
				pkg.WriteBoolean(hasNextSess);
                int a = this.SessionId + 1;
                pkg.WriteString("Show" + a.ToString() + ".jpg");
				pkg.WriteBoolean(canEnterFinall);
				pkg.WriteInt(base.PlayerCount);
				foreach (Player p in players)
				{
					p.LoadingProcess = 0;
					if (p.CurrentIsHitTarget)
					{
						p.TotalHitTargetCount++;
					}
					int experience = this.CalculateExperience(p);
					int score = this.CalculateScore(p);
					int rate = this.m_missionAI.CalculateScoreGrade(score);
					int hitRate = this.CalculateHitRate(p.TotalHitTargetCount, p.TotalShootCount);
					p.TotalAllHurt += p.TotalHurt;
					p.TotalAllCure += p.TotalCure;
					p.TotalAllHitTargetCount += p.TotalHitTargetCount;
					p.TotalAllShootCount += p.TotalShootCount;
					p.GainGP = p.PlayerDetail.AddGP(experience);
					p.TotalAllExperience += p.GainGP;
					p.TotalAllScore += score;
                    p.CanTakeOut = CanTakeCard == 2? p.PlayerDetail.PlayerCharacter.VipLevel > 5 ? 4 : p.PlayerDetail.PlayerCharacter.VipLevel > 0 ? 3 : 2:CanTakeCard;
                    pkg.WriteInt(p.PlayerDetail.PlayerCharacter.ID);
					pkg.WriteInt(p.PlayerDetail.PlayerCharacter.Grade);
					pkg.WriteInt(p.GainGP);
					pkg.WriteInt(p.TotalHurt);
					pkg.WriteInt(p.TotalCure);
					pkg.WriteInt(hitRate);
					pkg.WriteInt(score);
					pkg.WriteInt(rate);
					pkg.WriteBoolean(this.IsWin);
					pkg.WriteInt(p.CanTakeOut);
				}
				if (this.CanTakeCard > 0)
				{
					pkg.WriteInt(this.m_gameOverResources.Count);
					foreach (string res in this.m_gameOverResources)
					{
						pkg.WriteString(res);
					}
				}
				base.SendToAll(pkg);
				StringBuilder sb = new StringBuilder();
				foreach (Player p in players)
				{
					sb.Append(p.PlayerDetail.PlayerCharacter.ID).Append(",");
					p.Ready = false;
					p.PlayerDetail.OnMissionOver(p.Game, this.IsWin, this.MissionInfo.Id, p.TurnNum);
				}
				int winTeam = this.IsWin ? 1 : 2;
				string teamAStr = sb.ToString();
				string teamBStr = "";
				string dropTemplateIdsStr = "";
				StringBuilder BossWarRecord = new StringBuilder();
				if (this.IsWin && this.IsBossWar != "")
				{
					BossWarRecord.Append(this.IsBossWar).Append(",");
					foreach (Player p in players)
					{
						BossWarRecord.Append("玩家ID:").Append(p.PlayerDetail.PlayerCharacter.ID).Append(",");
						BossWarRecord.Append("等级:").Append(p.PlayerDetail.PlayerCharacter.Grade).Append(",");
						BossWarRecord.Append("攻击回合数:").Append(p.TurnNum).Append(",");
						BossWarRecord.Append("攻击:").Append(p.PlayerDetail.PlayerCharacter.Attack).Append(",");
						BossWarRecord.Append("防御:").Append(p.PlayerDetail.PlayerCharacter.Defence).Append(",");
						BossWarRecord.Append("敏捷:").Append(p.PlayerDetail.PlayerCharacter.Agility).Append(",");
						BossWarRecord.Append("幸运:").Append(p.PlayerDetail.PlayerCharacter.Luck).Append(",");
						BossWarRecord.Append("伤害:").Append(p.PlayerDetail.GetBaseAttack()).Append(",");
						BossWarRecord.Append("总血量:").Append(p.MaxBlood).Append(",");
						BossWarRecord.Append("护甲:").Append(p.PlayerDetail.GetBaseDefence()).Append(",");
						if (p.PlayerDetail.SecondWeapon != null)
						{
							BossWarRecord.Append("副武器:").Append(p.PlayerDetail.SecondWeapon.TemplateID).Append(",");
							BossWarRecord.Append("副武器强化等级:").Append(p.PlayerDetail.SecondWeapon.StrengthenLevel).Append(".");
						}
					}
				}
				this.bossWarField = BossWarRecord.ToString();
				base.OnGameOverLog(base.RoomId, base.RoomType, base.GameType, 0, this.beginTime, DateTime.Now, this.BeginPlayersCount, this.MissionInfo.Id, teamAStr, teamBStr, dropTemplateIdsStr, winTeam, this.bossWarField);
				if (this.CanTakeCard == 2 && this.HasNextSession())
				{
					base.WaitTime(25000);
				}
				else
				{
					if (this.CanTakeCard == 1 && this.HasNextSession())
					{
						base.WaitTime(10000);
					}
					else
					{
						base.WaitTime(2000);
					}
				}
				base.OnGameOverred();
				if (!this.IsWin)
				{
					base.OnRest();
				}
			}
		}
		public bool HasNextSession()
		{
			bool result;
			if (base.PlayerCount == 0 || !this.IsWin)
			{
				result = false;
			}
			else
			{
				int nextSessionId = this.SessionId + 1;
				result = this.Missions.ContainsKey(nextSessionId);
			}
			return result;
		}
		public void GameOverAllSession()
		{
			if (base.GameState == eGameState.GameOver)
			{
				this.m_gameState = eGameState.ALLSessionStopped;
				try
				{
					this.m_gameAI.OnGameOverAllSession();
				}
				catch (Exception ex)
				{
					PVEGame.log.ErrorFormat("game ai script {0} error:{1}", base.GameState, ex);
				}
				List<Player> players = base.GetAllFightPlayers();
				this.SendSyncLifeTime();
				GSPacketIn pkg = new GSPacketIn(91);
				pkg.WriteByte(115);
				foreach (Player p in players)
				{
					p.PlayerDetail.OnGameOver(this, this.IsWin, p.GainGP, false, base.CoupleFight(p));
				}
				pkg.WriteInt(base.PlayerCount);
				foreach (Player p in players)
				{
					int hitRate = this.CalculateHitRate(p.TotalAllHitTargetCount, p.TotalAllShootCount);
					int rate = this.m_gameAI.CalculateScoreGrade(p.TotalAllScore);
					pkg.WriteInt(p.PlayerDetail.PlayerCharacter.ID);
					pkg.WriteInt(p.PlayerDetail.PlayerCharacter.Grade);
					pkg.WriteInt(p.TotalAllExperience);
					pkg.WriteInt(p.TotalAllHurt);
					pkg.WriteInt(p.TotalAllCure);
					pkg.WriteInt(hitRate);
					pkg.WriteInt(rate);
					pkg.WriteBoolean(this.IsWin);
				}
				pkg.WriteInt(this.m_gameOverResources.Count);
				foreach (string res in this.m_gameOverResources)
				{
					pkg.WriteString(res);
				}
				base.SendToAll(pkg);
				if (this.m_info.Type == 5)
				{
					base.AddAction(new BaseAction(800));
				}
				else
				{
					if (this.CanTakeCard == 2)
					{
						base.AddAction(new BaseAction(25000));
					}
					else
					{
						base.AddAction(new BaseAction(2000));
					}
				}
				this.CanStopGame();
				base.OnRest();
			}
		}
		public void CanStopGame()
		{
			if (!this.IsWin)
			{
			}
		}
		public override void Stop()
		{
			if (base.GameState == eGameState.ALLSessionStopped)
			{
				this.m_gameState = eGameState.Stopped;
				if (this.IsWin)
				{
					List<Player> players = base.GetAllFightPlayers();
					foreach (Player p in players)
					{
						if (p.IsActive && p.CanTakeOut > 0)
						{
							p.HasPaymentTakeCard = true;
							int left = p.CanTakeOut;
							for (int i = 0; i < left; i++)
							{
								this.TakeCard(p, true);
							}
						}
					}
					this.SendSyncLifeTime();
					if (base.RoomType == eRoomType.Treasure || base.RoomType == eRoomType.Boss || base.RoomType == eRoomType.Training)
					{
						this.SendShowCards();
					}
					if (base.GameType == eGameType.Treasure)
					{
						foreach (Player p in players)
						{
							p.PlayerDetail.SetPvePermission(this.m_info.ID, this.m_hardLevel);
						}
					}
					if (base.GameType == eGameType.FightLab)
					{
						foreach (Player p in players)
						{
							p.PlayerDetail.SetFightLabPermission(this.m_info.ID, this.m_hardLevel, this.MissionInfo.Id);
						}
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
				base.OnGameStopped();
			}
		}
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			foreach (Living living in this.m_livings)
			{
				living.Dispose();
			}
			try
			{
				this.m_missionAI.Dispose();
			}
			catch (Exception ex)
			{
				PVEGame.log.ErrorFormat("game ai script m_missionAI.Dispose() error:{1}", ex);
			}
		}
		private int CalculateExperience(Player p)
		{
			int result;
			if (this.TotalKillCount == 0)
			{
				result = 1;
			}
			else
			{
				double gradeGap = Math.Abs((double)p.Grade - this.TotalNpcGrade / (double)this.TotalKillCount);
				if (gradeGap >= 7.0)
				{
					result = 1;
				}
				else
				{
					double behaveRevisal = 0.0;
					if (this.TotalKillCount > 0)
					{
						behaveRevisal += (double)p.TotalKill / (double)this.TotalKillCount * 0.4;
					}
					if (this.TotalHurt > 0)
					{
						behaveRevisal += (double)p.TotalHurt / (double)this.TotalHurt * 0.4;
					}
					if (p.IsLiving)
					{
						behaveRevisal += 0.4;
					}
					double gradeGapRevisal = 1.0;
					if (gradeGap >= 3.0 && gradeGap <= 4.0)
					{
						gradeGapRevisal = 0.7;
					}
					else
					{
						if (gradeGap >= 5.0 && gradeGap <= 6.0)
						{
							gradeGapRevisal = 0.4;
						}
					}
					double playerCountRevisal = (0.9 + (double)(this.BeginPlayersCount - 1) * 0.4) / (double)base.PlayerCount;
					double experience = this.TotalNpcExperience * behaveRevisal * gradeGapRevisal * playerCountRevisal;
					experience = ((experience < 1.0) ? 1.0 : experience);
					experience = (double)base.GainCoupleGP(p, (int)experience);
					result = (int)experience;
				}
			}
			return result;
		}
		private int CalculateScore(Player p)
		{
			int score = 2000 - base.TurnIndex * 5 + (int)((double)p.Blood / (double)p.MaxBlood) * 50;
			if (!this.IsWin)
			{
				score = 0;
			}
			return score;
		}
		private int CalculateHitRate(int hitTargetCount, int shootCount)
		{
			double toHit = 0.0;
			if (shootCount > 0)
			{
				toHit = (double)hitTargetCount / (double)shootCount;
			}
			return (int)(toHit * 100.0);
		}
		public override void CheckState(int delay)
		{
			base.AddAction(new CheckPVEGameStateAction(delay));
		}
		public bool TakeBossCard(Player player)
		{
			int index = 0;
			for (int i = 0; i < this.BossCards.Length; i++)
			{
				if (this.BossCards[i] == 0)
				{
					index = i;
					break;
				}
			}
			return this.TakeBossCard(player, index);
		}
		public bool TakeBossCard(Player player, int index)
		{
			bool result;
			if (!player.IsActive || player.CanTakeOut <= 0 || index < 0 || index > this.BossCards.Length || this.BossCards[index] > 0)
			{
				result = false;
			}
			else
			{
				int gold = 0;
				int money = 0;
				int giftToken = 0;
				int templateID = 0;
				List<ItemInfo> infos = null;
				int missId = this.Missions[this.SessionId].Id;
				if (DropInventory.BossDrop(missId, ref infos))
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
				player.CanTakeOut--;
				this.BossCards[index] = 1;
				int count = 0;
				if (templateID == -300)
				{
					count = giftToken;
				}
				else
				{
					if (templateID == -200)
					{
						count = money;
					}
					else
					{
						if (templateID == -100)
						{
							count = gold;
						}
						else
						{
							if (templateID == 0)
							{
								templateID = -100;
								gold = 100;
								count = 2000;
							}
						}
					}
				}
				player.PlayerDetail.AddGold(gold);
				player.PlayerDetail.AddMoney(money, LogMoneyType.Award, LogMoneyType.Award_TakeCard);
				player.PlayerDetail.AddGiftToken(giftToken);
				base.SendGamePlayerTakeCard(player, index, templateID, count, false);
				result = true;
			}
			return result;
		}
		public bool isTankCard()
		{
			bool result;
			foreach (Player player in this.m_players.Values)
			{
				if (player.CanTakeOut != 0)
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}
		public void SendMissionInfo()
		{
			if (this.m_missionInfo != null)
			{
				GSPacketIn pkg = new GSPacketIn(91);
				pkg.WriteByte(113);
				pkg.WriteString(this.m_missionInfo.Name);
				pkg.WriteString(this.m_missionInfo.Success);
				pkg.WriteString(this.m_missionInfo.Failure);
				pkg.WriteString(this.m_missionInfo.Description);
				pkg.WriteString(this.m_missionInfo.Title);
				pkg.WriteInt(this.TotalMissionCount);
				pkg.WriteInt(this.SessionId);
				pkg.WriteInt(this.TotalTurn);
				pkg.WriteInt(this.TotalCount);
				pkg.WriteInt(this.Param1);
				pkg.WriteInt(this.Param2);
				base.SendToAll(pkg);
			}
		}
		public void SendUpdateUiData()
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(104);
			int count = 0;
			try
			{
				count = this.m_missionAI.UpdateUIData();
			}
			catch (Exception ex)
			{
				PVEGame.log.ErrorFormat("game ai script {0} error:{1}", string.Format("m_missionAI.UpdateUIData()", new object[0]), ex);
			}
			pkg.WriteInt(base.TurnIndex);
			pkg.WriteInt(count);
			pkg.WriteInt(this.Param3);
			pkg.WriteInt(this.Param4);
			base.SendToAll(pkg);
		}
		internal void SendShowCards()
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(89);
			int count = 0;
			List<int> cardIndexs = new List<int>();
			for (int i = 0; i < this.Cards.Length; i++)
			{
				if (this.Cards[i] == 0)
				{
					cardIndexs.Add(i);
					count++;
				}
			}
			pkg.WriteInt(count);
			List<ItemInfo> infos = DropInventory.CopySystemDrop(this.m_missionInfo.Id, cardIndexs.Count);
			int awardCount = infos.Count;
			foreach (int index in cardIndexs)
			{
				pkg.WriteByte((byte)index);
				awardCount--;
				if (awardCount < 0)
				{
					pkg.WriteInt(-100);
					pkg.WriteInt(100);
				}
				else
				{
					pkg.WriteInt(infos[awardCount].Template.TemplateID);
					pkg.WriteInt(infos[awardCount].Count);
				}
			}
			base.SendToAll(pkg);
		}
		public void SendGameObjectFocus(int type, string name, int delay, int finishTime)
		{
			Physics[] physics = base.FindPhysicalObjByName(name);
			Physics[] array = physics;
			for (int i = 0; i < array.Length; i++)
			{
				Physics p = array[i];
				base.AddAction(new FocusAction(p.X, p.Y, type, delay, finishTime));
			}
		}
		public void SendLivingToTop(Living living)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(70);
			pkg.WriteInt(living.Id);
			base.SendToAll(pkg);
		}
		private void SendCreateGameToSingle(PVEGame game, IGamePlayer gamePlayer)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(121);
			pkg.WriteInt(game.Map.Info.ID);
			pkg.WriteInt((int)((byte)game.RoomType));
			pkg.WriteInt((int)((byte)game.GameType));
			pkg.WriteInt(game.TimeType);
			List<Player> players = game.GetAllFightPlayers();
			pkg.WriteInt(players.Count);
			foreach (Player p in players)
			{
				IGamePlayer gp = p.PlayerDetail;
				pkg.WriteInt(gp.PlayerCharacter.ID);
				pkg.WriteString(gp.PlayerCharacter.NickName);
				pkg.WriteBoolean(gp.PlayerCharacter.Sex);
				pkg.WriteInt(gp.PlayerCharacter.Hide);
				pkg.WriteString(gp.PlayerCharacter.Style);
				pkg.WriteString(gp.PlayerCharacter.Colors);
				pkg.WriteString(gp.PlayerCharacter.Skin);
				pkg.WriteInt(gp.PlayerCharacter.Grade);
				pkg.WriteInt(gp.PlayerCharacter.Repute);
				if (gp.MainWeapon == null)
				{
					pkg.WriteInt(0);
				}
				else
				{
					pkg.WriteInt(gp.MainWeapon.TemplateID);
				}
				if (gp.SecondWeapon == null)
				{
					pkg.WriteInt(0);
				}
				else
				{
					pkg.WriteInt(gp.SecondWeapon.TemplateID);
				}

                //add  
                /*
                   sp.Nimbus = pkg.readInt();
                 */
                pkg.WriteInt(gp.PlayerCharacter.Nimbus);
                //

				pkg.WriteInt(gp.PlayerCharacter.ConsortiaID);
				pkg.WriteString(gp.PlayerCharacter.ConsortiaName);
				pkg.WriteInt(gp.PlayerCharacter.ConsortiaLevel);
				pkg.WriteInt(gp.PlayerCharacter.ConsortiaRepute);
                //add
                /*
                					sp.WinCount = pkg.readInt();
					sp.TotalCount = pkg.readInt();
					sp.FightPower = pkg.readInt();
					
					sp.AchievementPoint = pkg.readInt();
					sp.honor			= pkg.readUTF();
					
					sp.commitChanges();
					
					//2.1版本显示结婚图标需求所需数据
					fp.info.IsMarried = pkg.readBoolean();
					if(fp.info.IsMarried)
					{
						fp.info.SpouseID = pkg.readInt();
						fp.info.SpouseName = pkg.readUTF();
					}
					
                fp.AdditionInfo.resetAddition();

                fp.AdditionInfo.GMExperienceAdditionType = Number(pkg.readInt() / 100);
                fp.AdditionInfo.AuncherExperienceAddition = Number(pkg.readInt() / 100);

                fp.AdditionInfo.GMOfferAddition = Number(pkg.readInt() / 100);
                fp.AdditionInfo.AuncherOfferAddition = Number(pkg.readInt() / 100);

                fp.AdditionInfo.GMRichesAddition = Number(pkg.readInt() / 100);
                fp.AdditionInfo.AuncherRichesAddition = Number(pkg.readInt() / 100);
                */
                pkg.WriteInt(gp.PlayerCharacter.Win);
                pkg.WriteInt(gp.PlayerCharacter.Total);
                pkg.WriteInt(gp.PlayerCharacter.FightPower);
                pkg.WriteInt(gp.PlayerCharacter.AchievementPoint);
                pkg.WriteString(gp.PlayerCharacter.Honor);
                bool a = gp.PlayerCharacter.IsMarried;
                pkg.WriteBoolean(a);
                if (a)
                {
                    pkg.WriteInt(gp.PlayerCharacter.SpouseID);
                    pkg.WriteString(gp.PlayerCharacter.SpouseName);
                }
                pkg.WriteInt(Convert.ToInt32(gp.GMExperienceRate * 100));
                pkg.WriteInt(Convert.ToInt32(gp.AuncherExperienceRate * 100));
                pkg.WriteInt(Convert.ToInt32(gp.GMOfferRate * 100));
                pkg.WriteInt(Convert.ToInt32(gp.AuncherOfferRate * 100));
                pkg.WriteInt(Convert.ToInt32(gp.GMRichesRate * 100));
                pkg.WriteInt(Convert.ToInt32(gp.AuncherRichesRate * 100));



                //
                pkg.WriteInt(p.Team);
				pkg.WriteInt(p.Id);
				pkg.WriteInt(p.MaxBlood);
				pkg.WriteBoolean(p.Ready);
			}
			int index = game.SessionId - 1;
			if (index == 0 || index > this.Missions.Count)
			{
				StringBuilder sb = new StringBuilder();
				foreach (MissionInfo mi in this.Missions.Values)
				{
					sb.Append(mi.Id).Append(",");
				}
				PVEGame.log.Error(string.Format("PVEGame.SendCreateGameToSingle KeyNotFoundException, game.SessionId : {0}, index : {1}, Missions.Ids : {2}, Missions.Count : {3}", new object[]
				{
					game.SessionId,
					index,
					sb.ToString(),
					this.Missions.Count
				}));
				if (index == 0)
				{
					index = 1;
				}
				if (index == this.Missions.Count)
				{
					index = this.Missions.Count;
				}
			}
			MissionInfo missionInfo = game.Missions[index];
			pkg.WriteString(missionInfo.Name);
			pkg.WriteString(missionInfo.Success);
			pkg.WriteString(missionInfo.Failure);
			pkg.WriteString(missionInfo.Description);
			pkg.WriteInt(game.TotalMissionCount);
			pkg.WriteInt(index);
			gamePlayer.SendTCP(pkg);
		}
		public void SendPlayerInfoInGame(PVEGame game, IGamePlayer gp, Player p)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(120);
			pkg.WriteInt(gp.AreaID);
			pkg.WriteInt(gp.PlayerCharacter.ID);
			pkg.WriteInt(p.Team);
			pkg.WriteInt(p.Id);
			pkg.WriteInt(p.MaxBlood);
			pkg.WriteBoolean(p.Ready);
			game.SendToAll(pkg);
		}
		public void SendPlaySound(string playStr)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(63);
			pkg.WriteString(playStr);
			base.SendToAll(pkg);
		}
		public void SendPlayBackgroundSound(bool isPlay)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(71);
			pkg.WriteBoolean(isPlay);
			base.SendToAll(pkg);
		}
		public void SendLoadResource(List<LoadingFileInfo> loadingFileInfos)
		{
			if (loadingFileInfos != null && loadingFileInfos.Count > 0)
			{
				GSPacketIn pkg = new GSPacketIn(91);
				pkg.WriteByte(67);
				pkg.WriteInt(loadingFileInfos.Count);
				foreach (LoadingFileInfo file in loadingFileInfos)
				{
					pkg.WriteInt(file.Type);
					pkg.WriteString(file.Path);
					pkg.WriteString(file.ClassName);
				}
				base.SendToAll(pkg);
			}
		}
		public void SendPassDrama(bool isShowPassButton)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(133);
			pkg.WriteBoolean(isShowPassButton);
			base.SendToAll(pkg);
		}
		public override void MinusDelays(int lowestDelay)
		{
			int[] keys = this.m_NpcTurnQueue.Keys.ToArray<int>();
			int[] array = keys;
			for (int i = 0; i < array.Length; i++)
			{
				int key = array[i];
				this.m_NpcTurnQueue[key] = ((this.m_NpcTurnQueue[key] - lowestDelay > 0) ? (this.m_NpcTurnQueue[key] - lowestDelay) : 0);
			}
			base.MinusDelays(lowestDelay);
		}
		public new void SendSyncLifeTime()
		{
			base.SendSyncLifeTime();
		}
		public int FindTurnNpcRank()
		{
			int turnNpc = 2147483647;
			int rank = 0;
			foreach (int i in this.m_NpcTurnQueue.Keys)
			{
				if (this.m_NpcTurnQueue[i] < turnNpc)
				{
					turnNpc = this.m_NpcTurnQueue[i];
					rank = i;
				}
			}
			return rank;
		}
		public void SendQuizWindow(int QuizID, int ArightResult, int NeedArightResult, int MaxQuizSize, int TimeOut, string Caption, string QuizStr, string ResultStrFirst, string ResultStrSecond, string ResultStrThird)
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(24);
			pkg.WriteBoolean(true);
			pkg.WriteInt(QuizID);
			pkg.WriteInt(ArightResult);
			pkg.WriteInt(NeedArightResult);
			pkg.WriteInt(MaxQuizSize);
			pkg.WriteInt(TimeOut);
			pkg.WriteString(Caption);
			pkg.WriteString(QuizStr);
			pkg.WriteString(ResultStrFirst);
			pkg.WriteString(ResultStrSecond);
			pkg.WriteString(ResultStrThird);
			base.SendToAll(pkg);
		}
		public void SendCloseQuizWindow()
		{
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(24);
			pkg.WriteBoolean(false);
			base.SendToAll(pkg);
		}
	}
}