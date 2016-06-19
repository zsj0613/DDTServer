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
    using Util.Introspection;

    /// <summary> Event handler called when an invalid reference is encountered.  Allows 
    /// the application to report errors or substitute return values. May be chained
    /// in sequence; the behavior will differ per method.
    /// 
    /// <p>This feature should be regarded as experimental.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <version>  $Id: IInvalidReferenceEventHandler.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    /// <summary> Defines the execution strategy for InvalidGetMethod</summary>
    class InvalidGetMethodExecutor : IEventHandlerMethodExecutor
    {
        virtual public object ReturnValue
        {
            get
            {
                return result;
            }

        }
        virtual public bool Done
        {
            get
            {
                return (result != null);
            }

        }
        private IContext context;
        private string reference;
        private object object_Renamed;
        private string property;
        private Info info;

        private object result;

        internal InvalidGetMethodExecutor(IContext context, string reference, object object_Renamed, string property, Info info)
        {
            this.context = context;
            this.reference = reference;
            this.object_Renamed = object_Renamed;
            this.property = property;
            this.info = info;
        }

        /// <summary> Call the method InvalidGetMethod()
        /// 
        /// </summary>
        /// <param name="handler">call the appropriate method on this handler
        /// </param>
        public virtual void Execute(IEventHandler handler)
        {
            result = ((IInvalidReferenceEventHandler)handler).InvalidGetMethod(context, reference, object_Renamed, property, info);
        }
    }
    /// <summary> Defines the execution strategy for InvalidGetMethod</summary>
    class InvalidSetMethodExecutor : IEventHandlerMethodExecutor
    {
        virtual public object ReturnValue
        {
            get
            {
                return null;
            }

        }
        virtual public bool Done
        {
            get
            {
                return result;
            }

        }
        private IContext context;
        private string leftreference;
        private string rightreference;
        private Info info;

        private bool result;

        internal InvalidSetMethodExecutor(IContext context, string leftreference, string rightreference, Info info)
        {
            this.context = context;
            this.leftreference = leftreference;
            this.rightreference = rightreference;
            this.info = info;
        }

        /// <summary> Call the method InvalidSetMethod()
        /// 
        /// </summary>
        /// <param name="handler">call the appropriate method on this handler
        /// </param>
        public virtual void Execute(IEventHandler handler)
        {
            result = ((IInvalidReferenceEventHandler)handler).InvalidSetMethod(context, leftreference, rightreference, info);
        }
    }
    /// <summary> Defines the execution strategy for InvalidGetMethod</summary>
    class InvalidMethodExecutor : IEventHandlerMethodExecutor
    {
        virtual public object ReturnValue
        {
            get
            {
                return result;
            }

        }
        virtual public bool Done
        {
            get
            {
                return executed && (result != null);
            }

        }
        private IContext context;
        private string reference;
        private object object_Renamed;
        private string method;
        private Info info;

        private object result;
        private bool executed = false;

        internal InvalidMethodExecutor(IContext context, string reference, object object_Renamed, string method, Info info)
        {
            this.context = context;
            this.reference = reference;
            this.object_Renamed = object_Renamed;
            this.method = method;
            this.info = info;
        }

        /// <summary> Call the method InvalidMethod()
        /// 
        /// </summary>
        /// <param name="handler">call the appropriate method on this handler
        /// </param>
        public virtual void Execute(IEventHandler handler)
        {
            executed = true;
            result = ((IInvalidReferenceEventHandler)handler).InvalidMethod(context, reference, object_Renamed, method, info);
        }
    }
    public interface IInvalidReferenceEventHandler : IEventHandler
    {

        /// <summary> Called when object is null or there is no getter for the given 
        /// property.  Also called for invalid references without properties.  
        /// InvalidGetMethod() will be called in sequence for
        /// each link in the chain until the first non-null value is
        /// returned.
        /// 
        /// </summary>
        /// <param name="context">the context when the reference was found invalid
        /// </param>
        /// <param name="reference">string with complete invalid reference
        /// </param>
        /// <param name="object">the object referred to, or null if not found
        /// </param>
        /// <param name="property">the property name from the reference
        /// </param>
        /// <param name="Info">contains template, line, column details
        /// </param>
        /// <returns> substitute return value for missing reference, or null if no substitute
        /// </returns>
        object InvalidGetMethod(IContext context, string reference, object object_Renamed, string property, Info info);

        /// <summary> Called when object is null or there is no setter for the given 
        /// property.  InvalidSetMethod() will be called in sequence for
        /// each link in the chain until a true value is returned.  It's
        /// recommended that false be returned as a default to allow
        /// for easy chaining.
        /// 
        /// </summary>
        /// <param name="context">the context when the reference was found invalid
        /// </param>
        /// <param name="leftreference">left reference being assigned to
        /// </param>
        /// <param name="rightreference">invalid reference on the right
        /// </param>
        /// <param name="Info">contains Info on template, line, col
        /// 
        /// </param>
        /// <returns> if true then stop calling InvalidSetMethod along the 
        /// chain.
        /// </returns>
        bool InvalidSetMethod(IContext context, string leftreference, string rightreference, Info info);

        /// <summary> Called when object is null or the given method does not exist.
        /// InvalidMethod() will be called in sequence for each link in 
        /// the chain until the first non-null value is returned. 
        /// 
        /// </summary>
        /// <param name="context">the context when the reference was found invalid
        /// </param>
        /// <param name="reference">string with complete invalid reference
        /// </param>
        /// <param name="object">the object referred to, or null if not found
        /// </param>
        /// <param name="method">the name of the (non-existent) method
        /// </param>
        /// <param name="Info">contains template, line, column details
        /// </param>
        /// <returns> substitute return value for missing reference, or null if no substitute
        /// </returns>
        object InvalidMethod(IContext context, string reference, object object_Renamed, string method, Info info);

    }
}
