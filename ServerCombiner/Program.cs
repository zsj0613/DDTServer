using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Lsj.Util;
using System.Threading.Tasks;

namespace ServerCombiner
{
    class Program
    {

        static void Main()
        {
            new Program().Do();
        }
        SqlConnection connection1;
        SqlConnection connection2;
        SqlCommand command;
        SqlDataAdapter dataadapter;

        int UserIDMax;
        int ConsortiaIDMax;
        List<int> ResultUserIDs;
        List<int> ResultConsortiaIDs;
        List<string> ResultNickNames;
        List<string> ResultConsortiaNames;

        ParallelQuery<DataRow> ResultActive_Number;
        ParallelQuery<DataRow> ResultAuction;
        ParallelQuery<DataRow> ResultConsortia;
        ParallelQuery<DataRow> ResultConsortia_Apply_Users;
        ParallelQuery<DataRow> ResultConsortia_Duty;
        ParallelQuery<DataRow> ResultConsortia_Equip_Control;
        ParallelQuery<DataRow> ResultConsortia_Event;
        ParallelQuery<DataRow> ResultConsortia_Invite_Users;
        ParallelQuery<DataRow> ResultConsortia_Users;
        ParallelQuery<DataRow> ResultMarry_Apply;
        ParallelQuery<DataRow> ResultMarry_Room_Info;
        ParallelQuery<DataRow> ResultQuestData;
        ParallelQuery<DataRow> ResultSys_Users_Charge;
        ParallelQuery<DataRow> ResultSys_Users_Detail;
        ParallelQuery<DataRow> ResultSys_Users_Fight;
        ParallelQuery<DataRow> ResultSys_Users_Friends;
        ParallelQuery<DataRow> ResultSys_Users_Goods;
        ParallelQuery<DataRow> ResultSys_Users_Info;
        ParallelQuery<DataRow> ResultSys_Users_Order;
        ParallelQuery<DataRow> ResultSys_Users_Password;
        ParallelQuery<DataRow> ResultSys_Users_Record;
        ParallelQuery<DataRow> ResultUser_Buff;


        ParallelQuery<DataRow> Active_Number;
        ParallelQuery<DataRow> Auction;
        ParallelQuery<DataRow> Consortia;
        ParallelQuery<DataRow> Consortia_Apply_Users;
        ParallelQuery<DataRow> Consortia_Duty;
        ParallelQuery<DataRow> Consortia_Equip_Control;
        ParallelQuery<DataRow> Consortia_Event;
        ParallelQuery<DataRow> Consortia_Invite_Users;
        ParallelQuery<DataRow> Consortia_Users;
        ParallelQuery<DataRow> Marry_Apply;
        ParallelQuery<DataRow> Marry_Room_Info;
        ParallelQuery<DataRow> QuestData;
        ParallelQuery<DataRow> Sys_Users_Charge;
        ParallelQuery<DataRow> Sys_Users_Detail;
        ParallelQuery<DataRow> Sys_Users_Fight;
        ParallelQuery<DataRow> Sys_Users_Friends;
        ParallelQuery<DataRow> Sys_Users_Goods;
        ParallelQuery<DataRow> Sys_Users_Info;
        ParallelQuery<DataRow> Sys_Users_Order;
        ParallelQuery<DataRow> Sys_Users_Password;
        ParallelQuery<DataRow> Sys_Users_Record;
        ParallelQuery<DataRow> User_Buff;

        List<int> ChairmanIDs;
        List<int> UserIDs;
        List<int> ConsortiaIDs;


        List<int> SendRename = new List<int>();
        List<int> SendConsortiaRename = new List<int>();




