namespace NVelocity.Util.Introspection
{
    using System.Reflection;

    /// <summary>
    /// 
    /// </summary>
    public sealed class PropertyEntry
    {
        /// <summary>
        /// 
        /// </summary>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IPropertyAccessor Accessor { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public PropertyEntry(PropertyInfo property)
        {
            Property = property;
            Accessor = new PropertyAccessor(property);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="accessor"></param>
        public PropertyEntry(PropertyInfo property, IPropertyAccessor accessor)
        {
            Property = property;
            Accessor = accessor;
        }
    }
}
