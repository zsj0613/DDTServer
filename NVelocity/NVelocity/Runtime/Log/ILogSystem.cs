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
    /// <summary> Old base interface that old logging systems needed to implement.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jon@latchkey.com">Jon S. Stevens</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <deprecated> Use LogChute instead!
    /// </deprecated>
    /// <version>  $Id: LogSystem.java 463298 2006-10-12 16:10:32Z henning $
    /// </version>

    public struct LogSystem_Fields
    {
        /// <deprecated> This is unused and meaningless
        /// </deprecated>
        public readonly static bool DEBUG_ON = true;
        /// <summary> ID for Debug messages.</summary>
        public readonly static int DEBUG_ID = 0;
        /// <summary> ID for Info messages.</summary>
        public readonly static int INFO_ID = 1;
        /// <summary> ID for warning messages.</summary>
        public readonly static int WARN_ID = 2;
        /// <summary> ID for Error messages.</summary>
        public readonly static int ERROR_ID = 3;
    }

    public interface ILogSystem
    {
        /// <summary> Initializes this LogSystem.</summary>
        /// <param name="rs">
        /// </param>
        /// <throws>  Exception </throws>
        void Init(IRuntimeServices rs);

        /// <param name="level">
        /// </param>
        /// <param name="message">
        /// </param>
        /// <deprecated> Use Log(level, message).
        /// </deprecated>
        void LogVelocityMessage(int level, System.String message);
    }
}