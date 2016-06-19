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
    using System.IO;

    using Commons.Collections;
    using Exception;

    /// <summary> This is abstract class the all text resource loaders should
    /// extend.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: ResourceLoader.java 687518 2008-08-21 00:18:03Z nbubna $
    /// </version>
    public abstract class ResourceLoader
    {
        /// <summary> Return the class name of this resource Loader</summary>
        /// <returns> Class name of the resource loader.
        /// </returns>
        public virtual string ClassName
        {
            get
            {
                return className;
            }

        }

        /// <summary> The Runtime uses this to find out whether this
        /// template loader wants the Runtime to cache
        /// templates created with InputStreams provided
        /// by this loader.
        /// </summary>
        /// <returns> True if this resource loader caches.
        /// </returns>
        /// <summary> Set the caching state. If true, then this loader
        /// would like the Runtime to cache templates that
        /// have been created with InputStreams provided
        /// by this loader.
        /// </summary>
        /// <param name="value">
        /// </param>
        public virtual bool CachingOn
        {
            get
            {
                return isCachingOn;
            }

            set
            {
                isCachingOn = value;
            }

        }

        /// <summary> Get the interval at which the InputStream source
        /// should be checked for modifications.
        /// </summary>
        /// <returns> The modification check interval.
        /// </returns>
        /// <summary> Set the interval at which the InputStream source
        /// should be checked for modifications.
        /// </summary>
        /// <param name="modificationCheckInterval">
        /// </param>
        public virtual long ModificationCheckInterval
        {
            get
            {
                return modificationCheckInterval;
            }

            set
            {
                this.modificationCheckInterval = value;
            }

        }
        /// <summary> Does this loader want templates produced with it
        /// cached in the Runtime.
        /// </summary>
        protected internal bool isCachingOn = false;

        /// <summary> This property will be passed on to the templates
        /// that are created with this loader.
        /// </summary>
        protected internal long modificationCheckInterval = 2;

        /// <summary> Class name for this loader, for logging/debuggin
        /// purposes.
        /// </summary>
        protected internal string className = null;

        protected internal IRuntimeServices rsvc = null;
        protected internal Log.Log log = null;

        /// <summary> This initialization is used by all resource
        /// loaders and must be called to set up common
        /// properties shared by all resource loaders
        /// </summary>
        /// <param name="rs">
        /// </param>
        /// <param name="configuration">
        /// </param>
        public virtual void CommonInit(IRuntimeServices rs, ExtendedProperties configuration)
        {
            this.rsvc = rs;
            this.log = rsvc.Log;

            /*
            *  these two properties are not required for all loaders.
            *  For example, for ClasspathLoader, what would cache mean?
            *  so adding default values which I think are the safest
            *
            *  don't cache, and modCheckInterval irrelevant...
            */

            try
            {
                isCachingOn = configuration.GetBoolean("cache", false);
            }
            catch (System.Exception e)
            {
                isCachingOn = false;
                string msg = "Exception parsing cache setting: " + configuration.GetString("cache");
                log.Error(msg, e);
                throw new VelocityException(msg, e);
            }
            try
            {
                modificationCheckInterval = configuration.GetLong("modificationCheckInterval", 0);
            }
            catch (System.Exception e)
            {
                modificationCheckInterval = 0;
                string msg = "Exception parsing modificationCheckInterval setting: " + configuration.GetString("modificationCheckInterval");
                log.Error(msg, e);
                throw new VelocityException(msg, e);
            }

            /*
            * this is a must!
            */
            className = typeof(ResourceCacheImpl).FullName;
            try
            {
                className = configuration.GetString("class", className);
            }
            catch (System.Exception e)
            {
                string msg = "Exception retrieving resource cache class name";
                log.Error(msg, e);
                throw new VelocityException(msg, e);
            }
        }

        /// <summary> Initialize the template loader with a
        /// a resources class.
        /// </summary>
        /// <param name="configuration">
        /// </param>
        public abstract void Init(ExtendedProperties configuration);

        /// <summary> Get the InputStream that the Runtime will parse
        /// to create a template.
        /// </summary>
        /// <param name="source">
        /// </param>
        /// <returns> The input stream for the requested resource.
        /// </returns>
        /// <throws>  ResourceNotFoundException </throws>
        public abstract Stream GetResourceStream(string source);

        /// <summary> Given a template, check to see if the source of InputStream
        /// has been modified.
        /// </summary>
        /// <param name="resource">
        /// </param>
        /// <returns> True if the resource has been modified.
        /// </returns>
        public abstract bool IsSourceModified(Resource resource);

        /// <summary> Get the last modified time of the InputStream source
        /// that was used to create the template. We need the template
        /// here because we have to extract the name of the template
        /// in order to locate the InputStream source.
        /// </summary>
        /// <param name="resource">
        /// </param>
        /// <returns> Time in millis when the resource has been modified.
        /// </returns>
        public abstract long GetLastModified(Resource resource);

        /// <summary> Check whether any given resource exists. This is not really
        /// a very efficient test and it can and should be overridden in the
        /// subclasses extending ResourceLoader. 
        /// 
        /// </summary>
        /// <param name="resourceName">The name of a resource.
        /// </param>
        /// <returns> true if a resource exists and can be accessed.
        /// </returns>
        /// <since> 1.6
        /// </since>
        public virtual bool ResourceExists(string resourceName)
        {
            System.IO.Stream is_Renamed = null;
            try
            {
                is_Renamed = GetResourceStream(resourceName);
            }
            catch (ResourceNotFoundException e)
            {
                if (log.DebugEnabled)
                {
                    log.Debug("Could not load resource '" + resourceName + "' from ResourceLoader " + this.GetType().FullName + ": ", e);
                }
            }
            finally
            {
                try
                {
                    if (is_Renamed != null)
                    {
                        is_Renamed.Close();
                    }
                }
                catch (System.Exception e)
                {
                    if (log.ErrorEnabled)
                    {
                        string msg = "While closing InputStream for resource '" + resourceName + "' from ResourceLoader " + this.GetType().FullName;
                        log.Error(msg, e);
                        throw new VelocityException(msg, e);
                    }
                }
            }
            return (is_Renamed != null);
        }
    }
}