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
    /// <summary> Wrapper for Strings containing templates, allowing to Add additional meta
    /// data like timestamps.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:eelco.hillenius@openedge.nl">Eelco Hillenius</a>
    /// </author>
    /// <author>  <a href="mailto:henning@apache.org">Henning P. Schmiedehausen</a>
    /// </author>
    /// <version>  $Id: StringResource.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public sealed class StringResource
    {
        /// <summary> Sets the template body.</summary>
        /// <returns> String containing the template body.
        /// </returns>
        /// <summary> Sets a new  value for the template body.</summary>
        /// <param name="body">New body value
        /// </param>
        public string Body
        {
            get
            {
                return body;
            }

            set
            {
                this.body = value;
                this.lastModified = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
            }

        }
      
        /// <summary> Returns the modification date of the template.</summary>
        /// <returns> Modification date in milliseconds.
        /// </returns>
        /// <summary> Changes the last modified parameter.</summary>
        /// <param name="lastModified">The modification time in millis.
        /// </param>
        public long LastModified
        {
            get
            {
                return lastModified;
            }

            set
            {
                this.lastModified = value;
            }

        }
     
        /// <summary> Returns the encoding of this String resource.
        /// 
        /// </summary>
        /// <returns> The encoding of this String resource.
        /// </returns>
        /// <summary> Sets the encoding of this string resource.
        /// 
        /// </summary>
        /// <param name="encoding">The new encoding of this resource.
        /// </param>
        public string Encoding
        {
            get
            {
                return this.encoding;
            }

            set
            {
                this.encoding = value;
            }

        }
        /// <summary>template body </summary>
        private string body;

        /// <summary>encoding </summary>
        private string encoding;

        /// <summary>last modified ts </summary>
        private long lastModified;

        /// <summary> convenience constructor; sets body to 'body' and sets lastModified to now</summary>
        /// <param name="body">
        /// </param>
        public StringResource(string body, string encoding)
        {
            Body = body;
            Encoding = encoding;
        }
    }
}
