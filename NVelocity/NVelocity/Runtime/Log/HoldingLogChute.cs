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

    /// <summary>  Pre-Init logger.  I believe that this was suggested by
    /// Carsten Ziegeler <cziegeler@sundn.de> and
    /// Jeroen C. van Gelderen.  If this isn't correct, let me
    /// know as this was a good idea...
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  <a href="mailto:nbubna@apache.org">Nathan Bubna</a>
    /// </author>
    /// <version>  $Id: HoldingLogChute.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class HoldingLogChute : ILogChute
    {
        private System.Collections.ArrayList pendingMessages = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
        private volatile bool transferring = false;

        /// <seealso cref="LogChute.Init(IRuntimeServices)">
        /// </seealso>
        public virtual void Init(IRuntimeServices rs)
        {
        }

        /// <summary> Logs messages. All we do is store them until 'later'.
        /// 
        /// </summary>
        /// <param name="level">severity level
        /// </param>
        /// <param name="message">complete Error message
        /// </param>
   
        public virtual void Log(int level, System.String message)
        {
            lock (this)
            {
                if (!transferring)
                {
                    System.Object[] data = new System.Object[2];
                    data[0] = (System.Int32)level;
                    data[1] = message;
                    pendingMessages.Add(data);
                }
            }
        }

        /// <summary> Logs messages and errors. All we do is store them until 'later'.
        /// 
        /// </summary>
        /// <param name="level">severity level
        /// </param>
        /// <param name="message">complete Error message
        /// </param>
        /// <param name="t">the accompanying java.lang.Throwable
        /// </param>
      
        public virtual void Log(int level, System.String message, System.Exception t)
        {
            lock (this)
            {
                if (!transferring)
                {
                    System.Object[] data = new System.Object[3];
                    data[0] = (System.Int32)level;
                    data[1] = message;
                    data[2] = t;
                    pendingMessages.Add(data);
                }
            }
        }

        /// <seealso cref="org.apache.velocity.runtime.Log.LogChute.IsLevelEnabled(int)">
        /// </seealso>
        public virtual bool IsLevelEnabled(int level)
        {
            return true;
        }

        /// <summary> Dumps the Log messages this chute is holding into a new chute</summary>
        /// <param name="newChute">
        /// </param>
       
        public virtual void TransferTo(ILogChute newChute)
        {
            lock (this)
            {
                if (!transferring && !(pendingMessages.Count == 0))
                {
                    // let the other methods know what's up
                    transferring = true;

                    // iterate and Log each individual message...
                 
                    for (System.Collections.IEnumerator i = pendingMessages.GetEnumerator(); i.MoveNext(); )
                    {
                       
                        System.Object[] data = (System.Object[])i.Current;
                        int level = ((System.Int32)data[0]);
                        System.String message = (System.String)data[1];
                        if (data.Length == 2)
                        {
                            newChute.Log(level, message);
                        }
                        else
                        {
                           
                            newChute.Log(level, message, (System.Exception)data[2]);
                        }
                    }
                }
            }
        }
    }
}
