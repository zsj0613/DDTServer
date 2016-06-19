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

namespace NVelocity.Runtime.Resource.Loader
{
    using System;

    using Exception;

    /// <summary> Factory to grab a template loader.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <version>  $Id: ResourceLoaderFactory.java 687177 2008-08-19 22:00:32Z nbubna $
    /// </version>
    public class ResourceLoaderFactory
    {
        /// <summary> Gets the loader specified in the configuration file.</summary>
        /// <param name="rs">
        /// </param>
        /// <param name="loaderClassName">
        /// </param>
        /// <returns> TemplateLoader
        /// </returns>
        /// <throws>  Exception </throws>
        public static ResourceLoader GetLoader(IRuntimeServices rs, string loaderClassName)
        {
            ResourceLoader loader = null;

            try
            {
                loader = (ResourceLoader)System.Activator.CreateInstance(Type.GetType(loaderClassName.Replace(';', ',')));

                rs.Log.Debug("ResourceLoader instantiated: " + loader.GetType().FullName);

                return loader;
            }
            // The ugly three strike again: ClassNotFoundException,IllegalAccessException,InstantiationException
            catch (System.Exception e)
            {
                string msg = "Problem instantiating the template loader: " + loaderClassName + ".\n" + "Look at your properties file and make sure the\n" + "name of the template loader is correct.";
                rs.Log.Error(msg, e);
                throw new VelocityException(msg, e);
            }
        }
    }
}