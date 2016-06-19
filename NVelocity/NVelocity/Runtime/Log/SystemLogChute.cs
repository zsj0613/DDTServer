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
    /// <summary> Logger used when no other is configured.  By default, all messages
    /// will be printed to the System.err output stream.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:nbubna@apache.org">Nathan Bubna</a>
    /// </author>
    /// <version>  $Id: SystemLogChute.java 718424 2008-11-17 22:50:43Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class SystemLogChute : ILogChute
    {
        /// <summary> Returns the current minimum level at which messages will be printed.</summary>
        /// <summary> Set the minimum level at which messages will be printed.</summary>
        virtual public int EnabledLevel
        {
            get
            {
                return this.enabled;
            }

            set
            {
                this.enabled = value;
            }

        }
   
        /// <summary> Returns the current minimum level at which messages will be printed
        /// to System.err instead of System.out.
        /// </summary>
        /// <summary> Set the minimum level at which messages will be printed to System.err
        /// instead of System.out.
        /// </summary>
        virtual public int SystemErrLevel
        {
            get
            {
                return this.errLevel;
            }

            set
            {
                this.errLevel = value;
            }

        }
        public const string RUNTIME_LOG_LEVEL_KEY = "runtime.log.logsystem.system.level";
        public const string RUNTIME_LOG_SYSTEM_ERR_LEVEL_KEY = "runtime.log.logsystem.system.err.level";

        private int enabled = LogChute_Fields.WARN_ID;
        private int errLevel = LogChute_Fields.TRACE_ID;

        public virtual void Init(IRuntimeServices rs)
        {
            // look for a level config property
            string level = (string)rs.GetProperty(RUNTIME_LOG_LEVEL_KEY);
            if (level != null)
            {
                // and set it accordingly
                EnabledLevel = toLevel(level);
            }

            // look for an errLevel config property
            string errLevel = (string)rs.GetProperty(RUNTIME_LOG_SYSTEM_ERR_LEVEL_KEY);
            if (errLevel != null)
            {
                SystemErrLevel = toLevel(errLevel);
            }
        }

        protected internal virtual int toLevel(string level)
        {
            if (level.ToUpper().Equals("debug".ToUpper()))
            {
                return LogChute_Fields.DEBUG_ID;
            }
            else if (level.ToUpper().Equals("info".ToUpper()))
            {
                return LogChute_Fields.INFO_ID;
            }
            else if (level.ToUpper().Equals("warn".ToUpper()))
            {
                return LogChute_Fields.WARN_ID;
            }
            else if (level.ToUpper().Equals("error".ToUpper()))
            {
                return LogChute_Fields.ERROR_ID;
            }
            else
            {
                return LogChute_Fields.TRACE_ID;
            }
        }

        protected internal virtual string getPrefix(int level)
        {
            switch (level)
            {

                case LogChute_Fields.WARN_ID:
                    return LogChute_Fields.WARN_PREFIX;

                case LogChute_Fields.DEBUG_ID:
                    return LogChute_Fields.DEBUG_PREFIX;

                case LogChute_Fields.TRACE_ID:
                    return LogChute_Fields.TRACE_PREFIX;

                case LogChute_Fields.ERROR_ID:
                    return LogChute_Fields.ERROR_PREFIX;

                case LogChute_Fields.INFO_ID:
                default:
                    return LogChute_Fields.INFO_PREFIX;
            }
        }

        /// <summary> Logs messages to either std.out or std.err
        /// depending on their severity.
        /// 
        /// </summary>
        /// <param name="level">severity level
        /// </param>
        /// <param name="message">complete Error message
        /// </param>
        public virtual void Log(int level, string message)
        {
            // pass it off
            Log(level, message, null);
        }

        /// <summary> Logs messages to the system console so long as the specified level
        /// is equal to or greater than the level this LogChute is enabled for.
        /// If the level is equal to or greater than LogChute.ERROR_ID, 
        /// messages will be printed to System.err. Otherwise, they will be 
        /// printed to System.out. If a java.lang.Throwable accompanies the 
        /// message, it's stack Trace will be printed to the same stream
        /// as the message.
        /// 
        /// </summary>
        /// <param name="level">severity level
        /// </param>
        /// <param name="message">complete Error message
        /// </param>
        /// <param name="t">the java.lang.Throwable
        /// </param>
        public virtual void Log(int level, string message, System.Exception t)
        {
            if (!IsLevelEnabled(level))
            {
                return;
            }

            string prefix = getPrefix(level);
            if (level >= this.errLevel)
            {
                System.IO.StreamWriter temp_writer;
                temp_writer = new System.IO.StreamWriter(System.Console.OpenStandardError(), System.Console.Error.Encoding);
                temp_writer.AutoFlush = true;
                write(temp_writer, prefix, message, t);
            }
            else
            {
                System.IO.StreamWriter temp_writer2;
                temp_writer2 = new System.IO.StreamWriter(System.Console.OpenStandardOutput(), System.Console.Out.Encoding);
                temp_writer2.AutoFlush = true;
                write(temp_writer2, prefix, message, t);
            }
        }

        protected internal  virtual void write(System.IO.TextWriter stream, string prefix, string message, System.Exception t)
        {
            stream.Write(prefix);
            stream.WriteLine(message);
            if (t != null)
            {
                stream.WriteLine(t.Message);
                SupportClass.WriteStackTrace(t, stream);
            }
        }

        /// <summary> This will return true if the specified level
        /// is equal to or higher than the level this
        /// LogChute is enabled for.
        /// </summary>
        public virtual bool IsLevelEnabled(int level)
        {
            return (level >= this.enabled);
        }
    }
}
