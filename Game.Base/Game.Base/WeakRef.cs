using System;
namespace Game.Base
{
	public class WeakRef : WeakReference
	{
		private class NullValue
		{
		}
		private static readonly WeakRef.NullValue NULL = new WeakRef.NullValue();
		public override object Target
		{
			get
			{
				object o = base.Target;
				return (o == WeakRef.NULL) ? null : o;
			}
			set
			{
				base.Target = ((value == null) ? WeakRef.NULL : value);
			}
		}
		public WeakRef(object target) : base((target == null) ? WeakRef.NULL : target)
		{
		}
		public WeakRef(object target, bool trackResurrection) : base((target == null) ? WeakRef.NULL : target, trackResurrection)
		{
		}
	}
}
