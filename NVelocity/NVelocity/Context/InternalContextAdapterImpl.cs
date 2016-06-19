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
    using App.Event;
    using Runtime.Resource;
    using Util.Introspection;

    /// <summary>  This adapter class is the container for all context types for internal
    /// use.  The AST now uses this class rather than the app-level Context
    /// interface to allow flexibility in the future.
    /// 
    /// Currently, we have two context interfaces which must be supported :
    /// <ul>
    /// <li> Context : used for application/template data access
    /// <li> InternalHousekeepingContext : used for internal housekeeping and caching
    /// <li> InternalWrapperContext : used for getting root cache context and other
    /// such.
    /// <li> InternalEventContext : for event handling.
    /// </ul>
    /// 
    /// This class implements the two interfaces to ensure that all methods are
    /// supported.  When adding to the interfaces, or adding more context
    /// functionality, the interface is the primary definition, so alter that first
    /// and then all classes as necessary.  As of this writing, this would be
    /// the only class affected by changes to InternalContext
    /// 
    /// This class ensures that an InternalContextBase is available for internal
    /// use.  If an application constructs their own Context-implementing
    /// object w/instance subclassing AbstractContext, it may be that support for
    /// InternalContext is not available.  Therefore, InternalContextAdapter will
    /// create an InternalContextBase if necessary for this support.  Note that
    /// if this is necessary, internal information such as node-cache data will be
    /// lost from use to use of the context.  This may or may not be important,
    /// depending upon application.
    /// 
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: InternalContextAdapterImpl.java 685724 2008-08-13 23:12:12Z nbubna $
    /// </version>
    public sealed class InternalContextAdapterImpl : IInternalContextAdapter
    {
        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.getCurrentTemplateName()">
        /// </seealso>
        public string CurrentTemplateName
        {
            get
            {
                return icb.CurrentTemplateName;
            }

        }
        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.getTemplateNameStack()">
        /// </seealso>
        public object[] TemplateNameStack
        {
            get
            {
                return icb.TemplateNameStack;
            }

        }
        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.getCurrentMacroName()">
        /// </seealso>
        /// <since> 1.6
        /// </since>
        public string CurrentMacroName
        {
            get
            {
                return icb.CurrentMacroName;
            }

        }
        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.getCurrentMacroCallDepth()">
        /// </seealso>
        /// <since> 1.6
        /// </since>
        public int CurrentMacroCallDepth
        {
            get
            {
                return icb.CurrentMacroCallDepth;
            }

        }
        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.getMacroNameStack()">
        /// </seealso>
        /// <since> 1.6
        /// </since>
        public object[] MacroNameStack
        {
            get
            {
                return icb.MacroNameStack;
            }

        }
     
        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.getCurrentResource()">
        /// </seealso>
        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.setCurrentResource(org.apache.velocity.runtime.resource.Resource)">
        /// </seealso>
        public Resource CurrentResource
        {
            get
            {
                return icb.CurrentResource;
            }

            set
            {
                icb.CurrentResource = value;
            }

        }
   
        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.getAllowRendering()">
        /// </seealso>
        /// <since> 1.5
        /// </since>
        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.setAllowRendering(boolean)">
        /// </seealso>
        /// <since> 1.5
        /// </since>
        public bool AllowRendering
        {
            get
            {
                return icb.AllowRendering;
            }

            set
            {
                icb.AllowRendering = value;
            }

        }
   
        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.getMacroLibraries()">
        /// </seealso>
        /// <since> 1.6
        /// </since>
        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.setMacroLibraries(List)">
        /// </seealso>
        /// <since> 1.6
        /// </since>
        public System.Collections.IList MacroLibraries
        {
            get
            {
                return icb.MacroLibraries;
            }

            set
            {
                icb.MacroLibraries = value;
            }

        }
        /// <seealso cref="org.apache.velocity.context.Context.getKeys()">
        /// </seealso>
        public object[] Keys
        {
            get
            {
                return context.Keys;
            }

        }
        /// <summary>  returns the user data context that
        /// we are wrapping
        /// </summary>
        /// <returns> The internal user data context.
        /// </returns>
        public IContext InternalUserContext
        {
            get
            {
                return context;
            }

        }
        /// <summary>  Returns the base context that we are
        /// wrapping. Here, its this, but for other thing
        /// like VM related context contortions, it can
        /// be something else
        /// </summary>
        /// <returns> The base context.
        /// </returns>
        public IInternalContextAdapter BaseContext
        {
            get
            {
                return this;
            }

        }
        /// <seealso cref="org.apache.velocity.context.InternalEventContext.getEventCartridge()">
        /// </seealso>
        public EventCartridge EventCartridge
        {
            get
            {
                if (iec != null)
                {
                    return iec.EventCartridge;
                }

                return null;
            }

        }
        /// <summary>  the user data Context that we are wrapping</summary>
        internal IContext context = null;

        /// <summary>  the ICB we are wrapping.  We may need to make one
        /// if the user data context implementation doesn't
        /// support one.  The default AbstractContext-derived
        /// VelocityContext does, and it's recommended that
        /// people derive new contexts from AbstractContext
        /// rather than piecing things together
        /// </summary>
        internal IInternalHousekeepingContext icb = null;

        /// <summary>  The InternalEventContext that we are wrapping.  If
        /// the context passed to us doesn't support it, no
        /// biggie.  We don't make it for them - since its a
        /// user context thing, nothing gained by making one
        /// for them now
        /// </summary>
        internal IInternalEventContext iec = null;

        /// <summary>  CTOR takes a Context and wraps it, delegating all 'data' calls
        /// to it.
        /// 
        /// For support of internal contexts, it will create an InternalContextBase
        /// if need be.
        /// </summary>
        /// <param name="c">
        /// </param>
        public InternalContextAdapterImpl(IContext c)
        {
            context = c;

            if (!(c is IInternalHousekeepingContext))
            {
                icb = new InternalContextBase();
            }
            else
            {
                icb = (IInternalHousekeepingContext)context;
            }

            if (c is IInternalEventContext)
            {
                iec = (IInternalEventContext)context;
            }
        }

        /* --- InternalHousekeepingContext interface methods --- */

        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.pushCurrentTemplateName(java.lang.String)">
        /// </seealso>
        public void PushCurrentTemplateName(string s)
        {
            icb.PushCurrentTemplateName(s);
        }

        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.popCurrentTemplateName()">
        /// </seealso>
        public void PopCurrentTemplateName()
        {
            icb.PopCurrentTemplateName();
        }

        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.pushCurrentMacroName(java.lang.String)">
        /// </seealso>
        /// <since> 1.6
        /// </since>
        public void PushCurrentMacroName(string s)
        {
            icb.PushCurrentMacroName(s);
        }

        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.popCurrentMacroName()">
        /// </seealso>
        /// <since> 1.6
        /// </since>
        public void PopCurrentMacroName()
        {
            icb.PopCurrentMacroName();
        }

        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.icacheGet(java.lang.Object)">
        /// </seealso>
        public IntrospectionCacheData ICacheGet(object key)
        {
            return icb.ICacheGet(key);
        }

        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.icachePut(java.lang.Object, org.apache.velocity.util.introspection.IntrospectionCacheData)">
        /// </seealso>
        public void ICachePut(object key, IntrospectionCacheData o)
        {
            icb.ICachePut(key, o);
        }

        /* ---  Context interface methods --- */

        /// <seealso cref="org.apache.velocity.context.Context.Put(java.lang.String, java.lang.Object)">
        /// </seealso>
        public object Put(string key, object value)
        {
            return context.Put(key, value);
        }

        /// <seealso cref="InternalWrapperContext.localPut(String, Object)">
        /// </seealso>
        /// <since> 1.5
        /// </since>
        public object LocalPut(string key, object value)
        {
            return Put(key, value);
        }

        /// <seealso cref="org.apache.velocity.context.Context.Get(java.lang.String)">
        /// </seealso>
        public object Get(string key)
        {
            return context.Get(key);
        }

        /// <seealso cref="org.apache.velocity.context.Context.containsKey(java.lang.Object)">
        /// </seealso>
        public bool ContainsKey(object key)
        {
            return context.ContainsKey(key);
        }

        /// <seealso cref="org.apache.velocity.context.Context.remove(java.lang.Object)">
        /// </seealso>
        public object Remove(object key)
        {
            return context.Remove(key);
        }


        /* ---- InternalWrapperContext --- */

        /* -----  InternalEventContext ---- */

        /// <seealso cref="org.apache.velocity.context.InternalEventContext.attachEventCartridge(org.apache.velocity.app.event.EventCartridge)">
        /// </seealso>
        public EventCartridge AttachEventCartridge(EventCartridge ec)
        {
            if (iec != null)
            {
                return iec.AttachEventCartridge(ec);
            }

            return null;
        }
    }
}