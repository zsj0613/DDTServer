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
    using Context;
    using Exception;
    using Runtime;
    using Util;
    using Util.Introspection;

    /// <summary> Calls on request all registered event handlers for a particular event. Each
    /// method accepts two event cartridges (typically one from the application and
    /// one from the context). All appropriate event handlers are executed in order
    /// until a stopping condition is met. See the docs for the individual methods to
    /// see what the stopping condition is for that method.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@wglass@forio.com">Will Glass-Husain </a>
    /// </author>
    /// <version>  $Id: EventHandlerUtil.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class EventHandlerUtil
    {


        /// <summary> Called before a reference is inserted. All event handlers are called in
        /// sequence. The default implementation inserts the reference as is.
        /// 
        /// This is a major hotspot method called by ASTReference render.
        /// 
        /// </summary>
        /// <param name="reference">reference from template about to be inserted
        /// </param>
        /// <param name="value">value about to be inserted (after toString() )
        /// </param>
        /// <param name="rsvc">current instance of RuntimeServices
        /// </param>
        /// <param name="context">The internal context adapter.
        /// </param>
        /// <returns> Object on which toString() should be called for output.
        /// </returns>
        public static object ReferenceInsert(IRuntimeServices rsvc, IInternalContextAdapter context, string reference, object value)
        {
            // app level cartridges have already been initialized

            /*
            * Performance modification: EventCartridge.getReferenceInsertionEventHandlers
            * now returns a null if there are no handlers. Thus we can avoid creating the
            * Iterator object.
            */
            EventCartridge ev1 = rsvc.ApplicationEventCartridge;
            System.Collections.IEnumerator applicationEventHandlerIterator = (ev1 == null) ? null : ev1.ReferenceInsertionEventHandlers;

            EventCartridge ev2 = context.EventCartridge;
            InitializeEventCartridge(rsvc, ev2);
            System.Collections.IEnumerator contextEventHandlerIterator = (ev2 == null) ? null : ev2.ReferenceInsertionEventHandlers;

            try
            {
                /*
                * Performance modification: methodExecutor is created only if one of the
                * iterators is not null.
                */

                IEventHandlerMethodExecutor methodExecutor = null;

                if (applicationEventHandlerIterator != null)
                {
                    methodExecutor = new ReferenceInsertExecutor(context, reference, value);
                    LterateOverEventHandlers(applicationEventHandlerIterator, methodExecutor);
                }

                if (contextEventHandlerIterator != null)
                {
                    if (methodExecutor == null)
                        methodExecutor = new ReferenceInsertExecutor(context, reference, value);

                    LterateOverEventHandlers(contextEventHandlerIterator, methodExecutor);
                }


                return methodExecutor != null ? methodExecutor.ReturnValue : value;
            }
            catch (System.SystemException e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new RuntimeException ("Exception in event handler.", e);
            }
        }

        /// <summary> Called when a null is evaluated during a #set. All event handlers are
        /// called in sequence until a false is returned. The default implementation
        /// always returns true.
        /// 
        /// </summary>
        /// <param name="lhs">Left hand side of the expression.
        /// </param>
        /// <param name="rhs">Right hand side of the expression.
        /// </param>
        /// <param name="rsvc">current instance of RuntimeServices
        /// </param>
        /// <param name="context">The internal context adapter.
        /// </param>
        /// <returns> true if to be logged, false otherwise
        /// </returns>
        public static bool ShouldLogOnNullSet(IRuntimeServices rsvc, IInternalContextAdapter context, string lhs, string rhs)
        {
            // app level cartridges have already been initialized
            EventCartridge ev1 = rsvc.ApplicationEventCartridge;
            System.Collections.IEnumerator applicationEventHandlerIterator = (ev1 == null) ? null : ev1.NullSetEventHandlers;

            EventCartridge ev2 = context.EventCartridge;
            InitializeEventCartridge(rsvc, ev2);
            System.Collections.IEnumerator contextEventHandlerIterator = (ev2 == null) ? null : ev2.NullSetEventHandlers;

            try
            {
                IEventHandlerMethodExecutor methodExecutor = new ShouldLogOnNullSetExecutor(context, lhs, rhs);

                CallEventHandlers(applicationEventHandlerIterator, contextEventHandlerIterator, methodExecutor);

                return ((System.Boolean)methodExecutor.ReturnValue);
            }
            catch (System.SystemException e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new RuntimeException("Exception in event handler.", e);
            }
        }

        /// <summary> Called when a method exception is generated during Velocity merge. Only
        /// the first valid event handler in the sequence is called. The default
        /// implementation simply rethrows the exception.
        /// 
        /// </summary>
        /// <param name="claz">Class that is causing the exception
        /// </param>
        /// <param name="method">method called that causes the exception
        /// </param>
        /// <param name="e">Exception thrown by the method
        /// </param>
        /// <param name="rsvc">current instance of RuntimeServices
        /// </param>
        /// <param name="context">The internal context adapter.
        /// </param>
        /// <returns> Object to return as method result
        /// </returns>
        /// <throws>  Exception </throws>
        /// <summary>             to be wrapped and propogated to app
        /// </summary>
        public static object MethodException(IRuntimeServices rsvc, IInternalContextAdapter context, System.Type claz, string method, System.Exception e)
        {
            // app level cartridges have already been initialized
            EventCartridge ev1 = rsvc.ApplicationEventCartridge;
            System.Collections.IEnumerator applicationEventHandlerIterator = (ev1 == null) ? null : ev1.MethodExceptionEventHandlers;

            EventCartridge ev2 = context.EventCartridge;
            InitializeEventCartridge(rsvc, ev2);
            System.Collections.IEnumerator contextEventHandlerIterator = (ev2 == null) ? null : ev2.MethodExceptionEventHandlers;

            IEventHandlerMethodExecutor methodExecutor = new NVelocity.App.Event.MethodExceptionExecutor(context, claz, method, e);

         
            if (((applicationEventHandlerIterator == null) || !applicationEventHandlerIterator.MoveNext()) && ((contextEventHandlerIterator == null) || !contextEventHandlerIterator.MoveNext()))
            {
                throw e;
            }

            CallEventHandlers(applicationEventHandlerIterator, contextEventHandlerIterator, methodExecutor);

            return methodExecutor.ReturnValue;
        }

        /// <summary> Called when an include-type directive is encountered (#include or
        /// #parse). All the registered event handlers are called unless null is
        /// returned. The default implementation always processes the included
        /// resource.
        /// 
        /// </summary>
        /// <param name="includeResourcePath">the path as given in the include directive.
        /// </param>
        /// <param name="currentResourcePath">the path of the currently rendering template that includes the
        /// include directive.
        /// </param>
        /// <param name="directiveName">name of the directive used to include the resource. (With the
        /// standard directives this is either "parse" or "include").
        /// </param>
        /// <param name="rsvc">current instance of RuntimeServices
        /// </param>
        /// <param name="context">The internal context adapter.
        /// 
        /// </param>
        /// <returns> a new resource path for the directive, or null to block the
        /// include from occurring.
        /// </returns>
        public static string IncludeEvent(IRuntimeServices rsvc, IInternalContextAdapter context, string includeResourcePath, string currentResourcePath, string directiveName)
        {
            // app level cartridges have already been initialized
            EventCartridge ev1 = rsvc.ApplicationEventCartridge;
            System.Collections.IEnumerator applicationEventHandlerIterator = (ev1 == null) ? null : ev1.IncludeEventHandlers;

            EventCartridge ev2 = context.EventCartridge;
            InitializeEventCartridge(rsvc, ev2);
            System.Collections.IEnumerator contextEventHandlerIterator = (ev2 == null) ? null : ev2.IncludeEventHandlers;

            try
            {
                IEventHandlerMethodExecutor methodExecutor = new NVelocity.App.Event.IncludeEventExecutor(context, includeResourcePath, currentResourcePath, directiveName);

                CallEventHandlers(applicationEventHandlerIterator, contextEventHandlerIterator, methodExecutor);

                return (string)methodExecutor.ReturnValue;
            }
            catch (System.SystemException e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new RuntimeException("Exception in event handler.", e);
            }
        }


        /// <summary> Called when an invalid get method is encountered.
        /// 
        /// </summary>
        /// <param name="rsvc">current instance of RuntimeServices
        /// </param>
        /// <param name="context">the context when the reference was found invalid
        /// </param>
        /// <param name="reference">complete invalid reference
        /// </param>
        /// <param name="object">object from reference, or null if not available
        /// </param>
        /// <param name="property">name of property, or null if not relevant
        /// </param>
        /// <param name="Info">contains Info on template, line, col
        /// </param>
        /// <returns> substitute return value for missing reference, or null if no substitute
        /// </returns>
        public static object InvalidGetMethod(IRuntimeServices rsvc, IInternalContextAdapter context, string reference, object object_Renamed, string property, Info info)
        {
            return InvalidReferenceHandlerCall(new NVelocity.App.Event.InvalidGetMethodExecutor(context, reference, object_Renamed, property, info), rsvc, context);
        }


        /// <summary> Called when an invalid set method is encountered.
        /// 
        /// </summary>
        /// <param name="rsvc">current instance of RuntimeServices
        /// </param>
        /// <param name="context">the context when the reference was found invalid
        /// </param>
        /// <param name="leftreference">left reference being assigned to
        /// </param>
        /// <param name="rightreference">invalid reference on the right
        /// </param>
        /// <param name="Info">contains Info on template, line, col
        /// </param>
        public static void InvalidSetMethod(IRuntimeServices rsvc, IInternalContextAdapter context, string leftreference, string rightreference, Info info)
        {
            /**
            * ignore return value
            */
            InvalidReferenceHandlerCall(new NVelocity.App.Event.InvalidSetMethodExecutor(context, leftreference, rightreference, info), rsvc, context);
        }

        /// <summary> Called when an invalid method is encountered.
        /// 
        /// </summary>
        /// <param name="rsvc">current instance of RuntimeServices
        /// </param>
        /// <param name="context">the context when the reference was found invalid
        /// </param>
        /// <param name="reference">complete invalid reference
        /// </param>
        /// <param name="object">object from reference, or null if not available
        /// </param>
        /// <param name="method">name of method, or null if not relevant
        /// </param>
        /// <param name="Info">contains Info on template, line, col
        /// </param>
        /// <returns> substitute return value for missing reference, or null if no substitute
        /// </returns>
        public static object InvalidMethod(IRuntimeServices rsvc, IInternalContextAdapter context, string reference, object object_Renamed, string method, Info info)
        {
            return InvalidReferenceHandlerCall(new NVelocity.App.Event.InvalidMethodExecutor(context, reference, object_Renamed, method, info), rsvc, context);
        }


        /// <summary> Calls event handler method with appropriate chaining across event handlers.
        /// 
        /// </summary>
        /// <param name="methodExecutor">
        /// </param>
        /// <param name="rsvc">current instance of RuntimeServices
        /// </param>
        /// <param name="context">The current context
        /// </param>
        /// <returns> return value from method, or null if no return value
        /// </returns>
        public static object InvalidReferenceHandlerCall(IEventHandlerMethodExecutor methodExecutor, IRuntimeServices rsvc, IInternalContextAdapter context)
        {
            // app level cartridges have already been initialized
            EventCartridge ev1 = rsvc.ApplicationEventCartridge;
            System.Collections.IEnumerator applicationEventHandlerIterator = (ev1 == null) ? null : ev1.InvalidReferenceEventHandlers;

            EventCartridge ev2 = context.EventCartridge;
            InitializeEventCartridge(rsvc, ev2);
            System.Collections.IEnumerator contextEventHandlerIterator = (ev2 == null) ? null : ev2.InvalidReferenceEventHandlers;

            try
            {
                CallEventHandlers(applicationEventHandlerIterator, contextEventHandlerIterator, methodExecutor);

                return methodExecutor.ReturnValue;
            }
            catch (System.SystemException e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new RuntimeException("Exception in event handler.", e);
            }
        }

        /// <summary> Initialize the event cartridge if appropriate.
        /// 
        /// </summary>
        /// <param name="rsvc">current instance of RuntimeServices
        /// </param>
        /// <param name="eventCartridge">the event cartridge to be initialized
        /// </param>
        private static void InitializeEventCartridge(IRuntimeServices rsvc, EventCartridge eventCartridge)
        {
            if (eventCartridge != null)
            {
                try
                {
                    eventCartridge.Initialize(rsvc);
                }
                catch (System.Exception e)
                {
                    throw new RuntimeException("Couldn't initialize event cartridge : ", e);
                }
            }
        }


        /// <summary> Loop through both the application level and context-attached event handlers.
        /// 
        /// </summary>
        /// <param name="applicationEventHandlerIterator">Iterator that loops through all global event handlers declared at application level
        /// </param>
        /// <param name="contextEventHandlerIterator">Iterator that loops through all global event handlers attached to context
        /// </param>
        /// <param name="eventExecutor">Strategy object that executes event handler method
        /// </param>
        /// <exception cref="Exception">generic exception potentially thrown by event handlers
        /// </exception>
        private static void CallEventHandlers(System.Collections.IEnumerator applicationEventHandlerIterator, System.Collections.IEnumerator contextEventHandlerIterator, IEventHandlerMethodExecutor eventExecutor)
        {
            /**
            * First loop through the event handlers configured at the app level
            * in the properties file.
            */
            LterateOverEventHandlers(applicationEventHandlerIterator, eventExecutor);

            /**
            * Then loop through the event handlers attached to the context.
            */
            LterateOverEventHandlers(contextEventHandlerIterator, eventExecutor);
        }

        /// <summary> Loop through a given iterator of event handlers.
        /// 
        /// </summary>
        /// <param name="handlerIterator">Iterator that loops through event handlers
        /// </param>
        /// <param name="eventExecutor">Strategy object that executes event handler method
        /// </param>
        /// <exception cref="Exception">generic exception potentially thrown by event handlers
        /// </exception>
        private static void LterateOverEventHandlers(System.Collections.IEnumerator handlerIterator, IEventHandlerMethodExecutor eventExecutor)
        {
            if (handlerIterator != null)
            {
                
                for (System.Collections.IEnumerator i = handlerIterator; i.MoveNext(); )
                {
                  
                    IEventHandler eventHandler = (IEventHandler)i.Current;

                    if (!eventExecutor.Done)
                    {
                        eventExecutor.Execute(eventHandler);
                    }
                }
            }
        }
    }
}
