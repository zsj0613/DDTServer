using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using Bussiness.Managers;
using Game.Logic;
using Game.Base.Packets;

namespace GameServerScript.AI.Messions
{
    public class NTM1083 : AMissionControl
    {
        private int mapId = 1118;
        private int indexOf = 0;
        private int tag = 0;
        private int redNpcID = 201;
        private int blueNpcID = 202;
        private int totalNpcCount = 5;

        private bool isPlayPwMovie = false;
        private bool isPlayPropMovie = false;
        private bool isShooted = false;
        private bool isUseProp = false;
        private bool isUsePw = false;
        private bool isPlayKillAll = false;
        private bool isAgain = false;
        private PhysicalObj tip = null;
        private List<SimpleNpc> simpleNpcList = new List<SimpleNpc>();

        public override int CalculateScoreGrade(int score)
        {
            base.CalculateScoreGrade(score);
            if (score > 900)
            {
                return 3;
            }
            if (score > 825)
            {
                return 2;
            }
            if (score > 725)
            {
                return 1;
            }
            return 0;
        }

        public override void OnPrepareNewSession()
        {
            base.OnPrepareNewSession();
            int[] npcIdList = { redNpcID, blueNpcID };
            Game.LoadResources(npcIdList);
            Game.LoadNpcGameOverResources(npcIdList);
            Game.AddLoadingFile(2, "image/map/1118/object/Asset.swf", "com.map.trainer.TankTrainerAssetII");
            Game.SetMap(mapId);
        }

        public override void OnStartGame()
        {
            CreateNpc();
            tip = Game.CreateTip(390, 120, "firstFront", "com.map.trainer.TankTrainerAssetII", "Empty", 1, 1, 0, 2);
        }

        public override void OnPrepareNewGame()
        {
            base.OnPrepareNewGame();
        }

        public override void OnNewTurnStarted()
        {
            List<ItemInfo> items = new List<ItemInfo>();
            List<ItemTemplateInfo> goods = new List<ItemTemplateInfo>();
            PVEGame pveGame = Game as PVEGame;
            int turnNpcRank = pveGame.FindTurnNpcRank();

            if ((isUsePw || isUseProp) && !isShooted)
                Game.CurrentLiving.Delay = 0;

            if (Game.CurrentPlayer.Delay < pveGame.NpcTurnQueue[turnNpcRank])
            {
                if (Game.TotalKillCount >= 1)
                {
                    if (indexOf == 0)
                    {
                        isPlayPropMovie = true;
                        indexOf++;
                    }

                    Game.CurrentPlayer.BeforePlayerShoot += new PlayerShootEventHandle(Shooted);
                }

                if (isPlayPropMovie)
                    Game.CurrentPlayer.PlayerUseProp += new PlayerUserPropEventHandle(UseProp);

                if (isPlayPwMovie)
                {
                    Game.CurrentPlayer.PlayerUseSpecialSkill += new PlayerUseSpecialSkillEventHandle(UseSpecialSkill);
                    Game.CurrentPlayer.AddDander(200);
                }

                if (tip.CurrentAction == "Empty")
                {
                    tip.PlayMovie("tip1", 0, 3000);
                }
                else if (isPlayPropMovie)
                {
                    //播放使用道具
                    if (tag == 0)
                    {
                        tip.PlayMovie("tip2", 0, 2000);
                        tag++;
                    }
                    else
                    {
                        if (!isUseProp && !isShooted && isAgain == false)
                        {
                            tip.PlayMovie("tip2", 0, 2000);
                        }
                        else//再试一次
                        {
                            isAgain = true;
                            isUseProp = false;
                            tip.PlayMovie("tip3", 0, 2000);
                        }
                    }

                }
                else if (isPlayPwMovie)
                {
                    isUseProp = false;
                    //播放使用必杀  
                    if (tag == 0)
                    {
                        isAgain = false;
                        tip.PlayMovie("tip4", 0, 2000);
                        tag++;
                    }
                    else
                    {
                        if (!isUsePw && !isShooted && isAgain == false)
                        {
                            tip.PlayMovie("tip4", 0, 2000);
                        }
                        else//再试一次
                        {
                            isAgain = true;
                            isUsePw = false;
                            tip.PlayMovie("tip5", 0, 2000);
                        }
                    }
                }
                else if (isPlayKillAll)
                {
                    //播放杀死所有的怪
                    tip.PlayMovie("tip6", 0, 2000);
                }
            }
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
        }

        public override bool CanGameOver()
        {
            foreach (SimpleNpc npc in simpleNpcList)
                if (npc.IsLiving)
                    return false;

            if (simpleNpcList.Count == totalNpcCount)
                return true;
            else
                return false;
        }

        public override void OnPrepareGameOver()
        {
            base.OnPrepareGameOver();
        }

        public override int UpdateUIData()
        {
            return Game.TotalKillCount;
        }

        public override void OnGameOver()
        {
            base.OnGameOver();
            Game.IsWin = true;
        }
        private void CreateNpc()
        {
            int[,] points = new int[,]
            {
                { 585, 553 }, 
                { 635, 554 }, 
                { 735, 553 }, 
                { 835, 551 }
            };
            for (int i = 0; i < 4; i++)
            {
                simpleNpcList.Add(Game.CreateNpc(redNpcID, points[i, 0], points[i, 1], 1, 1));
            }
            simpleNpcList.Add(Game.CreateNpc(blueNpcID, 685, 553, 1, 1));
        }

        private void Shooted(Player player, int ball)
        {
            player.BeforePlayerShoot -= new PlayerShootEventHandle(Shooted);
            isShooted = true;
            if (isUseProp)
            {
                isShooted = false;
                isPlayPropMovie = false;
                isPlayPwMovie = true;
                tag = 0;
            }

            if (isUsePw)
            {
                isShooted = false;
                isPlayPwMovie = false;
                isPlayKillAll = true;
            }
        }

        private void UseProp(Player player)
        {
            player.PlayerUseProp -= new PlayerUserPropEventHandle(UseProp);
            isUseProp = true;
        }

        private void UseSpecialSkill(Player player)
        {
            player.PlayerUseSpecialSkill -= new PlayerUseSpecialSkillEventHandle(UseSpecialSkill);
            isUsePw = true;
        }
    }
}
