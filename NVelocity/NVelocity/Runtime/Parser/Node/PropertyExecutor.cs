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

    /// <summary> Returned the value of object property when executed.</summary>
    public class PropertyExecutor : AbstractExecutor
    {
        /// <returns> The current introspector.
        /// </returns>
        /// <since> 1.5
        /// </since>
        virtual protected internal Introspector Introspector
        {
            get
            {
                return this.introspector;
            }

        }

        private Introspector introspector;

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
        public PropertyExecutor(Log log, Introspector introspector, System.Type clazz, string property)
        {
            this.log = log;
            this.introspector = introspector;

            // Don't allow passing in the empty string or null because
            // it will either fail with a StringIndexOutOfBounds Error
            // or the introspector will Get confused.
            if (!string.IsNullOrEmpty(property))
            {
                Discover(clazz, property);
            }
        }


        /// <param name="clazz">
        /// </param>
        /// <param name="property">
        /// </param>
        protected internal virtual void Discover(System.Type clazz, string property)
        {
            /*
            *  this is gross and linear, but it keeps it straightforward.
            */

            try
            {
                Property = introspector.GetProperty(clazz, property);
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
                string msg = "Exception while looking for property getter for '" + property;
                log.Error(msg, e);
                throw new VelocityException(msg, e);
            }
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.AbstractExecutor.Execute(java.lang.Object)">
        /// </seealso>
        public override object Execute(object o)
        {
            return Alive ? Property.Accessor.GetValue(o) : null;
        }
    }
}