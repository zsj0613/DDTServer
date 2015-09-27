using Game.Logic.Phy.Object;
using System;
namespace Game.Logic.Effects
{
	public class BasePlayerEffect : AbstractEffect
	{
		public BasePlayerEffect(eEffectType type) : base(type)
		{
		}
		public override bool Start(Living living)
		{
			return living is Player && base.Start(living);
		}
		public sealed override void OnAttached(Living living)
		{
			if (living is Player)
			{
				this.OnAttachedToPlayer(living as Player);
			}
		}
		public sealed override void OnRemoved(Living living)
		{
			if (living is Player)
			{
				this.OnRemovedFromPlayer(living as Player);
			}
		}
		protected virtual void OnAttachedToPlayer(Player player)
		{
		}
		protected virtual void OnRemovedFromPlayer(Player player)
		{
		}
	}
}
