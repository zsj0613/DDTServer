using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Center.Server.Managers;
using Game.Base.Packets;

namespace Center.Server
{
    public static class OpenAPIs
    {
        public static bool SendNotice(string str)
        {
            if (CenterServer.Instance != null && str != null && str!="")
            {
                CenterServer.Instance.SendSystemNotice(str);
                return true;
            }
            return false;
        }
        public static bool KitoffUser(int playerID, string msg)
        {
            bool result;
            try
            {
                ServerClient client = LoginMgr.GetServerClient(playerID);
                if (client != null)
                {
                    msg = (string.IsNullOrEmpty(msg) ? "You are kicking out by GM!" : msg);
                    client.SendKitoffUser(playerID, msg);
                    LoginMgr.RemovePlayer(playerID);
                    result = true;
                    return result;
                }
            }
            catch
            {
            }
            result = false;
            return result;
        }
        public static bool MailNotice(int playerID)
        {
            bool result;
            try
            {
                ServerClient client = LoginMgr.GetServerClient(playerID);
                if (client != null)
                {
                    GSPacketIn pkgMsg = new GSPacketIn(117);
                    pkgMsg.WriteInt(playerID);
                    pkgMsg.WriteInt(1);
                    client.SendTCP(pkgMsg);
                    result = true;
                    return result;
                }
            }
            catch
            {
            }
            result = false;
            return result;
        }
        public static bool CreatePlayer(int id, string name, string password, bool isFirst)
        {
            bool result;
            try
            {
                LoginMgr.CreatePlayer(new Player
                {
                    Id = id,
                    Name = name,
                    Password = password,
                    IsFirst = isFirst
                });
                result = true;
                return result;
            }
            catch (Exception e)
            {
                CenterServer.log.Error(e);
            }
            result = false;
            return result;
        }
        public static bool ValidateLoginAndGetID(string name, string password, ref int userID, ref bool isFirst)
        {
            bool result;
            try
            {
                Player[] list = LoginMgr.GetAllPlayer();
                if (list != null)
                {
                    Player[] array = list;
                    for (int i = 0; i < array.Length; i++)
                    {
                        Player p = array[i];
                        if (p.Name == name && p.Password == password)
                        {
                            userID = p.Id;
                            isFirst = p.IsFirst;
                            result = true;
                            return result;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CenterServer.log.Error(e);
            }
            result = false;
            return result;
        }
    }
}
