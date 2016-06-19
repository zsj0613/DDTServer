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

namespace NVelocity.Exception
{
    using System;

    /// <summary> Application-level exception thrown when macro calls within macro calls
    /// exceeds the maximum allowed depth. The maximum allowable depth is given
    /// in the configuration as velocimacro.max.depth.
    /// </summary>
    /// <since> 1.6
    /// </since>
    [Serializable]
    public class MacroOverflowException : VelocityException
    {

        /// <param name="exceptionMessage">The message to register.
        /// </param>
        public MacroOverflowException(string exceptionMessage)
            : base(exceptionMessage)
        {
        }

        /// <param name="exceptionMessage">The message to register.
        /// </param>
        /// <param name="wrapped">A throwable object that caused the Exception.
        /// </param>
        public MacroOverflowException(string exceptionMessage, System.Exception wrapped)
            : base(exceptionMessage, wrapped)
        {
        }

        /// <param name="wrapped">A throwable object that caused the Exception.
        /// </param>
        public MacroOverflowException(System.Exception wrapped)
            : base(string.Empty, wrapped)
        {
        }
    }
}
