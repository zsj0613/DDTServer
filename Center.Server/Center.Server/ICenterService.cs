using System;
using System.Collections.Generic;
using System.ServiceModel;
namespace Center.Server
{
	[ServiceContract]
	public interface ICenterService
	{
		[OperationContract]
		List<ServerData> GetServerList();
		[OperationContract]
		bool ChargeMoney(int userID, string chargeID);
		[OperationContract]
		bool ChargeGiftToken(int userID, int giftToken);
		[OperationContract]
		bool SystemNotice(string msg);
		[OperationContract]
		bool KitoffUser(int playerID, string msg);
		[OperationContract]
		bool ReLoadServerList();
		[OperationContract]
		bool MailNotice(int playerID);
		[OperationContract]
		bool ActivePlayer(bool isActive);
		[OperationContract]
		bool CreatePlayer(int id, string name, string password, bool isFirst);
		[OperationContract]
		bool ValidateLoginAndGetID(string name, string password, ref int userID, ref bool isFirst);
		//[OperationContract]
		//bool AASUpdateState(bool state);
		//[OperationContract]
		//int AASGetState();
		[OperationContract]
		int ExperienceRateUpdate(int serverId);
		[OperationContract]
		int NoticeServerUpdate(int serverId, int type);
		//[OperationContract]
		//bool UpdateConfigState(int type, bool state);
		//[OperationContract]
		//int GetConfigState(int type);
		[OperationContract]
		bool Reload(string type);
		[OperationContract]
		bool CheckUserValidate(int playerID, string keyString);
		[OperationContract]
		bool SendAreaBigBugle(int playerID, int areaID, string nickName, string msg);

      //  [OperationContract]
      //  void RunFight();

      //  [OperationContract]
      //  void RunLoad();



    }
}
