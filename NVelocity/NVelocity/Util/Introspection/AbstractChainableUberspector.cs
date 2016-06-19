/*
* Licensed to the Apache Software Foundation (ASF) under one
* or more contributor license agreements.  See the NOTICE file
* distributed with this work for additional information
* regarding copyright ownership.  The ASF licenses this file
* to you under the Apache License, Version 2.0 (the
* "License"); you may not use this file except in compliance
* with the License.  You may obtain a copy of the License at
*
*   http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing,
* software distributed under the License is distributed on an
* "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
* KIND, either express or implied.  See the License for the
* specific language governing permissions and limitations
* under the License.    
*/
namespace NVelocity.Util.Introspection
{
    /// <summary> Default implementation of a {@link ChainableUberspector chainable uberspector} that forwards all calls to the wrapped
    /// uberspector (when that is possible). It should be used as the base class for all chainable uberspectors.
    /// 
    /// </summary>
    /// <version>  $Id: $
    /// </version>
    /// <since> 1.6
    /// </since>
    /// <seealso cref="ChainableUberspector">
    /// </seealso>
    public abstract class AbstractChainableUberspector : UberspectImpl, IChainableUberspector
    {
        /// <summary>The wrapped (decorated) uberspector. </summary>
        protected internal IUberspect inner;

        /// <summary> {@inheritDoc}
        /// 
        /// </summary>
        /// <seealso cref="ChainableUberspector.wrap(org.apache.velocity.util.introspection.Uberspect)">
        /// </seealso>
        /// <seealso cref="inner">
        /// </seealso>
        public virtual void Wrap(IUberspect inner)
        {
            this.inner = inner;
        }

        /// <summary> Init - the chainable uberspector is responsible for the initialization of the wrapped uberspector
        /// 
        /// </summary>
        /// <seealso cref="org.apache.velocity.util.introspection.Uberspect.Init()">
        /// </seealso>
        //@Override
        public override void Init()
        {
            if (this.inner != null)
            {
                this.inner.Init();
            }
        }

        /// <summary> {@inheritDoc}
        /// 
        /// </summary>
        /// <seealso cref="org.apache.velocity.util.introspection.Uberspect.getIterator(java.lang.Object,">
        /// org.apache.velocity.util.introspection.Info)
        /// </seealso>
        //@SuppressWarnings("unchecked")
        //@Override
        public override System.Collections.IEnumerator GetIterator(object obj, Info i)
        {
            return (this.inner != null) ? this.inner.GetIterator(obj, i) : null;
        }

        /// <summary> {@inheritDoc}
        /// 
        /// </summary>
        /// <seealso cref="org.apache.velocity.util.introspection.Uberspect.getMethod(java.lang.Object, java.lang.String,">
        /// java.lang.Object[], org.apache.velocity.util.introspection.Info)
        /// </seealso>
        //@Override
        public override IVelMethod GetMethod(object obj, string methodName, object[] args, Info i)
        {
            return (this.inner != null) ? this.inner.GetMethod(obj, methodName, args, i) : null;
        }

        /// <summary> {@inheritDoc}
        /// 
        /// </summary>
        /// <seealso cref="org.apache.velocity.util.introspection.Uberspect.getPropertyGet(java.lang.Object, java.lang.String,">
        /// org.apache.velocity.util.introspection.Info)
        /// </seealso>
        //@Override
        public override IVelPropertyGet GetPropertyGet(object obj, string identifier, Info i)
        {
            return (this.inner != null) ? this.inner.GetPropertyGet(obj, identifier, i) : null;
        }

        /// <summary> {@inheritDoc}
        /// 
        /// </summary>
        /// <seealso cref="org.apache.velocity.util.introspection.Uberspect.getPropertySet(java.lang.Object, java.lang.String,">
        /// java.lang.Object, org.apache.velocity.util.introspection.Info)
        /// </seealso>
        //@Override
        public override IVelPropertySet GetPropertySet(object obj, string identifier, object arg, Info i)
        {
            return (this.inner != null) ? this.inner.GetPropertySet(obj, identifier, arg, i) : null;
        }
    }
}
