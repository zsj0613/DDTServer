using System;
using Lsj.Util;
namespace Game.Server.Managers
{
	public class LevelMgr
	{
        private static int[] levels = new int[]
        {
            0,
            1000,
            2000,
            3000,
            4000,
            5000,
            6000,
            7000,
            8000,
            9000,
            10000,
            20000,
            30000,
            40000,
            50000,
            60000,
            70000,
            80000,
            90000,
            100000,
            120000,
            140000,
            160000,
            180000,
            200000,
            220000,
            240000,
            260000,
            280000,
            300000,
            320000,
            340000,
            360000,
            380000,
            400000,
            420000,
            440000,
            460000,
            480000,
            500000,
            550000,
            600000,
            650000,
            700000,
            750000,
            800000,
            850000,
            900000,
            950000,
            1000000,
            1050000,
            1100000,
            1150000,
            1200000,
            1250000,
            1300000,
            1350000,
            1400000,
            1450000,
            1500000,
            1600000,
            1700000,
            1800000,
            1900000,
            2000000,
            2100000,
            2200000,
            2300000,
            2400000,
            2500000,
            2700000,
            2900000,
            3100000,
            3300000,
            3500000,
            3700000,
            3900000,
            4100000,
            4300000,
            4500000,
            5000000,
            5500000,
            6000000,
            6500000,
            7000000,
            7500000,
            8000000,
            8500000,
            9000000,
            9500000,
            10500000,
            11500000,
            12500000,
            13500000,
            14500000,
            16500000,
            18500000,
            20500000,
            22500000,
            25000000,
            30000000,
            2147483647
        };


        public static int GetLevel(int GP)
		{
			int result;
			for (int i = 0; i < LevelMgr.levels.Length; i++)
			{
				if (GP < LevelMgr.levels[i])
				{
					result = i;
					return result;
				}
			}
			result = 1;
			return result;
		}
		public static int GetGP(int level)
		{
			int result;
			if (LevelMgr.levels.Length > level && level > 0)
			{
				result = LevelMgr.levels[level - 1];
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public static int ReduceGP(int level, int totalGP)
		{
			int result;
			if (LevelMgr.levels.Length > level && level > 0)
			{
				totalGP -= LevelMgr.levels[level - 1];
				if (totalGP < level * 12)
				{
					result = ((totalGP < 0) ? 0 : totalGP);
				}
				else
				{
					result = level * 12;
				}
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public static int IncreaseGP(int level, int totalGP)
		{
			int result = 0;
			//if (LevelMgr.levels.Length > level && level > 0)
			//{
			//	result = level * 12;
			//}
			//else
			//{
			//	result = 0;
			//}
			return result;
		}
		public static int GetSpaGoldGP(int level)
		{
			int result = 0;
			//if (LevelMgr.spaGpGoldLevels.Length > level && level > 0)
			//{
			//	result = LevelMgr.spaGpGoldLevels[level - 1];
			//}
			//else
			//{
			//	result = 0;
			//}
			return result;
		}
		public static int GetSpaMoneyGP(int level)
		{
			int result = 0;
			//if (LevelMgr.spaGpMoneyLevels.Length > level && level > 0)
			//{
			//	result = LevelMgr.spaGpMoneyLevels[level - 1];
			//}
			//else
			//{
			//	result = 0;
			//}
			return result;
		}

        public static bool Init()
        {
            levels = GameServer.Instance.Configuration.Levels.Split(',').ConvertToIntArray();
            return true;
        }
    }
}
