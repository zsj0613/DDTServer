using System;

namespace NVelocity.Runtime.Directive
{
	public class ForeachAfterSection : AbstractForeachSection
	{
		public override ForeachSectionEnum Section
		{
			get
			{
				return ForeachSectionEnum.After;
			}
		}
	}
}
