namespace NVelocity.Util.Introspection
{
    using System.Reflection;

    /// <summary>
    /// 
    /// </summary>
    public sealed class MethodEntry
    {
        /// <summary>
        /// 
        /// </summary>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IMethodInvoker Invoker { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        public MethodEntry(MethodInfo method)
        {
            Method = method;
            Invoker = new MethodInvoker(method);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="invoker"></param>
        public MethodEntry(MethodInfo method, IMethodInvoker invoker)
        {
            Method = method;
            Invoker = invoker;
        }
    }
}
