
namespace NVelocity.Util.Introspection
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// 
    /// </summary>
    public interface IPropertyAccessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        object GetValue(object instance);
    }

    /// <summary>
    /// 
    /// </summary>
    public class PropertyAccessor : IPropertyAccessor
    {
        private Func<object, object> m_getter;

        /// <summary>
        /// 
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            this.PropertyInfo = propertyInfo;
            this.InitializeGet(propertyInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        private void InitializeGet(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead) return;

            // Target: (object)(((TInstance)instance).Property)

            // preparing parameter, object type
            var instance = Expression.Parameter(typeof(object), "instance");

            // non-instance for static method, or ((TInstance)instance)
            var instanceCast = propertyInfo.GetGetMethod(true).IsStatic ? null :
                Expression.Convert(instance, propertyInfo.ReflectedType);

            // ((TInstance)instance).Property
            var propertyAccess = Expression.Property(instanceCast, propertyInfo);

            // (object)(((TInstance)instance).Property)
            var castPropertyValue = Expression.Convert(propertyAccess, typeof(object));

            // Lambda expression
            var lambda = Expression.Lambda<Func<object, object>>(castPropertyValue, instance);

            this.m_getter = lambda.Compile();
        }

     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public object GetValue(object o)
        {
            if (this.m_getter == null)
            {
                throw new NotSupportedException("Get method is not defined for this property.");
            }

            return this.m_getter(o);
        }


        #region IPropertyAccessor Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        object IPropertyAccessor.GetValue(object instance)
        {
            return this.GetValue(instance);
        }

        #endregion
    }
}
