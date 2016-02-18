using System;

namespace NVelocity.Runtime.Directive
{
	public class ForeachBeforeSection : AbstractForeachSection
	{
		public override ForeachSectionEnum Section
		{
			get
			{
				return ForeachSectionEnum.Before;
			}
		}
	}
}
