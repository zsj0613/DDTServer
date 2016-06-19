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

namespace NVelocity.Context
{
    /// <summary>  interface for internal context wrapping functionality
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: InternalWrapperContext.java 471908 2006-11-06 22:39:28Z henning $
    /// </version>
    public interface IInternalWrapperContext
    {
        /// <summary> Returns the wrapped user context.</summary>
        /// <returns> The wrapped user context.
        /// </returns>
        IContext InternalUserContext
        {
            get;

        }
        /// <summary> Returns the base full context impl.</summary>
        /// <returns> The base full context impl.
        /// 
        /// </returns>
        IInternalContextAdapter BaseContext
        {
            get;

        }

        /// <summary> Allows callers to explicitly Put objects in the local context.
        /// Objects added to the context through this method always end up
        /// in the top-level context of possible wrapped contexts.
        /// 
        /// </summary>
        /// <param name="key">name of item to set.
        /// </param>
        /// <param name="value">object to set to key.
        /// </param>
        /// <returns> old stored object
        /// </returns>
        object LocalPut(string key, object value);
    }
}