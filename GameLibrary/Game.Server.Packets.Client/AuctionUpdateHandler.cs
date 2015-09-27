using Bussiness;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(193, "更新拍卖")]
	public class AuctionUpdateHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			int price = packet.ReadInt();
			bool result = false;
			GSPacketIn pkg = packet.Clone();
			pkg.ClearContext();
			string msg = "AuctionUpdateHandler.Fail";
			int result2;
			if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				result2 = 0;
			}
			else
			{
				using (PlayerBussiness db = new PlayerBussiness())
				{
					AuctionInfo info = db.GetAuctionSingle(id);
					if (info == null)
					{
						msg = "AuctionUpdateHandler.Msg1";
					}
					else
					{
						if (info.PayType == 0 && price > player.PlayerCharacter.Gold)
						{
							msg = "AuctionUpdateHandler.Msg2";
						}
						else
						{
							if (info.PayType == 1 && price > player.PlayerCharacter.Money)
							{
								msg = "AuctionUpdateHandler.Msg3";
							}
							else
							{
								if (info.BuyerID == 0 && info.Price > price)
								{
									msg = "AuctionUpdateHandler.Msg4";
								}
								else
								{
									if (info.BuyerID != 0 && info.Price + info.Rise > price && (info.Mouthful == 0 || info.Mouthful > price))
									{
										msg = "AuctionUpdateHandler.Msg5";
									}
									else
									{
										int oldBuyerID = info.BuyerID;
										info.BuyerID = player.PlayerCharacter.ID;
										info.BuyerName = player.PlayerCharacter.NickName;
										info.Price = price;
										if (info.Mouthful != 0 && price >= info.Mouthful)
										{
											info.Price = info.Mouthful;
											info.IsExist = false;
										}
										if (db.UpdateAuction(info))
										{
											if (info.PayType == 0)
											{
												player.RemoveGold(info.Price);
											}
											else
											{
												player.RemoveMoney(info.Price, LogMoneyType.Auction, LogMoneyType.Auction_Update);
											}
											if (info.IsExist)
											{
												msg = "AuctionUpdateHandler.Msg6";
											}
											else
											{
												msg = "AuctionUpdateHandler.Msg7";
												player.Out.SendMailResponse(info.AuctioneerID, eMailRespose.Receiver);
												player.Out.SendMailResponse(info.BuyerID, eMailRespose.Receiver);
											}
											if (oldBuyerID != 0)
											{
												player.Out.SendMailResponse(oldBuyerID, eMailRespose.Receiver);
											}
											result = true;
										}
									}
								}
							}
						}
					}
					player.Out.SendAuctionRefresh(info, id, info != null && info.IsExist, null);
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(msg, new object[0]));
				}
				pkg.WriteBoolean(result);
				pkg.WriteInt(id);
				player.Out.SendTCP(pkg);
				result2 = 0;
			}
			return result2;
		}
	}
}
