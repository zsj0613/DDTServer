using Bussiness;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using log4net;
using System;
using System.Reflection;
using Game.Language;
namespace Game.Server.SpaRooms.CommandHandle
{
	[SpaCommandAttbute(3)]
	public class ContinuationCommand : ISpaCommandHandler
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public bool HandleCommand(GamePlayer player, GSPacketIn packet)
		{
			bool result;
			if (player.CurrentSpaRoom == null)
			{
				result = false;
			}
			else
			{
				if (player.PlayerCharacter.ID != player.CurrentSpaRoom.Spa_Room_Info.PlayerID)
				{
					result = false;
				}
				else
				{
					string[] array = new string[]
					{
						"1",
						"2"
					};
					string[] money = GameProperties.PRICE_SPA_ROOM.Split(new char[]
					{
						','
					});
					if (money.Length < 2)
					{
						if (ContinuationCommand.log.IsErrorEnabled)
						{
							ContinuationCommand.log.Error("SpaRoomCreateMoney node in configuration file is wrong");
						}
						result = false;
					}
					else
					{
						int needMoney = 0;
						if (player.CurrentSpaRoom.Spa_Room_Info.MaxCount == 4)
						{
							needMoney = int.Parse(money[0]);
						}
						else
						{
							if (player.CurrentSpaRoom.Spa_Room_Info.MaxCount == 8)
							{
								needMoney = int.Parse(money[1]);
							}
						}
						if (player.PlayerCharacter.Money < needMoney)
						{
							player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("SpaRoomContinationHandler.Failed1", new object[0]));
							result = false;
						}
						else
						{
							player.RemoveMoney(needMoney, LogMoneyType.Spa, LogMoneyType.Spa_Room_Continue);
							if (player.CurrentSpaRoom.RoomContinuation(SpaRoomMgr.priRoomContinue_MinLimit, player))
							{
								player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("SpaRoomContinationHandler.Success", new object[0]));
								result = true;
							}
							else
							{
								player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("SpaRoomContinationHandler.Failed2", new object[0]));
								result = false;
							}
						}
					}
				}
			}
			return result;
		}
	}
}
