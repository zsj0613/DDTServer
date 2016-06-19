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
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Runtime.Log;
    /// <summary> This is the internal introspector cache implementation.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:henning@apache.org">Henning P. Schmiedehausen</a>
    /// </author>
    /// <version>  $Id: IntrospectorCacheImpl.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public sealed class IntrospectorCacheImpl : IIntrospectorCache
    {
        /// <summary> define a public string so that it can be looked for if interested</summary>
        public const string CACHEDUMP_MSG = "IntrospectorCache detected classloader change. Dumping cache.";

        /// <summary>Class logger </summary>
        private Log log;

        /// <summary> Holds the method maps for the classes we know about. Map: Class --&gt; ClassMap object.</summary>
        private IDictionary<Type, ClassMap> classMapCache = new Dictionary<Type, ClassMap>();

        /// <summary> C'tor</summary>
        public IntrospectorCacheImpl(Log log)
        {
            this.log = log;
        }

        /// <summary> Clears the internal cache.</summary>
        public void Clear()
        {
            classMapCache.Clear();
        }

        /// <summary> Lookup a given Class object in the cache. If it does not exist, 
        /// check whether this is due to a class change and purge the caches
        /// eventually.
        /// 
        /// </summary>
        /// <param name="c">The class to look up.
        /// </param>
        /// <returns> A ClassMap object or null if it does not exist in the cache.
        /// </returns>
        public ClassMap Get(System.Type c)
        {
            if (c == null)
            {
                throw new System.ArgumentException("class is null!");
            }

            ClassMap classMap = null;

            classMapCache.TryGetValue(c, out classMap);

            return classMap;
        }

        /// <summary> Creates a class map for specific class and registers it in the
        /// cache.  Also adds the qualified name to the name-&gt;class map
        /// for later Classloader change detection.
        /// 
        /// </summary>
        /// <param name="c">The class for which the class map gets generated.
        /// </param>
        /// <returns> A ClassMap object.
        /// </returns>
        public ClassMap Put(System.Type c)
        {
            ClassMap classMap = new ClassMap(c, log);

            classMapCache[c] = classMap;

            return classMap;
        }
    }
}
