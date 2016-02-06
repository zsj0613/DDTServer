using Game.Logic.Phy.Object;
using Lsj.Util.Logs;
using System;
using System.Collections;
using System.Reflection;
using System.Threading;
namespace Game.Logic.Effects
{
	public class EffectList
	{
		private static LogProvider log => LogProvider.Default;
		protected ArrayList m_effects;
		protected readonly Living m_owner;
		protected volatile sbyte m_changesCount;
		protected int m_immunity;
		public ArrayList List
		{
			get
			{
				return this.m_effects;
			}
		}
		public EffectList(Living owner, int immunity)
		{
			this.m_owner = owner;
			this.m_effects = new ArrayList(3);
			this.m_immunity = immunity;
		}
		public bool CanAddEffect(int id)
		{
			return this.m_owner.IsLiving && (id > 35 || id < 0 || (1 << id - 1 & this.m_immunity) == 0);
		}
		public virtual bool Add(AbstractEffect effect)
		{
			bool result;
			if (this.CanAddEffect(effect.TypeValue))
			{
				ArrayList effects;
				Monitor.Enter(effects = this.m_effects);
				try
				{
					this.m_effects.Add(effect);
				}
				finally
				{
					Monitor.Exit(effects);
				}
				effect.OnAttached(this.m_owner);
				this.OnEffectsChanged(effect);
				result = true;
			}
			else
			{
				if (effect.TypeValue == 9 && this.m_owner is SimpleBoss)
				{
					this.m_owner.State = 0;
				}
				result = false;
			}
			return result;
		}
		public virtual bool Remove(AbstractEffect effect)
		{
			int index = -1;
			ArrayList effects;
			Monitor.Enter(effects = this.m_effects);
			bool result;
			try
			{
				index = this.m_effects.IndexOf(effect);
				if (index < 0)
				{
					result = false;
					return result;
				}
				this.m_effects.RemoveAt(index);
			}
			finally
			{
				Monitor.Exit(effects);
			}
			if (index != -1)
			{
				effect.OnRemoved(this.m_owner);
				this.OnEffectsChanged(effect);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public virtual void OnEffectsChanged(AbstractEffect changedEffect)
		{
			if (this.m_changesCount <= 0)
			{
				this.UpdateChangedEffects();
			}
		}
		public void BeginChanges()
		{
			this.m_changesCount += 1;
		}
		public virtual void CommitChanges()
		{
			if ((this.m_changesCount -= 1) < 0)
			{
				
					EffectList.log.Warn("changes count is less than zero, forgot BeginChanges()?\n" + Environment.StackTrace);
				
				this.m_changesCount = 0;
			}
			bool update = this.m_changesCount == 0;
			if (update)
			{
				this.UpdateChangedEffects();
			}
		}
		protected virtual void UpdateChangedEffects()
		{
		}
		public virtual AbstractEffect GetOfType(eEffectType effectType)
		{
			ArrayList effects;
			Monitor.Enter(effects = this.m_effects);
			AbstractEffect result;
			try
			{
				foreach (AbstractEffect effect in this.m_effects)
				{
					if (effect.Type == effectType)
					{
						result = effect;
						return result;
					}
				}
			}
			finally
			{
				Monitor.Exit(effects);
			}
			result = null;
			return result;
		}
		public virtual IList GetAllOfType(Type effectType)
		{
			ArrayList list = new ArrayList();
			ArrayList effects;
			Monitor.Enter(effects = this.m_effects);
			try
			{
				foreach (AbstractEffect effect in this.m_effects)
				{
					if (effect.GetType().Equals(effectType))
					{
						list.Add(effect);
					}
				}
			}
			finally
			{
				Monitor.Exit(effects);
			}
			return list;
		}
		public void StopEffect(Type effectType)
		{
			IList fx = this.GetAllOfType(effectType);
			this.BeginChanges();
			foreach (AbstractEffect effect in fx)
			{
				effect.Stop();
			}
			this.CommitChanges();
		}
		public void StopAllEffect()
		{
			if (this.m_effects.Count > 0)
			{
				if (!this.m_owner.SyncAtTime)
				{
					this.m_owner.SyncAtTime = true;
				}
				AbstractEffect[] temp_effects = new AbstractEffect[this.m_effects.Count];
				this.m_effects.CopyTo(temp_effects);
				AbstractEffect[] array = temp_effects;
				for (int i = 0; i < array.Length; i++)
				{
					AbstractEffect effect = array[i];
					effect.Stop();
				}
				this.m_effects.Clear();
			}
		}
	}
}
