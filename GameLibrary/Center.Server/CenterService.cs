using Bussiness;
using Bussiness.Interface;
//using Center.Server.Statics;
using Game.Base.Packets;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;

namespace Center.Server
{
	public class CenterService : ICenterService
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static ServiceHost host;
		public List<ServerData> GetServerList()
		{
			ServerInfo[] sl = ServerMgr.Servers;
			List<ServerData> list = new List<ServerData>();
			ServerInfo[] array = sl;
			for (int i = 0; i < array.Length; i++)
			{
				ServerInfo s = array[i];
				list.Add(new ServerData
				{
					Id = s.ID,
					Name = s.Name,
					Ip = s.IP,
					Port = s.Port,
					State = s.State,
					MustLevel = s.MustLevel,
					LowestLevel = s.LowestLevel,
					Online = s.Online
				});
			}
			return list;
		}
		
		public bool SendAreaBigBugle(int playerID, int areaID, string nickName, string msg)
		{
			GSPacketIn pkg = new GSPacketIn(25);
			pkg.WriteInt(areaID);
			pkg.WriteInt(playerID);
			pkg.WriteString(nickName);
			pkg.WriteString(msg);
			ServerClient[] clients = CenterServer.Instance.GetAllClients();
			ServerClient[] array = clients;
			for (int i = 0; i < array.Length; i++)
			{
				ServerClient client = array[i];
				client.SendAreaBigBugleToServer(pkg);
			}
			return true;
		}
		public bool ChargeMoney(int userID, string chargeID)
		{
			ServerClient client = LoginMgr.GetServerClient(userID);
			ChargeRewardMgr.DoChargeReward(userID);
			bool result;
			if (client != null)
			{
				client.SendChargeMoney(userID, chargeID);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public bool ChargeGiftToken(int userID, int giftToken)
		{
			PlayerInfo info = null;
			bool result;
			using (PlayerBussiness db = new PlayerBussiness())
			{
				info = db.GetUserSingleByUserID(userID);
				if (info == null)
				{
					result = false;
					return result;
				}
			}
			MailInfo mail = new MailInfo();
			mail.Content = LanguageMgr.GetTranslation("ChargeGiftTokenToUser.Content", new object[]
			{
				giftToken
			});
			mail.Title = LanguageMgr.GetTranslation("ChargeGiftTokenToUser.Title", new object[0]);
			mail.Gold = 0;
			mail.IsExist = true;
			mail.Money = 0;
			mail.GiftToken = giftToken;
			mail.Receiver = info.NickName;
			mail.ReceiverID = info.ID;
			mail.Sender = info.NickName;
			mail.SenderID = info.ID;
			mail.Type = 1;
			using (PlayerBussiness db = new PlayerBussiness())
			{
				if (db.SendMail(mail))
				{
					ServerClient client = LoginMgr.GetServerClient(userID);
					if (client != null)
					{
						GSPacketIn pkg = new GSPacketIn(117);
						pkg.WriteInt(userID);
						pkg.WriteInt(1);
						CenterServer.Instance.SendToALL(pkg);
					}
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
		public bool SystemNotice(string msg)
		{
			bool result;
			try
			{
				CenterServer.Instance.SendSystemNotice(msg);
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}
		public bool KitoffUser(int playerID, string msg)
		{
			bool result;
			try
			{
				ServerClient client = LoginMgr.GetServerClient(playerID);
				if (client != null)
				{
					msg = (string.IsNullOrEmpty(msg) ? "You are kicking out by GM!" : msg);
					client.SendKitoffUser(playerID, msg);
					//Console.WriteLine(msg);
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
		public bool ReLoadServerList()
		{
			return ServerMgr.ReLoadServerList();
		}
		public bool MailNotice(int playerID)
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
		public bool AASUpdateState(bool state)
		{
			bool result;
			try
			{
				GameProperties.ASS_STATE = state;
				GameProperties.Save();
				result = CenterServer.Instance.ClientsExecuteCmd("/load /property");
				return result;
			}
			catch
			{
			}
			result = false;
			return result;
		}
		public int AASGetState()
		{
			int result;
			try
			{
				result = (GameProperties.ASS_STATE ? 1 : 0);
				return result;
			}
			catch
			{
			}
			result = 2;
			return result;
		}
		public int ExperienceRateUpdate(int serverId)
		{
			int result;
			try
			{
				result = CenterServer.Instance.RateUpdate(serverId);
				return result;
			}
			catch
			{
			}
			result = 2;
			return result;
		}
		public int NoticeServerUpdate(int serverId, int type)
		{
			int result;
			try
			{
				result = CenterServer.Instance.NoticeServerUpdate(serverId, type);
				return result;
			}
			catch
			{
			}
			result = 2;
			return result;
		}
		public bool UpdateConfigState(int type, bool state)
		{
			bool result;
			try
			{
				if (type == 1)
				{
					GameProperties.ASS_STATE = state;
				}
				else
				{
					GameProperties.DAILY_AWARD_STATE = state;
				}
				GameProperties.Save();
				result = CenterServer.Instance.ClientsExecuteCmd("/load /property");
				return result;
			}
			catch
			{
			}
			result = false;
			return result;
		}
		public int GetConfigState(int type)
		{
			int result;
			try
			{
				switch (type)
				{
				case 1:
					result = (GameProperties.ASS_STATE ? 1 : 0);
					return result;
				case 2:
					result = (GameProperties.DAILY_AWARD_STATE ? 1 : 0);
					return result;
				}
			}
			catch
			{
			}
			result = 2;
			return result;
		}
		public bool Reload(string type)
		{
			bool result;
			try
			{
				result = CenterServer.Instance.ClientsExecuteCmd("/load /" + type);
				return result;
			}
			catch
			{
			}
			result = false;
			return result;
		}
		public bool ActivePlayer(bool isActive)
		{
			bool result;
			try
			{
				if (isActive)
				{
					//LogMgr.AddRegCount();
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
		public bool CreatePlayer(int id, string name, string password, bool isFirst)
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
			catch(Exception e)
			{
                CenterService.log.Error(e);
			}
			result = false;
			return result;
		}
		public bool ValidateLoginAndGetID(string name, string password, ref int userID, ref bool isFirst)
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
			catch(Exception e)
			{
                CenterService.log.Error(e);
			}
			result = false;
			return result;
		}
		public bool CheckUserValidate(int playerID, string keyString)
		{
			bool result;
			try
			{
				Player player = LoginMgr.GetPlayer(playerID);
				if (player != null)
				{
					result = (BaseInterface.md5(player.Password) == keyString);
					return result;
				}
			}
			catch (Exception ex)
			{
				CenterService.log.Error(string.Format("Check User Validate Error:{0}", ex));
			}
			result = false;
			return result;
		}
		public static bool Start()
		{
			bool result;
			try
			{
				CenterService.host = new ServiceHost(typeof(CenterService), new Uri[0]);
				CenterService.host.Open();
				CenterService.log.Info("Center Service started!");
				result = true;
			}
			catch (Exception ex)
			{
				CenterService.log.ErrorFormat("Start center server failed:{0}", ex);
				result = false;
			}
			return result;
		}
		public static void Stop()
		{
			try
			{
				if (CenterService.host != null)
				{
					CenterService.host.Close();
					CenterService.host = null;
				}
			}
			catch
			{
			}
		}
	}
}
