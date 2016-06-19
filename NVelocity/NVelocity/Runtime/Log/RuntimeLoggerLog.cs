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

    /// <summary> A temporary RuntimeLogger wrapper to make the deprecation
    /// of UberspectLoggable.setRuntimeLogger(RuntimeLogger) feasible.
    /// This overrides all LogMessage methods, either throwing
    /// UnsupportedOperationExceptions or passing things off to the
    /// theoretical RuntimeLogger used to create it.  Oh, and all the
    /// is<Level>Enabled() methods return true.  Of course, ideally
    /// there is no one out there who actually created their own
    /// RuntimeLogger instance to use with UberspectLoggable.setRuntimeLogger()
    /// and this class will therefore never be used.  But it's here just in case.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:nbubna@apache.org">Nathan Bubna</a>
    /// </author>
    /// <version>  $Id: RuntimeLoggerLog.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <deprecated> This will be removed along with the RuntimeLogger interface.
    /// </deprecated>
    /// <since> 1.5
    /// </since>
    public class RuntimeLoggerLog : Log
    {
        /// <seealso cref="org.apache.velocity.runtime.Log.LogMessage.isTraceEnabled()">
        /// </seealso>
        override public bool TraceEnabled
        {
            get
            {
                return true;
            }

        }
        /// <seealso cref="org.apache.velocity.runtime.Log.LogMessage.isDebugEnabled()">
        /// </seealso>
        override public bool DebugEnabled
        {
            get
            {
                return true;
            }

        }
        /// <seealso cref="org.apache.velocity.runtime.Log.LogMessage.isInfoEnabled()">
        /// </seealso>
        override public bool InfoEnabled
        {
            get
            {
                return true;
            }

        }
        /// <seealso cref="org.apache.velocity.runtime.Log.LogMessage.isWarnEnabled()">
        /// </seealso>
        override public bool WarnEnabled
        {
            get
            {
                return true;
            }

        }
        /// <seealso cref="org.apache.velocity.runtime.Log.LogMessage.isErrorEnabled()">
        /// </seealso>
        override public bool ErrorEnabled
        {
            get
            {
                return true;
            }

        }

        private IRuntimeLogger rlog;

        /// <summary> Creates a new LogMessage that wraps a PrimordialLogChute.</summary>
        /// <param name="rlog">
        /// </param>
        public RuntimeLoggerLog(IRuntimeLogger rlog)
        {
            if (rlog == null)
            {
                throw new System.NullReferenceException("RuntimeLogger cannot be null!");
            }
            this.rlog = rlog;
        }

        /// <seealso cref="org.apache.velocity.runtime.Log.LogMessage.setLogChute(org.apache.velocity.runtime.Log.LogChute)">
        /// </seealso>
        protected internal override void SetLogChute(ILogChute newLogChute)
        {
            throw new System.NotSupportedException("RuntimeLoggerLog does not support this method.");
        }

        /// <seealso cref="org.apache.velocity.runtime.Log.LogMessage.getLogChute()">
        /// </seealso>
        protected internal override ILogChute GetLogChute()
        {
            throw new System.NotSupportedException("RuntimeLoggerLog does not support this method.");
        }

        /// <param name="showStacks">
        /// </param>
        protected internal virtual void SetShowStackTraces(bool showStacks)
        {
            throw new System.NotSupportedException("RuntimeLoggerLog does not support this method.");
        }

        /// <returns> True if Stack traces should be shown.
        /// </returns>
        public virtual bool GetShowStackTraces()
        {
            throw new System.NotSupportedException("RuntimeLoggerLog does not support this method.");
        }

        /// <seealso cref="org.apache.velocity.runtime.Log.LogMessage.Trace(java.lang.Object)">
        /// </seealso>
        public override void Trace(System.Object message)
        {
            Debug(message);
        }

        /// <seealso cref="org.apache.velocity.runtime.Log.LogMessage.Trace(java.lang.Object, java.lang.Throwable)">
        /// </seealso>
        public override void Trace(System.Object message, System.Exception t)
        {
            Debug(message, t);
        }

        /// <seealso cref="org.apache.velocity.runtime.Log.LogMessage.Debug(java.lang.Object)">
        /// </seealso>
        public override void Debug(System.Object message)
        {
            rlog.Debug(message);
        }

        /// <seealso cref="org.apache.velocity.runtime.Log.LogMessage.Debug(java.lang.Object, java.lang.Throwable)">
        /// </seealso>
        public override void Debug(System.Object message, System.Exception t)
        {
            rlog.Debug(message);
            rlog.Debug(t);
        }

        /// <seealso cref="org.apache.velocity.runtime.Log.LogMessage.Info(java.lang.Object)">
        /// </seealso>
        public override void Info(System.Object message)
        {
            rlog.Info(message);
        }

        /// <seealso cref="org.apache.velocity.runtime.Log.LogMessage.Info(java.lang.Object, java.lang.Throwable)">
        /// </seealso>
        public override void Info(System.Object message, System.Exception t)
        {
            rlog.Info(message);
            rlog.Info(t);
        }

        /// <seealso cref="org.apache.velocity.runtime.Log.LogMessage.Warn(java.lang.Object)">
        /// </seealso>
        public override void Warn(System.Object message)
        {
            rlog.Warn(message);
        }

        /// <seealso cref="org.apache.velocity.runtime.Log.LogMessage.Warn(java.lang.Object, java.lang.Throwable)">
        /// </seealso>
        public override void Warn(System.Object message, System.Exception t)
        {
            rlog.Warn(message);
            rlog.Warn(t);
        }

        /// <seealso cref="org.apache.velocity.runtime.Log.LogMessage.Error(java.lang.Object)">
        /// </seealso>
        public override void Error(System.Object message)
        {
            rlog.Error(message);
        }

        /// <seealso cref="org.apache.velocity.runtime.Log.LogMessage.Error(java.lang.Object, java.lang.Throwable)">
        /// </seealso>
        public override void Error(System.Object message, System.Exception t)
        {
            rlog.Error(message);
            rlog.Error(t);
        }
    }
}
