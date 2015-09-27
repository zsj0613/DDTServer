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
namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(3)]
	public class ContinuationCommand : IMarryCommandHandler
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			bool result;
			if (player.CurrentMarryRoom == null)
			{
				result = false;
			}
			else
			{
				if (player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID && player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID)
				{
					result = false;
				}
				else
				{
					int hour = packet.ReadInt();
					string[] money = GameProperties.PRICE_MARRY_ROOM.Split(new char[]
					{
						','
					});
					if (money.Length < 3)
					{
						if (ContinuationCommand.log.IsErrorEnabled)
						{
							ContinuationCommand.log.Error("MarryRoomCreateMoney node in configuration file is wrong");
						}
						result = false;
					}
					else
					{
						int needMoney;
						switch (hour)
						{
						case 2:
							needMoney = int.Parse(money[0]);
							break;
						case 3:
							needMoney = int.Parse(money[1]);
							break;
						case 4:
							needMoney = int.Parse(money[2]);
							break;
						default:
							needMoney = int.Parse(money[2]);
							hour = 4;
							break;
						}
						if (player.PlayerCharacter.Money < needMoney)
						{
							player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg1", new object[0]));
							result = false;
						}
						else
						{
							player.RemoveMoney(needMoney, LogMoneyType.Marry, LogMoneyType.Marry_RoomAdd);
							CountBussiness.InsertSystemPayCount(player.PlayerCharacter.ID, needMoney, 0, 0, 0);
							player.CurrentMarryRoom.RoomContinuation(hour);
							GSPacketIn pkg = player.Out.SendContinuation(player, player.CurrentMarryRoom.Info);
							int spouseID;
							if (player.PlayerCharacter.ID == player.CurrentMarryRoom.Info.GroomID)
							{
								spouseID = player.CurrentMarryRoom.Info.BrideID;
							}
							else
							{
								spouseID = player.CurrentMarryRoom.Info.GroomID;
							}
							GamePlayer spouse = WorldMgr.GetPlayerById(spouseID);
							if (spouse != null)
							{
								spouse.Out.SendTCP(pkg);
							}
							player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ContinuationCommand.Successed", new object[0]));
							result = true;
						}
					}
				}
			}
			return result;
		}
	}
}
