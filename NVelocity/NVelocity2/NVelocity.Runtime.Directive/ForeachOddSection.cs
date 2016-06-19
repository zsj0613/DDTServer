using System;

namespace NVelocity.Runtime.Directive
{
	public class ForeachOddSection : AbstractForeachSection
	{
		public override ForeachSectionEnum Section
		{
			get
			{
				return ForeachSectionEnum.Odd;
			}
		}
	}
}
