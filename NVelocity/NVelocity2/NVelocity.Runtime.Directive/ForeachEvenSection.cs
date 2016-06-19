using System;

namespace NVelocity.Runtime.Directive
{
	public class ForeachEvenSection : AbstractForeachSection
	{
		public override ForeachSectionEnum Section
		{
			get
			{
				return ForeachSectionEnum.Even;
			}
		}
	}
}
