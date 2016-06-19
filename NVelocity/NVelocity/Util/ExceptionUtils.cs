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

namespace NVelocity.Util
{
    /// <summary> Use this to create a new Exception.  This will run under JDK 1.3 or greater.
    /// However, it running under JDK 1.4 it will set the cause.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:isidore@setgame.com">Llewellyn Falco</a>
    /// </author>
    /// <since> 1.5
    /// </since>
    public class ExceptionUtils
    {
        private static bool causesAllowed = true;

        /// <summary> Create a new RuntimeException, setting the cause if possible.</summary>
        /// <param name="message">
        /// </param>
        /// <param name="cause">
        /// </param>
        /// <returns> A runtime exception object.
        /// </returns>
        //UPGRADE_NOTE: 异常 'java.lang.Throwable' 已转换为具有不同的行为的 'System.Exception'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
        public static System.SystemException createRuntimeException(string message, System.Exception cause)
        {
            return (System.SystemException)createWithCause(typeof(System.SystemException), message, cause);
        }

        /// <summary> Create a new Exception, setting the cause if possible.</summary>
        /// <param name="clazz">
        /// </param>
        /// <param name="message">
        /// </param>
        /// <param name="cause">
        /// </param>
        /// <returns> A Throwable.
        /// </returns>
        //UPGRADE_NOTE: 异常 'java.lang.Throwable' 已转换为具有不同的行为的 'System.Exception'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
        public static System.Exception createWithCause(System.Type clazz, string message, System.Exception cause)
        {
            //UPGRADE_NOTE: 异常 'java.lang.Throwable' 已转换为具有不同的行为的 'System.Exception'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
            System.Exception re = null;
            if (causesAllowed)
            {
                try
                {
                    //UPGRADE_NOTE: 异常 'java.lang.Throwable' 已转换为具有不同的行为的 'System.Exception'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
                    System.Reflection.ConstructorInfo constructor = clazz.GetConstructor(new System.Type[] { typeof(string), typeof(System.Exception) });
                    //UPGRADE_NOTE: 异常 'java.lang.Throwable' 已转换为具有不同的行为的 'System.Exception'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
                    re = (System.Exception)constructor.Invoke(new object[] { message, cause });
                }
                catch (System.SystemException e)
                {
                    throw e;
                }
                catch (System.Exception e)
                {
                    causesAllowed = false;
                }
            }
            if (re == null)
            {
                try
                {
                    System.Reflection.ConstructorInfo constructor = clazz.GetConstructor(new System.Type[] { typeof(string) });
                    //UPGRADE_TODO: 在 .NET 中，方法“java.lang.Throwable.toString”的等效项可能返回不同的值。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                    //UPGRADE_NOTE: 异常 'java.lang.Throwable' 已转换为具有不同的行为的 'System.Exception'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
                    re = (System.Exception)constructor.Invoke(new object[] { message + " caused by " + cause });
                }
                catch (System.SystemException e)
                {
                    throw e;
                }
                catch (System.Exception e)
                {
                    //UPGRADE_TODO: 在 .NET 中，方法“java.lang.Throwable.toString”的等效项可能返回不同的值。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                    throw new System.SystemException("Error caused " + e); // should be impossible
                }
            }
            return re;
        }

        /// <summary> Set the cause of the Exception.  Will detect if this is not allowed.</summary>
        /// <param name="onObject">
        /// </param>
        /// <param name="cause">
        /// </param>
        //UPGRADE_NOTE: 异常 'java.lang.Throwable' 已转换为具有不同的行为的 'System.Exception'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
        public static void setCause(System.Exception onObject, System.Exception cause)
        {
            if (causesAllowed)
            {
                try
                {
                    //UPGRADE_NOTE: 异常 'java.lang.Throwable' 已转换为具有不同的行为的 'System.Exception'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1100'"
                    System.Reflection.MethodInfo method = onObject.GetType().GetMethod("initCause", (new System.Type[] { typeof(System.Exception) } == null) ? new System.Type[0] : (System.Type[])new System.Type[] { typeof(System.Exception) });
                    method.Invoke(onObject, new object[] { cause });
                }
                catch (System.SystemException e)
                {
                    throw e;
                }
                catch (System.Exception e)
                {
                    causesAllowed = false;
                }
            }
        }
    }
}
