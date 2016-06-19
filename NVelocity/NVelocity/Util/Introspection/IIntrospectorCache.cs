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
    /// <summary> The introspector cache API definition.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:henning@apache.org">Henning P. Schmiedehausen</a>
    /// </author>
    /// <version>  $Id: IntrospectorCache.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public interface IIntrospectorCache
    {

        /// <summary> Clears the internal cache.</summary>
        void Clear();

        /// <summary> Lookup a given Class object in the cache. If it does not exist, 
        /// check whether this is due to a class change and purge the caches
        /// eventually.
        /// 
        /// </summary>
        /// <param name="c">The class to look up.
        /// </param>
        /// <returns> A ClassMap object or null if it does not exist in the cache.
        /// </returns>
        ClassMap Get(System.Type c);

        /// <summary> Creates a class map for specific class and registers it in the
        /// cache.  Also adds the qualified name to the name-&gt;class map
        /// for later Classloader change detection.
        /// 
        /// </summary>
        /// <param name="c">The class for which the class map gets generated.
        /// </param>
        /// <returns> A ClassMap object.
        /// </returns>
        ClassMap Put(System.Type c);
    }
}
