using Bussiness;
using Game.Base.Packets;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Center.Server
{
    public class ChargeMgr
    {
        private static LogProvider log => CenterServer.log;
        public static void Do()
        {
            try {
                using (PlayerBussiness db = new PlayerBussiness())
                {
                    ChargeInfo[] a = db.GetUserChargeList();
                    if (a.Length != 0)
                    {
                        foreach (var b in a)
                        {
                            PlayerInfo info = db.GetUserSingleByUserID(b.UserID);

                            MailInfo c = new MailInfo();
                            c.Content = "充值成功！";
                            c.Title = "充值成功！";
                            c.Gold = 0;
                            c.IsExist = true;
                            c.Money = Convert.ToInt32(b.Money*100*(GetRate(info)));
                            c.GiftToken = 0;
                            c.Receiver = info.NickName;
                            c.ReceiverID = b.UserID;
                            c.Sender = "充值系统";
                            c.SenderID = 0;
                            c.Type = 1;
                            if (db.SendMail(c))
                            {
                                if (db.DoUserCharge(b.ID))
                                {
                                    ServerClient client = LoginMgr.GetServerClient(b.UserID);
                                    if (client != null)
                                    {
                                        GSPacketIn pkgMsg = new GSPacketIn(118);
                                        pkgMsg.WriteInt(b.UserID);
                                        pkgMsg.WriteInt(1);
                                        client.SendTCP(pkgMsg);
                                        
                                       // result = true;
                                       // return result;
                                       
                                    }
                                    ChargeMgr.log.Warn("用户" + info.UserName + "充值" + b.Money + "成功");
                                }
                            }

                    }
                    }
                }
            }
            catch(Exception ex)
            {
                ChargeMgr.log.Error(ex);
            }
        }
        public static decimal GetRate(PlayerInfo info)
        {
            //V1  5%   V2  7%  V3 10%  V412%  V5 15%  V6-V10 20%
            switch (info.VipLevel)
            {
                case 1:
                    return (decimal)1.05;
                case 2:
                    return (decimal)1.07;
                case 3:
                    return (decimal)1.10;
                case 4:
                    return (decimal)1.12;
                case 5:
                    return (decimal)1.15;
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    return (decimal)1.20;
                default:
                    return (decimal)1;
            }
        }
    }
}
