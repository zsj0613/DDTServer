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
    using System;

    using Exception;
    using Log;
    using Util.Introspection;

    /// <summary> Executor that simply tries to Execute a Get(key)
    /// operation. This will try to find a Get(key) method
    /// for any type of object, not just objects that
    /// implement the Map interface as was previously
    /// the case.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <version>  $Id: GetExecutor.java 687177 2008-08-19 22:00:32Z nbubna $
    /// </version>
    public class GetExecutor : AbstractExecutor
    {
        private Introspector introspector;

        // This is still threadsafe because this object is only read except in the C'tor.
        private object[] parameters = new object[1];

        /// <param name="Log">
        /// </param>
        /// <param name="introspector">
        /// </param>
        /// <param name="clazz">
        /// </param>
        /// <param name="property">
        /// </param>
        /// <since> 1.5
        /// </since>
        public GetExecutor(Log log, Introspector introspector, Type clazz, string property)
        {
            this.log = log;
            this.introspector = introspector;

            // If you passed in null as property, we don't use the value
            // for parameter lookup. Instead we just look for Get() without
            // any parameters.
            //
            // In any other case, the following condition will set up an array
            // for looking up Get(String) on the class.

            if (!string.IsNullOrEmpty(property))
            {
                this.parameters = new object[] { property };
            }
            Discover(clazz);
        }

        /// <since> 1.5
        /// </since>
        protected internal virtual void Discover(System.Type clazz)
        {
            try
            {
                Method = introspector.GetMethod(clazz, "get_Item", parameters);

                if (Method == null)
                {
                    Method = introspector.GetMethod(clazz, "Get", parameters);
                }
            }
            /**
            * pass through application level runtime exceptions
            */
            catch (System.SystemException e)
            {
                throw e;
            }
            catch (System.Exception e)
            {

                string msg = "Exception while looking for Get('" + parameters[0] + "') method";
                log.Error(msg, e);
                throw new VelocityException(msg, e);
            }
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.AbstractExecutor.Execute(java.lang.Object)">
        /// </seealso>
        public override object Execute(object o)
        {
            return Alive ? Method.Invoker.Invoke(o, (object[])parameters) : null;
        }
    }
}