using System;
using System.Collections;

namespace NVelocity.Runtime.Directive
{
	public interface IDirectiveManager
	{
		void Register(string directiveTypeName);

		Directive Create(string name, Stack directiveStack);

		bool Contains(string name);
	}
}
