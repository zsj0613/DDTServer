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
namespace NVelocity.Util.Introspection
{
    /// <summary>  Method used for regular method invocation
    /// 
    /// $foo.bar()
    /// 
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: VelMethod.java 510625 2007-02-22 19:06:28Z nbubna $
    /// </version>
    public interface IVelMethod
    {
        /// <summary>  specifies if this VelMethod is cacheable and able to be
        /// reused for this class of object it was returned for
        /// 
        /// </summary>
        /// <returns> true if can be reused for this class, false if not
        /// </returns>
        bool Cacheable
        {
            get;

        }
        /// <summary>  returns the method name used</summary>
        /// <returns> The method name used
        /// </returns>
        string MethodName
        {
            get;

        }
        /// <summary>  returns the return type of the method invoked</summary>
        /// <returns> The return type of the method invoked
        /// </returns>
        System.Type ReturnType
        {
            get;

        }
        /// <summary>  invocation method - called when the method invocation should be
        /// performed and a value returned
        /// </summary>
        /// <param name="instance">
        /// </param>
        /// <param name="parameters">
        /// </param>
        /// <returns> The resulting object.
        /// </returns>
        /// <throws>  Exception </throws>
        object Invoke(object o, object[] parameters);
    }
}