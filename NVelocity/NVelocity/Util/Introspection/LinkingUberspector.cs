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
    /// <summary> <p>
    /// When the runtime.introspection.uberspect configuration property contains several
    /// uberspector class names, it means those uberspectors will be chained. When an
    /// uberspector in the list other than the leftmost does not implement ChainableUberspector,
    /// then this utility class is used to provide a basic default chaining where the
    /// first non-null result is kept for each introspection call.
    /// </p>
    /// 
    /// </summary>
    /// <since> 1.6
    /// </since>
    /// <seealso cref="ChainableUberspector">
    /// </seealso>
    /// <version>  $Id: LinkingUberspector.java 10959 2008-07-01 00:12:29Z sdumitriu $
    /// </version>
    public class LinkingUberspector : AbstractChainableUberspector
    {
        private IUberspect leftUberspect;
        private IUberspect rightUberspect;

        /// <summary> Constructor that takes the two uberspectors to link</summary>
        public LinkingUberspector(IUberspect left, IUberspect right)
        {
            leftUberspect = left;
            rightUberspect = right;
        }

        /// <summary> {@inheritDoc}
        /// <p>
        /// Init both wrapped uberspectors
        /// </p>
        /// 
        /// </summary>
        /// <seealso cref="org.apache.velocity.util.introspection.Uberspect.Init()">
        /// </seealso>
        //@Override
        public override void Init()
        {
            leftUberspect.Init();
            rightUberspect.Init();
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
            System.Collections.IEnumerator it = leftUberspect.GetIterator(obj, i);
            return it != null ? it : rightUberspect.GetIterator(obj, i);
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
            IVelMethod method = leftUberspect.GetMethod(obj, methodName, args, i);
            return method != null ? method : rightUberspect.GetMethod(obj, methodName, args, i);
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
            IVelPropertyGet getter = leftUberspect.GetPropertyGet(obj, identifier, i);
            return getter != null ? getter : rightUberspect.GetPropertyGet(obj, identifier, i);
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
            IVelPropertySet setter = leftUberspect.GetPropertySet(obj, identifier, arg, i);
            return setter != null ? setter : rightUberspect.GetPropertySet(obj, identifier, arg, i);
        }
    }
}
