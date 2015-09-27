using System;
namespace Game.Base.Events
{
	public abstract class RoadEvent
	{
		protected string m_EventName;
		public string Name
		{
			get
			{
				return this.m_EventName;
			}
		}
		public RoadEvent(string name)
		{
			this.m_EventName = name;
		}
		public override string ToString()
		{
			return "DOLEvent(" + this.m_EventName + ")";
		}
		public virtual bool IsValidFor(object o)
		{
			return true;
		}
	}
}
