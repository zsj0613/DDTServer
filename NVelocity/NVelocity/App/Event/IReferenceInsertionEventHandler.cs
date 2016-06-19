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
    using Util;

    /// <summary>  Reference 'Stream insertion' event handler.  Called with object
    /// that will be inserted into stream via value.toString().
    /// 
    /// Please return an Object that will toString() nicely :)
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: IReferenceInsertionEventHandler.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <summary> Defines the execution strategy for ReferenceInsert</summary>
    /// <since> 1.5
    /// </since>
    class ReferenceInsertExecutor : IEventHandlerMethodExecutor
    {
        virtual public object ReturnValue
        {
            get
            {
                return value;
            }

        }
        /// <summary> Continue to end of event handler iteration
        /// 
        /// </summary>
        /// <returns> always returns false
        /// </returns>
        virtual public bool Done
        {
            get
            {
                return false;
            }

        }
        private IContext context;
        private string reference;
        private object value;

        internal ReferenceInsertExecutor(IContext context, string reference, object value)
        {
            this.context = context;
            this.reference = reference;
            this.value = value;
        }

        /// <summary> Call the method ReferenceInsert()
        /// 
        /// </summary>
        /// <param name="handler">call the appropriate method on this handler
        /// </param>
        public virtual void Execute(IEventHandler handler)
        {
            IReferenceInsertionEventHandler eh = (IReferenceInsertionEventHandler)handler;

            if (eh is IContextAware)
                ((IContextAware)eh).Context = context;

            /**
            * Every successive call will alter the same value
            */
            value = ((IReferenceInsertionEventHandler)handler).ReferenceInsert(reference, value);
        }
    }
    public interface IReferenceInsertionEventHandler : IEventHandler
    {
        /// <summary> A call-back which is executed during Velocity merge before a reference
        /// value is inserted into the output stream. All registered
        /// ReferenceInsertionEventHandlers are called in sequence. If no
        /// ReferenceInsertionEventHandlers are are registered then reference value
        /// is inserted into the output stream as is.
        /// 
        /// </summary>
        /// <param name="reference">Reference from template about to be inserted.
        /// </param>
        /// <param name="value">Value about to be inserted (after its <code>toString()</code>
        /// method is called).
        /// </param>
        /// <returns> Object on which <code>toString()</code> should be called for
        /// output.
        /// </returns>
        object ReferenceInsert(string reference, object value);

    }
}