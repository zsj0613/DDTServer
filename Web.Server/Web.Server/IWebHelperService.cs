using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Web.Server
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IWebHelperService”。
    [ServiceContract]
    public interface IWebHelperService
    {
        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/Register", ReplyAction = "http://127.0.0.1/WebHelperClient/RegisterResponse")]
        bool Register(string username, string password, int inviteid);
        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/GetIDByUserName", ReplyAction = "http://127.0.0.1/WebHelperClient/GetIDByUserNameResponse")]
        int[] GetIDByUserName(string username);
        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/IsOpen", ReplyAction = "http://127.0.0.1/WebHelperClient/IsOpenResponse")]
        bool IsOpen();
        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/CheckUser", ReplyAction = "http://127.0.0.1/WebHelperClient/CheckUserResponse")]
        bool CheckUser(string username, string password,int inviteid);
        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/GetUserType", ReplyAction = "http://127.0.0.1/WebHelperClient/GetUserTypeResponse")]
        int GetUserType(string name);
        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/GetUserNameByID", ReplyAction = "http://127.0.0.1/WebHelperClient/GetUserNameByIDResponse")]
        string GetUserNameByID(int userid);
        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/ExistsUsername", ReplyAction = "http://127.0.0.1/WebHelperClient/ExistsUsernameResponse")]
        bool ExistsUsername(string name);
        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/AddPlayer", ReplyAction = "http://127.0.0.1/WebHelperClient/AddPlayerResponse")]
        void AddPlayer(string user, string pass);
        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/Login", ReplyAction = "http://127.0.0.1/WebHelperClient/LoginResponse")]
        string Login(string p, string site, string IP);
        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/GetRunMgr", ReplyAction = "http://127.0.0.1/WebHelperClient/GetRunMgrResponse")]
        List<bool> GetRunMgr();

        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/GetAllUserInfo", ReplyAction = "http://127.0.0.1/WebHelperClient/GetAllUserInfoResponse")]
        List<UserInfo> GetAllUserInfo();
        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/GetSingleCategoryItemTemplates", ReplyAction = "http://127.0.0.1/WebHelperClient/GetSingleCategoryItemTemplatesResponse")]
        List<ItemTemplateInfo> GetSingleCategoryItemTemplates(int i);
        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/GetSingleItemTemplate", ReplyAction = "http://127.0.0.1/WebHelperClient/GetSingleItemTemplateResponse")]
        ItemTemplateInfo GetSingleItemTemplate(int i);
        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/SendMailAndItem", ReplyAction = "http://127.0.0.1/WebHelperClient/SendMailAndItemResponse")]
        void SendMailAndItem(string title, string content, int b, int gold, int money, int giftToken, string str);
        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/Reconnect", ReplyAction = "http://127.0.0.1/WebHelperClient/ReconnectResponse")]
        bool Reconnect();
        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/GMAction", ReplyAction = "http://127.0.0.1/WebHelperClient/GMActionResponse")]
        string GMAction(string action, Dictionary<string, string> param);
        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/ServerList", ReplyAction = "http://127.0.0.1/WebHelperClient/ServerListResponse")]
        string ServerList();
        [OperationContract(Action = "http://127.0.0.1/WebHelperClient/MailNotice", ReplyAction = "http://127.0.0.1/WebHelperClient/MailNoticeResponse")]
        void MailNotice(int userid);
    }
}
