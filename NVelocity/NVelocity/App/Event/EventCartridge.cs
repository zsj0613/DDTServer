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
    using System;

    using Context;
    using Runtime;
    using Util;

    /// <summary> Stores the event handlers. Event handlers can be assigned on a per
    /// VelocityEngine instance basis by specifying the class names in the
    /// velocity.properties file. Event handlers may also be assigned on a per-page
    /// basis by creating a new instance of EventCartridge, adding the event
    /// handlers, and then calling AttachToContext. For clarity, it's recommended
    /// that one approach or the other be followed, as the second method is primarily
    /// presented for backwards compatibility.
    /// 
    /// <P>
    /// Note that Event Handlers follow a filter pattern, with multiple event
    /// handlers allowed for each event. When the appropriate event occurs, all the
    /// appropriate event handlers are called in the sequence they were added to the
    /// Event Cartridge. See the javadocs of the specific event handler interfaces
    /// for more details.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@wglass@forio.com">Will Glass-Husain </a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr. </a>
    /// </author>
    /// <author>  <a href="mailto:j_a_fernandez@yahoo.com">Jose Alberto Fernandez </a>
    /// </author>
    /// <version>  $Id: EventCartridge.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    public class EventCartridge
    {
        /// <summary> Iterate through all the stored IReferenceInsertionEventHandler objects
        /// 
        /// </summary>
        /// <returns> iterator of handler objects, null if there are not handlers
        /// </returns>
        /// <since> 1.5
        /// </since>
        virtual public System.Collections.IEnumerator ReferenceInsertionEventHandlers
        {
            get
            {
                return referenceHandlers.Count == 0 ? null : referenceHandlers.GetEnumerator();
            }

        }
        /// <summary> Iterate through all the stored NullSetEventHandler objects
        /// 
        /// </summary>
        /// <returns> iterator of handler objects
        /// </returns>
        /// <since> 1.5
        /// </since>
        virtual public System.Collections.IEnumerator NullSetEventHandlers
        {
            get
            {
                return nullSetHandlers.GetEnumerator();
            }

        }
        /// <summary> Iterate through all the stored IMethodExceptionEventHandler objects
        /// 
        /// </summary>
        /// <returns> iterator of handler objects
        /// </returns>
        /// <since> 1.5
        /// </since>
        virtual public System.Collections.IEnumerator MethodExceptionEventHandlers
        {
            get
            {
                return methodExceptionHandlers.GetEnumerator();
            }

        }
        /// <summary> Iterate through all the stored IncludeEventHandlers objects
        /// 
        /// </summary>
        /// <returns> iterator of handler objects
        /// </returns>
        virtual public System.Collections.IEnumerator IncludeEventHandlers
        {
            get
            {
                return includeHandlers.GetEnumerator();
            }

        }
        /// <summary> Iterate through all the stored InvalidReferenceEventHandlers objects
        /// 
        /// </summary>
        /// <returns> iterator of handler objects
        /// </returns>
        /// <since> 1.5
        /// </since>
        virtual public System.Collections.IEnumerator InvalidReferenceEventHandlers
        {
            get
            {
                return invalidReferenceHandlers.GetEnumerator();
            }

        }
        private System.Collections.IList referenceHandlers = new System.Collections.ArrayList();
        private System.Collections.IList nullSetHandlers = new System.Collections.ArrayList();
        private System.Collections.IList methodExceptionHandlers = new System.Collections.ArrayList();
        private System.Collections.IList includeHandlers = new System.Collections.ArrayList();
        private System.Collections.IList invalidReferenceHandlers = new System.Collections.ArrayList();

        /// <summary> Ensure that handlers are not initialized more than once.</summary>

        internal System.Collections.Generic.HashSet<object> initializedHandlers = new System.Collections.Generic.HashSet<object>();

        /// <summary>  Adds an event handler(s) to the Cartridge.  This method
        /// will find all possible event handler interfaces supported
        /// by the passed in object.
        /// 
        /// </summary>
        /// <param name="ev">object impementing a valid EventHandler-derived interface
        /// </param>
        /// <returns> true if a supported interface, false otherwise or if null
        /// </returns>
        public virtual bool AddEventHandler(IEventHandler ev)
        {
            if (ev == null)
            {
                return false;
            }

            bool found = false;

            if (ev is IReferenceInsertionEventHandler)
            {
                AddReferenceInsertionEventHandler((IReferenceInsertionEventHandler)ev);
                found = true;
            }

            if (ev is INullSetEventHandler)
            {
                AddNullSetEventHandler((INullSetEventHandler)ev);
                found = true;
            }

            if (ev is IMethodExceptionEventHandler)
            {
                AddMethodExceptionHandler((IMethodExceptionEventHandler)ev);
                found = true;
            }

            if (ev is IIncludeEventHandler)
            {
                AddIncludeEventHandler((IIncludeEventHandler)ev);
                found = true;
            }

            if (ev is IInvalidReferenceEventHandler)
            {
                AddInvalidReferenceEventHandler((IInvalidReferenceEventHandler)ev);
                found = true;
            }

            return found;
        }

        /// <summary>  Add a reference insertion event handler to the Cartridge.
        /// 
        /// </summary>
        /// <param name="ev">IReferenceInsertionEventHandler
        /// </param>
        /// <since> 1.5
        /// </since>
        public virtual void AddReferenceInsertionEventHandler(IReferenceInsertionEventHandler ev)
        {
            referenceHandlers.Add(ev);
        }

        /// <summary>  Add a null set event handler to the Cartridge.
        /// 
        /// </summary>
        /// <param name="ev">NullSetEventHandler
        /// </param>
        /// <since> 1.5
        /// </since>
        public virtual void AddNullSetEventHandler(INullSetEventHandler ev)
        {
            nullSetHandlers.Add(ev);
        }

        /// <summary>  Add a method exception event handler to the Cartridge.
        /// 
        /// </summary>
        /// <param name="ev">IMethodExceptionEventHandler
        /// </param>
        /// <since> 1.5
        /// </since>
        public virtual void AddMethodExceptionHandler(IMethodExceptionEventHandler ev)
        {
            methodExceptionHandlers.Add(ev);
        }

        /// <summary>  Add an include event handler to the Cartridge.
        /// 
        /// </summary>
        /// <param name="ev">IIncludeEventHandler
        /// </param>
        /// <since> 1.5
        /// </since>
        public virtual void AddIncludeEventHandler(IIncludeEventHandler ev)
        {
            includeHandlers.Add(ev);
        }

        /// <summary>  Add an invalid reference event handler to the Cartridge.
        /// 
        /// </summary>
        /// <param name="ev">IInvalidReferenceEventHandler
        /// </param>
        /// <since> 1.5
        /// </since>
        public virtual void AddInvalidReferenceEventHandler(IInvalidReferenceEventHandler ev)
        {
            invalidReferenceHandlers.Add(ev);
        }


        /// <summary> Removes an event handler(s) from the Cartridge. This method will find all
        /// possible event handler interfaces supported by the passed in object and
        /// remove them.
        /// 
        /// </summary>
        /// <param name="ev"> object impementing a valid EventHandler-derived interface
        /// </param>
        /// <returns> true if event handler was previously registered, false if not
        /// found
        /// </returns>
        public virtual bool RemoveEventHandler(EventHandler ev)
        {
            if (ev == null)
            {
                return false;
            }

            bool found = false;

            if (ev.GetType() is IReferenceInsertionEventHandler)
            {
                System.Boolean tempBoolean;
                tempBoolean = referenceHandlers.Contains(ev);
                referenceHandlers.Remove(ev);
                return tempBoolean;
            }

            if (ev.GetType() is INullSetEventHandler)
            {
                System.Boolean tempBoolean2;
                tempBoolean2 = nullSetHandlers.Contains(ev);
                nullSetHandlers.Remove(ev);
                return tempBoolean2;
            }

            if (ev.GetType() is IMethodExceptionEventHandler)
            {
                System.Boolean tempBoolean3;
                tempBoolean3 = methodExceptionHandlers.Contains(ev);
                methodExceptionHandlers.Remove(ev);
                return tempBoolean3;
            }

            if (ev.GetType() is IIncludeEventHandler)
            {
                System.Boolean tempBoolean4;
                tempBoolean4 = includeHandlers.Contains(ev);
                includeHandlers.Remove(ev);
                return tempBoolean4;
            }

            if (ev.GetType() is IInvalidReferenceEventHandler)
            {
                System.Boolean tempBoolean5;
                tempBoolean5 = invalidReferenceHandlers.Contains(ev);
                invalidReferenceHandlers.Remove(ev);
                return tempBoolean5;
            }

            return found;
        }

        /// <summary>  Attached the EventCartridge to the context
        /// 
        /// Final because not something one should mess with lightly :)
        /// 
        /// </summary>
        /// <param name="context">context to attach to
        /// </param>
        /// <returns> true if successful, false otherwise
        /// </returns>
        public bool AttachToContext(IContext context)
        {
            if (context is IInternalEventContext)
            {
                IInternalEventContext iec = (IInternalEventContext)context;

                iec.AttachEventCartridge(this);

                /**
                * while it's tempting to call setContext on each handler from here,
                * this needs to be done before each method call.  This is
                * because the specific context will change as inner contexts
                * are linked in through macros, foreach, or directly by the user.
                */

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary> Initialize the handlers.  For global handlers this is called when Velocity
        /// is initialized. For local handlers this is called when the first handler
        /// is executed.  Handlers will not be initialized more than once.
        /// 
        /// </summary>
        /// <param name="rs">
        /// </param>
        /// <throws>  Exception </throws>
        /// <since> 1.5
        /// </since>
        public virtual void Initialize(IRuntimeServices rs)
        {


            for (System.Collections.IEnumerator i = referenceHandlers.GetEnumerator(); i.MoveNext(); )
            {

                IEventHandler eh = (IEventHandler)i.Current;
                if ((eh is IRuntimeServicesAware) && !initializedHandlers.Contains(eh))
                {
                    ((IRuntimeServicesAware)eh).SetRuntimeServices(rs);
                    initializedHandlers.Add(eh);
                }
            }


            for (System.Collections.IEnumerator i = nullSetHandlers.GetEnumerator(); i.MoveNext(); )
            {

                IEventHandler eh = (IEventHandler)i.Current;
                if ((eh is IRuntimeServicesAware) && !initializedHandlers.Contains(eh))
                {
                    ((IRuntimeServicesAware)eh).SetRuntimeServices(rs);
                    initializedHandlers.Add(eh);
                }
            }


            for (System.Collections.IEnumerator i = methodExceptionHandlers.GetEnumerator(); i.MoveNext(); )
            {

                IEventHandler eh = (IEventHandler)i.Current;
                if ((eh is IRuntimeServicesAware) && !initializedHandlers.Contains(eh))
                {
                    ((IRuntimeServicesAware)eh).SetRuntimeServices(rs);
                    initializedHandlers.Add(eh);
                }
            }


            for (System.Collections.IEnumerator i = includeHandlers.GetEnumerator(); i.MoveNext(); )
            {

                IEventHandler eh = (IEventHandler)i.Current;
                if ((eh is IRuntimeServicesAware) && !initializedHandlers.Contains(eh))
                {
                    ((IRuntimeServicesAware)eh).SetRuntimeServices(rs);
                    initializedHandlers.Add(eh);
                }
            }


            for (System.Collections.IEnumerator i = invalidReferenceHandlers.GetEnumerator(); i.MoveNext(); )
            {

                IEventHandler eh = (IEventHandler)i.Current;
                if ((eh is IRuntimeServicesAware) && !initializedHandlers.Contains(eh))
                {
                    ((IRuntimeServicesAware)eh).SetRuntimeServices(rs);
                    initializedHandlers.Add(eh);
                }
            }
        }
    }
}