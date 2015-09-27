using System;
namespace Game.Logic.Phy.Object
{
	public class Layer : PhysicalObj
	{
		private int m_type;
		public override int Type
		{
			get
			{
				return this.m_type;
			}
		}
		public Layer(int id, string name, string model, string defaultAction, int scaleX, int scaleY, int rotation) : base(id, name, model, defaultAction, scaleX, scaleY, rotation)
		{
			this.m_type = 2;
		}
		public Layer(int id, string name, string model, string defaultAction, int scaleX, int scaleY, int rotation, int type) : base(id, name, model, defaultAction, scaleX, scaleY, rotation)
		{
			this.m_type = type;
		}
	}
}
