
namespace NVelocity.Util
{
    sealed class ClassNullOp<T>
         : INullOp<T>
         where T : class
    {
        public bool HasValue(T value)
        {
            return value != null;
        }
        public bool AddIfNotNull(ref T accumulator, T value)
        {
            if (value != null)
            {
                accumulator = accumulator == null ?
                    value : Operator<T>.Add(accumulator, value);
                return true;
            }
            return false;
        }
    }
}
