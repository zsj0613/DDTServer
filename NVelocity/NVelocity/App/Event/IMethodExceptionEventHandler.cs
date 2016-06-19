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
    using Util;

    /// <summary>  Event handler called when a method throws an exception.  This gives the
    /// application a chance to deal with it and either
    /// return something nice, or throw.
    /// 
    /// Please return what you want rendered into the output stream.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: IMethodExceptionEventHandler.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <summary> Defines the execution strategy for MethodException</summary>
    /// <since> 1.5
    /// </since>
    class MethodExceptionExecutor : IEventHandlerMethodExecutor
    {
        virtual public object ReturnValue
        {
            get
            {
                return result;
            }

        }
        /// <summary> Only run the first IMethodExceptionEventHandler
        /// 
        /// </summary>
        /// <returns> true after this is executed once.
        /// </returns>
        virtual public bool Done
        {
            get
            {
                return executed;
            }

        }
        private IContext context;
        private System.Type claz;
        private string method;
        private System.Exception e;

        private object result;
        private bool executed = false;

        internal MethodExceptionExecutor(IContext context, System.Type claz, string method, System.Exception e)
        {
            this.context = context;
            this.claz = claz;
            this.method = method;
            this.e = e;
        }

        /// <summary> Call the method MethodException()
        /// 
        /// </summary>
        /// <param name="handler">call the appropriate method on this handler
        /// </param>
        /// <exception cref="Exception">generic exception thrown by MethodException event handler method call
        /// </exception>
        public virtual void Execute(IEventHandler handler)
        {
            IMethodExceptionEventHandler eh = (IMethodExceptionEventHandler)handler;

            if (eh is IContextAware)
                ((IContextAware)eh).Context = context;

            executed = true;
            result = ((IMethodExceptionEventHandler)handler).MethodException(claz, method, e);
        }
    }
    public interface IMethodExceptionEventHandler : IEventHandler
    {
        /// <summary> Called when a method throws an exception.
        /// Only the first registered IMethodExceptionEventHandler is called.  If
        /// none are registered a MethodInvocationException is thrown.
        /// 
        /// </summary>
        /// <param name="claz">the class of the object the method is being applied to
        /// </param>
        /// <param name="method">the method
        /// </param>
        /// <param name="e">the thrown exception
        /// </param>
        /// <returns> an object to insert in the page
        /// </returns>
        /// <throws>  Exception an exception to be thrown instead inserting an object </throws>
        object MethodException(System.Type claz, string method, System.Exception e);

    }
}