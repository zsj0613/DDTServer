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
    using Loader;

    /// <summary> Default implementation of StringResourceRepository.
    /// Uses a HashMap for storage
    /// 
    /// </summary>
    /// <author>  <a href="mailto:eelco.hillenius@openedge.nl">Eelco Hillenius</a>
    /// </author>
    /// <author>  <a href="mailto:henning@apache.org">Henning P. Schmiedehausen</a>
    /// </author>
    /// <version>  $Id: StringResourceRepositoryImpl.java 685724 2008-08-13 23:12:12Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class StringResourceRepositoryImpl : IStringResourceRepository
    {
        public StringResourceRepositoryImpl()
        {
            InitBlock();
        }
        private void InitBlock()
        {
            encoding = StringResourceLoader.REPOSITORY_ENCODING_DEFAULT;
        }
       
        /// <seealso cref="org.apache.velocity.runtime.resource.util.StringResourceRepository.getEncoding()">
        /// </seealso>
        /// <seealso cref="org.apache.velocity.runtime.resource.util.StringResourceRepository.setEncoding(java.lang.String)">
        /// </seealso>
        virtual public string Encoding
        {
            get
            {
                return encoding;
            }

            set
            {
                this.encoding = value;
            }

        }
        /// <summary> mem store</summary>
      
        protected internal System.Collections.IDictionary resources = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());

        /// <summary> Current Repository encoding.</summary>
        private string encoding;

        /// <seealso cref="StringResourceRepository.GetStringResource(java.lang.String)">
        /// </seealso>
        public virtual StringResource GetStringResource(string name)
        {
            return (StringResource)resources[name];
        }

        /// <seealso cref="StringResourceRepository.PutStringResource(java.lang.String, java.lang.String)">
        /// </seealso>
        public virtual void PutStringResource(string name, string body)
        {
            resources[name] = new StringResource(body, Encoding);
        }

        /// <seealso cref="StringResourceRepository.PutStringResource(java.lang.String, java.lang.String, java.lang.String)">
        /// </seealso>
        /// <since> 1.6
        /// </since>
        public virtual void PutStringResource(string name, string body, string encoding)
        {
            resources[name] = new StringResource(body, encoding);
        }

        /// <seealso cref="StringResourceRepository.RemoveStringResource(java.lang.String)">
        /// </seealso>
        public virtual void RemoveStringResource(string name)
        {
            resources.Remove(name);
        }
    }
}
