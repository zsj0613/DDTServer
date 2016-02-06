using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Game.Server.Packets.Client
{
	[PacketHandler(124, "背包整理")]
	public class ArrangeBagHandler : AbstractPlayerPacketHandler
	{
		private static LogProvider log => LogProvider.Default;
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int count = packet.ReadInt();
			PlayerInventory bag = player.GetInventory((eBageType)packet.ReadInt());
			Dictionary<int, int> switches = new Dictionary<int, int>();
			for (int i = 0; i < count; i++)
			{
				int old_place = packet.ReadInt();
				int new_place = packet.ReadInt();
				if (!switches.ContainsKey(old_place))
				{
					switches.Add(old_place, new_place);
				}
				else
				{
					ArrangeBagHandler.log.Error(string.Format("client:{0}   error client data,index already exist in the dics.", player.PlayerId));
				}
			}
			int result2;
			if (switches.Count != bag.GetItems(bag.BeginSlot, bag.Capalility - 1).Count)
			{
				result2 = 0;
			}
			else
			{
				Dictionary<int, ItemInfo> rawitems = bag.GetRawSpaces();
				bag.BeginChanges();
				bool result = false;
				try
				{
					bag.Clear(bag.BeginSlot, bag.Capalility - 1);
					foreach (KeyValuePair<int, int> sp in switches)
					{
						if (sp.Key < bag.BeginSlot || sp.Value < bag.BeginSlot)
						{
							throw new Exception(string.Format("can't operate that place: old {0}  new  {1}", sp.Key, sp.Value));
						}
						ItemInfo it = rawitems[sp.Key];
						if (!bag.AddItemTo(it, sp.Value))
						{
							throw new Exception(string.Format("move item error: old place:{0} new place:{1}", sp.Key, sp.Value));
						}
					}
					result = true;
				}
				catch (Exception ex)
				{
					ArrangeBagHandler.log.ErrorFormat("Arrange bag errror,user id:{0}   msg:{1}", player.PlayerId, ex.Message);
				}
				finally
				{
					if (!result)
					{
						bag.Clear(0, bag.Capalility - 1);
						foreach (KeyValuePair<int, ItemInfo> sp2 in rawitems)
						{
							bag.AddItemTo(sp2.Value, sp2.Key);
						}
					}
					bag.CommitChanges();
				}
				result2 = 0;
			}
			return result2;
		}
	}
}
