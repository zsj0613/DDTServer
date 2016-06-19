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

    /// <summary> Executor that simply tries to Execute a Put(key, value)
    /// operation. This will try to find a Put(key) method
    /// for any type of object, not just objects that
    /// implement the Map interface as was previously
    /// the case.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:henning@apache.org">Henning P. Schmiedehausen</a>
    /// </author>
    /// <version>  $Id: PutExecutor.java 687177 2008-08-19 22:00:32Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class PutExecutor : SetExecutor
    {
        private readonly Introspector introspector;
     
        private string property;

        /// <param name="Log">
        /// </param>
        /// <param name="introspector">
        /// </param>
        /// <param name="clazz">
        /// </param>
        /// <param name="arg">
        /// </param>
        /// <param name="property">
        /// </param>
        public PutExecutor(Log log, Introspector introspector, System.Type clazz, object arg, string property)
        {
            this.log = log;
            this.introspector = introspector;
            this.property = property;

            Discover(clazz, arg);
        }

        /// <param name="clazz">
        /// </param>
        /// <param name="arg">
        /// </param>
        protected internal virtual void Discover(System.Type clazz, object arg)
        {
            object[] params_Renamed;

            // If you passed in null as property, we don't use the value
            // for parameter lookup. Instead we just look for Put(Object) without
            // any parameters.
            //
            // In any other case, the following condition will set up an array
            // for looking up Put(String, Object) on the class.

            if (property == null)
            {
                // The passed in arg object is used by the Cache to look up the method.
                params_Renamed = new object[] { arg };
            }
            else
            {
                params_Renamed = new object[] { property, arg };
            }

            try
            {
                Method = introspector.GetMethod(clazz, "Put", params_Renamed);
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
                
                string msg = "Exception while looking for Put('" + params_Renamed[0] + "') method";
                log.Error(msg, e);
                throw new VelocityException(msg, e);
            }
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SetExecutor.Execute(java.lang.Object, java.lang.Object)">
        /// </seealso>
        public override object Execute(object o, object value)
        {
            object[] params_Renamed;

            if (Alive)
            {
                // If property != null, pass in the name for Put(key, value). Else just Put(value).
                if (property == null)
                {
                    params_Renamed = new object[] { value };
                }
                else
                {
                    params_Renamed = new object[] { property, value };
                }

                return Method.Invoker.Invoke(o, (object[])params_Renamed);
            }

            return null;
        }
    }
}
