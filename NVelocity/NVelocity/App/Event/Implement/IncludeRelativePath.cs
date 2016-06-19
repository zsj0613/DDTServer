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
    /// <summary> <p>Event handler that looks for included files relative to the path of the
    /// current template. The handler assumes that paths are separated by a forward
    /// slash "/" or backwards slash "\".
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain </a>
    /// </author>
    /// <version>  $Id: IncludeRelativePath.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class IncludeRelativePath : IIncludeEventHandler
    {

        /// <summary> Return path relative to the current template's path.
        /// 
        /// </summary>
        /// <param name="includeResourcePath"> the path as given in the include directive.
        /// </param>
        /// <param name="currentResourcePath">the path of the currently rendering template that includes the
        /// include directive.
        /// </param>
        /// <param name="directiveName"> name of the directive used to include the resource. (With the
        /// standard directives this is either "parse" or "include").
        /// </param>
        /// <returns> new path relative to the current template's path
        /// </returns>
        public virtual string IncludeEvent(string includeResourcePath, string currentResourcePath, string directiveName)
        {
            // if the resource name starts with a slash, it's not a relative path
            if (includeResourcePath.StartsWith("/") || includeResourcePath.StartsWith("\\"))
            {
                return includeResourcePath;
            }

            int lastslashpos = System.Math.Max(currentResourcePath.LastIndexOf("/"), currentResourcePath.LastIndexOf("\\"));

            // root of resource tree
            if (lastslashpos == -1)
            {
                return includeResourcePath;
            }

            // prepend path to the include path
            return currentResourcePath.Substring(0, (lastslashpos) - (0)) + "/" + includeResourcePath;
        }
    }
}
