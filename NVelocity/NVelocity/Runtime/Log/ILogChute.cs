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
    /// <summary> Base interface that logging systems need to implement. This
    /// is the blessed descendant of the old LogSystem interface.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jon@latchkey.com">Jon S. Stevens</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  <a href="mailto:nbubna@apache.org">Nathan Bubna</a>
    /// </author>
    /// <version>  $Id: LogChute.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public struct LogChute_Fields
    {
        /// <summary>Prefix string for Trace messages. </summary>
        public const string TRACE_PREFIX = " [trace] ";
        /// <summary>Prefix string for Debug messages. </summary>
        public const string DEBUG_PREFIX = " [debug] ";
        /// <summary>Prefix string for Info messages. </summary>
        public const string INFO_PREFIX = "  [info] ";
        /// <summary>Prefix string for Warn messages. </summary>
        public const string WARN_PREFIX = "  [warn] ";
        /// <summary>Prefix string for Error messages. </summary>
        public const string ERROR_PREFIX = " [error] ";
        /// <summary>ID for Trace messages. </summary>
        public const int TRACE_ID = -1;
        /// <summary>ID for Debug messages. </summary>
        public const int DEBUG_ID = 0;
        /// <summary>ID for Info messages. </summary>
        public const int INFO_ID = 1;
        /// <summary>ID for warning messages. </summary>
        public const int WARN_ID = 2;
        /// <summary>ID for Error messages. </summary>
        public const int ERROR_ID = 3;
    }
    public interface ILogChute
    {
        /// <summary> Initializes this LogChute.</summary>
        /// <param name="rs">
        /// </param>
        /// <throws>  Exception </throws>
        void Init(IRuntimeServices rs);

        /// <summary> Send a Log message from Velocity.</summary>
        /// <param name="level">
        /// </param>
        /// <param name="message">
        /// </param>
        void Log(int level, string message);

        /// <summary> Send a Log message from Velocity along with an exception or Error</summary>
        /// <param name="level">
        /// </param>
        /// <param name="message">
        /// </param>
        /// <param name="t">
        /// </param>
        void Log(int level, string message, System.Exception t);

        /// <summary> Tell whether or not a Log level is enabled.</summary>
        /// <param name="level">
        /// </param>
        /// <returns> True if a level is enabled.
        /// </returns>
        bool IsLevelEnabled(int level);
    }
}
