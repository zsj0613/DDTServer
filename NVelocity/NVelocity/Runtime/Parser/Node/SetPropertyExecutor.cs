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
    using Exception;
    using Log;
    using Util.Introspection;

    /// <summary> Executor for looking up property names in the passed in class
    /// This will try to find a set&lt;foo&gt;(key, value) method
    /// 
    /// </summary>
    /// <author>  <a href="mailto:henning@apache.org">Henning P. Schmiedehausen</a>
    /// </author>
    /// <version>  $Id: SetPropertyExecutor.java 687177 2008-08-19 22:00:32Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class SetPropertyExecutor : SetExecutor
    {
        /// <returns> The current introspector.
        /// </returns>
        virtual protected internal Introspector Introspector
        {
            get
            {
                return this.introspector;
            }

        }
       
        private readonly Introspector introspector;

        /// <param name="Log">
        /// </param>
        /// <param name="introspector">
        /// </param>
        /// <param name="clazz">
        /// </param>
        /// <param name="property">
        /// </param>
        /// <param name="arg">
        /// </param>
        public SetPropertyExecutor(Log log, Introspector introspector, System.Type clazz, string property, object arg)
        {
            this.log = log;
            this.introspector = introspector;

            // Don't allow passing in the empty string or null because
            // it will either fail with a StringIndexOutOfBounds Error
            // or the introspector will Get confused.
            if (!string.IsNullOrEmpty(property))
            {
                Discover(clazz, property, arg);
            }
        }

        /// <param name="clazz">
        /// </param>
        /// <param name="property">
        /// </param>
        /// <param name="arg">
        /// </param>
        protected internal virtual void Discover(System.Type clazz, string property, object arg)
        {
            object[] parameters = new object[] { arg };

            try
            {
                Method = (introspector.GetMethod(clazz, "set_" + property, parameters));
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
                string msg = "Exception while looking for property setter for '" + property;
                log.Error(msg, e);
                throw new VelocityException(msg, e);
            }
        }

        /// <summary> Execute method against context.</summary>
        /// <param name="instance">
        /// </param>
        /// <param name="value">
        /// </param>
        /// <returns> The value of the invocation.
        /// </returns>
        /// <throws>  IllegalAccessException </throws>
        /// <throws>  InvocationTargetException </throws>
        public override object Execute(object o, object value)
        {
            object[] params_Renamed = new object[] { value };
            return Alive ? Method.Invoker.Invoke(o, (object[])params_Renamed) : null;
        }
    }
}
