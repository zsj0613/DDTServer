using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
namespace Game.Logic.Spells.FightingSpell
{
	[SpellAttibute(12)]
	public class ShootStraightSpell : ISpellHandler
	{
		public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
		{
			player.ControlBall = true;
			player.CurrentShootMinus *= 0.5f;
		}
		public void Execute(BaseGame game, Player player, ItemInfo item)
		{
		}
	}
}