        void Do()
        {
            
            Console.WriteLine("Please input the name of database of the main server.");
            input1:
            var str = Console.ReadLine();
            connection1 = new SqlConnection($"Data Source=.;Initial Catalog={str};Integrated Security=SSPI");
            try
            {
                connection1.Open();
            }
            catch
            {
                Console.WriteLine("Fail to connect to the database, please try again");
                goto input1;
            }
            Console.WriteLine("Please input the name of database of the second server.");
            input2:
            str = Console.ReadLine();
            connection2 = new SqlConnection($"Data Source=.;Initial Catalog={str};Integrated Security=SSPI");
            try
            {
                connection2.Open();
            }
            catch
            {
                Console.WriteLine("Fail to connect to the database, please try again");
                goto input2;
            }
            Console.WriteLine("Please make sure all the data like activities, goods templates and more are the same in the databases. And Make Sure to Have Backed Up the Data. Press Any Key to Continue.");
            Console.ReadKey();
            Console.WriteLine("Read data from the main server, please wait.");
            this.Read(connection1);
            this.Optimize();
            this.CopyToResult();
            Console.WriteLine("Read data from the second server, please wait.");
            this.Read(connection2);
            this.Optimize();
            Console.WriteLine("Combine the data, please wait.");
            this.Combine();
            Console.WriteLine("Write back the data to the database, please wait.");
            this.WriteBack();
            Console.WriteLine("Send the rename card, please wait.");
            this.SendRenameCard();



            Console.WriteLine("Please press any key to exit");
            Console.ReadKey();

        }


        void SendRenameCard()
        {
            
            Parallel.ForEach(SendRename, (x) =>
            {
                SendItem(x,11994);
            });
            Parallel.ForEach(SendConsortiaRename, (x) =>
            {
                SendItem(x,11993);
            });
        }

        void CopyToResult()
        {
            this.ResultUserIDs = this.UserIDs;
            this.ResultConsortiaIDs = this.ConsortiaIDs;
            this.ResultNickNames = this.Sys_Users_Detail.Select((a) => ((string)a["NickName"])).ToList();
            this.ResultConsortiaNames = this.Consortia.Select((a) => ((string)a["ConsortiaName"])).ToList();

            this.UserIDMax = this.ResultUserIDs.Count>0?ResultUserIDs.Max():0;
            Console.WriteLine(UserIDMax);
            this.ConsortiaIDMax = this.ResultConsortiaIDs.Count > 0 ? ResultConsortiaIDs.Max() : 0;
            Console.WriteLine(ConsortiaIDMax);


            this.ResultActive_Number = this.Active_Number;
            this.ResultAuction = this.Auction;
            this.ResultConsortia = this.Consortia;
            this.ResultConsortia_Apply_Users = this.Consortia_Apply_Users;
            this.ResultConsortia_Duty = this.Consortia_Duty;
            this.ResultConsortia_Equip_Control = this.Consortia_Equip_Control;
            this.ResultConsortia_Event = this.Consortia_Event;
            this.ResultConsortia_Invite_Users = this.Consortia_Invite_Users;
            this.ResultConsortia_Users = this.Consortia_Users;
            this.ResultMarry_Apply = this.Marry_Apply;
            this.ResultMarry_Room_Info = this.Marry_Room_Info;
            this.ResultQuestData = this.QuestData;
            this.ResultSys_Users_Charge = this.Sys_Users_Charge;
            this.ResultSys_Users_Detail = this.Sys_Users_Detail;
            this.ResultSys_Users_Fight = this.Sys_Users_Fight;
            this.ResultSys_Users_Friends = this.Sys_Users_Friends;
            this.ResultSys_Users_Goods = this.Sys_Users_Goods;
            this.ResultSys_Users_Info = this.Sys_Users_Info;
            this.ResultSys_Users_Order = this.Sys_Users_Order;
            this.ResultSys_Users_Password = this.Sys_Users_Password;
            this.ResultSys_Users_Record = this.Sys_Users_Record;
            this.ResultUser_Buff = this.User_Buff;
        }

