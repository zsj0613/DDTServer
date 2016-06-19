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

namespace NVelocity.Runtime.Parser.Node
{
    using System.Reflection;

    using Log;
    using Util.Introspection;

    /// <summary> Abstract class that is used to Execute an arbitrary
    /// method that is in introspected. This is the superclass
    /// for the GetExecutor and PropertyExecutor.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: AbstractExecutor.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    public abstract class AbstractExecutor
    {
        private MethodEntry method;
        private PropertyEntry property;

        /// <summary> Tell whether the executor is alive by looking
        /// at the value of the method.
        /// 
        /// </summary>
        /// <returns> True if executor is alive.
        /// </returns>
        public virtual bool Alive
        {
            get
            {
                return (property != null);
            }

        }

        protected internal Log log = null;



        /// <summary> Execute method against context.</summary>
        /// <param name="instance">
        /// </param>
        /// <returns> The resulting object.
        /// </returns>
        /// <throws>  IllegalAccessException </throws>
        /// <throws>  InvocationTargetException </throws>
        public abstract object Execute(object o);

        /// <summary> Method to be executed.</summary>
        /// <param name="method">
        /// </param>
        /// <since> 1.5
        /// </since>
        /// <returns> The current method.
        /// </returns>
        public virtual MethodEntry Method
        {
            get
            {
                return method;
            }
            protected set
            {
                method = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual PropertyEntry Property
        {
            get
            {
                return property;
            }

            protected set
            {
                property = value;
            }
        }
    }
}