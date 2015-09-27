using System;
namespace Game.Logic.Phy.Object
{
	public delegate void KillLivingEventHanlde(Living living, Living target, int damageAmount, int criticalAmount, int delay);
}
