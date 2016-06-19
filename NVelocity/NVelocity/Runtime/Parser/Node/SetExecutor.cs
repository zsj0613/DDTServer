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
    /// for the PutExecutor and SetPropertyExecutor.
    /// 
    /// There really should be a superclass for this and AbstractExecutor (which should
    /// be refactored to GetExecutor) because they differ only in the Execute() method.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  <a href="mailto:henning@apache.org">Henning P. Schmiedehausen</a>
    /// </author>
    /// <version>  $Id: SetExecutor.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public abstract class SetExecutor
    {
        /// <summary> Tell whether the executor is alive by looking
        /// at the value of the method.
        /// </summary>
        /// <returns> True if the executor is alive.
        /// </returns>
        virtual public bool Alive
        {
            get
            {
                return (method != null);
            }

        }
        /// <summary>Class logger </summary>
        protected internal Log log = null;

        /// <summary> Method to be executed.</summary>
        private MethodEntry method;

        /// <summary> Execute method against context.</summary>
        /// <param name="instance">
        /// </param>
        /// <param name="value">
        /// </param>
        /// <returns> The result of the invocation.
        /// </returns>
        /// <throws>  IllegalAccessException </throws>
        /// <throws>  InvocationTargetException </throws>
        public abstract object Execute(object o, object value);

        /// <returns> The method to invoke.
        /// </returns>
        ///  /// <param name="method">
        /// </param>
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
    }
}
