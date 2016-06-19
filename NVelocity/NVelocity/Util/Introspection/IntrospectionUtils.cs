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
    /// <author>  Nathan Bubna
    /// </author>
    /// <version>  $Id: IntrospectionUtils.java 476785 2006-11-19 10:06:21Z henning $
    /// </version>
    /// <since> 1.6
    /// </since>
    public class IntrospectionUtils
    {

        /// <summary> Determines whether a type represented by a class object is
        /// convertible to another type represented by a class object using a
        /// method invocation conversion, treating object types of primitive
        /// types as if they were primitive types (that is, a Boolean parameters
        /// parameter type matches boolean primitive formal type). This behavior
        /// is because this method is used to determine applicable methods for
        /// an parameters parameter list, and primitive types are represented by
        /// their object duals in reflective method calls.
        /// 
        /// </summary>
        /// <param name="formal">the formal parameter type to which the parameters
        /// parameter type should be convertible
        /// </param>
        /// <param name="parameters">the parameters parameter type.
        /// </param>
        /// <param name="possibleVarArg">whether or not we're dealing with the last parameter
        /// in the method declaration
        /// </param>
        /// <returns> true if either formal type is assignable from parameters type,
        /// or formal is a primitive type and parameters is its corresponding object
        /// type or an object type of a primitive type that can be converted to
        /// the formal type.
        /// </returns>
        public static bool IsMethodInvocationConvertible(System.Type formal, System.Type actual, bool possibleVarArg)
        {
            /* if it's a null, it means the arg was null */
            if (actual == null && !formal.IsPrimitive)
            {
                return true;
            }

            /* Check for identity or widening reference conversion */
            if (actual != null && formal.IsAssignableFrom(actual))
            {
                return true;
            }

            /* Check for boxing with widening primitive conversion. Note that
            * parameters parameters are never primitives. */
            if (formal.IsPrimitive)
            {
                if (formal == System.Type.GetType("System.Boolean") && actual == typeof(System.Boolean))
                    return true;
                if (formal == System.Type.GetType("System.Char") && actual == typeof(System.Char))
                    return true;
                if (formal == System.Type.GetType("System.SByte") && actual == typeof(System.SByte))
                    return true;
                if (formal == System.Type.GetType("System.Int16") && (actual == typeof(System.Int16) || actual == typeof(System.SByte)))
                    return true;
                if (formal == System.Type.GetType("System.Int32") && (actual == typeof(System.Int32) || actual == typeof(System.Int16) || actual == typeof(System.SByte)))
                    return true;
                if (formal == System.Type.GetType("System.Int64") && (actual == typeof(System.Int64) || actual == typeof(System.Int32) || actual == typeof(System.Int16) || actual == typeof(System.SByte)))
                    return true;
                if (formal == System.Type.GetType("System.Single") && (actual == typeof(System.Single) || actual == typeof(System.Int64) || actual == typeof(System.Int32) || actual == typeof(System.Int16) || actual == typeof(System.SByte)))
                    return true;
                if (formal == System.Type.GetType("System.Double") && (actual == typeof(System.Double) || actual == typeof(System.Single) || actual == typeof(System.Int64) || actual == typeof(System.Int32) || actual == typeof(System.Int16) || actual == typeof(System.SByte)))
                    return true;
            }

            /* Check for vararg conversion. */
            if (possibleVarArg && formal.IsArray)
            {
                if (actual.IsArray)
                {
                    actual = actual.GetElementType();
                }
                return IsMethodInvocationConvertible(formal.GetElementType(), actual, false);
            }
            return false;
        }

        /// <summary> Determines whether a type represented by a class object is
        /// convertible to another type represented by a class object using a
        /// method invocation conversion, without matching object and primitive
        /// types. This method is used to determine the more specific type when
        /// comparing signatures of methods.
        /// 
        /// </summary>
        /// <param name="formal">the formal parameter type to which the parameters
        /// parameter type should be convertible
        /// </param>
        /// <param name="parameters">the parameters parameter type.
        /// </param>
        /// <param name="possibleVarArg">whether or not we're dealing with the last parameter
        /// in the method declaration
        /// </param>
        /// <returns> true if either formal type is assignable from parameters type,
        /// or formal and parameters are both primitive types and parameters can be
        /// subject to widening conversion to formal.
        /// </returns>
        public static bool IsStrictMethodInvocationConvertible(System.Type formal, System.Type actual, bool possibleVarArg)
        {
            /* we shouldn't Get a null into, but if so */
            if (actual == null && !formal.IsPrimitive)
            {
                return true;
            }

            /* Check for identity or widening reference conversion */
            if (formal.IsAssignableFrom(actual))
            {
                return true;
            }

            /* Check for widening primitive conversion. */
            if (formal.IsPrimitive)
            {
                if (formal == System.Type.GetType("System.Int16") && (actual == System.Type.GetType("System.SByte")))
                    return true;
                if (formal == System.Type.GetType("System.Int32") && (actual == System.Type.GetType("System.Int16") || actual == System.Type.GetType("System.SByte")))
                    return true;
                if (formal == System.Type.GetType("System.Int64") && (actual == System.Type.GetType("System.Int32") || actual == System.Type.GetType("System.Int16") || actual == System.Type.GetType("System.SByte")))
                    return true;
                if (formal == System.Type.GetType("System.Single") && (actual == System.Type.GetType("System.Int64") || actual == System.Type.GetType("System.Int32") || actual == System.Type.GetType("System.Int16") || actual == System.Type.GetType("System.SByte")))
                    return true;
                if (formal == System.Type.GetType("System.Double") && (actual == System.Type.GetType("System.Single") || actual == System.Type.GetType("System.Int64") || actual == System.Type.GetType("System.Int32") || actual == System.Type.GetType("System.Int16") || actual == System.Type.GetType("System.SByte")))
                    return true;
            }

            /* Check for vararg conversion. */
            if (possibleVarArg && formal.IsArray)
            {
                if (actual.IsArray)
                {
                    actual = actual.GetElementType();
                }
                return IsStrictMethodInvocationConvertible(formal.GetElementType(), actual, false);
            }
            return false;
        }
    }
}
