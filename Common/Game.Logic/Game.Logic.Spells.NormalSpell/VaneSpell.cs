using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
namespace Game.Logic.Spells.NormalSpell
{
	[SpellAttibute(7)]
	public class VaneSpell : ISpellHandler
	{
		public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
		{
			game.UpdateWind(-game.Wind, true);
		}
		public void Execute(BaseGame game, Player player, ItemInfo item)
		{
		}
	}
}
