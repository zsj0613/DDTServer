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
    using Runtime.Resource;
    using Util.Introspection;


    /// <summary>  interface to encapsulate the 'stuff' for internal operation of velocity.
    /// We use the context as a thread-safe storage : we take advantage of the
    /// fact that it's a visitor  of sorts  to all nodes (that matter) of the
    /// AST during Init() and render().
    /// 
    /// Currently, it carries the template name for namespace
    /// support, as well as node-local context data introspection caching.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  <a href="mailto:Christoph.Reck@dlr.de">Christoph Reck</a>
    /// </author>
    /// <version>  $Id: InternalHousekeepingContext.java 679861 2008-07-25 17:17:50Z nbubna $
    /// </version>
    public interface IInternalHousekeepingContext
    {
        /// <summary>  Get the current template name
        /// 
        /// </summary>
        /// <returns> String current template name
        /// </returns>
        string CurrentTemplateName
        {
            get;

        }
        /// <summary>  Returns the template name stack in form of an array.
        /// 
        /// </summary>
        /// <returns> Object[] with the template name stack contents.
        /// </returns>
        object[] TemplateNameStack
        {
            get;

        }
        /// <summary>  Get the current macro name
        /// 
        /// </summary>
        /// <returns> String current macro name
        /// </returns>
        string CurrentMacroName
        {
            get;

        }
        /// <summary>  Get the current macro call depth
        /// 
        /// </summary>
        /// <returns> int current macro call depth
        /// </returns>
        int CurrentMacroCallDepth
        {
            get;

        }
        /// <summary>  Returns the macro name stack in form of an array.
        /// 
        /// </summary>
        /// <returns> Object[] with the macro name stack contents.
        /// </returns>
        object[] MacroNameStack
        {
            get;

        }
     
        /// <summary>  temporary fix to enable #include() to figure out
        /// current encoding.
        /// 
        /// </summary>
        /// <returns> The current resource.
        /// </returns>
        /// <param name="r">
        /// </param>
        Resource CurrentResource
        {
            get;

            set;

        }
     
        /// <summary> Checks to see if rendering should be allowed.  Defaults to true but will
        /// return false after a #stop directive.
        /// 
        /// </summary>
        /// <returns> true if rendering is allowed, false if no rendering should occur
        /// </returns>
        /// <summary> Set whether rendering is allowed.  Defaults to true but is set to
        /// false after a #stop directive.
        /// </summary>
        /// <param name="v">
        /// </param>
        bool AllowRendering
        {
            get;

            set;

        }
      
        /// <summary> Get the macro library list for the current template.
        /// 
        /// </summary>
        /// <returns> List of macro library names
        /// </returns>
        /// <summary> Set the macro library list for the current template.
        /// 
        /// </summary>
        /// <param name="macroLibraries">list of macro libraries to set
        /// </param>
        System.Collections.IList MacroLibraries
        {
            get;

            set;

        }
        /// <summary>  set the current template name on top of stack
        /// 
        /// </summary>
        /// <param name="s">current template name
        /// </param>
        void PushCurrentTemplateName(string s);

        /// <summary>  remove the current template name from stack</summary>
        void PopCurrentTemplateName();

        /// <summary>  set the current macro name on top of stack
        /// 
        /// </summary>
        /// <param name="s">current macro name
        /// </param>
        void PushCurrentMacroName(string s);

        /// <summary>  remove the current macro name from stack</summary>
        void PopCurrentMacroName();

        /// <seealso cref="IntrospectionCacheData)">
        /// object if exists for the key
        /// 
        /// </seealso>
        /// <param name="key"> key to find in cache
        /// </param>
        /// <returns> cache object
        /// </returns>
        IntrospectionCacheData ICacheGet(object key);

        /// <seealso cref="IntrospectionCacheData)">
        /// element in the cache for specified key
        /// 
        /// </seealso>
        /// <param name="key"> key
        /// </param>
        /// <param name="instance"> IntrospectionCacheData object to place in cache
        /// </param>
        void ICachePut(object key, IntrospectionCacheData o);
    }
}