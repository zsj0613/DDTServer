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

namespace NVelocity.Runtime.Resource.Util
{
    /// <summary> A StringResourceRepository functions as a central repository for Velocity templates
    /// stored in Strings.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:eelco.hillenius@openedge.nl">Eelco Hillenius</a>
    /// </author>
    /// <author>  <a href="mailto:henning@apache.org">Henning P. Schmiedehausen</a>
    /// </author>
    /// <version>  $Id: StringResourceRepository.java 685724 2008-08-13 23:12:12Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public interface IStringResourceRepository
    {
        /// <summary> Returns the current encoding of this repository.
        /// 
        /// </summary>
        /// <returns> The current encoding of this repository.
        /// </returns>
        /// <summary> Sets the default encoding of the repository. Encodings can also be stored per
        /// template string. The default implementation does this correctly.
        /// 
        /// </summary>
        /// <param name="encoding">The encoding to use.
        /// </param>
        string Encoding
        {
            get;

            set;

        }
        /// <summary> Get the string resource that is stored with given key</summary>
        /// <param name="name">String name to retrieve from the repository.
        /// </param>
        /// <returns> A StringResource containing the template.
        /// </returns>
        StringResource GetStringResource(string name);

        /// <summary> Add a string resource with given key.</summary>
        /// <param name="name">The String name to store the template under.
        /// </param>
        /// <param name="body">A String containing a template.
        /// </param>
        void PutStringResource(string name, string body);

        /// <summary> Add a string resource with given key.</summary>
        /// <param name="name">The String name to store the template under.
        /// </param>
        /// <param name="body">A String containing a template.
        /// </param>
        /// <param name="encoding">The encoding of this string template
        /// </param>
        /// <since> 1.6
        /// </since>
        void PutStringResource(string name, string body, string encoding);

        /// <summary> delete a string resource with given key.</summary>
        /// <param name="name">The string name to remove from the repository.
        /// </param>
        void RemoveStringResource(string name);
    }
}
