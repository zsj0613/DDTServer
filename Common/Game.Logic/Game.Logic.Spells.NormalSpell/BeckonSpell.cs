using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
namespace Game.Logic.Spells.NormalSpell
{
	[SpellAttibute(6)]
	public class BeckonSpell : ISpellHandler
	{
		public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
		{
		}
		public void Execute(BaseGame game, Player player, ItemInfo item)
		{
		}
	}
}
