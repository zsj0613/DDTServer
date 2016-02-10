﻿using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;

namespace GameServerScript.AI.Messions
{
    public class DCNM2101 : AMissionControl
    {
        private List<SimpleNpc> someNpc = new List<SimpleNpc>();

        private int dieRedCount = 0;

        private int[] npcIDs = { 2101, 2102 };

        private int[] birthX = { 52, 115, 183, 253, 320, 1206, 1275, 1342, 1410, 1475 };

        public override int CalculateScoreGrade(int score)
        {
            base.CalculateScoreGrade(score);
            if (score > 1870)
            {
                return 3;
            }
            else if (score > 1825)
            {
                return 2;
            }
            else if (score > 1780)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override void OnPrepareNewSession()
        {
            base.OnPrepareNewSession();
            int[] resources = { npcIDs[0], npcIDs[1] };
            int[] gameOverResources = { npcIDs[1], npcIDs[0], npcIDs[0], npcIDs[0] };
            Game.LoadResources(resources);
            Game.LoadNpcGameOverResources(gameOverResources);
            Game.SetMap(1120);
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

            //左边五只小怪
            int index = Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(Game.CreateNpc(npcIDs[index], 52, 506, 1));
            index = Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(Game.CreateNpc(npcIDs[index], 100, 507, 1));
            index = Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(Game.CreateNpc(npcIDs[index], 155, 508, 1));
            index = Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(Game.CreateNpc(npcIDs[index], 210, 507, 1));
            index = Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(Game.CreateNpc(npcIDs[index], 253, 507, 1));

            //右边五只小怪
            index = Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(Game.CreateNpc(npcIDs[index], 1275, 508, 1));
            index = Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(Game.CreateNpc(npcIDs[index], 1325, 506, 1));
            index = Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(Game.CreateNpc(npcIDs[index], 1360, 508, 1));
            index = Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(Game.CreateNpc(npcIDs[index], 1410, 506, 1));
            index = Game.Random.Next(0, npcIDs.Length);
            someNpc.Add(Game.CreateNpc(npcIDs[index], 1475, 508, 1));
        }

        public override void OnNewTurnStarted()
        {
            base.OnNewTurnStarted();

            PVEGame pveGame = Game as PVEGame;
            int turnNpcRank = pveGame.FindTurnNpcRank();

            if (Game.GetLivedLivings().Count == 0)
            {
                pveGame.NpcTurnQueue[turnNpcRank] = 0;
            }

            if (Game.TurnIndex > 1 && Game.CurrentPlayer.Delay > pveGame.NpcTurnQueue[turnNpcRank])
            {
                if (Game.GetLivedLivings().Count < 10)
                {
                    for (int i = 0; i < 10 - Game.GetLivedLivings().Count; i++)
                    {
                        if (someNpc.Count == Game.MissionInfo.TotalCount)
                        {
                            break;
                        }
                        else
                        {
                            int index = Game.Random.Next(0, birthX.Length);
                            int NpcX = birthX[index];

                            int direction = -1;

                            if (NpcX <= 320)
                            {
                                direction = 1;
                            }

                            index = Game.Random.Next(0, npcIDs.Length);

                            if (index == 1 && GetNpcCountByID(npcIDs[1]) < 10)
                            {
                                someNpc.Add(Game.CreateNpc(npcIDs[1], NpcX, 506, 1, direction));
                            }
                            else
                            {
                                someNpc.Add(Game.CreateNpc(npcIDs[0], NpcX, 506, 1, direction));
                            }
                        }
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
            bool result = true;

            base.CanGameOver();

            dieRedCount = 0;

            foreach (SimpleNpc redNpc in someNpc)
            {
                if (redNpc.IsLiving)
                {
                    result = false;
                }
                else
                {
                    dieRedCount++;
                }
            }

            if (result && dieRedCount == Game.MissionInfo.TotalCount)
            {
                Game.IsWin = true;
                return true;
            }

            return false;
        }

        public override int UpdateUIData()
        {
            return Game.TotalKillCount;
        }

        public override void OnPrepareGameOver()
        {
            base.OnPrepareGameOver();
        }

        public override void OnGameOver()
        {
            base.OnGameOver();
            if (Game.GetLivedLivings().Count == 0)
            {
                Game.IsWin = true;
            }
            else
            {
                Game.IsWin = false;
            }
            List<LoadingFileInfo> loadingFileInfos = new List<LoadingFileInfo>();
            loadingFileInfos.Add(new LoadingFileInfo(2, "image/map/2/show2", ""));
            Game.SendLoadResource(loadingFileInfos);
        }

        protected int GetNpcCountByID(int Id)
        {
            int Count = 0;
            foreach (SimpleNpc npc in someNpc)
            {
                if (npc.NpcInfo.ID == Id)
                    Count++;
            }
            return Count;
        }
    }
}