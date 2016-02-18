using System;

namespace NVelocity.Runtime.Directive
{
	public class ForeachEachSection : AbstractForeachSection
	{
		public override ForeachSectionEnum Section
		{
			get
			{
				return ForeachSectionEnum.Each;
			}
		}
	}
}
