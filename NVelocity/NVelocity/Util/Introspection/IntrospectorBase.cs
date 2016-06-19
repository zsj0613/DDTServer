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
    using System.Reflection;

    using Runtime.Log;

    /// <summary> Lookup a a Method object for a particular class given the name of a method
    /// and its parameters.
    /// 
    /// The first time the Introspector sees a
    /// class it creates a class method map for the
    /// class in question. Basically the class method map
    /// is a Hashtable where Method objects are keyed by a
    /// concatenation of the method name and the names of
    /// classes that make up the parameters.
    /// 
    /// For example, a method with the following signature:
    /// 
    /// public void method(String a, StringBuffer b)
    /// 
    /// would be mapped by the key:
    /// 
    /// "method" + "java.lang.String" + "java.lang.StringBuffer"
    /// 
    /// This mapping is performed for all the methods in a class
    /// and stored for.
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:bob@werken.com">Bob McWhirter</a>
    /// </author>
    /// <author>  <a href="mailto:szegedia@freemail.hu">Attila Szegedi</a>
    /// </author>
    /// <author>  <a href="mailto:paulo.gaspar@krankikom.de">Paulo Gaspar</a>
    /// </author>
    /// <author>  <a href="mailto:henning@apache.org">Henning P. Schmiedehausen</a>
    /// </author>
    /// <version>  $Id: IntrospectorBase.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    public abstract class IntrospectorBase
    {
        /// <summary> Return the internal IntrospectorCache object.
        /// 
        /// </summary>
        /// <returns> The internal IntrospectorCache object.
        /// </returns>
        /// <since> 1.5
        /// </since>
        protected virtual  internal IIntrospectorCache IntrospectorCache
        {
            get
            {
                return introspectorCache;
            }

        }
        /// <summary>Class logger </summary>
        protected internal Log log;

        /// <summary>The Introspector Cache </summary>
        private IIntrospectorCache introspectorCache;

        /// <summary> C'tor.</summary>
        protected internal IntrospectorBase(Log log)
        {
            this.log = log;
            introspectorCache = new IntrospectorCacheImpl(log); // TODO: Load that from properties.
        }

        /// <summary> Gets the method defined by <code>name</code> and
        /// <code>params</code> for the Class <code>c</code>.
        /// 
        /// </summary>
        /// <param name="c">Class in which the method search is taking place
        /// </param>
        /// <param name="name">Name of the method being searched for
        /// </param>
        /// <param name="params">An array of Objects (not Classes) that describe the
        /// the parameters
        /// 
        /// </param>
        /// <returns> The desired Method object.
        /// </returns>
        /// <throws>  IllegalArgumentException When the parameters passed in can not be used for introspection. </throws>
        /// <throws>  MethodMap.AmbiguousException When the method map contains more than one match for the requested signature. </throws>
        public virtual MethodEntry GetMethod(Type c, string name, object[] parameters)
        {
            if (c == null)
            {
                throw new System.ArgumentException("class object is null!");
            }

            IIntrospectorCache ic = IntrospectorCache;

            ClassMap classMap = ic.Get(c);

            if (classMap == null)
            {
                classMap = ic.Put(c);
            }

            return classMap.FindMethod(name, parameters);
        }

        public virtual PropertyEntry GetProperty(Type c, string name)
        {
            if (c == null)
            {
                throw new System.ArgumentException("class object is null!");
            }


            IIntrospectorCache ic = IntrospectorCache;

            ClassMap classMap = ic.Get(c);

            if (classMap == null)
            {
                classMap = ic.Put(c);
            }

            return classMap.FindProperty(name);
        }
    }
}