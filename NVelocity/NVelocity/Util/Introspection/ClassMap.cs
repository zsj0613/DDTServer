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
    using System.Reflection;
    using System.Text;
    using System.Linq;

    using Runtime.Log;

    /// <summary> A cache of introspection information for a specific class instance.
    /// Keys {@link java.lang.reflect.Method} objects by a concatenation of the
    /// method name and the names of classes that make up the parameters.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:bob@werken.com">Bob McWhirter</a>
    /// </author>
    /// <author>  <a href="mailto:szegedia@freemail.hu">Attila Szegedi</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  <a href="mailto:henning@apache.org">Henning P. Schmiedehausen</a>
    /// </author>
    /// <author>  Nathan Bubna
    /// </author>
    /// <version>  $Id: ClassMap.java 698376 2008-09-23 22:15:49Z nbubna $
    /// </version>
    public class ClassMap
    {
        private static readonly string NULL_ARGS = "NULL_ARGS";

        /// <summary> Returns the class object whose methods are cached by this map.
        /// 
        /// </summary>
        /// <returns> The class object whose methods are cached by this map.
        /// </returns>
        public  System.Type CachedClass
        {
            get
            {
                return clazz;
            }

        }

        /// <summary>Class logger </summary>
        private Log log;

        /// <summary> Class passed into the constructor used to as
        /// the basis for the Method map.
        /// </summary>
        private System.Type clazz;

        //private MethodCache methodCache;

        private readonly Dictionary<string, PropertyEntry> propertyCache =
            new Dictionary<string, PropertyEntry>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, Dictionary<string, MethodEntry>> methodCache = new Dictionary<string, Dictionary<string, MethodEntry>>(StringComparer.OrdinalIgnoreCase);

        /// <summary> Standard constructor</summary>
        /// <param name="clazz">The class for which this ClassMap gets constructed.
        /// </param>
        public ClassMap(System.Type clazz, Log log)
        {
            this.clazz = clazz;
            this.log = log;

            CreatePropertyCache();
            CreateMethodCache();
        }

        private void CreatePropertyCache()
        {
            // Get all publicly accessible methods
            IList<PropertyInfo> properties = GetAccessibleProperties(clazz);

            // map and cache them
            foreach (PropertyInfo property in properties)
            {
                try
                {
                    propertyCache[property.Name] = new PropertyEntry(property);
                }
                catch
                {

                }
            }
        }

        private void CreateMethodCache()
        {
            // Get all publicly accessible methods
            IList<MethodInfo> methods = GetAccessibleMethods(clazz);

            // map and cache them
            foreach (MethodInfo method in methods)
            {
                if (!method.IsGenericMethod)
                {
                    string methodName = method.Name;

                    if (methodCache.ContainsKey(methodName))
                    {
                        Dictionary<string, MethodEntry> methodInofs = methodCache[methodName];

                        if (methodInofs == null)
                        {
                            methodInofs = new Dictionary<string, MethodEntry>();
                        }

                        try
                        {
                            methodInofs[MakeMethodKey(method)] = new MethodEntry(method);
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        try
                        {
                            Dictionary<string, MethodEntry> methodInofs = new Dictionary<string, MethodEntry>(StringComparer.OrdinalIgnoreCase);
                            methodInofs[MakeMethodKey(method)] = new MethodEntry(method);
                            methodCache[methodName] = methodInofs;
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        private static IList<PropertyInfo> GetAccessibleProperties(Type type)
        {
            List<PropertyInfo> props = new List<PropertyInfo>();

            foreach (Type interfaceType in type.GetInterfaces())
            {
                props.AddRange(interfaceType.GetProperties());
            }

            props.AddRange(type.GetProperties());

            return props;
        }

        private static IList<MethodInfo> GetAccessibleMethods(Type type)
        {
            List<MethodInfo> methods = new List<MethodInfo>();

            foreach (Type interfaceType in type.GetInterfaces())
            {
                methods.AddRange(interfaceType.GetMethods());
            }

            methods.AddRange(type.GetMethods());

            return methods;
        }

        private static string MakeMethodKey(MethodInfo method)
        {
            StringBuilder methodKey = new StringBuilder();

            ParameterInfo[] parameters = method.GetParameters();

            if (parameters == null || parameters.Length == 0)
            {
                methodKey.Append(NULL_ARGS);
            }
            else
            {

                for (int i = 0; i < parameters.Length; i++)
                {
                    methodKey.Append(parameters[i].ParameterType.FullName);

                    if (i < parameters.Length - 1)
                    {
                        methodKey.Append(",");
                    }
                }
            }

            return methodKey.ToString();
        }

        private static string MakeMethodKey(object[] parameters)
        {
            StringBuilder methodKey = new StringBuilder();

            if (parameters == null || parameters.Length == 0)
            {
                methodKey.Append(NULL_ARGS);
            }
            else
            {

                for (int i = 0; i < parameters.Length; i++)
                {
                    methodKey.Append(parameters[i].GetType().FullName);

                    if (i < parameters.Length - 1)
                    {
                        methodKey.Append(",");
                    }
                }
            }

            return methodKey.ToString();
        }

        private static bool IsMatchType(string src, string dest)
        {
            if (src.Equals(dest, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else if (dest.Equals("System.Object", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary> Find a Method using the method name and parameter objects.
        /// 
        /// </summary>
        /// <param name="name">The method name to look up.
        /// </param>
        /// <param name="params">An array of parameters for the method.
        /// </param>
        /// <returns> A Method object representing the method to invoke or null.
        /// </returns>
        /// <throws>  MethodMap.AmbiguousException When more than one method is a match for the parameters. </throws>
        public  MethodEntry FindMethod(string name, object[] parameters)
        {
            Dictionary<string, MethodEntry> methodEntry = null;
            methodCache.TryGetValue(name, out methodEntry);

            if (methodEntry == null || methodEntry.Count == 0)
            {
                return null;
            }

            if (methodEntry.Count == 1)
            {
                return methodEntry.Values.ToArray()[0];
            }

            string methodKey = MakeMethodKey(parameters);

            if (methodEntry.ContainsKey(methodKey))
            {
                return methodEntry[methodKey];
            }

            foreach (string key in methodEntry.Keys)
            {
                string[] srcArgs = methodKey.Split(',');
                string[] targetArgs = key.Split(',');

                if (srcArgs.Length == targetArgs.Length)
                {
                    bool isMatch = true;

                    for (int i = 0; i < srcArgs.Length; i++)
                    {
                        isMatch &= IsMatchType(srcArgs[i], targetArgs[i]);
                    }

                    if (isMatch)
                    {
                        return methodEntry[key];
                    }
                }
            }

            return null;
        }

        public  PropertyEntry FindProperty(string name)
        {
            PropertyEntry entry = null;

            propertyCache.TryGetValue(name, out entry);

            return entry;
        }
    }
}