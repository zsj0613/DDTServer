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

namespace NVelocity.Runtime.Log
{
    /// <summary> This is a wrapper around a Log object, that can Add a prefix to Log messages
    /// and also turn logging on and off dynamically. It is mainly used to control the
    /// logging of VelociMacro generation messages but is actually generic enough code.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:henning@apache.org">Henning P. Schmiedehausen</a>
    /// </author>
    /// <version>  $Id: LogDisplayWrapper.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class LogDisplayWrapper : Log
    {
        /// <summary>The prefix to record with every Log message </summary>
        private System.String prefix;

        /// <summary>Log messages only if true </summary>
        private bool outputMessages;

        /// <summary>The LogMessage object we wrap </summary>

        private Log log;

        /// <summary> Create a new LogDisplayWrapper</summary>
        /// <param name="Log">The LogMessage object to wrap.
        /// </param>
        /// <param name="prefix">The prefix to record with all messages.
        /// </param>
        /// <param name="outputMessages">True when messages should actually Get logged.
        /// </param>
        public LogDisplayWrapper(Log log, System.String prefix, bool outputMessages)
            : base(log.GetLogChute())
        {
            this.log = log;
            this.prefix = prefix;
            this.outputMessages = outputMessages;
        }

        /// <summary> make sure that we always use the right LogChute Object</summary>
        protected internal override ILogChute GetLogChute()
        {
            return log.GetLogChute();
        }

        /// <seealso cref="LogMessage.Log(int, Object)">
        /// </seealso>
        protected internal override void LogMessage(int level, System.Object message)
        {
            Log(outputMessages, level, message);
        }

        protected internal virtual void Log(bool doLogging, int level, System.Object message)
        {
            if (doLogging)
            {
                GetLogChute().Log(level, prefix + System.Convert.ToString(message));
            }
        }

        /// <seealso cref="LogMessage.Log(int, Object, Throwable)">
        /// </seealso>
        protected internal override void LogMessage(int level, System.Object message, System.Exception t)
        {
            Log(outputMessages, level, message);
        }

        protected internal virtual void Log(bool doLogging, int level, System.Object message, System.Exception t)
        {
            if (doLogging)
            {
                GetLogChute().Log(level, prefix + System.Convert.ToString(message), t);
            }
        }

        /// <summary> LogMessage a Trace message.</summary>
        /// <param name="doLogging">LogMessage only if this parameter is true.
        /// </param>
        /// <param name="message">
        /// </param>
        public virtual void Trace(bool doLogging, System.Object message)
        {
            Log(doLogging, LogChute_Fields.TRACE_ID, message);
        }

        /// <summary> LogMessage a Trace message and accompanying Throwable.</summary>
        /// <param name="doLogging">LogMessage only if this parameter is true.
        /// </param>
        /// <param name="message">
        /// </param>
        /// <param name="t">
        /// </param>
        public virtual void Trace(bool doLogging, System.Object message, System.Exception t)
        {
            Log(doLogging, LogChute_Fields.TRACE_ID, message, t);
        }

        /// <summary> LogMessage a Debug message.</summary>
        /// <param name="doLogging">LogMessage only if this parameter is true.
        /// </param>
        /// <param name="message">
        /// </param>
        public virtual void Debug(bool doLogging, System.Object message)
        {
            Log(doLogging, LogChute_Fields.DEBUG_ID, message);
        }

        /// <summary> LogMessage a Debug message and accompanying Throwable.</summary>
        /// <param name="doLogging">LogMessage only if this parameter is true.
        /// </param>
        /// <param name="message">
        /// </param>
        /// <param name="t">
        /// </param>
        public virtual void Debug(bool doLogging, System.Object message, System.Exception t)
        {
            Log(doLogging, LogChute_Fields.DEBUG_ID, message, t);
        }

        /// <summary> LogMessage an Info message.</summary>
        /// <param name="doLogging">LogMessage only if this parameter is true.
        /// </param>
        /// <param name="message">
        /// </param>
        public virtual void Info(bool doLogging, System.Object message)
        {
            Log(doLogging, LogChute_Fields.INFO_ID, message);
        }

        /// <summary> LogMessage an Info message and accompanying Throwable.</summary>
        /// <param name="doLogging">LogMessage only if this parameter is true.
        /// </param>
        /// <param name="message">
        /// </param>
        /// <param name="t">
        /// </param>
        public virtual void Info(bool doLogging, System.Object message, System.Exception t)
        {
            Log(doLogging, LogChute_Fields.INFO_ID, message, t);
        }

        /// <summary> LogMessage a warning message.</summary>
        /// <param name="doLogging">LogMessage only if this parameter is true.
        /// </param>
        /// <param name="message">
        /// </param>
        public virtual void Warn(bool doLogging, System.Object message)
        {
            Log(doLogging, LogChute_Fields.WARN_ID, message);
        }

        /// <summary> LogMessage a warning message and accompanying Throwable.</summary>
        /// <param name="doLogging">LogMessage only if this parameter is true.
        /// </param>
        /// <param name="message">
        /// </param>
        /// <param name="t">
        /// </param>
        public virtual void Warn(bool doLogging, System.Object message, System.Exception t)
        {
            Log(doLogging, LogChute_Fields.WARN_ID, message, t);
        }

        /// <summary> LogMessage an Error message.</summary>
        /// <param name="doLogging">LogMessage only if this parameter is true.
        /// </param>
        /// <param name="message">
        /// </param>
        public virtual void Error(bool doLogging, System.Object message)
        {
            Log(doLogging, LogChute_Fields.ERROR_ID, message);
        }

        /// <summary> LogMessage an Error message and accompanying Throwable.</summary>
        /// <param name="doLogging">LogMessage only if this parameter is true.
        /// </param>
        /// <param name="message">
        /// </param>
        /// <param name="t">
        /// </param>
        public virtual void Error(bool doLogging, System.Object message, System.Exception t)
        {
            Log(doLogging, LogChute_Fields.ERROR_ID, message, t);
        }
    }
}
