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

namespace NVelocity.App.Event
{
    using NVelocity.Context;
    using NVelocity.Util;

    /// <summary>  Event handler for include type directives (e.g. <code>#include()</code>, <code>#parse()</code>)
    /// Allows the developer to modify the path of the resource returned.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <version>  $Id: IIncludeEventHandler.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    /// <summary> Defines the execution strategy for IncludeEvent</summary>
    class IncludeEventExecutor : IEventHandlerMethodExecutor
    {
        virtual public object ReturnValue
        {
            get
            {
                return includeResourcePath;
            }

        }
        virtual public bool Done
        {
            get
            {
                return executed && (includeResourcePath == null);
            }

        }
        private IContext context;
        private string includeResourcePath;
        private string currentResourcePath;
        private string directiveName;

        private bool executed = false;

        internal IncludeEventExecutor(IContext context, string includeResourcePath, string currentResourcePath, string directiveName)
        {
            this.context = context;
            this.includeResourcePath = includeResourcePath;
            this.currentResourcePath = currentResourcePath;
            this.directiveName = directiveName;
        }

        /// <summary> Call the method IncludeEvent()
        /// 
        /// </summary>
        /// <param name="handler">call the appropriate method on this handler
        /// </param>
        public virtual void Execute(IEventHandler handler)
        {
            IIncludeEventHandler eh = (IIncludeEventHandler)handler;

            if (eh is IContextAware)
                ((IContextAware)eh).Context = context;

            executed = true;
            includeResourcePath = ((IIncludeEventHandler)handler).IncludeEvent(includeResourcePath, currentResourcePath, directiveName);
        }
    }
    public interface IIncludeEventHandler : IEventHandler
    {
        /// <summary> Called when an include-type directive is encountered (
        /// <code>#include</code> or <code>#parse</code>). May modify the path
        /// of the resource to be included or may block the include entirely. All the
        /// registered IncludeEventHandlers are called unless null is returned. If
        /// none are registered the template at the includeResourcePath is retrieved.
        /// 
        /// </summary>
        /// <param name="includeResourcePath"> the path as given in the include directive.
        /// </param>
        /// <param name="currentResourcePath">the path of the currently rendering template that includes the
        /// include directive.
        /// </param>
        /// <param name="directiveName"> name of the directive used to include the resource. (With the
        /// standard directives this is either "parse" or "include").
        /// 
        /// </param>
        /// <returns> a new resource path for the directive, or null to block the
        /// include from occurring.
        /// </returns>
        string IncludeEvent(string includeResourcePath, string currentResourcePath, string directiveName);



    }
}
