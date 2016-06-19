using System;

namespace NVelocity.Runtime.Directive
{
	public class ForeachNoDataSection : AbstractForeachSection
	{
		public override ForeachSectionEnum Section
		{
			get
			{
				return ForeachSectionEnum.NoData;
			}
		}
	}
}
