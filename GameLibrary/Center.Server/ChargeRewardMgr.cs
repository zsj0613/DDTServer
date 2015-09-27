using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Reflection;
namespace Center.Server
{
	internal class ChargeRewardMgr
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public static void DoChargeReward(int userID)
		{
			try
			{
				RebateChargeInfo[] allChargeInfo = null;
				DateTime firstChargeDate = DateTime.Now;
				int playerSex = 0;
				PlayerInfo playerInfo;
				using (PlayerBussiness db = new PlayerBussiness())
				{
					playerInfo = db.GetUserSingleByUserID(userID);
					if (playerInfo == null)
					{
						return;
					}
					playerSex = (playerInfo.Sex ? 1 : 2);
				}
				using (PlayerBussiness db = new PlayerBussiness())
				{
					allChargeInfo = db.GetChargeInfo(playerInfo.UserName, playerInfo.NickName, ref firstChargeDate);
				}
				RebateChargeInfo[] array = allChargeInfo;
				for (int i = 0; i < array.Length; i++)
				{
					RebateChargeInfo chargeInfo = array[i];
					string rewardItems = "";
					string rewardInfo = "";
					int totalMoney = 0;
					int totalGold = 0;
					int totalGiftToken = 0;
					using (PlayerBussiness db = new PlayerBussiness())
					{
						RebateItemInfo[] allItemsInfo = db.GetChargeRewardItemsInfo(chargeInfo.Money, chargeInfo.Date);
						RebateItemInfo[] array2 = allItemsInfo;
						for (int j = 0; j < array2.Length; j++)
						{
							RebateItemInfo itemInfo = array2[j];
							if (itemInfo.FirstChargeNeeded && chargeInfo.Date.CompareTo(firstChargeDate) != 0)
							{
								break;
							}
							totalMoney += itemInfo.Money;
							totalGold += itemInfo.Gold;
							totalGiftToken += itemInfo.GiftToken;
							if (itemInfo.ItemTemplateID > 0)
							{
								if (itemInfo.NeedSex == 0 || itemInfo.NeedSex == playerSex)
								{
									rewardItems += string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}|", new object[]
									{
										itemInfo.ItemTemplateID,
										itemInfo.Count,
										itemInfo.ValidDate,
										itemInfo.StrengthLevel,
										itemInfo.AttackCompose,
										itemInfo.DefendCompose,
										itemInfo.AgilityCompose,
										itemInfo.LuckCompose,
										itemInfo.IsBind ? 1 : 0
									});
									rewardInfo += string.Format("{0}_{1}|", itemInfo.GiftPackageID, itemInfo.RewardItemID);
								}
							}
						}
						if (rewardItems.Length > 1)
						{
							rewardItems = rewardItems.Substring(0, rewardItems.Length - 1);
						}
					}
					using (PlayerBussiness db = new PlayerBussiness())
					{
						db.DoChargeReward(userID, chargeInfo, totalMoney, totalGold, totalGiftToken, rewardItems, rewardInfo);
					}
				}
			}
			catch (Exception e)
			{
				if (ChargeRewardMgr.log.IsErrorEnabled)
				{
					ChargeRewardMgr.log.Error("Init", e);
				}
			}
			finally
			{
			}
		}
	}
}
