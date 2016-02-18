using System;
using System.Collections;
using System.IO;

namespace NVelocityTemplateEngine.Interfaces
{
	public interface INVelocityEngine
	{
		string Process(IDictionary context, string template);

		void Process(IDictionary context, TextWriter writer, string template);
	}
}
