using System;

namespace NVelocity.Runtime.Directive
{
	public class ForeachAfterAllSection : AbstractForeachSection
	{
		public override ForeachSectionEnum Section
		{
			get
			{
				return ForeachSectionEnum.AfterAll;
			}
		}
	}
}
