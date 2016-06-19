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
    using System.Reflection;

    using Runtime.Log;

    /// <summary> <p>Prevent "dangerous" classloader/reflection related calls.  Use this
    /// introspector for situations in which template writers are numerous
    /// or untrusted.  Specifically, this introspector prevents creation of
    /// arbitrary objects and prevents reflection on objects.
    /// 
    /// <p>See documentation of CheckObjectExecutePermission() for
    /// more information on specific classes and methods blocked.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <version>  $Id: SecureIntrospectorImpl.java 705375 2008-10-16 22:06:30Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class SecureIntrospectorImpl : Introspector, ISecureIntrospectorControl
    {
        private string[] badClasses;
        private string[] badPackages;

        public SecureIntrospectorImpl(string[] badClasses, string[] badPackages, Log log)
            : base(log)
        {
            this.badClasses = badClasses;
            this.badPackages = badPackages;
        }

        /// <summary> Get the Method object corresponding to the given class, name and parameters.
        /// Will check for appropriate Execute permissions and return null if the method
        /// is not allowed to be executed.
        /// 
        /// </summary>
        /// <param name="clazz">Class on which method will be called
        /// </param>
        /// <param name="methodName">Name of method to be called
        /// </param>
        /// <param name="params">array of parameters to method
        /// </param>
        /// <returns> Method object retrieved by Introspector
        /// </returns>
        /// <throws>  IllegalArgumentException The parameter passed in were incorrect. </throws>
        public override MethodEntry GetMethod(System.Type clazz, string methodName, object[] params_Renamed)
        {
            if (!CheckObjectExecutePermission(clazz, methodName))
            {
              
                log.Warn("Cannot retrieve method " + methodName + " from object of class " + clazz.FullName + " due to security restrictions.");
                return null;
            }
            else
            {
                return base.GetMethod(clazz, methodName, params_Renamed);
            }
        }

        /// <summary> Determine which methods and classes to prevent from executing.  Always blocks
        /// methods wait() and notify().  Always allows methods on Number, Boolean, and String.
        /// Prohibits method calls on classes related to reflection and system operations.
        /// For the complete list, see the properties <code>introspector.restrict.classes</code>
        /// and <code>introspector.restrict.packages</code>.
        /// 
        /// </summary>
        /// <param name="clazz">Class on which method will be called
        /// </param>
        /// <param name="methodName">Name of method to be called
        /// </param>
        /// <seealso cref="org.apache.velocity.util.introspection.SecureIntrospectorControl.CheckObjectExecutePermission(java.lang.Class, java.lang.String)">
        /// </seealso>
        public virtual bool CheckObjectExecutePermission(System.Type clazz, string methodName)
        {
            /**
            * check for wait and notify
            */
            if (methodName != null && (methodName.Equals("wait") || methodName.Equals("notify")))
            {
                return false;
            }
            /**
            * Always allow the most common classes - Number, Boolean and String
            */
            else if (typeof(System.ValueType).IsAssignableFrom(clazz))
            {
                return true;
            }
            else if (typeof(System.Boolean).IsAssignableFrom(clazz))
            {
                return true;
            }
            else if (typeof(string).IsAssignableFrom(clazz))
            {
                return true;
            }
            /**
            * Always allow Class.getName()
            */
            else if (typeof(System.Type).IsAssignableFrom(clazz) && (methodName != null) && methodName.Equals("getName"))
            {
                return true;
            }

            /**
            * check the classname (minus any array Info)
            * whether it matches disallowed classes or packages
            */
          
            string className = clazz.FullName;
            if (className.StartsWith("[L") && className.EndsWith(";"))
            {
                className = className.Substring(2, (className.Length - 1) - (2));
            }

            int dotPos = className.LastIndexOf('.');
            string packageName = (dotPos == -1) ? "" : className.Substring(0, (dotPos) - (0));

            for (int i = 0, size = badPackages.Length; i < size; i++)
            {
                if (packageName.Equals(badPackages[i]))
                {
                    return false;
                }
            }

            for (int i = 0, size = badClasses.Length; i < size; i++)
            {
                if (className.Equals(badClasses[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
