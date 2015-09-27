using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Logic
{
	public interface IGamePlayer
	{
		PlayerInfo PlayerCharacter
		{
			get;
		}
		ItemTemplateInfo MainWeapon
		{
			get;
		}
		ItemInfo SecondWeapon
		{
			get;
		}
		bool CanUseProp
		{
			get;
			set;
		}
		int GamePlayerId
		{
			get;
			set;
		}
		int AreaID
		{
			get;
		}
		bool IsArea
		{
			get;
		}
		string AreaName
		{
			get;
		}
		List<int> EquipEffect
		{
			get;
			set;
		}
		double OfferPlusRate
		{
			get;
		}
		double GPPlusRate
		{
			get;
		}
		float GMExperienceRate
		{
			get;
		}
		float GMOfferRate
		{
			get;
		}
		float GMRichesRate
		{
			get;
		}
		float AuncherExperienceRate
		{
			get;
		}
		float AuncherOfferRate
		{
			get;
		}
		float AuncherRichesRate
		{
			get;
		}
		double AntiAddictionRate
		{
			get;
		}
		double GetBaseBlood();
		double GetBaseAttack();
		double GetBaseDefence();
		int AddGP(int gp);
		int AddGpDirect(int gp);
		int RemoveGP(int gp);
		int AddGold(int value);
		int RemoveGold(int value);
		int AddMoney(int value, LogMoneyType Master, LogMoneyType Son);
		int RemoveMoney(int value, LogMoneyType Master, LogMoneyType Son);
		int AddGiftToken(int value);
		int RemoveGiftToken(int value);
		int AddRobRiches(int value);
		int AddOffer(int value);
		int RemoveOffer(int value);
		bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count);
		void ClearTempBag();
		void ClearFightBag();
		bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving);
		void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage, bool isSpanArea);
		void OnGameOver(AbstractGame game, bool isWin, int gainGP, bool isSpanArea, bool IsCouple);
		void OnMissionOver(AbstractGame game, bool isWin, int MissionID, int TurnNum);
		int ConsortiaFight(int consortiaWin, int consortiaLose, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count);
		void SendConsortiaFight(int consortiaID, int riches, string msg);
		bool SetPvePermission(int missionId, eHardLevel hardLevel);
		bool IsPvePermission(int missionId, eHardLevel hardLevel);
		bool SetFightLabPermission(int copyId, eHardLevel hardLevel, int missionId);
		bool IsFightLabPermission(int missionId, eHardLevel hardLevel);
		void Disconnect();
		void SendInsufficientMoney(int type);
		void SendMessage(eMessageType type, string message);
		void SendTCP(GSPacketIn pkg);
		void UpdateAnswerSite(int id);
        string Honor
        {
            get;
        }
	}
}
