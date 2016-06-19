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

namespace NVelocity.Util
{
    using Context;

    /// <summary> Event handlers implementing this interface will automatically
    /// have the method setContext called before each event.  This
    /// allows the event handler to use information in the latest context
    /// when responding to the event.
    /// 
    /// <P>Important Note: Only local event handlers attached to the context
    /// (as opposed to global event handlers initialized in the velocity.properties
    /// file) should implement ContextAware.  Since global event handlers are
    /// singletons individual requests will not be able to count on the
    /// correct context being loaded before a request.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <version>  $Id: ContextAware.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public interface IContextAware
    {
        /// <summary> Initialize the EventHandler.</summary>
        /// <param name="context">
        /// </param>
        IContext Context
        {
            set;

        }
    }
}
