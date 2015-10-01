using System;

namespace NVelocity.Runtime
{
	public interface IRuntimeLogger
	{
		void Warn(object message);

		void Info(object message);

		void Error(object message);

		void Debug(object message);
	}
}
