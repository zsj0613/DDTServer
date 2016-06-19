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

    /// <summary>  Event handler called when the RHS of #set is null.  Lets an app approve / veto
    /// writing a Log message based on the specific reference.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: NullSetEventHandler.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <summary> Defines the execution strategy for ShouldLogOnNullSet</summary>
    /// <since> 1.5
    /// </since>
    class ShouldLogOnNullSetExecutor : IEventHandlerMethodExecutor
    {
        virtual public object ReturnValue
        {
            get
            {
                // return new Boolean(result);
                return result ? true : false;
            }

        }
        virtual public bool Done
        {
            get
            {
                return executed && !result;
            }

        }
        private IContext context;
        private string lhs;
        private string rhs;

        /// <summary> when this is false, quit iterating</summary>
        private bool result = true;
        private bool executed = false;

        internal ShouldLogOnNullSetExecutor(IContext context, string lhs, string rhs)
        {
            this.context = context;
            this.lhs = lhs;
            this.rhs = rhs;
        }

        /// <summary> Call the method ShouldLogOnNullSet()
        /// 
        /// </summary>
        /// <param name="handler">call the appropriate method on this handler
        /// </param>
        public virtual void Execute(IEventHandler handler)
        {
            INullSetEventHandler eh = (INullSetEventHandler)handler;

            if (eh is IContextAware)
                ((IContextAware)eh).Context = context;

            executed = true;
            result = ((INullSetEventHandler)handler).shouldLogOnNullSet(lhs, rhs);
        }
    }
    public interface INullSetEventHandler : IEventHandler
    {
        /// <summary> Called when the RHS of a #set() is null, which will result
        /// in a null LHS. All NullSetEventHandlers
        /// are called in sequence until a false is returned.  If no NullSetEventHandler
        /// is registered all nulls will be logged.
        /// 
        /// </summary>
        /// <param name="lhs"> reference literal of left-hand-side of set statement
        /// </param>
        /// <param name="rhs"> reference literal of right-hand-side of set statement
        /// </param>
        /// <returns> true if Log message should be written, false otherwise
        /// </returns>
        bool shouldLogOnNullSet(string lhs, string rhs);

    }
}