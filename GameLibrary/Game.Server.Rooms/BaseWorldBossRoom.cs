using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
using System.Collections.Generic;

namespace Game.Server.Rooms
{
    public class BaseWorldBossRoom
    {
        private DateTime _begin_time;

        private string _bossResourceId = "0";

        private int _currentPVE = 0;

        private System.DateTime _end_time;

        private int _fight_time;

        private bool _fightOver = true;

        private string _name = "boss";

        private bool _roomClose = true;

        private bool _worldOpen = false;

        public int addInjureBuffMoney = 30;

        public int addInjureValue = 200;

        private long m_blood;

        private bool m_die = false;

        private Dictionary<int, GamePlayer> m_list = new Dictionary<int, GamePlayer>();

        private long MAX_BLOOD;

        public int need_ticket_count;

        public int playerDefaultPosX = 265;

        public int playerDefaultPosY = 1030;

        public int reFightMoney = 12;

        public int reviveMoney = 10;

        public int ticketID = 11573;

        public int timeCD = 15;

        public DateTime begin_time
        {
            get
            {
                return this._begin_time;
            }
        }

        public long Blood
        {
            get
            {
                return this.m_blood;
            }
            set
            {
                this.m_blood = value;
            }
        }

        public string bossResourceId
        {
            get
            {
                return this._bossResourceId;
            }
        }

        public int currentPVE
        {
            get
            {
                return this._currentPVE;
            }
        }

        public DateTime end_time
        {
            get
            {
                return this._end_time;
            }
        }

        public int fight_time
        {
            get
            {
                return this._fight_time;
            }
        }

        public bool fightOver
        {
            get
            {
                return this._fightOver;
            }
        }

        public bool IsDie
        {
            get
            {
                return this.m_die;
            }
            set
            {
                this.m_die = value;
            }
        }

        public long MaxBlood
        {
            get
            {
                return this.MAX_BLOOD;
            }
        }

        public string name
        {
            get
            {
                return this._name;
            }
        }

        public bool roomClose
        {
            get
            {
                return this._roomClose;
            }
        }

        public bool worldOpen
        {
            get
            {
                return this._worldOpen;
            }
        }

        public bool AddPlayer(GamePlayer player)
        {
            bool flag = false;
            lock (this.m_list)
            {
                if (!this.m_list.ContainsKey(player.PlayerId))
                {
                    player.IsInWorldBossRoom = true;
                    this.m_list.Add(player.PlayerId, player);
                    flag = true;
                    this.ShowRank();
                    this.SendPrivateInfo(player.PlayerCharacter.get_NickName());
                    if (this.m_blood < this.MAX_BLOOD)
                    {
                        this.SendUpdateBlood();
                    }
                }
            }
            if (flag)
            {
                GSPacketIn gSPacketIn = new GSPacketIn(102);
                gSPacketIn.WriteByte(3);
                gSPacketIn.WriteInt(player.PlayerCharacter.get_Grade());
                gSPacketIn.WriteInt(player.PlayerCharacter.get_Hide());
                gSPacketIn.WriteInt(player.PlayerCharacter.get_Repute());
                gSPacketIn.WriteInt(player.PlayerCharacter.get_ID());
                gSPacketIn.WriteString(player.PlayerCharacter.get_NickName());
                gSPacketIn.WriteByte(player.PlayerCharacter.get_typeVIP());
                gSPacketIn.WriteInt(player.PlayerCharacter.get_VIPLevel());
                gSPacketIn.WriteBoolean(player.PlayerCharacter.get_Sex());
                gSPacketIn.WriteString(player.PlayerCharacter.get_Style());
                gSPacketIn.WriteString(player.PlayerCharacter.get_Colors());
                gSPacketIn.WriteString(player.PlayerCharacter.get_Skin());
                gSPacketIn.WriteInt(player.WorldBossX);
                gSPacketIn.WriteInt(player.WorldBossY);
                gSPacketIn.WriteInt(player.PlayerCharacter.get_FightPower());
                gSPacketIn.WriteInt(player.PlayerCharacter.get_Win());
                gSPacketIn.WriteInt(player.PlayerCharacter.get_Total());
                gSPacketIn.WriteInt(player.PlayerCharacter.get_Offer());
                gSPacketIn.WriteByte(player.WorldBossState);
                this.SendToALL(gSPacketIn);
            }
            return flag;
        }

