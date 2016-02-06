using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
namespace Game.Logic.Spells
{
	public interface ISpellHandler
	{
		void Execute(BaseGame game, Player player, ItemTemplateInfo item);
		void Execute(BaseGame game, Player player, ItemInfo item);
	}
}
