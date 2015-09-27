using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
namespace Game.Logic.Spells.FightingSpell
{
	[SpellAttibute(31)]
	public class ShieldSpell : ISpellHandler
	{
		public void Execute(BaseGame game, Player player, ItemInfo item)
		{
			new ContinueReduceDamageEffect((int)((double)item.Template.Property7 * Math.Pow(1.1, (double)player.PlayerDetail.SecondWeapon.StrengthenLevel)), item.Template.Property5).Start(player);
		}
		public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
		{
		}
	}
}
