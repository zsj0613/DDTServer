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
    /// <summary>  Logger used in case of failure. Does nothing.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  <a href="mailto:nbubna@optonline.net">Nathan Bubna.</a>
    /// </author>
    /// <version>  $Id: NullLogChute.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class NullLogChute : ILogChute
    {

        /// <seealso cref="org.apache.velocity.runtime.Log.LogChute.Init(org.apache.velocity.runtime.RuntimeServices)">
        /// </seealso>
        public virtual void Init(IRuntimeServices rs)
        {
        }

        /// <summary> logs messages to the great Garbage Collector in the sky
        /// 
        /// </summary>
        /// <param name="level">severity level
        /// </param>
        /// <param name="message">complete Error message
        /// </param>
        public virtual void Log(int level, System.String message)
        {
        }

        /// <summary> logs messages and their accompanying Throwables
        /// to the great Garbage Collector in the sky
        /// 
        /// </summary>
        /// <param name="level">severity level
        /// </param>
        /// <param name="message">complete Error message
        /// </param>
        /// <param name="t">the java.lang.Throwable
        /// </param>
        public virtual void Log(int level, System.String message, System.Exception t)
        {
        }

        /// <seealso cref="org.apache.velocity.runtime.Log.LogChute.IsLevelEnabled(int)">
        /// </seealso>
        public virtual bool IsLevelEnabled(int level)
        {
            return false;
        }
    }
}
