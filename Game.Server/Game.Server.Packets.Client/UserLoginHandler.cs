using Bussiness;
using Bussiness.Interface;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Text;

namespace Game.Server.Packets.Client
{
	[PacketHandler(1, "User Login handler")]
	public class UserLoginHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
            GameServer.log.Debug("User Login handler");
            int result;
			try
			{
				GSPacketIn pkg = packet.Clone();
				pkg.ClearContext();
				if (client.Player == null)
				{
                  //  GameServer.log.Debug("TryLogin");
                    int version = packet.ReadInt();
					int clientType = packet.ReadInt();
					byte[] src = packet.ReadBytes();
					try
					{
						src = WorldMgr.RsaCryptor.Decrypt(src, false);
					}
					//catch (ExecutionEngineException e)
					//{
					//	client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.RsaCryptorError", new object[0]));
					///	client.Disconnect();
					//	GameServer.log.Error("ExecutionEngineException", e);
					//	result = 0;
					//	return result;
					//}
					catch (Exception ex)
					{
						client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.RsaCryptorError", new object[0]));
						client.Disconnect();
						GameServer.log.Error("RsaCryptor", ex);
						result = 0;
						return result;
					}
					int fms_key = ((int)src[7] << 8) + (int)src[8];
					client.SetFsm(fms_key, version);
					string edition = GameServer.Edition;
					if (version < int.Parse(GameServer.Edition) ||version >= int.Parse(GameServer.Edition)+1000)
					{
						client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.EditionError", new object[0]));
                       // GameServer.log.Error("Edition Error");
                        client.Disconnect();
						result = 0;
						return result;
					}
					string[] temp = Encoding.UTF8.GetString(src, 9, src.Length - 9).Split(new char[]
					{
						','
					});
                  //  GameServer.log.Debug("Debug1");
                    if (temp.Length == 2)
					{
						string user = temp[0];
						string pass = temp[1];
                       // GameServer.log.Debug("Debug2");
                        if (!LoginMgr.ContainsUser(user))
						{
                          //  GameServer.log.Debug("Debug3");
                            bool isFirst = false;
							BaseInterface inter = BaseInterface.CreateInterface();
							PlayerInfo cha = inter.LoginGame(user, pass, ref isFirst);
                           
                            if (cha != null && cha.ID != 0)
							{
                            //    GameServer.log.Debug("Debug4");
                                if (cha.ID == -2)
								{
                              //      GameServer.log.Debug("Debug5");
                                    client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Forbid", new object[0]));
									client.Disconnect();
									result = 0;
									return result;
								}
								if (!isFirst)
								{
									client.Player = new GamePlayer(cha.ID, user, client, cha, this.GetClientType(clientType));
									LoginMgr.Add(cha.ID, client);
									client.Server.LoginServer.SendAllowUserLogin(cha.ID);
									client.Version = version;
                                  //  GameServer.log.Warn("Logined");
                                }
								else
								{
                                  //  GameServer.log.Debug("Debug6");
                                    client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Register", new object[0]));
									client.Disconnect();
								}
							}
							else
							{
                            //    GameServer.log.Debug("Debug7");
                             //   GameServer.log.Debug("cha.ID");
                                client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.OverTime", new object[0]));
								client.Disconnect();
							}
						}
						else
						{
                            //GameServer.log.Debug("Debug8");
                            client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.LoginError", new object[0]));
							client.Disconnect();
						}
					}
					else
					{
                        //GameServer.log.Debug("Debug9");
                        client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.LengthError", new object[0]));
						client.Disconnect();
					}
				}
			}
			catch (Exception ex)
			{
                //GameServer.log.Debug("Debug10");
                client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.ServerError", new object[0]));
				client.Disconnect();
				GameServer.log.Error(LanguageMgr.GetTranslation("UserLoginHandler.ServerError", new object[0]), ex);
			}
			result = 1;
			return result;
		}
		private eClientType GetClientType(int clientType)
		{
			eClientType result;
			switch (clientType)
			{
			case 0:
				result = eClientType.WEB;
				break;
			case 1:
				result = eClientType.Auncher;
				break;
			default:
				result = eClientType.WEB;
				break;
			}
			return result;
		}
	}
}
