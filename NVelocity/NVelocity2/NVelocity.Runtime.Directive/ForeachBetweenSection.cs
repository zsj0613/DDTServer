using System;

namespace NVelocity.Runtime.Directive
{
	public class ForeachBetweenSection : AbstractForeachSection
	{
		public override ForeachSectionEnum Section
		{
			get
			{
				return ForeachSectionEnum.Between;
			}
		}
	}
}
