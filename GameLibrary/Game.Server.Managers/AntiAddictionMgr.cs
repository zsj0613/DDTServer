using Bussiness;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Managers
{
	public class AntiAddictionMgr
	{
		private static bool _isASSon;
		public static bool ISASSon
		{
			get
			{
				return AntiAddictionMgr._isASSon;
			}
		}
		public static void SetASSState(bool state)
		{
			if (AntiAddictionMgr._isASSon != state)
			{
				AntiAddictionMgr._isASSon = state;
				GamePlayer[] players = WorldMgr.GetAllPlayers();
				GamePlayer[] array = players;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer p = array[i];
					p.Out.SendAASControl(AntiAddictionMgr._isASSon, p.IsAASInfo, p.IsMinor);
				}
			}
		}
		public static double GetAntiAddictionCoefficient(int onlineTime)
		{
			double result;
			if (AntiAddictionMgr._isASSon)
			{
				if (0 <= onlineTime && onlineTime <= 240)
				{
					result = 1.0;
				}
				else
				{
					if (240 < onlineTime && onlineTime <= 300)
					{
						result = 0.5;
					}
					else
					{
						result = 0.0;
					}
				}
			}
			else
			{
				result = 1.0;
			}
			return result;
		}
		public static int AASStateGet(GamePlayer player)
		{
			int userID = player.PlayerCharacter.ID;
			bool result = true;
			player.IsAASInfo = false;
			player.IsMinor = true;
			using (ProduceBussiness db = new ProduceBussiness())
			{
				string ID = db.GetASSInfoSingle(userID);
				if (ID != "")
				{
					player.IsAASInfo = true;
					result = false;
					int Age = Convert.ToInt32(ID.Substring(6, 4));
					int month = Convert.ToInt32(ID.Substring(10, 2));
					if (DateTime.Now.Year.CompareTo(Age + 18) > 0 || (DateTime.Now.Year.CompareTo(Age + 18) == 0 && DateTime.Now.Month.CompareTo(month) >= 0))
					{
						player.IsMinor = false;
					}
				}
			}
			if (result && player.PlayerCharacter.IsFirst != 0 && player.PlayerCharacter.DayLoginCount < 1 && AntiAddictionMgr.ISASSon)
			{
				player.Out.SendAASState(result);
			}
			player.PlayerCharacter.DayLoginCount++;
			player.Out.SendAASControl(AntiAddictionMgr.ISASSon, player.IsAASInfo, player.IsMinor);
			return 0;
		}
	}
}
