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
    using System.Collections;

    using Exception;
    using Log;
    using Util.Introspection;

    /// <summary> GetExecutor that is smart about Maps. If it detects one, it does not
    /// use Reflection but a cast to access the getter. 
    /// 
    /// </summary>
    /// <author>  <a href="mailto:henning@apache.org">Henning P. Schmiedehausen</a>
    /// </author>
    /// <version>  $Id: MapGetExecutor.java 687177 2008-08-19 22:00:32Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class MapGetExecutor : AbstractExecutor
    {
        private readonly string property;

        public MapGetExecutor(Log log, System.Type clazz, string property)
        {
            this.log = log;
            this.property = property;
            Discover(clazz);
        }

        protected internal virtual void Discover(System.Type clazz)
        {
            if (!string.IsNullOrEmpty(property))
            {
                IDictionary interfaces = clazz as IDictionary;

                if (interfaces != null)
                {
                    try
                    {
                        Property = new PropertyEntry(typeof(System.Collections.IDictionary).GetProperty("Item"), null);
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
                        string msg = "Exception while looking for get('" + property + "') method";
                        log.Error(msg, e);
                        throw new VelocityException(msg, e);
                    }
                }
            }
        }

        public override object Execute(object o)
        {
            return ((System.Collections.IDictionary)o)[property];
        }
    }
}
