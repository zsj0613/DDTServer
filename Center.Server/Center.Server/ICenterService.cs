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
		bool SystemNotice(string msg);
		[OperationContract]
		bool KitoffUser(int playerID, string msg);
		[OperationContract]
		bool ReLoadServerList();
		[OperationContract]
		bool MailNotice(int playerID);
		[OperationContract]
		bool CreatePlayer(int id, string name, string password, bool isFirst);
		[OperationContract]
		bool ValidateLoginAndGetID(string name, string password, ref int userID, ref bool isFirst);
		[OperationContract]
		int ExperienceRateUpdate(int serverId);
		[OperationContract]
		bool Reload(string type);
		[OperationContract]
		bool CheckUserValidate(int playerID, string keyString);



    }
}
