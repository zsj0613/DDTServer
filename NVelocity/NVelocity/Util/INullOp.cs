using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NVelocity.Util
{
    interface INullOp<T>
    {
        bool HasValue(T value);
        bool AddIfNotNull(ref T accumulator, T value);
    }
}