        public void FightOver()
        {
            GSPacketIn packet = new GSPacketIn(82);
            GameServer.Instance.LoginServer.SendPacket(packet);
        }

        public GamePlayer[] GetPlayersSafe()
        {
            GamePlayer[] array = null;
            lock (this.m_list)
            {
                array = new GamePlayer[this.m_list.Count];
                this.m_list.Values.CopyTo(array, 0);
            }
            GamePlayer[] result;
            if (array != null)
            {
                result = array;
            }
            else
            {
                result = new GamePlayer[0];
            }
            return result;
        }

        public void ReduceBlood(int value)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(84);
            gSPacketIn.WriteInt(value);
            GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
        }

        public bool RemovePlayer(GamePlayer player)
        {
            bool flag = false;
            lock (this.m_list)
            {
                flag = this.m_list.Remove(player.PlayerId);
                GSPacketIn gSPacketIn = new GSPacketIn(102);
                gSPacketIn.WriteByte(4);
                gSPacketIn.WriteInt(player.PlayerId);
                this.SendToALL(gSPacketIn);
            }
            if (flag)
            {
                player.Out.SendSceneRemovePlayer(player);
            }
            return true;
        }

        public void SendFightOver()
        {
            GSPacketIn gSPacketIn = new GSPacketIn(102);
            gSPacketIn.WriteByte(8);
            gSPacketIn.WriteBoolean(true);
            this.SendToALLPlayers(gSPacketIn);
        }

