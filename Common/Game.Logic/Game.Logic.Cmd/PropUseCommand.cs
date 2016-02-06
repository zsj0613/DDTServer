using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
namespace Game.Logic.Cmd
{
	[GameCommand(32, "使用道具")]
	public class PropUseCommand : ICommandHandler
	{
		public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
		{
			if (game.GameState == eGameState.Playing && !player.GetSealState())
			{
				int type = (int)packet.ReadByte();
				int place = packet.ReadInt();
				int templateID = packet.ReadInt();
				ItemTemplateInfo template = ItemMgr.FindItemTemplate(templateID);
				if (player.CanUseItem(template))
				{
					if (player.PlayerDetail.UsePropItem(game, type, place, templateID, player.IsLiving))
					{
						if (!player.UseItem(template))
						{
							BaseGame.log.Error("Using prop error");
						}
					}
				}
			}
		}
	}
}
