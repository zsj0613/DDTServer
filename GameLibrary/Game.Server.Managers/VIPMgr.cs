using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Server.GameObjects;
using Lsj.Util;
using Game.Logic;
using SqlDataProvider.Data;
using Game.Server.Packets;

namespace Game.Server.Managers
{
    public class VIPMgr
    {
        private static int[] moneys = new int[] { 0, 5, 30, 50, 100, 200, 500, 1000, 1500, 2500, 5000, int.MaxValue };
        public static int GetVIPlevel(int money)
        {
            int result;
			for (int i = 0; i< VIPMgr.moneys.Length; i++)
			{
				if (money< VIPMgr.moneys[i])
				{
					result = i-1;
					return result;
				}
            }
            result = 0;
			return result;
        }

        public static bool Init()
        {
            moneys = GameServer.Instance.Configuration.VIP_MONEYS.Split(',').ConvertToIntArray();
            return true;
        }

        public static void CheckReward(GamePlayer player)
        {
            if (player.VIPLevel > player.PlayerCharacter.VIPGiftLevel)
            {
                var a = new List<ItemInfo>();
                var b = player.PlayerCharacter.VIPGiftLevel + 1;
                if (DropInventory.VIPRewardDrop(b, ref a))
                {
                    if (a.Count == 0)
                    {
                        GameServer.log.Error("VIP" + b.ToString() + "奖励为空！");
                        return;
                    }
                    else
                    {
                        var c = "VIP奖励" + b.ToString();
                        player.SendItemsToMail(a, c, c, eMailType.Default);
                    }
                }
                player.PlayerCharacter.VIPGiftLevel++;
                CheckReward(player);
            }
        }
    }
}
