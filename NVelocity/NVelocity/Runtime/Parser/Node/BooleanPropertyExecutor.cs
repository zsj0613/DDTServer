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

    /// <summary>  Handles discovery and valuation of a
    /// boolean object property, of the
    /// form public boolean is<property> when executed.
    /// 
    /// We do this separately as to preserve the current
    /// quasi-broken semantics of Get<as is property>
    /// Get< flip 1st char> Get("property") and now followed
    /// by is<Property>
    /// 
    /// </summary>
    /// <author>  <a href="geirm@apache.org">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: BooleanPropertyExecutor.java 687502 2008-08-20 23:19:52Z nbubna $
    /// </version>
    public class BooleanPropertyExecutor : PropertyExecutor
    {
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
        public BooleanPropertyExecutor(Log log, Introspector introspector, System.Type clazz, string property)
            : base(log, introspector, clazz, property)
        {
        }

        protected internal override void Discover(System.Type clazz, string property)
        {
            try
            {
                Property = Introspector.GetProperty(clazz,property);

                if (Property != null && Property.Property.PropertyType.Equals(typeof(Boolean)))
                {
                    return;
                }

                Property = null;
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
                string msg = "Exception while looking for boolean property getter for '" + property;
                log.Error(msg, e);
                throw new VelocityException(msg, e);
            }
        }
    }
}