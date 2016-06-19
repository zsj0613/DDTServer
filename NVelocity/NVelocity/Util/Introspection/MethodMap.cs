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

    /// <summary> </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:bob@werken.com">Bob McWhirter</a>
    /// </author>
    /// <author>  <a href="mailto:Christoph.Reck@dlr.de">Christoph Reck</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  <a href="mailto:szegedia@freemail.hu">Attila Szegedi</a>
    /// </author>
    /// <version>  $Id: MethodMap.java 723123 2008-12-03 23:14:34Z nbubna $
    /// </version>
    public class MethodMap
    {
        private const int MORE_SPECIFIC = 0;
        private const int LESS_SPECIFIC = 1;
        private const int INCOMPARABLE = 2;

        /// <summary> Keep track of all methods with the same name.</summary>
        //UPGRADE_TODO: Class“java.util.HashMap”被转换为具有不同行为的 'System.Collections.Hashtable'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMap'"
        internal System.Collections.IDictionary methodByNameMap = new System.Collections.Hashtable();

        /// <summary> Add a method to a list of methods by name.
        /// For a particular class we are keeping track
        /// of all the methods with the same name.
        /// </summary>
        /// <param name="method">
        /// </param>
        public virtual void add(System.Reflection.MethodInfo method)
        {
            string methodName = method.Name;

            System.Collections.IList l = get_Renamed(methodName);

            if (l == null)
            {
                l = new System.Collections.ArrayList();
                methodByNameMap[methodName] = l;
            }

            l.Add(method);
        }

        /// <summary> Return a list of methods with the same name.
        /// 
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <returns> List list of methods
        /// </returns>
        public virtual System.Collections.IList get_Renamed(string key)
        {
            return (System.Collections.IList)methodByNameMap[key];
        }

        /// <summary>  <p>
        /// Find a method.  Attempts to find the
        /// most specific applicable method using the
        /// algorithm described in the JLS section
        /// 15.12.2 (with the exception that it can't
        /// distinguish a primitive type argument from
        /// an object type argument, since in reflection
        /// primitive type arguments are represented by
        /// their object counterparts, so for an argument of
        /// type (say) java.lang.Integer, it will not be able
        /// to decide between a method that takes int and a
        /// method that takes java.lang.Integer as a parameter.
        /// </p>
        /// 
        /// <p>
        /// This turns out to be a relatively rare case
        /// where this is needed - however, functionality
        /// like this is needed.
        /// </p>
        /// 
        /// </summary>
        /// <param name="methodName">name of method
        /// </param>
        /// <param name="args">the parameters arguments with which the method is called
        /// </param>
        /// <returns> the most specific applicable method, or null if no
        /// method is applicable.
        /// </returns>
        /// <throws>  AmbiguousException if there is more than one maximally </throws>
        /// <summary>  specific applicable method
        /// </summary>
        public virtual System.Reflection.MethodInfo find(string methodName, object[] args)
        {
            System.Collections.IList methodList = get_Renamed(methodName);

            if (methodList == null)
            {
                return null;
            }

            int l = args.Length;
            System.Type[] classes = new System.Type[l];

            for (int i = 0; i < l; ++i)
            {
                object arg = args[i];

                /*
                * if we are careful down below, a null argument goes in there
                * so we can know that the null was passed to the method
                */
                classes[i] = arg == null ? null : arg.GetType();
            }

            return getBestMatch(methodList, classes);
        }

        private static System.Reflection.MethodInfo getBestMatch(System.Collections.IList methods, System.Type[] args)
        {
            System.Collections.IList equivalentMatches = null;
            System.Reflection.MethodInfo bestMatch = null;
            System.Reflection.ParameterInfo[] bestMatchTypes = null;
            //UPGRADE_TODO: 方法“java.util.Iterator.hasNext”被转换为具有不同行为的 'System.Collections.IEnumerator.MoveNext'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
            for (System.Collections.IEnumerator i = methods.GetEnumerator(); i.MoveNext(); )
            {
                //UPGRADE_TODO: 方法“java.util.Iterator.next”被转换为具有不同行为的 'System.Collections.IEnumerator.Current'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
                System.Reflection.MethodInfo method = (System.Reflection.MethodInfo)i.Current;
                if (isApplicable(method, args))
                {
                    if (bestMatch == null)
                    {
                        bestMatch = method;
                        //UPGRADE_TODO: 在 .NET 中，方法“java.lang.reflect.Method.getParameterTypes”的等效项可能返回不同的值。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                        bestMatchTypes = method.GetParameters();
                    }
                    else
                    {
                        //UPGRADE_TODO: 在 .NET 中，方法“java.lang.reflect.Method.getParameterTypes”的等效项可能返回不同的值。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                        System.Reflection.ParameterInfo[] methodTypes = method.GetParameters();
                        switch (compare(methodTypes, bestMatchTypes))
                        {

                            case MORE_SPECIFIC:
                                if (equivalentMatches == null)
                                {
                                    bestMatch = method;
                                    bestMatchTypes = methodTypes;
                                }
                                else
                                {
                                    // have to beat all other ambiguous ones...
                                    int ambiguities = equivalentMatches.Count;
                                    for (int a = 0; a < ambiguities; a++)
                                    {
                                        System.Reflection.MethodInfo other = (System.Reflection.MethodInfo)equivalentMatches[a];
                                        //UPGRADE_TODO: 在 .NET 中，方法“java.lang.reflect.Method.getParameterTypes”的等效项可能返回不同的值。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                                        switch (compare(methodTypes, other.GetParameters()))
                                        {

                                            case MORE_SPECIFIC:
                                                // ...and thus replace them all...
                                                bestMatch = method;
                                                bestMatchTypes = methodTypes;
                                                equivalentMatches = null;
                                                ambiguities = 0;
                                                break;


                                            case INCOMPARABLE:
                                                // ...join them...
                                                equivalentMatches.Add(method);
                                                break;


                                            case LESS_SPECIFIC:
                                                // ...or just go away.
                                                break;
                                        }
                                    }
                                }
                                break;


                            case INCOMPARABLE:
                                if (equivalentMatches == null)
                                {
                                    equivalentMatches = new System.Collections.ArrayList(bestMatchTypes.Length);
                                }
                                equivalentMatches.Add(method);
                                break;


                            case LESS_SPECIFIC:
                                // do nothing
                                break;
                        }
                    }
                }
            }

            if (equivalentMatches != null)
            {
                throw new AmbiguousException();
            }
            return bestMatch;
        }

        /// <summary>  Simple distinguishable exception, used when
        /// we run across ambiguous overloading.  Caught
        /// by the introspector.
        /// </summary>
        [Serializable]
        public class AmbiguousException : System.SystemException
        {
            
        }

        /// <summary> Determines which method signature (represented by a class array) is more
        /// specific. This defines a partial ordering on the method signatures.
        /// </summary>
        /// <param name="c1">first signature to Compare
        /// </param>
        /// <param name="c2">second signature to Compare
        /// </param>
        /// <returns> MORE_SPECIFIC if c1 is more specific than c2, LESS_SPECIFIC if
        /// c1 is less specific than c2, INCOMPARABLE if they are incomparable.
        /// </returns>
        private static int compare(System.Reflection.ParameterInfo[] c1, System.Reflection.ParameterInfo[] c2)
        {
            bool c1MoreSpecific = false;
            bool c2MoreSpecific = false;

            // Compare lengths to handle comparisons where the size of the arrays
            // doesn't match, but the methods are both applicable due to the fact
            // that one is a varargs method
            if (c1.Length > c2.Length)
            {
                return MORE_SPECIFIC;
            }
            if (c2.Length > c1.Length)
            {
                return LESS_SPECIFIC;
            }

            // ok, move on and Compare those of equal lengths
            for (int i = 0; i < c1.Length; ++i)
            {
                if (c1[i] != c2[i])
                {
                    bool last = (i == c1.Length - 1);
                    c1MoreSpecific = c1MoreSpecific || isStrictConvertible(c2[i].GetType(), c1[i].GetType(), last);
                    c2MoreSpecific = c2MoreSpecific || isStrictConvertible(c1[i].GetType(), c2[i].GetType(), last);
                }
            }

            if (c1MoreSpecific)
            {
                if (c2MoreSpecific)
                {
                    /*
                    * If one method accepts varargs and the other does not,
                    * call the non-vararg one more specific.
                    */
                    bool last1Array = c1[c1.Length - 1].GetType().IsArray;
                    bool last2Array = c2[c2.Length - 1].GetType().IsArray;
                    if (last1Array && !last2Array)
                    {
                        return LESS_SPECIFIC;
                    }
                    if (!last1Array && last2Array)
                    {
                        return MORE_SPECIFIC;
                    }

                    /*
                    *  Incomparable due to cross-assignable arguments (i.e.
                    * foo(String, Object) vs. foo(Object, String))
                    */
                    return INCOMPARABLE;
                }

                return MORE_SPECIFIC;
            }

            if (c2MoreSpecific)
            {
                return LESS_SPECIFIC;
            }

            /*
            * Incomparable due to non-related arguments (i.e.
            * foo(Runnable) vs. foo(Serializable))
            */

            return INCOMPARABLE;
        }

        /// <summary> Returns true if the supplied method is applicable to parameters
        /// argument types.
        /// 
        /// </summary>
        /// <param name="method">method that will be called
        /// </param>
        /// <param name="classes">arguments to method
        /// </param>
        /// <returns> true if method is applicable to arguments
        /// </returns>
        private static bool isApplicable(System.Reflection.MethodInfo method, System.Type[] classes)
        {
            //UPGRADE_TODO: 在 .NET 中，方法“java.lang.reflect.Method.getParameterTypes”的等效项可能返回不同的值。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            System.Reflection.ParameterInfo[] methodArgs = method.GetParameters();

            if (methodArgs.Length > classes.Length)
            {
                // if there's just one more methodArg than class arg
                // and the last methodArg is an array, then treat it as a vararg
                if (methodArgs.Length == classes.Length + 1 && methodArgs[methodArgs.Length - 1].GetType().IsArray)
                {
                    // all the args preceding the vararg must match
                    for (int i = 0; i < classes.Length; i++)
                    {
                        if (!isConvertible(methodArgs[i].GetType(), classes[i], false))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            else if (methodArgs.Length == classes.Length)
            {
                // this will properly match when the last methodArg
                // is an array/varargs and the last class is the type of array
                // (e.g. String when the method is expecting String...)
                for (int i = 0; i < classes.Length; ++i)
                {
                    if (!isConvertible(methodArgs[i].GetType(), classes[i], false))
                    {
                        // if we're on the last arg and the method expects an array
                        if (i == classes.Length - 1 && methodArgs[i].GetType().IsArray)
                        {
                            // check to see if the last arg is convertible
                            // to the array's component type
                            return isConvertible(methodArgs[i].GetType(), classes[i], true);
                        }
                        return false;
                    }
                }
            }
            else if (methodArgs.Length > 0)
            // more arguments given than the method accepts; check for varargs
            {
                // check that the last methodArg is an array
                System.Type lastarg = methodArgs[methodArgs.Length - 1].GetType();
                if (!lastarg.IsArray)
                {
                    return false;
                }

                // check that they all match up to the last method arg
                for (int i = 0; i < methodArgs.Length - 1; ++i)
                {
                    if (!isConvertible(methodArgs[i].GetType(), classes[i], false))
                    {
                        return false;
                    }
                }

                // check that all remaining arguments are convertible to the vararg type
                System.Type vararg = lastarg.GetElementType();
                for (int i = methodArgs.Length - 1; i < classes.Length; ++i)
                {
                    if (!isConvertible(vararg, classes[i], false))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool isConvertible(System.Type formal, System.Type actual, bool possibleVarArg)
        {
            return IntrospectionUtils.IsMethodInvocationConvertible(formal, actual, possibleVarArg);
        }

        private static bool isStrictConvertible(System.Type formal, System.Type actual, bool possibleVarArg)
        {
            return IntrospectionUtils.IsStrictMethodInvocationConvertible(formal, actual, possibleVarArg);
        }
    }
}