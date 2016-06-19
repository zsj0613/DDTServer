
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
    using Runtime;
    /// <summary> Use a custom introspector that prevents classloader related method 
    /// calls.  Use this introspector for situations in which template 
    /// writers are numerous or untrusted.  Specifically, this introspector 
    /// prevents creation of arbitrary objects or reflection on objects.
    /// 
    /// <p>To use this introspector, set the following property:
    /// <pre>
    /// runtime.introspector.uberspect = org.apache.velocity.util.introspection.SecureUberspector
    /// </pre>
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <version>  $Id: SecureUberspector.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class SecureUberspector : UberspectImpl, IRuntimeServicesAware
    {
        internal IRuntimeServices runtimeServices;

        public SecureUberspector()
            : base()
        {
        }

        /// <summary>  Init - generates the Introspector. As the setup code
        /// makes sure that the Log gets set before this is called,
        /// we can Initialize the Introspector using the Log object.
        /// </summary>
        public override void Init()
        {
            string[] badPackages = runtimeServices.Configuration.GetStringArray(RuntimeConstants.INTROSPECTOR_RESTRICT_PACKAGES);

            string[] badClasses = runtimeServices.Configuration.GetStringArray(RuntimeConstants.INTROSPECTOR_RESTRICT_CLASSES);

            introspector = new SecureIntrospectorImpl(badClasses, badPackages, log);
        }

        /// <summary> Get an iterator from the given object.  Since the superclass method
        /// this secure version checks for Execute permission.
        /// 
        /// </summary>
        /// <param name="obj">object to iterate over
        /// </param>
        /// <param name="i">line, column, template Info
        /// </param>
        /// <returns> Iterator for object
        /// </returns>
        /// <throws>  Exception </throws>
        public override System.Collections.IEnumerator GetIterator(object obj, Info i)
        {
            if ((obj != null) && !((ISecureIntrospectorControl)introspector).CheckObjectExecutePermission(obj.GetType(), null))
            {
                log.Warn("Cannot retrieve iterator from object of class " + obj.GetType().FullName + " due to security restrictions.");
                return null;
            }
            else
            {
                return base.GetIterator(obj, i);
            }
        }

        /// <summary> Store the RuntimeServices before the object is initialized..</summary>
        /// <param name="rs">RuntimeServices object for initialization
        /// </param>
        public virtual void SetRuntimeServices(IRuntimeServices rs)
        {
            this.runtimeServices = rs;
        }
    }
}
