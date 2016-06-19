using System;

namespace NVelocity.Runtime.Directive
{
	public class ForeachBeforeAllSection : AbstractForeachSection
	{
		public override ForeachSectionEnum Section
		{
			get
			{
				return ForeachSectionEnum.BeforeAll;
			}
		}
	}
}