        void WriteBack()
        {
            RunProcedure("SP_Sys_Truncate_UserAllInfo", new SqlParameter[0]);
            using (SqlBulkCopy sbc = new SqlBulkCopy(connection1, SqlBulkCopyOptions.KeepIdentity, null))
            {
                DataTable dt;
                if (ResultActive_Number.Count() > 0)
                {
                    dt = ResultActive_Number.CopyToDataTable();
                    sbc.DestinationTableName = "Active_Number";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultActive_Number.Count() > 0)
                {
                    dt = ResultConsortia.CopyToDataTable();
                    sbc.DestinationTableName = "Consortia";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }


                if (ResultMarry_Apply.Count() > 0)
                {
                    dt = ResultMarry_Apply.CopyToDataTable();
                    sbc.DestinationTableName = "Marry_Apply";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultSys_Users_Charge.Count() > 0)
                {
                    dt = ResultSys_Users_Charge.CopyToDataTable();
                    sbc.DestinationTableName = "Sys_Users_Charge";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultSys_Users_Detail.Count() > 0)
                {
                    dt = ResultSys_Users_Detail.CopyToDataTable();
                    sbc.DestinationTableName = "Sys_Users_Detail";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultSys_Users_Fight.Count() > 0)
                {
                    dt = ResultSys_Users_Fight.CopyToDataTable();
                    sbc.DestinationTableName = "Sys_Users_Fight";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultSys_Users_Order.Count() > 0)
                {
                    dt = ResultSys_Users_Order.CopyToDataTable();
                    sbc.DestinationTableName = "Sys_Users_Order";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultSys_Users_Password.Count() > 0)
                {
                    dt = ResultSys_Users_Password.CopyToDataTable();
                    sbc.DestinationTableName = "Sys_Users_Password";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultSys_Users_Record.Count() > 0)
                {
                    dt = ResultSys_Users_Record.CopyToDataTable();
                    sbc.DestinationTableName = "Sys_Users_Record";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultUser_Buff.Count() > 0)
                {
                    dt = ResultUser_Buff.CopyToDataTable();
                    sbc.DestinationTableName = "User_Buff";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
            }
            using (SqlBulkCopy sbc = new SqlBulkCopy(connection1))
            {
                DataTable dt;
                if (ResultAuction.Count() > 0)
                {
                    dt = ResultAuction.CopyToDataTable();
                    sbc.DestinationTableName = "Auction";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultConsortia_Apply_Users.Count() > 0)
                {
                    dt = ResultConsortia_Apply_Users.CopyToDataTable();
                    sbc.DestinationTableName = "Consortia_Apply_Users";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultConsortia_Duty.Count() > 0)
                {
                    dt = ResultConsortia_Duty.CopyToDataTable();
                    sbc.DestinationTableName = "Consortia_Duty";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultConsortia_Equip_Control.Count() > 0)
                {
                    dt = ResultConsortia_Equip_Control.CopyToDataTable();
                    sbc.DestinationTableName = "Consortia_Equip_Control";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultConsortia_Event.Count() > 0)
                {
                    dt = ResultConsortia_Event.CopyToDataTable();
                    sbc.DestinationTableName = "Consortia_Event";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultConsortia_Invite_Users.Count() > 0)
                {
                    dt = ResultConsortia_Invite_Users.CopyToDataTable();
                    sbc.DestinationTableName = "Consortia_Invite_Users";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultConsortia_Users.Count() > 0)
                {
                    dt = ResultConsortia_Users.CopyToDataTable();
                    sbc.DestinationTableName = "Consortia_Users";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultMarry_Room_Info.Count() > 0)
                {
                    dt = ResultMarry_Room_Info.CopyToDataTable();
                    sbc.DestinationTableName = "Marry_Room_Info";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultQuestData.Count() > 0)
                {
                    dt = ResultQuestData.CopyToDataTable();
                    sbc.DestinationTableName = "QuestData";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultSys_Users_Friends.Count() > 0)
                {
                    dt = ResultSys_Users_Friends.CopyToDataTable();
                    sbc.DestinationTableName = "Sys_Users_Friends";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultSys_Users_Goods.Count() > 0)
                {
                    dt = ResultSys_Users_Goods.CopyToDataTable();
                    sbc.DestinationTableName = "Sys_Users_Goods";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
                if (ResultSys_Users_Info.Count() > 0)
                {
                    dt = ResultSys_Users_Info.CopyToDataTable();
                    sbc.DestinationTableName = "Sys_Users_Info";
                    sbc.BulkCopyTimeout = 0;
                    sbc.WriteToServer(dt);
                }
            }
        }

        void Combine()
        {
            this.UserIDMax = this.UserIDs.Max()>this.UserIDMax? this.UserIDs.Max():this.UserIDMax;
            this.ConsortiaIDMax = this.ConsortiaIDs.Max()>this.ConsortiaIDMax? this.ConsortiaIDs.Max():ConsortiaIDMax;


            Console.WriteLine("Combine Users.");

          //  Parallel.ForEach(Sys_Users_Detail, (x) =>
            foreach(var x in Sys_Users_Detail)
             {
                 int id = 0;
                 string name = "";
                 int newid = 0;
                 string newname = "";
                 id = (int)x["UserID"];
                 name = (string)x["NickName"];
                 if (ResultUserIDs.Contains(id))
                 {
                     newid = id + UserIDMax;
                     Active_Number.Where((a) => ((int)a["UserID"] == id)).Update((a) =>
                     {
                         a["UserID"] = newid;
                     });

                     Auction.Where((a) => ((int)a["AuctioneerID"] == id)).Update((a) =>
                     {
                         a["Auctioneerid"] = newid;
                     }).Where((a) => ((int)a["BuyerID"] == id)).Update((a) =>
                     {
                         a["Buyerid"] = newid;
                     });
                     Consortia.Where((a) => ((int)a["ChairmanID"] == id)).Update((a) =>
                     {
                         a["UserID"] = newid;
                     });
                     Consortia_Apply_Users.Where((a) => ((int)a["UserID"] == id)).Update((a) =>
                     {
                         a["UserID"] = newid;
                     });
                     Consortia_Invite_Users.Where((a) => ((int)a["UserID"] == id)).Update((a) =>
                     {
                         a["UserID"] = newid;
                     }).Where((a) => ((int)a["InviteID"] == id)).Update((a) =>
                     {
                         a["InviteID"] = newid;
                     });
                     Consortia_Users.Where((a) => ((int)a["UserID"] == id)).Update((a) =>
                     {
                         a["UserID"] = newid;
                     }).Where((a) => ((int)a["RatifierID"] == id)).Update((a) =>
                     {
                         a["RatifierID"] = newid;
                     });
                     Marry_Apply.Where((a) => ((int)a["UserID"] == id)).Update((a) =>
                     {
                         a["UserID"] = newid;
                     }).Where((a) => ((int)a["ApplyUserID"] == id)).Update((a) =>
                     {
                         a["ApplyUserID"] = newid;
                     });
                     Marry_Room_Info.Where((a) => ((int)a["PlayerID"] == id)).Update((a) =>
                     {
                         a["PlayerID"] = newid;
                     }).Where((a) => ((int)a["GroomUserID"] == id)).Update((a) =>
                     {
                         a["GroomUserID"] = newid;
                     }).Where((a) => ((int)a["BrideUserID"] == id)).Update((a) =>
                     {
                         a["BrideUserID"] = newid;
                     });
                     QuestData.Where((a) => ((int)a["UserID"] == id)).Update((a) =>
                     {
                         a["UserID"] = newid;
                     });
                     Sys_Users_Charge.Where((a) => ((int)a["UserID"] == id)).Update((a) =>
                     {
                         a["UserID"] = newid;
                     });
                     Sys_Users_Detail.Where((a) => ((int)a["UserID"] == id)).Update((a) =>
                     {
                         a["UserID"] = newid;
                     }).Where((a) => ((int)a["SpouseID"] == id)).Update((a) =>
                     {
                         a["SpouseID"] = newid;
                     });
                     Sys_Users_Fight.Where((a) => ((int)a["UserID"] == id)).Update((a) =>
                     {
                         a["UserID"] = newid;
                     });
                     Sys_Users_Friends.Where((a) => ((int)a["UserID"] == id)).Update((a) =>
                     {
                         a["UserID"] = newid;
                     }).Where((a) => ((int)a["FriendID"] == id)).Update((a) =>
                     {
                         a["FriendID"] = newid;
                     });
                     Sys_Users_Goods.Where((a) => ((int)a["UserID"] == id)).Update((a) =>
                     {
                         a["UserID"] = newid;
                     });
                     Sys_Users_Info.Where((a) => ((int)a["UserID"] == id)).Update((a) =>
                     {
                         a["UserID"] = newid;
                     });
                     Sys_Users_Order.Where((a) => ((int)a["UserID"] == id)).Update((a) =>
                     {
                         a["UserID"] = newid;
                     });
                     Sys_Users_Password.Where((a) => ((int)a["UserID"] == id)).Update((a) =>
                     {
                         a["UserID"] = newid;
                     });
                     Sys_Users_Record.Where((a) => ((int)a["UserID"] == id)).Update((a) =>
                     {
                         a["UserID"] = newid;
                     });
                     ;
                     User_Buff.Where((a) => ((int)a["UserID"] == id)).Update((a) =>
                     {
                         a["UserID"] = newid;
                     });

                     id = newid;
                 }
                 if (ResultNickNames.Contains(name))
                 {
                     newname = name + ".rename";
                     while (ResultNickNames.Contains(newname))
                     {
                         newname += ".rename";
                     }
                     Auction.Where((a) => ((string)a["AuctioneerName"] == name)).Update((a) =>
                      {
                         a["AuctioneerName"] = newname;
                     }).Where((a) => ((string)a["BuyerName"] == name)).Update((a) =>
                     {
                         a["BuyerName"] = newname;
                     });
                     Consortia.Where((a) => ((string)a["ChairmanName"] == name)).Update((a) =>
                     {
                         a["ChairmanName"] = newname;
                     });
                     Consortia_Apply_Users.Where((a) => ((string)a["UserName"] == name)).Update((a) =>
                      {
                         a["UserName"] = newname;
                     });
                     Consortia_Invite_Users.Where((a) => ((string)a["UserName"] == name)).Update((a) =>
                      {
                         a["UserName"] = newname;
                     }).Where((a) => ((string)a["InviteName"] == name)).Update((a) =>
                     {
                         a["InviteName"] = newname;
                     });
                     Consortia_Users.Where((a) => ((string)a["UserName"] == name)).Update((a) =>
                     {
                         a["UserName"] = newname;
                     }).Where((a) => ((string)a["RatifierName"] == name)).Update((a) =>
                     {
                         a["RatifierName"] = newname;
                     });
                     Marry_Apply.Where((a) => ((string)a["ApplyUserName"] == name)).Update((a) =>
                     {
                         a["ApplyUserName"] = newname;
                     });
                     Marry_Room_Info.Where((a) => ((string)a["PlayerName"] == name)).Update((a) =>
                     {
                         a["PlayerName"] = newname;
                     }).Where((a) => ((string)a["GroomName"] == name)).Update((a) =>
                     {
                         a["GroomName"] = newname;
                     }).Where((a) => ((string)a["BrideName"] == name)).Update((a) =>
                     {
                         a["BrideName"] = newname;
                     });

                     Sys_Users_Detail.Where((a) => ((string)a["NickName"] == name)).Update((a) =>
                     {
                         a["NickName"] = newname;
                     }).Where((a) => ((string)a["SpouseName"] == name)).Update((a) =>
                     {
                         a["SpouseName"] = newname;
                     });
                     this.SendRename.Add(id);
                 }
             };
            Console.WriteLine("Combine Consortia.");


          //  Parallel.ForEach(this.Consortia, (y) =>
                        foreach (var y in Consortia)
            {
                 int id = 0;
                 string name = "";
                 int newid = 0;
                 string newname = "";
                 id = (int)y["ConsortiaID"];
                 name = (string)y["ConsortiaName"];
                 if (ResultConsortiaIDs.Contains(id))
                 {
                     newid = id + ConsortiaIDMax;

                     Consortia.Where((a) => ((int)a["ConsortiaID"] == id)).Update((a) =>
                    {
                        a["ConsortiaID"] = newid;
                    });
                     Consortia_Apply_Users.Where((a) => ((int)a["ConsortiaID"] == id)).Update((a) =>
                    {
                        a["ConsortiaID"] = newid;
                    });
                     Consortia_Duty.Where((a) => ((int)a["ConsortiaID"] == id)).Update((a) =>
                    {
                        a["ConsortiaID"] = newid;
                    });
                     Consortia_Equip_Control.Where((a) => ((int)a["ConsortiaID"] == id)).Update((a) =>
                    {
                        a["ConsortiaID"] = newid;
                    });
                     Consortia_Event.Where((a) => ((int)a["ConsortiaID"] == id)).Update((a) =>
                    {
                        a["ConsortiaID"] = newid;
                    });
                     Consortia_Invite_Users.Where((a) => ((int)a["ConsortiaID"] == id)).Update((a) =>
                    {
                        a["ConsortiaID"] = newid;
                    });
                     Consortia_Users.Where((a) => ((int)a["ConsortiaID"] == id)).Update((a) =>
                    {
                        a["ConsortiaID"] = newid;
                    });

                     Sys_Users_Detail.Where((a) => ((int)a["ConsortiaID"] == id)).Update((a) =>
                    {
                        a["ConsortiaID"] = newid;
                    });
                     id = newid;
                 }
                 if (ResultConsortiaNames.Contains(name))
                 {
                     newname = name + ".rename";
                     while (ResultConsortiaNames.Contains(newname))
                     {
                         newname += ".rename";
                     }
                     Consortia.Where((a) => ((string)a["ConsortiaName"] == name)).Update((a) =>
                    {
                        a["ConsortiaName"] = newname;
                    });
                    Consortia_Apply_Users.Where((a) => ((string)a["ConsortiaName"] == name)).Update((a) =>
                    {
                        a["ConsortiaName"] = newname;
                    });
                     this.SendConsortiaRename.Add((int)y["ChairmanID"]);
                 }
             };




            this.ResultActive_Number = (ParallelQuery<DataRow>)ResultActive_Number.Concat(Active_Number);
            this.ResultAuction = (ParallelQuery<DataRow>)ResultAuction.Concat(Auction);
            this.ResultConsortia = (ParallelQuery<DataRow>)ResultConsortia.Concat(Consortia);
            this.ResultConsortia_Apply_Users = (ParallelQuery<DataRow>)ResultConsortia_Apply_Users.Concat(Consortia_Apply_Users);
            this.ResultConsortia_Duty = (ParallelQuery<DataRow>)ResultConsortia_Duty.Concat(Consortia_Duty);
            this.ResultConsortia_Equip_Control = (ParallelQuery<DataRow>)ResultConsortia_Equip_Control.Concat(Consortia_Equip_Control);
            this.ResultConsortia_Event = (ParallelQuery<DataRow>)ResultConsortia_Event.Concat(Consortia_Event);
            this.ResultConsortia_Invite_Users = (ParallelQuery<DataRow>)ResultConsortia_Invite_Users.Concat(Consortia_Invite_Users);
            this.ResultConsortia_Users = (ParallelQuery<DataRow>)ResultConsortia_Users.Concat(Consortia_Users);
            this.ResultMarry_Apply = (ParallelQuery<DataRow>)ResultMarry_Apply.Concat(Marry_Apply);
            this.ResultMarry_Room_Info = (ParallelQuery<DataRow>)ResultMarry_Room_Info.Concat(Marry_Room_Info);
            this.ResultQuestData = (ParallelQuery<DataRow>)ResultQuestData.Concat(QuestData);
            this.ResultSys_Users_Charge = (ParallelQuery<DataRow>)ResultSys_Users_Charge.Concat(Sys_Users_Charge);
            this.ResultSys_Users_Detail = (ParallelQuery<DataRow>)ResultSys_Users_Detail.Concat(Sys_Users_Detail);
            this.ResultSys_Users_Fight = (ParallelQuery<DataRow>)ResultSys_Users_Fight.Concat(Sys_Users_Fight);
            this.ResultSys_Users_Friends = (ParallelQuery<DataRow>)ResultSys_Users_Friends.Concat(Sys_Users_Friends);
            this.ResultSys_Users_Goods = (ParallelQuery<DataRow>)ResultSys_Users_Goods.Concat(Sys_Users_Goods);
            this.ResultSys_Users_Info = (ParallelQuery<DataRow>)ResultSys_Users_Info.Concat(Sys_Users_Info);
            this.ResultSys_Users_Order = (ParallelQuery<DataRow>)ResultSys_Users_Order.Concat(Sys_Users_Order);
            this.ResultSys_Users_Password = (ParallelQuery<DataRow>)ResultSys_Users_Password.Concat(Sys_Users_Password);
            this.ResultSys_Users_Record = (ParallelQuery<DataRow>)ResultSys_Users_Record.Concat(Sys_Users_Record);
            this.ResultUser_Buff = (ParallelQuery<DataRow>)ResultUser_Buff.Concat(User_Buff);
        }

        void Optimize()
        {
            Console.WriteLine("Optimize the data");
            this.ChairmanIDs = this.Consortia.Select((a) => ((int)a["ChairmanID"])).ToList();
            Console.WriteLine("Optimize the users");
            Console.WriteLine("Remove users");
            this.Sys_Users_Detail = this.Sys_Users_Detail.Where((a) => (!(a["LastDate"] is DBNull)&& !(a["NickName"] is DBNull) && (DateTime)a["LastDate"] - DateTime.Now <= new TimeSpan(30, 0, 0, 0)&&!ChairmanIDs.Contains((int)a["UserID"]) &&(int)a["ChargedMoney"]==0));
            this.UserIDs = this.Sys_Users_Detail.Select((a) => ((int)a["UserID"])).ToList();
            Console.WriteLine("Divorce");
            Parallel.ForEach(Sys_Users_Detail, (a) => {
                if (!UserIDs.Contains((int)a["UserID"]))
                {
                    a["IsMarried"] = false;
                    a["SpouseID"] = 0;
                    a["SpouseName"] = "";
                }
            });
            this.Sys_Users_Charge = this.Sys_Users_Charge.Where((a) => (!(bool)a["HasCharged"] && UserIDs.Contains((int)a["UserID"])));
            this.Sys_Users_Fight = this.Sys_Users_Fight.Where((a) => ((bool)a["IsExist"] && UserIDs.Contains((int)a["UserID"])));
            this.Sys_Users_Friends = this.Sys_Users_Friends.Where((a) => ((bool)a["IsExist"] && UserIDs.Contains((int)a["UserID"]) && UserIDs.Contains((int)a["FriendID"])));
            Console.WriteLine("Optimize the user goods");
            this.Sys_Users_Goods = this.Sys_Users_Goods.Where((a) => ((bool)a["IsExist"] && UserIDs.Contains((int)a["UserID"])));
            this.Sys_Users_Info = this.Sys_Users_Info.Where((a) => (UserIDs.Contains((int)a["UserID"]))).Where((a) => ((int)a["InviteID"] != 0 && UserIDs.Contains((int)a["InviteID"]))).Update((a) =>
            {
                a["InviteID"] = 0;
            });
            this.Sys_Users_Order = this.Sys_Users_Order.Where((a) => (UserIDs.Contains((int)a["UserID"])));
            this.Sys_Users_Password = this.Sys_Users_Password.Where((a) => (UserIDs.Contains((int)a["UserID"])));
            this.Sys_Users_Record = this.Sys_Users_Record.Where((a) => (UserIDs.Contains((int)a["UserID"])));
            this.User_Buff = this.User_Buff.Where((a) => ((bool)a["IsExist"] && UserIDs.Contains((int)a["UserID"])));
            Console.WriteLine("Optimize the user message");
            Console.WriteLine("Optimize the activity");
            this.Active_Number = this.Active_Number.Where((a) => (UserIDs.Contains((int)a["UserID"])|| (int)a["UserID"]==0));
            Console.WriteLine("Optimize the auction");
            this.Auction = this.Auction.Where((a)=>(UserIDs.Contains((int)a["AuctioneerID"])&&(int)a["BuyerID"]==0));
            Console.WriteLine("Optimize the consortia");
            this.Consortia = this.Consortia.Where((a) => ((bool)a["IsExist"]));
            this.ConsortiaIDs = this.Consortia.Select((a) => ((int)a["ConsortiaID"])).ToList();
            this.Consortia_Apply_Users = this.Consortia_Apply_Users.Where((a)=>((bool)a["IsExist"]&&ConsortiaIDs.Contains((int)a["ConsortiaID"])));
            this.Consortia_Duty = this.Consortia_Duty.Where((a) => ((bool)a["IsExist"] && ConsortiaIDs.Contains((int)a["ConsortiaID"])));
            this.Consortia_Equip_Control = this.Consortia_Equip_Control.Where((a) => ((bool)a["IsExist"] && ConsortiaIDs.Contains((int)a["ConsortiaID"])));
            this.Consortia_Event = this.Consortia_Event.Where((a) => ((bool)a["IsExist"] && ConsortiaIDs.Contains((int)a["ConsortiaID"])));
            this.Consortia_Invite_Users = this.Consortia_Invite_Users.Where((a) => ((bool)a["IsExist"] && ConsortiaIDs.Contains((int)a["ConsortiaID"])&& UserIDs.Contains((int)a["UserID"]) && UserIDs.Contains((int)a["InviteID"])));
            this.Consortia_Users = this.Consortia_Users.Where((a) => ((bool)a["IsExist"] && ConsortiaIDs.Contains((int)a["ConsortiaID"])));
            Console.WriteLine("Optimize the marry");
            this.Marry_Apply = this.Marry_Apply.Where((a) => ((bool)a["IsExist"] && UserIDs.Contains((int)a["UserID"]) && UserIDs.Contains((int)a["ApplyUserID"])));
            this.Marry_Room_Info = this.Marry_Room_Info.Where((a) => ((bool)a["IsExist"] && UserIDs.Contains((int)a["PlayerID"]) && UserIDs.Contains((int)a["GroomID"]) && UserIDs.Contains((int)a["BrideID"])));
            Console.WriteLine("Optimize the quest");
            this.QuestData = this.QuestData.Where((a) => ((bool)a["IsExist"] && UserIDs.Contains((int)a["UserID"])));
        }

        void Read(SqlConnection con)
        {
            this.Active_Number = GetDataTable("Active_Number", con).AsEnumerable().AsParallel();
            this.Auction = GetDataTable("Auction", con).AsEnumerable().AsParallel();
            this.Consortia = GetDataTable("Consortia", con).AsEnumerable().AsParallel();
            this.Consortia_Apply_Users = GetDataTable("Consortia_Apply_Users", con).AsEnumerable().AsParallel();
            this.Consortia_Duty = GetDataTable("Consortia_Duty", con).AsEnumerable().AsParallel();
            this.Consortia_Equip_Control = GetDataTable("Consortia_Equip_Control", con).AsEnumerable().AsParallel();
            this.Consortia_Event = GetDataTable("Consortia_Event", con).AsEnumerable().AsParallel();
            this.Consortia_Invite_Users = GetDataTable("Consortia_Invite_Users", con).AsEnumerable().AsParallel();
            this.Consortia_Users = GetDataTable("Consortia_Users", con).AsEnumerable().AsParallel();
            this.Marry_Apply = GetDataTable("Marry_Apply", con).AsEnumerable().AsParallel();
            this.Marry_Room_Info = GetDataTable("Marry_Room_Info", con).AsEnumerable().AsParallel();
            this.QuestData = GetDataTable("QuestData", con).AsEnumerable().AsParallel();
            this.Sys_Users_Charge = GetDataTable("Sys_Users_Charge", con).AsEnumerable().AsParallel();
            this.Sys_Users_Detail = GetDataTable("Sys_Users_Detail", con).AsEnumerable().AsParallel();
            this.Sys_Users_Fight = GetDataTable("Sys_Users_Fight", con).AsEnumerable().AsParallel();
            this.Sys_Users_Friends = GetDataTable("Sys_Users_Friends", con).AsEnumerable().AsParallel();
            this.Sys_Users_Goods = GetDataTable("Sys_Users_Goods", con).AsEnumerable().AsParallel();
            this.Sys_Users_Info = GetDataTable("Sys_Users_Info", con).AsEnumerable().AsParallel();
            this.Sys_Users_Order = GetDataTable("Sys_Users_Order", con).AsEnumerable().AsParallel();
            this.Sys_Users_Password = GetDataTable("Sys_Users_Password", con).AsEnumerable().AsParallel();
            this.Sys_Users_Record = GetDataTable("Sys_Users_Record", con).AsEnumerable().AsParallel();
            this.User_Buff = GetDataTable("User_Buff", con).AsEnumerable().AsParallel();
        }

        SqlCommand InitCommand(string com,SqlConnection con)
        {
            this.command = new SqlCommand();
            this.command.CommandType = CommandType.Text;
            this.command.CommandText = com;
            this.command.Connection = con;
            return command;
        }
        DataTable GetDataTable(string tablename, SqlConnection con)
        {
            var com = $"select * from {tablename}";
            var table = new DataTable("tablename");
            this.InitCommand(com, con);
            this.dataadapter = new SqlDataAdapter(command);
            this.dataadapter.Fill(table);
            return table;
        }

        void RunProcedure(string ProcedureName, SqlParameter[] SqlParameters)
        {
            var _SqlCommand = new SqlCommand();
            _SqlCommand.Connection = this.connection1;
            _SqlCommand.CommandType = CommandType.StoredProcedure;
            _SqlCommand.CommandText = ProcedureName;
            for (int i = 0; i < SqlParameters.Length; i++)
            {
                SqlParameter parameter = SqlParameters[i];
                _SqlCommand.Parameters.Add(parameter);
            }
            _SqlCommand.ExecuteNonQuery();

        }

        void SendItem(int userid, int templateid)
        {
            SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@Title", "由于合区导致名称重复，特补偿一张改名卡"),
                    new SqlParameter("@Content", "由于合区导致名称重复，特补偿一张改名卡"),
                    new SqlParameter("@UserID", userid),
                    new SqlParameter("@Gold", 0),
                    new SqlParameter("@Money", 0),
                    new SqlParameter("@GiftToken", 0),
                    new SqlParameter("@Param", $"{templateid},1,0,0,0,0,0,0,1"),
                    new SqlParameter("@Result", SqlDbType.Int)
                };
            para[7].Direction = ParameterDirection.ReturnValue;
            this.RunProcedure("SP_Admin_SendAllItem", para);

        }


    }
}
