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

namespace NVelocity.App.Event.Implement
{
    using Runtime;
    using Util;

    /// <summary> Simple event handler that checks to see if an included page is available.
    /// If not, it includes a designated replacement page instead.
    /// 
    /// <P>By default, the name of the replacement page is "notfound.vm", however this
    /// page name can be changed by setting the Velocity property
    /// <code>eventhandler.include.notfound</code>, for example:
    /// <code>
    /// <PRE>
    /// eventhandler.include.notfound = Error.vm
    /// </PRE>
    /// </code>
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <version>  $Id: IncludeNotFound.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class IncludeNotFound : IIncludeEventHandler, IRuntimeServicesAware
    {

        private const string DEFAULT_NOT_FOUND = "notfound.vm";
        private const string PROPERTY_NOT_FOUND = "eventhandler.include.notfound";
        private IRuntimeServices rs = null;
        internal string notfound;

        /// <summary> Chseck to see if included file exists, and display "not found" page if it
        /// doesn't. If "not found" page does not exist, Log an Error and return
        /// null.
        /// 
        /// </summary>
        /// <param name="includeResourcePath">
        /// </param>
        /// <param name="currentResourcePath">
        /// </param>
        /// <param name="directiveName">
        /// </param>
        /// <returns> message.
        /// </returns>
        public virtual string IncludeEvent(string includeResourcePath, string currentResourcePath, string directiveName)
        {

            /**
            * check to see if page exists
            */
            bool exists = (rs.GetLoaderNameForResource(includeResourcePath) != null);
            if (!exists)
            {
                if (rs.GetLoaderNameForResource(notfound) != null)
                {
                    return notfound;
                }
                else
                {
                    /**
                    * can't find not found, so display nothing
                    */
                    rs.Log.Error("Can't find include not found page: " + notfound);
                    return null;
                }
            }
            else
                return includeResourcePath;
        }


        /// <seealso cref="org.apache.velocity.util.RuntimeServicesAware.setRuntimeServices(org.apache.velocity.runtime.RuntimeServices)">
        /// </seealso>
        public virtual void SetRuntimeServices(IRuntimeServices rs)
        {
            this.rs = rs;
            notfound = StringUtils.NullTrim(rs.GetString(PROPERTY_NOT_FOUND, DEFAULT_NOT_FOUND));
        }
    }
}