        public void SendPrivateInfo(string name)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(85);
            gSPacketIn.WriteString(name);
            GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
        }

        public void SendPrivateInfo(string name, int damage, int honor)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(102);
            gSPacketIn.WriteByte(22);
            gSPacketIn.WriteInt(damage);
            gSPacketIn.WriteInt(honor);
            GamePlayer[] playersSafe = this.GetPlayersSafe();
            for (int i = 0; i < playersSafe.Length; i++)
            {
                GamePlayer gamePlayer = playersSafe[i];
                if (gamePlayer.PlayerCharacter.get_NickName() == name)
                {
                    gamePlayer.Out.SendTCP(gSPacketIn);
                    break;
                }
            }
        }

        public void SendRoomClose()
        {
            GSPacketIn gSPacketIn = new GSPacketIn(102);
            gSPacketIn.WriteByte(9);
            this.SendToALLPlayers(gSPacketIn);
        }

        public void SendToALL(GSPacketIn packet)
        {
            this.SendToALL(packet, null);
        }

        public void SendToALL(GSPacketIn packet, GamePlayer except)
        {
            GamePlayer[] array = null;
            lock (this.m_list)
            {
                array = new GamePlayer[this.m_list.Count];
                this.m_list.Values.CopyTo(array, 0);
            }
            if (array != null)
            {
                GamePlayer[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    GamePlayer gamePlayer = array2[i];
                    if (gamePlayer != null && gamePlayer != except)
                    {
                        gamePlayer.Out.SendTCP(packet);
                    }
                }
            }
        }

        public void SendToALLPlayers(GSPacketIn packet)
        {
            GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
            for (int i = 0; i < allPlayers.Length; i++)
            {
                GamePlayer gamePlayer = allPlayers[i];
                gamePlayer.SendTCP(packet);
            }
        }

        public void SendUpdateBlood()
        {
            GSPacketIn gSPacketIn = new GSPacketIn(102);
            gSPacketIn.WriteByte(5);
            gSPacketIn.WriteBoolean(false);
            gSPacketIn.WriteLong(this.MAX_BLOOD);
            gSPacketIn.WriteLong(this.m_blood);
            this.SendToALL(gSPacketIn);
        }

        public void ShowRank()
        {
            GSPacketIn packet = new GSPacketIn(86);
            GameServer.Instance.LoginServer.SendPacket(packet);
        }

        public void UpdateRank(int damage, int honor, string nickName)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(81);
            gSPacketIn.WriteInt(damage);
            gSPacketIn.WriteInt(honor);
            gSPacketIn.WriteString(nickName);
            GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
        }

        public void UpdateWorldBoss(GSPacketIn pkg)
        {
            long mAX_BLOOD = pkg.ReadLong();
            long num = pkg.ReadLong();
            string name = pkg.ReadString();
            string bossResourceId = pkg.ReadString();
            int currentPVE = pkg.ReadInt();
            this._fightOver = pkg.ReadBoolean();
            this._roomClose = pkg.ReadBoolean();
            this._begin_time = pkg.ReadDateTime();
            this._end_time = pkg.ReadDateTime();
            this._fight_time = pkg.ReadInt();
            bool worldOpen = pkg.ReadBoolean();
            if (!this._worldOpen)
            {
                this.MAX_BLOOD = mAX_BLOOD;
                this.m_blood = num;
                this._name = name;
                this._bossResourceId = bossResourceId;
                this._currentPVE = currentPVE;
                this._worldOpen = worldOpen;
                GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
                for (int i = 0; i < allPlayers.Length; i++)
                {
                    GamePlayer gamePlayer = allPlayers[i];
                    gamePlayer.Out.SendOpenWorldBoss(gamePlayer.WorldBossX, gamePlayer.WorldBossY);
                }
            }
            if (num < this.m_blood)
            {
                this.m_blood = num;
                this.SendUpdateBlood();
            }
        }

        public void UpdateWorldBossRankCrosszone(GSPacketIn packet)
        {
            GSPacketIn gSPacketIn = new GSPacketIn(102);
            gSPacketIn.WriteByte(10);
            bool flag = packet.ReadBoolean();
            int num = packet.ReadInt();
            gSPacketIn.WriteBoolean(flag);
            gSPacketIn.WriteInt(num);
            for (int i = 0; i < num; i++)
            {
                int num2 = packet.ReadInt();
                string text = packet.ReadString();
                int num3 = packet.ReadInt();
                gSPacketIn.WriteInt(num2);
                gSPacketIn.WriteString(text);
                gSPacketIn.WriteInt(num3);
            }
            if (flag)
            {
                this.SendToALLPlayers(gSPacketIn);
            }
            else
            {
                this.SendToALL(gSPacketIn);
            }
        }

        public void ViewOtherPlayerRoom(GamePlayer player)
        {
            GamePlayer[] playersSafe = this.GetPlayersSafe();
            for (int i = 0; i < playersSafe.Length; i++)
            {
                GamePlayer gamePlayer = playersSafe[i];
                if (gamePlayer != player)
                {
                    GSPacketIn gSPacketIn = new GSPacketIn(102);
                    gSPacketIn.WriteByte(3);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.get_Grade());
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.get_Hide());
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.get_Repute());
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.get_ID());
                    gSPacketIn.WriteString(gamePlayer.PlayerCharacter.get_NickName());
                    gSPacketIn.WriteByte(gamePlayer.PlayerCharacter.get_typeVIP());
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.get_VIPLevel());
                    gSPacketIn.WriteBoolean(gamePlayer.PlayerCharacter.get_Sex());
                    gSPacketIn.WriteString(gamePlayer.PlayerCharacter.get_Style());
                    gSPacketIn.WriteString(gamePlayer.PlayerCharacter.get_Colors());
                    gSPacketIn.WriteString(gamePlayer.PlayerCharacter.get_Skin());
                    gSPacketIn.WriteInt(gamePlayer.WorldBossX);
                    gSPacketIn.WriteInt(gamePlayer.WorldBossY);
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.get_FightPower());
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.get_Win());
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.get_Total());
                    gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.get_Offer());
                    gSPacketIn.WriteByte(gamePlayer.WorldBossState);
                    player.SendTCP(gSPacketIn);
                }
            }
        }

        public void WorldBossClose()
        {
            this._worldOpen = false;
        }
    }
}
