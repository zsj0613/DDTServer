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
    /// <summary> Strategy object used to Execute event handler method.  Will be called
    /// while looping through all the chained event handler implementations.
    /// Each EventHandler method call should have a parallel executor object
    /// defined.  
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <version>  $Id: EventHandlerMethodExecutor.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public interface IEventHandlerMethodExecutor
    {
        /// <summary> Called after Execute() to see if iterating should stop. Should
        /// always return false before method Execute() is run.
        /// 
        /// </summary>
        /// <returns> true if no more event handlers for this method should be called.
        /// </returns>
        bool Done
        {
            get;

        }
        /// <summary> Get return value at end of all the iterations
        /// 
        /// </summary>
        /// <returns> null if no return value is required
        /// </returns>
        object ReturnValue
        {
            get;

        }
        /// <summary> Execute the event handler method.  If Object is not null, do not 
        /// iterate further through the handler chain.
        /// If appropriate, the returned Object will be the return value.
        /// 
        /// </summary>
        /// <param name="handler">call the appropriate method on this handler
        /// </param>
        /// <exception cref="Exception">generic exception potentially thrown by event handlers
        /// </exception>
        void Execute(IEventHandler handler);
    }
}
