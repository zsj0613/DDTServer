using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(192, "添加拍卖")]
	public class AuctionAddHandler : AbstractPlayerPacketHandler
	{
		private static ThreadSafeRandom random = new ThreadSafeRandom();
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			eBageType bagType = (eBageType)packet.ReadByte();
			int place = packet.ReadInt();
			int payType = (int)packet.ReadByte();
			int price = packet.ReadInt();
			int mouthful = packet.ReadInt();
			int validDate = packet.ReadInt();
			string msg = "AuctionAddHandler.Fail";
			payType = 1;
			int result;
			if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				result = 0;
			}
			else
			{
				if (price < 0 || (mouthful != 0 && mouthful < price))
				{
					result = 0;
				}
				else
				{
					if (payType != 0)
					{
						payType = 1;
					}
					int needGold = (validDate == 0) ? 100 : ((validDate == 1) ? 200 : 300);
					needGold = ((needGold < 1) ? 1 : needGold);
					ItemInfo goods = player.GetItemAt(bagType, place);
					if (price < 0)
					{
						msg = "AuctionAddHandler.Msg1";
					}
					else
					{
						if (mouthful != 0 && mouthful < price)
						{
							msg = "AuctionAddHandler.Msg2";
						}
						else
						{
							if (needGold > player.PlayerCharacter.Gold)
							{
								msg = "AuctionAddHandler.Msg3";
							}
							else
							{
								if (goods == null)
								{
									msg = "AuctionAddHandler.Msg4";
								}
								else
								{
									if (goods.IsBinds)
									{
										msg = "AuctionAddHandler.Msg5";
									}
									else
									{
										if (goods.ItemID == 0)
										{
											using (PlayerBussiness db = new PlayerBussiness())
											{
												db.AddGoods(goods);
											}
										}
										AuctionInfo info = new AuctionInfo();
										info.AuctioneerID = player.PlayerCharacter.ID;
										info.AuctioneerName = player.PlayerCharacter.NickName;
										info.BeginDate = DateTime.Now;
										info.BuyerID = 0;
										info.BuyerName = "";
										info.IsExist = true;
										info.ItemID = goods.ItemID;
										info.Mouthful = mouthful;
										info.PayType = payType;
										info.Price = price;
										info.Rise = price / 10;
										info.Rise = ((info.Rise < 1) ? 1 : info.Rise);
										info.Name = goods.Template.Name;
										info.Category = goods.Template.CategoryID;
										info.ValidDate = ((validDate == 0) ? 8 : ((validDate == 1) ? 24 : 48));
										info.TemplateID = goods.TemplateID;
										info.Random = AuctionAddHandler.random.Next(45, 75);
										using (PlayerBussiness db = new PlayerBussiness())
										{
											if (db.AddAuction(info))
											{
												player.TakeOutItem(goods);
												player.SaveIntoDatabase();
												player.RemoveGold(needGold);
												msg = "AuctionAddHandler.Msg6";
												player.Out.SendAuctionRefresh(info, info.AuctionID, true, goods);
											}
										}
									}
								}
							}
						}
					}
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(msg, new object[0]));
					result = 0;
				}
			}
			return result;
		}
	}
}
