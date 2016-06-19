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
    /// <summary> 'Federated' introspection/reflection interface to allow the introspection
    /// behavior in Velocity to be customized.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@apache.org">Geir Magusson Jr.</a>
    /// </author>
    /// <version>  $Id: Uberspect.java 463298 2006-10-12 16:10:32Z henning $
    /// </version>
    public interface IUberspect
    {
        /// <summary>  Initializer - will be called before use</summary>
        /// <throws>  Exception </throws>
        void Init();

        /// <summary>  To support iteratives - #foreach()</summary>
        /// <param name="obj">
        /// </param>
        /// <param name="Info">
        /// </param>
        /// <returns> An Iterator.
        /// </returns>
        /// <throws>  Exception </throws>
        System.Collections.IEnumerator GetIterator(object obj, Info info);

        /// <summary>  Returns a general method, corresponding to $foo.bar( $woogie )</summary>
        /// <param name="obj">
        /// </param>
        /// <param name="method">
        /// </param>
        /// <param name="args">
        /// </param>
        /// <param name="Info">
        /// </param>
        /// <returns> A Velocity Method.
        /// </returns>
        /// <throws>  Exception </throws>
        IVelMethod GetMethod(object obj, string method, object[] args, Info info);

        /// <summary> Property getter - returns VelPropertyGet appropos for #set($foo = $bar.woogie)</summary>
        /// <param name="obj">
        /// </param>
        /// <param name="identifier">
        /// </param>
        /// <param name="Info">
        /// </param>
        /// <returns> A Velocity Getter.
        /// </returns>
        /// <throws>  Exception </throws>
        IVelPropertyGet GetPropertyGet(object obj, string identifier, Info info);

        /// <summary> Property setter - returns VelPropertySet appropos for #set($foo.bar = "geir")</summary>
        /// <param name="obj">
        /// </param>
        /// <param name="identifier">
        /// </param>
        /// <param name="arg">
        /// </param>
        /// <param name="Info">
        /// </param>
        /// <returns> A Velocity Setter.
        /// </returns>
        /// <throws>  Exception </throws>
        IVelPropertySet GetPropertySet(object obj, string identifier, object arg, Info info);
    }
}