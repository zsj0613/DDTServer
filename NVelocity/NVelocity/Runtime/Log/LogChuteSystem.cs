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
    /// <summary> Wrapper to make user's custom LogSystem implementations work
    /// with the new LogChute setup.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:nbubna@apache.org">Nathan Bubna</a>
    /// </author>
    /// <version>  $Id: LogChuteSystem.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class LogChuteSystem : ILogChute
    {

        private ILogSystem logSystem;

        /// <summary> Only classes in this package should be creating this.
        /// Users should not have to mess with this class.
        /// </summary>
        /// <param name="wrapMe">
        /// </param>
        protected internal LogChuteSystem(ILogSystem wrapMe)
        {
            this.logSystem = wrapMe;
        }

        /// <seealso cref="org.apache.velocity.runtime.Log.LogChute.Init(org.apache.velocity.runtime.RuntimeServices)">
        /// </seealso>
        public virtual void Init(IRuntimeServices rs)
        {
            logSystem.Init(rs);
        }

        /// <seealso cref="org.apache.velocity.runtime.Log.LogChute.Log(int, java.lang.String)">
        /// </seealso>
        public virtual void Log(int level, System.String message)
        {
            logSystem.LogVelocityMessage(level, message);
        }

        /// <summary> First passes off the message at the specified level,
        /// then passes off stack Trace of the Throwable as a
        /// 2nd message at the same level.
        /// </summary>
        /// <param name="level">
        /// </param>
        /// <param name="message">
        /// </param>
        /// <param name="t">
        /// </param>
        public virtual void Log(int level, System.String message, System.Exception t)
        {
            logSystem.LogVelocityMessage(level, message);
            logSystem.LogVelocityMessage(level, t.Message);
        }

        /// <seealso cref="org.apache.velocity.runtime.Log.LogChute.IsLevelEnabled(int)">
        /// </seealso>
        public virtual bool IsLevelEnabled(int level)
        {
            return true;
        }
    }
}
