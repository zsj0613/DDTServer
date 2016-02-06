using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
namespace Game.Logic.Spells.FightingSpell
{
	[SpellAttibute(32)]
	public class AddBloodGunSpell : ISpellHandler
	{
		public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
		{
		}
		public void Execute(BaseGame game, Player player, ItemInfo item)
		{
			player.SetCurrentWeapon(item.Template);
		}
	}
}
