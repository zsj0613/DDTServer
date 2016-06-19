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

namespace NVelocity.Runtime.Resource
{
    using System;

    using Loader;

    /// <summary> This class represent a general text resource that
    /// may have been retrieved from any number of possible
    /// sources.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: Resource.java 685724 2008-08-13 23:12:12Z nbubna $
    /// </version>
    public abstract class Resource
    {
        /// <param name="rs">
        /// </param>
        public virtual IRuntimeServices RuntimeServices
        {
            set
            {
                rsvc = value;
            }

        }
        /// <returns> True if source has been modified.
        /// </returns>
        public virtual bool IsSourceModified
        {
            get
            {
                return resourceLoader.IsSourceModified(this);
            }

        }
        /// <summary> Set the modification check interval.</summary>
        /// <param name="ModificationCheckInterval">The interval (in seconds).
        /// </param>
        public virtual long ModificationCheckInterval
        {
            set
            {
                this.modificationCheckInterval = value;
            }

        }

        /// <summary> Get the name of this template.</summary>
        /// <returns> The name of this template.
        /// </returns>
        /// <summary> Set the name of this resource, for example
        /// test.vm.
        /// </summary>
        /// <param name="Name">
        /// </param>
        public virtual string Name
        {
            get
            {
                return name;
            }

            set
            {
                this.name = value;
            }

        }

        /// <summary>  Get the encoding of this resource
        /// for example, "ISO-8859-1"
        /// </summary>
        /// <returns> The encoding of this resource.
        /// </returns>
        /// <summary>  set the encoding of this resource
        /// for example, "ISO-8859-1"
        /// </summary>
        /// <param name="Encoding">
        /// </param>
        public virtual string Encoding
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

        /// <summary> Return the lastModifed time of this
        /// resource.
        /// </summary>
        /// <returns> The lastModifed time of this resource.
        /// </returns>
        /// <summary> Set the last modified time for this
        /// resource.
        /// </summary>
        /// <param name="LastModified">
        /// </param>
        public virtual long LastModified
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

        /// <summary> Return the template loader that pulled
        /// in the template stream
        /// </summary>
        /// <returns> The resource loader for this resource.
        /// </returns>
        /// <summary> Set the template loader for this template. Set
        /// when the Runtime determines where this template
        /// came from the list of possible sources.
        /// </summary>
        /// <param name="ResourceLoader">
        /// </param>
        public virtual ResourceLoader ResourceLoader
        {
            get
            {
                return resourceLoader;
            }

            set
            {
                this.resourceLoader = value;
            }

        }

        /// <summary> Get arbitrary data object that might be used
        /// by the resource.
        /// </summary>
        /// <returns> The data object for this resource.
        /// </returns>
        /// <summary> Set arbitrary data object that might be used
        /// by the resource.
        /// </summary>
        /// <param name="Data">
        /// </param>
        public virtual object Data
        {
            get
            {
                return data;
            }

            set
            {
                this.data = value;
            }

        }

        /// <returns> type code of the Resource
        /// </returns>
        /// <since> 1.6
        /// </since>
        /// <summary> Sets the type of this Resource (RESOURCE_TEMPLATE or RESOURCE_CONTENT)</summary>
        /// <since> 1.6
        /// </since>
        public virtual int Type
        {
            get
            {
                return type;
            }

            set
            {
                this.type = value;
            }

        }
        protected internal IRuntimeServices rsvc = null;

        /// <summary> The template loader that initially loaded the input
        /// stream for this template, and knows how to check the
        /// source of the input stream for modification.
        /// </summary>
        protected internal ResourceLoader resourceLoader;

        /// <summary> The number of milliseconds in a minute, used to calculate the
        /// check interval.
        /// </summary>
        protected internal const long MILLIS_PER_SECOND = 1000;

        /// <summary> How often the file modification time is checked (in seconds).</summary>
        protected internal long modificationCheckInterval = 0;

        /// <summary> The file modification time (in milliseconds) for the cached template.</summary>
        protected internal long lastModified = 0;

        /// <summary> The next time the file modification time will be checked (in
        /// milliseconds).
        /// </summary>
        protected internal long nextCheck = 0;

        /// <summary>  Name of the resource</summary>
        protected internal string name;

        /// <summary>  Character encoding of this resource</summary>
        protected internal string encoding = NVelocity.Runtime.RuntimeConstants.ENCODING_DEFAULT;

        /// <summary>  Resource might require ancillary storage of some kind</summary>
        protected internal object data = null;

        /// <summary>  Resource type (RESOURCE_TEMPLATE or RESOURCE_CONTENT)</summary>
        protected internal int type;

        /// <summary>  Default constructor</summary>
        public Resource()
        {
        }

        /// <summary> Perform any subsequent processing that might need
        /// to be done by a resource. In the case of a template
        /// the parameters parsing of the input stream needs to be
        /// performed.
        /// 
        /// </summary>
        /// <returns> Whether the resource could be processed successfully.
        /// For a {@link org.apache.velocity.Template} or {@link
        /// org.apache.velocity.runtime.resource.ContentResource}, this
        /// indicates whether the resource could be read.
        /// </returns>
        /// <exception cref="ResourceNotFoundException">Similar in semantics as
        /// returning <code>false</code>.
        /// </exception>
        /// <throws>  ParseErrorException </throws>
        /// <throws>  Exception </throws>
        public abstract bool Process();

        /// <summary> Is it time to check to see if the resource
        /// source has been updated?
        /// </summary>
        /// <returns> True if resource must be checked.
        /// </returns>
        public virtual bool RequiresChecking()
        {
            /*
            *  short circuit this if modificationCheckInterval == 0
            *  as this means "don't check"
            */

            if (modificationCheckInterval <= 0)
            {
                return false;
            }

            /*
            *  see if we need to check now
            */

            return ((DateTime.Now.Ticks - 621355968000000000) / 10000 >= nextCheck);
        }

        /// <summary> 'Touch' this template and thereby resetting
        /// the nextCheck field.
        /// </summary>
        public virtual void Touch()
        {
            nextCheck = (DateTime.Now.Ticks - 621355968000000000) / 10000 + (MILLIS_PER_SECOND * modificationCheckInterval);
        }
    }
}