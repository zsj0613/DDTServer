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

namespace NVelocity.App.Event.Implement
{
    using Runtime;
    using Util;

    /// <summary> Simple event handler that renders method exceptions in the page
    /// rather than throwing the exception.  Useful for debugging.
    /// 
    /// <P>By default this event handler renders the exception name only.
    /// To include both the exception name and the message, set the property
    /// <code>eventhandler.methodexception.message</code> to <code>true</code>.  To render
    /// the stack Trace, set the property <code>eventhandler.methodexception.stacktrace</code>
    /// to <code>true</code>.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <version>  $Id: PrintExceptions.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class PrintExceptions : IMethodExceptionEventHandler, IRuntimeServicesAware
    {

        private static string SHOW_MESSAGE = "eventhandler.methodexception.message";
        private static string SHOW_STACK_TRACE = "eventhandler.methodexception.stacktrace";

        /// <summary>Reference to the runtime service </summary>
        private IRuntimeServices rs = null;

        /// <summary> Render the method exception, and optionally the exception message and stack Trace.
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
        public virtual object MethodException(System.Type claz, string method, System.Exception e)
        {
            bool showMessage = rs.GetBoolean(SHOW_MESSAGE, false);
            bool showStackTrace = rs.GetBoolean(SHOW_STACK_TRACE, false);

            System.Text.StringBuilder st;
            if (showMessage && showStackTrace)
            {
                st = new System.Text.StringBuilder(200);
            
                st.Append(e.GetType().FullName).Append("\n");
               
                st.Append(e.Message).Append("\n");
                st.Append(GetStackTrace(e));
            }
            else if (showMessage)
            {
                st = new System.Text.StringBuilder(50);
               
                st.Append(e.GetType().FullName).Append("\n");
               
                st.Append(e.Message).Append("\n");
            }
            else if (showStackTrace)
            {
                st = new System.Text.StringBuilder(200);
             
                st.Append(e.GetType().FullName).Append("\n");
                st.Append(GetStackTrace(e));
            }
            else
            {
                st = new System.Text.StringBuilder(15);
               
                st.Append(e.GetType().FullName).Append("\n");
            }

            return st.ToString();
        }


       
        private static string GetStackTrace(System.Exception throwable)
        {
            System.IO.StringWriter stackTraceWriter = new System.IO.StringWriter();

            try
            {

                SupportClass.WriteStackTrace(throwable, stackTraceWriter);
                stackTraceWriter.Flush();
                return stackTraceWriter.ToString();
            }
            finally
            {
                if (stackTraceWriter != null)
                {
                  
                    stackTraceWriter.Close();
                }
            }
        }


        /// <seealso cref="org.apache.velocity.util.RuntimeServicesAware.setRuntimeServices(org.apache.velocity.runtime.RuntimeServices)">
        /// </seealso>
        public virtual void SetRuntimeServices(IRuntimeServices rs)
        {
            this.rs = rs;
        }
    }
}
