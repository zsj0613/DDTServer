using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;


namespace GameServerScript.AI.Game
{
    public class NewtPairGame : APVEGameControl
    {

        static Random rand = new Random();
        static string[] Missions = new string[] { "5001", "5002", "5003", "5004" };
        public string GetMission()
        {
            int id = rand.Next(Missions.Length);
            return Missions[id];
        }

        public override void OnCreated()
        {
            Game.SetupMissions(GetMission());
            Game.TotalMissionCount = 1;
        }

        public override void OnPrepated()
        {
            Game.SessionId = 0;
        }

        public override int CalculateScoreGrade(int score)
        {
            if (score > 800)
            {
                return 3;
            }
            else if (score > 725)
            {
                return 2;
            }
            else if (score > 650)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override void OnGameOverAllSession()
        {
        }
    }
}
