using Game.Base.Packets;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Quests;
using Game.Server.Rooms;
using Game.Server.SceneMarryRooms;
using Game.Server.SpaRooms;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Packets.Server
{
	public class ConsolePacketLib : IPacketLib
	{
		public GSPacketIn SendMessage(eMessageType type, string message)
		{
			Console.WriteLine(message);
			return null;
		}
		public void SendTCP(GSPacketIn packet)
		{
			throw new NotImplementedException();
		}
		public void SendLoginSuccess()
		{
			throw new NotImplementedException();
		}
		public void SendCheckCode()
		{
			throw new NotImplementedException();
		}
		public void SendLoginFailed(string msg)
		{
			throw new NotImplementedException();
		}
		public void SendKitoff(string msg)
		{
			throw new NotImplementedException();
		}
		public void SendEditionError(string msg)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUpdateNickname(string name)
		{
			throw new NotImplementedException();
		}
		public void SendDateTime()
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendDailyAward(GamePlayer player, int getWay)
		{
			throw new NotImplementedException();
		}
		public void SendPingTime(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public void SendUpdatePrivateInfo(PlayerInfo info)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUpdatePublicPlayer(PlayerInfo info)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendNetWork(int id, long delay)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUserEquip(PlayerInfo info, List<ItemInfo> items)
		{
			throw new NotImplementedException();
		}
		public void SendWaitingRoom(bool result)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUpdateRoomList(List<BaseRoom> room)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendSceneAddPlayer(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendSceneRemovePlayer(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomCreate(BaseRoom room)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomLoginResult(bool result)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomPlayerAdd(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomPlayerRemove(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomUpdatePlayerStates(byte[] states)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomUpdatePlacesStates(int[] states)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomPlayerChangedTeam(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomPairUpStart(BaseRoom room)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomPairUpCancel(BaseRoom room)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendEquipChange(GamePlayer player, int place, int goodsID, string style)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomChange(BaseRoom room)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendFusionPreview(GamePlayer player, Dictionary<int, double> previewItemList, bool isBind, int MinValid)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendFusionResult(GamePlayer player, bool result)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRefineryPreview(GamePlayer player, int templateid, bool isbind, ItemInfo item)
		{
			throw new NotImplementedException();
		}
		public void SendUpdateInventorySlot(PlayerInventory bag, int[] updatedSlots)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendFriendRemove(int FriendID)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendFriendState(int playerID, bool state)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUpdateBuffer(GamePlayer player, BufferInfo[] infos)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendBufferList(GamePlayer player, List<AbstractBuffer> infos)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUpdateQuests(GamePlayer player, byte[] states, BaseQuest[] quests)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendInitAchievements(List<UsersRecordInfo> infos)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUpdateAchievements(UsersRecordInfo info)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendUpdateAchievementData(List<AchievementDataInfo> infos)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendMailResponse(int playerID, eMailRespose type)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendAuctionRefresh(AuctionInfo info, int auctionID, bool isExist, ItemInfo item)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendIDNumberCheck(bool result)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendAASState(bool result)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendAASInfoSet(bool result)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendAASControl(bool result, bool IsAASInfo, bool IsMinor)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendGameRoomInfo(GamePlayer player, BaseRoom game)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendMarryInfoRefresh(MarryInfo info, int ID, bool isExist)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendMarryRoomInfo(GamePlayer player, MarryRoom room)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerEnterMarryRoom(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerMarryStatus(GamePlayer player, int userID, bool isMarried)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerMarryApply(GamePlayer player, int userID, string userName, string loveProclamation, int ID)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerDivorceApply(GamePlayer player, bool result, bool isProposer)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendMarryApplyReply(GamePlayer player, int UserID, string UserName, bool result, bool isApplicant, int ID)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendBigSpeakerMsg(GamePlayer player, string msg)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerLeaveMarryRoom(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendMarryRoomLogin(GamePlayer player, bool result)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendMarryRoomInfoToPlayer(GamePlayer player, bool state, MarryRoomInfo info)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendMarryInfo(GamePlayer player, MarryInfo info)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendContinuation(GamePlayer player, MarryRoomInfo info)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendMarryProp(GamePlayer player, MarryProp info)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendRoomType(GamePlayer player, BaseRoom game)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendInsufficientMoney(GamePlayer player, int type)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendDispatchesMsg(GSPacketIn pkg)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendAreaBigSpeakerMsg(GamePlayer player, string msg)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPickUpNPC()
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendSpaRoomInfo(GamePlayer player, SpaRoom room)
		{
			throw new NotFiniteNumberException();
		}
		public GSPacketIn SendPlayerLeaveSpaRoom(GamePlayer player, string msg)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendPlayerLeaveSpaRoomForTimeOut(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendSpaRoomLogin(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendSpaRoomList(GamePlayer player, SpaRoom[] rooms)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendSpaRoomAddGuest(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendSpaRoomRemoveGuest(GamePlayer removePlayer)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendSpaRoomInfoPerMin(GamePlayer player, int leftTime)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendSpaRoomLoginRemind(SpaRoom room)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendConsortiaMailMessage(GamePlayer player)
		{
			throw new NotImplementedException();
		}
		public GSPacketIn SendIsContinueNextDay(GamePlayer player)
		{
			throw new NotImplementedException();
		}
        public GSPacketIn SendUpdateVIP(GamePlayer player)
        {
            throw new NotImplementedException();
        }
    }
}
