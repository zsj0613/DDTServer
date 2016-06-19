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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Commons.Collections;
    using Loader;
    using Exception;
    using NVelocity.Util;

    /// <summary> Class to manage the text resource for the Velocity Runtime.
    /// 
    /// </summary>
    /// <author>   <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <author>   <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>   <a href="mailto:paulo.gaspar@krankikom.de">Paulo Gaspar</a>
    /// </author>
    /// <author>   <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>   <a href="mailto:henning@apache.org">Henning P. Schmiedehausen</a>
    /// </author>
    /// <version>   $Id: ResourceManagerImpl.java 692505 2008-09-05 18:21:51Z nbubna $
    /// </version>
    public class ResourceManagerImpl : IResourceManager
    {

        /// <summary>A template resources. </summary>
        public const int RESOURCE_TEMPLATE = 1;

        /// <summary>A static content resource. </summary>
        public const int RESOURCE_CONTENT = 2;

        /// <summary>token used to identify the loader internally. </summary>
        private const string RESOURCE_LOADER_IDENTIFIER = "_RESOURCE_LOADER_IDENTIFIER_";

        /// <summary>Object implementing ResourceCache to be our resource manager's Resource cache. </summary>
        protected internal IResourceCache globalCache = null;

        /// <summary>The List of templateLoaders that the Runtime will use to locate the InputStream source of a template. </summary>
        protected internal IList<ResourceLoader> resourceLoaders = new List<ResourceLoader>();

        /// <summary> This is a list of the template input stream source initializers, basically properties for a particular template stream
        /// source. The order in this list reflects numbering of the properties i.e.
        /// 
        /// <p>&lt;loader-id&gt;.resource.loader.&lt;property&gt; = &lt;value&gt;</p>
        /// </summary>
        private IList sourceInitializerList = new ArrayList();

        /// <summary> Has this Manager been initialized?</summary>
        private bool isInit = false;

        /// <summary>switch to turn off Log notice when a resource is found for the first time. </summary>
        private bool logWhenFound = true;

        /// <summary>The internal RuntimeServices object. </summary>
        protected internal IRuntimeServices rsvc = null;

        /// <summary>Logging. </summary>
        protected internal Log.Log log = null;

        private static readonly object syncOjb = new object();

        /// <summary> Initialize the ResourceManager.
        /// 
        /// </summary>
        /// <param name="rsvc"> The Runtime Services object which is associated with this Resource Manager.
        /// 
        /// </param>
        /// <throws>   Exception </throws>
        public virtual void Initialize(IRuntimeServices rsvc)
        {
            lock (syncOjb)
            {
                if (isInit)
                {
                    log.Debug("Re-initialization of ResourceLoader attempted and ignored.");
                    return;
                }

                ResourceLoader resourceLoader = null;

                this.rsvc = rsvc;
                log = rsvc.Log;

                log.Trace("Default ResourceManager initializing. (" + this.GetType() + ")");

                AssembleResourceLoaderInitializers();


                for (IEnumerator it = sourceInitializerList.GetEnumerator(); it.MoveNext(); )
                {
                    /**
                    * Resource loader can be loaded either via class name or be passed
                    * in as an instance.
                    */

                    ExtendedProperties configuration = (ExtendedProperties)it.Current;

                    string loaderClass = StringUtils.NullTrim(configuration.GetString("class"));
                    ResourceLoader loaderInstance = (ResourceLoader)configuration["instance"];

                    if (loaderInstance != null)
                    {
                        resourceLoader = loaderInstance;
                    }
                    else if (loaderClass != null)
                    {
                        resourceLoader = ResourceLoaderFactory.GetLoader(rsvc, loaderClass);
                    }
                    else
                    {
                        string msg = "Unable to find '" + configuration.GetString(RESOURCE_LOADER_IDENTIFIER) + ".resource.loader.class' specification in configuration." + " This is a critical value.  Please adjust configuration.";
                        log.Error(msg);
                        throw new System.Exception(msg);
                    }

                    resourceLoader.CommonInit(rsvc, configuration);
                    resourceLoader.Init(configuration);
                    resourceLoaders.Add(resourceLoader);
                }

                /*
                * now see if this is overridden by configuration
                */

                logWhenFound = rsvc.GetBoolean(NVelocity.Runtime.RuntimeConstants.RESOURCE_MANAGER_LOGWHENFOUND, true);

                /*
                *  now, is a global cache specified?
                */

                string cacheClassName = rsvc.GetString(RuntimeConstants.RESOURCE_MANAGER_CACHE_CLASS);

                object cacheObject = null;

                if (!string.IsNullOrEmpty(cacheClassName))
                {

                    try
                    {
                        cacheObject = System.Activator.CreateInstance(Type.GetType(cacheClassName.Replace(';', ',')));
                    }
                    catch (System.Exception cnfe)
                    {
                        string msg = "The specified class for ResourceCache (" + cacheClassName + ") does not exist or is not accessible to the current classloader.";
                        log.Error(msg, cnfe);
                        throw cnfe;
                    }

                    if (!(cacheObject is IResourceCache))
                    {
                        string msg = "The specified resource cache class (" + cacheClassName + ") must implement " + typeof(IResourceCache).FullName;
                        log.Error(msg);
                        throw new System.SystemException(msg);
                    }
                }

                /*
                *  if we didn't Get through that, just use the default.
                */
                if (cacheObject == null)
                {
                    cacheObject = new ResourceCacheImpl();
                }

                globalCache = (IResourceCache)cacheObject;

                globalCache.Initialize(rsvc);

                log.Trace("Default ResourceManager initialization complete.");
            }
        }

        /// <summary> This will produce a List of Hashtables, each hashtable contains the intialization Info for a particular resource loader. This
        /// Hashtable will be passed in when initializing the the template loader.
        /// </summary>
        private void AssembleResourceLoaderInitializers()
        {
            ArrayList resourceLoaderNames = rsvc.Configuration.GetVector(RuntimeConstants.RESOURCE_LOADER);
            StringUtils.TrimStrings(resourceLoaderNames);

            for (IEnumerator it = resourceLoaderNames.GetEnumerator(); it.MoveNext(); )
            {

                /*
                * The loader id might look something like the following:
                *
                * file.resource.loader
                *
                * The loader id is the prefix used for all properties
                * pertaining to a particular loader.
                */
                string loaderName = (string)it.Current;
                StringBuilder loaderID = new StringBuilder(loaderName);
                loaderID.Append(".").Append(RuntimeConstants.RESOURCE_LOADER);

                ExtendedProperties loaderConfiguration = rsvc.Configuration.Subset(loaderID.ToString());

                /*
                *  we can't really count on ExtendedProperties to give us an empty set
                */
                if (loaderConfiguration == null)
                {
                    log.Debug("ResourceManager : No configuration information found " + "for resource loader named '" + loaderName + "' (id is " + loaderID + "). Skipping it...");
                    continue;
                }

                /*
                *  Add the loader name token to the initializer if we need it
                *  for reference later. We can't count on the user to fill
                *  in the 'name' field
                */

                loaderConfiguration.SetProperty(RESOURCE_LOADER_IDENTIFIER, loaderName);

                /*
                * Add resources to the list of resource loader
                * initializers.
                */
                sourceInitializerList.Add(loaderConfiguration);
            }
        }

        /// <summary> Gets the named resource. Returned class type corresponds to specified type (i.e. <code>Template</code> to <code>
        /// RESOURCE_TEMPLATE</code>).
        /// 
        /// This method is now unsynchronized which requires that ResourceCache
        /// implementations be thread safe (as the default is).
        /// 
        /// </summary>
        /// <param name="resourceName"> The name of the resource to retrieve.
        /// </param>
        /// <param name="resourceType"> The type of resource (<code>RESOURCE_TEMPLATE</code>, <code>RESOURCE_CONTENT</code>, etc.).
        /// </param>
        /// <param name="encoding"> The character encoding to use.
        /// 
        /// </param>
        /// <returns>  Resource with the template parsed and ready.
        /// 
        /// </returns>
        /// <throws>   ResourceNotFoundException  if template not found from any available source. </throws>
        /// <throws>   ParseErrorException  if template cannot be parsed due to syntax (or other) Error. </throws>
        /// <throws>   Exception  if a problem in parse </throws>
        public virtual Resource GetResource(string resourceName, int resourceType, string encoding)
        {
            /*
            * Check to see if the resource was placed in the cache.
            * If it was placed in the cache then we will use
            * the cached version of the resource. If not we
            * will load it.
            *
            * Note: the type is included in the key to differentiate ContentResource
            * (static content from #include) with a Template.
            */

            string resourceKey = resourceType + resourceName;
            Resource resource = globalCache.Get(resourceKey);

            if (resource != null)
            {
                try
                {
                    // avoids additional method call to refreshResource
                    if (resource.RequiresChecking())
                    {
                        /*
                        * both loadResource() and refreshResource() now return
                        * a new Resource instance when they are called
                        * (Put in the cache when appropriate) in order to allow
                        * several threads to parse the same template simultaneously.
                        * It is redundant work and will cause more garbage collection but the
                        * benefit is that it allows concurrent parsing and processing
                        * without race conditions when multiple requests try to
                        * refresh/load the same template at the same time.
                        *
                        * Another alternative is to limit template parsing/retrieval
                        * so that only one thread can parse each template at a time
                        * but that creates a scalability bottleneck.
                        *
                        * See VELOCITY-606, VELOCITY-595 and VELOCITY-24
                        */
                        resource = RefreshResource(resource, encoding);
                    }
                }
                catch (ResourceNotFoundException)
                {
                    /*
                    *  something exceptional happened to that resource
                    *  this could be on purpose,
                    *  so clear the cache and try again
                    */

                    globalCache.Remove(resourceKey);

                    return GetResource(resourceName, resourceType, encoding);
                }
                catch (ParseErrorException pee)
                {
                    log.Error("ResourceManager.getResource() exception", pee);
                    throw pee;
                }
                catch (System.SystemException re)
                {
                    log.Error("ResourceManager.getResource() exception", re);
                    throw re;
                }
                catch (System.Exception e)
                {
                    log.Error("ResourceManager.getResource() exception", e);
                    throw e;
                }
            }
            else
            {
                try
                {
                    /*
                    *  it's not in the cache, so load it.
                    */
                    resource = LoadResource(resourceName, resourceType, encoding);

                    if (resource.ResourceLoader.CachingOn)
                    {
                        globalCache.Put(resourceKey, resource);
                    }
                }
                catch (ResourceNotFoundException rnfe)
                {
                    log.Error("ResourceManager : unable to find resource '" + resourceName + "' in any resource loader.");
                    throw rnfe;
                }
                catch (ParseErrorException pee)
                {
                    log.Error("ResourceManager.getResource() parse exception", pee);
                    throw pee;
                }
                catch (System.SystemException re)
                {
                    log.Error("ResourceManager.getResource() load exception", re);
                    throw re;
                }
                catch (System.Exception e)
                {
                    log.Error("ResourceManager.getResource() exception new", e);
                    throw e;
                }
            }

            return resource;
        }

        /// <summary> Create a new Resource of the specified type.
        /// 
        /// </summary>
        /// <param name="resourceName"> The name of the resource to retrieve.
        /// </param>
        /// <param name="resourceType"> The type of resource (<code>RESOURCE_TEMPLATE</code>, <code>RESOURCE_CONTENT</code>, etc.).
        /// </param>
        /// <returns>  new instance of appropriate resource type
        /// </returns>
        /// <since> 1.6
        /// </since>
        protected internal virtual Resource CreateResource(string resourceName, int resourceType)
        {
            return ResourceFactory.getResource(resourceName, resourceType);
        }

        /// <summary> Loads a resource from the current set of resource loaders.
        /// 
        /// </summary>
        /// <param name="resourceName"> The name of the resource to retrieve.
        /// </param>
        /// <param name="resourceType"> The type of resource (<code>RESOURCE_TEMPLATE</code>, <code>RESOURCE_CONTENT</code>, etc.).
        /// </param>
        /// <param name="encoding"> The character encoding to use.
        /// 
        /// </param>
        /// <returns>  Resource with the template parsed and ready.
        /// 
        /// </returns>
        /// <throws>   ResourceNotFoundException  if template not found from any available source. </throws>
        /// <throws>   ParseErrorException  if template cannot be parsed due to syntax (or other) Error. </throws>
        /// <throws>   Exception  if a problem in parse </throws>
        protected internal virtual Resource LoadResource(string resourceName, int resourceType, string encoding)
        {
            Resource resource = CreateResource(resourceName, resourceType);
            resource.RuntimeServices = rsvc;
            resource.Name = resourceName;
            resource.Encoding = encoding;

            /*
            * Now we have to try to find the appropriate
            * loader for this resource. We have to cycle through
            * the list of available resource loaders and see
            * which one gives us a stream that we can use to
            * make a resource with.
            */

            long howOldItWas = 0;

            for (IEnumerator<ResourceLoader> it = resourceLoaders.GetEnumerator(); it.MoveNext(); )
            {
                ResourceLoader resourceLoader = it.Current;
                resource.ResourceLoader = resourceLoader;

                /*
                *  catch the ResourceNotFound exception
                *  as that is ok in our new multi-loader environment
                */

                try
                {

                    if (resource.Process())
                    {
                        /*
                        *  FIXME  (gmj)
                        *  moved in here - technically still
                        *  a problem - but the resource needs to be
                        *  processed before the loader can figure
                        *  it out due to to the new
                        *  multi-path support - will revisit and fix
                        */

                        if (logWhenFound && log.DebugEnabled)
                        {
                            log.Debug("ResourceManager : found " + resourceName + " with loader " + resourceLoader.ClassName);
                        }

                        howOldItWas = resourceLoader.GetLastModified(resource);

                        break;
                    }
                }
                catch (ResourceNotFoundException)
                {
                    /*
                    *  that's ok - it's possible to fail in
                    *  multi-loader environment
                    */
                }
            }

            /*
            * Return null if we can't find a resource.
            */
            if (resource.Data == null)
            {
                throw new ResourceNotFoundException("Unable to find resource '" + resourceName + "'");
            }

            /*
            *  some final cleanup
            */

            resource.LastModified = howOldItWas;
            resource.ModificationCheckInterval = resource.ResourceLoader.ModificationCheckInterval;

            resource.Touch();

            return resource;
        }

        /// <summary> Takes an existing resource, and 'refreshes' it. This generally means that the source of the resource is checked for changes
        /// according to some cache/check algorithm and if the resource changed, then the resource data is reloaded and re-parsed.
        /// 
        /// </summary>
        /// <param name="resource"> resource to refresh
        /// </param>
        /// <param name="encoding"> character encoding of the resource to refresh.
        /// 
        /// </param>
        /// <throws>   ResourceNotFoundException  if template not found from current source for this Resource </throws>
        /// <throws>   ParseErrorException  if template cannot be parsed due to syntax (or other) Error. </throws>
        /// <throws>   Exception  if a problem in parse </throws>
        protected internal virtual Resource RefreshResource(Resource resource, string encoding)
        {
            /*
            * The resource knows whether it needs to be checked
            * or not, and the resource's loader can check to
            * see if the source has been modified. If both
            * these conditions are true then we must reload
            * the input stream and parse it to make a new
            * AST for the resource.
            */

            /*
            *  touch() the resource to reset the counters
            */
            resource.Touch();

            if (resource.IsSourceModified)
            {
                /*
                *  now check encoding Info.  It's possible that the newly declared
                *  encoding is different than the encoding already in the resource
                *  this strikes me as bad...
                */

                if (!string.Equals(resource.Encoding, encoding))
                {
                    log.Warn("Declared encoding for template '" + resource.Name + "' is different on reload. Old = '" + resource.Encoding + "' New = '" + encoding);

                    resource.Encoding = encoding;
                }

                /*
                *  read how old the resource is _before_
                *  processing (=>reading) it
                */
                long howOldItWas = resource.ResourceLoader.GetLastModified(resource);

                string resourceKey = resource.Type + resource.Name;

                /* 
                * we create a copy to avoid partially overwriting a
                * template which may be in use in another thread
                */

                Resource newResource = ResourceFactory.getResource(resource.Name, resource.Type);

                newResource.RuntimeServices = rsvc;
                newResource.Name = resource.Name;
                newResource.Encoding = resource.Encoding;
                newResource.ResourceLoader = resource.ResourceLoader;
                newResource.ModificationCheckInterval = resource.ResourceLoader.ModificationCheckInterval;

                newResource.Process();
                newResource.LastModified = howOldItWas;
                resource = newResource;

                globalCache.Put(resourceKey, newResource);
            }
            return resource;
        }

        /// <summary> Determines if a template exists, and returns name of the loader that provides it. This is a slightly less hokey way to
        /// support the Velocity.TemplateExists() utility method, which was broken when per-template encoding was introduced. We can
        /// revisit this.
        /// 
        /// </summary>
        /// <param name="resourceName"> Name of template or content resource
        /// 
        /// </param>
        /// <returns>  class name of loader than can provide it
        /// </returns>
        public virtual string GetLoaderNameForResource(string resourceName)
        {
            /*
            *  loop through our loaders...
            */
            for (IEnumerator<ResourceLoader> it = resourceLoaders.GetEnumerator(); it.MoveNext(); )
            {
                ResourceLoader resourceLoader = it.Current;
                if (resourceLoader.ResourceExists(resourceName))
                {
                    return resourceLoader.GetType().ToString();
                }
            }
            return null;
        }
    }
}