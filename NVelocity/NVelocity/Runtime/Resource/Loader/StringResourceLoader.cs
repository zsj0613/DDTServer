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

    using Commons.Collections;
    using Exception;
    using Util;


    /// <summary> Resource loader that works with Strings. Users should manually Add
    /// resources to the repository that is used by the resource loader instance.
    /// 
    /// Below is an example configuration for this loader.
    /// Note that 'repository.class' is not necessary;
    /// if not provided, the factory will fall back on using 
    /// {@link StringResourceRepositoryImpl} as the default.
    /// <pre>
    /// resource.loader = string
    /// string.resource.loader.description = Velocity StringResource loader
    /// string.resource.loader.class = org.apache.velocity.runtime.resource.loader.StringResourceLoader
    /// string.resource.loader.repository.class = org.apache.velocity.runtime.resource.loader.StringResourceRepositoryImpl
    /// </pre>
    /// Resources can be added to the repository like this:
    /// <pre><code>
    /// StringResourceRepository repo = StringResourceLoader.GetRepository();
    /// 
    /// String myTemplateName = "/some/imaginary/path/hello.vm";
    /// String myTemplate = "Hi, ${username}... this is some template!";
    /// repo.PutStringResource(myTemplateName, myTemplate);
    /// </code></pre>
    /// 
    /// After this, the templates can be retrieved as usual.
    /// <br>
    /// <p>If there will be multiple StringResourceLoaders used in an application,
    /// you should consider specifying a 'string.resource.loader.repository.name = foo'
    /// property in order to keep you string resources in a non-default repository.
    /// This can help to avoid conflicts between different frameworks or components
    /// that are using StringResourceLoader.
    /// You can then retrieve your named repository like this:
    /// <pre><code>
    /// StringResourceRepository repo = StringResourceLoader.GetRepository("foo");
    /// </code></pre>
    /// and Add string resources to the repo just as in the previous example.
    /// </p>
    /// <p>If you have concerns about memory leaks or for whatever reason do not wish
    /// to have your string repository stored statically as a class member, then you
    /// should set 'string.resource.loader.repository.static = false' in your properties.
    /// This will tell the resource loader that the string repository should be stored
    /// in the Velocity application attributes.  To retrieve the repository, do:
    /// <pre><code>
    /// StringResourceRepository repo = velocityEngine.getApplicationAttribute("foo");
    /// </code></pre>
    /// If you did not specify a name for the repository, then it will be stored under the
    /// class name of the repository implementation class (for which the default is 
    /// 'org.apache.velocity.runtime.resource.util.StringResourceRepositoryImpl'). 
    /// Incidentally, this is also true for the default statically stored repository.
    /// </p>
    /// <p>Whether your repository is stored statically or in Velocity's application
    /// attributes, you can also manually create and set it prior to Velocity
    /// initialization.  For a static repository, you can do something like this:
    /// <pre><code>
    /// StringResourceRepository repo = new MyStringResourceRepository();
    /// repo.magicallyAddSomeStringResources();
    /// StringResourceLoader.SetRepository("foo", repo);
    /// </code></pre>
    /// Or for a non-static repository:
    /// <pre><code>
    /// StringResourceRepository repo = new MyStringResourceRepository();
    /// repo.magicallyAddSomeStringResources();
    /// velocityEngine.SetApplicationAttribute("foo", repo);
    /// </code></pre>
    /// Then, assuming the 'string.resource.loader.repository.name' property is
    /// set to 'some.name', the StringResourceLoader will use that already created
    /// repository, rather than creating a new one.
    /// </p>
    /// 
    /// </summary>
    /// <author>  <a href="mailto:eelco.hillenius@openedge.nl">Eelco Hillenius</a>
    /// </author>
    /// <author>  <a href="mailto:henning@apache.org">Henning P. Schmiedehausen</a>
    /// </author>
    /// <author>  Nathan Bubna
    /// </author>
    /// <version>  $Id: StringResourceLoader.java 687518 2008-08-21 00:18:03Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class StringResourceLoader : ResourceLoader
    {
        /// <summary> Key to determine whether the repository should be set as the static one or not.</summary>
        /// <since> 1.6
        /// </since>
        public const string REPOSITORY_STATIC = "repository.static";

        /// <summary> By default, repositories are stored statically (shared across the VM).</summary>
        /// <since> 1.6
        /// </since>
        public const bool REPOSITORY_STATIC_DEFAULT = true;

        /// <summary>Key to look up the repository implementation class. </summary>
        public const string REPOSITORY_CLASS = "repository.class";

        /// <summary>The default implementation class. </summary>
        public static readonly string REPOSITORY_CLASS_DEFAULT;

        /// <summary> Key to look up the name for the repository to be used.</summary>
        /// <since> 1.6
        /// </since>
        public const string REPOSITORY_NAME = "repository.name";

        /// <summary>The default name for string resource repositories
        /// ('org.apache.velocity.runtime.resource.util.StringResourceRepository').
        /// </summary>
        /// <since> 1.6
        /// </since>
        public static readonly string REPOSITORY_NAME_DEFAULT;

        /// <summary>Key to look up the repository char encoding. </summary>
        public const string REPOSITORY_ENCODING = "repository.encoding";

        /// <summary>The default repository encoding. </summary>
        public const string REPOSITORY_ENCODING_DEFAULT = "UTF-8";


        protected internal static readonly System.Collections.IDictionary STATIC_REPOSITORIES = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());

        /// <summary> Returns a reference to the default static repository.</summary>
        public static IStringResourceRepository GetRepository()
        {
            return GetRepository(REPOSITORY_NAME_DEFAULT);
        }

        /// <summary> Returns a reference to the repository stored statically under the
        /// specified name.
        /// </summary>
        /// <since> 1.6
        /// </since>
        public static IStringResourceRepository GetRepository(string name)
        {
            return (IStringResourceRepository)STATIC_REPOSITORIES[name];
        }

        /// <summary> Sets the specified {@link StringResourceRepository} in static storage
        /// under the specified name.
        /// </summary>
        /// <since> 1.6
        /// </since>
        public static void SetRepository(string name, IStringResourceRepository repo)
        {
            STATIC_REPOSITORIES[name] = repo;
        }

        /// <summary> Removes the {@link StringResourceRepository} stored under the specified
        /// name.
        /// </summary>
        /// <since> 1.6
        /// </since>
        public static IStringResourceRepository RemoveRepository(string name)
        {
            object tempObject;
            tempObject = STATIC_REPOSITORIES[name];
            STATIC_REPOSITORIES.Remove(name);
            return (IStringResourceRepository)tempObject;
        }

        /// <summary> Removes all statically stored {@link StringResourceRepository}s.</summary>
        /// <since> 1.6
        /// </since>
        public static void ClearRepositories()
        {
            STATIC_REPOSITORIES.Clear();
        }


        // the repository used internally by this resource loader
        protected internal IStringResourceRepository repository;


        /// <seealso cref="org.apache.velocity.runtime.resource.loader.ResourceLoader.Init(org.apache.commons.collections.ExtendedProperties)">
        /// </seealso>
        public override void Init(ExtendedProperties configuration)
        {
            log.Trace("StringResourceLoader : initialization starting.");

            // Get the repository configuration Info
            string repoClass = configuration.GetString(REPOSITORY_CLASS, REPOSITORY_CLASS_DEFAULT);
            string repoName = configuration.GetString(REPOSITORY_NAME, REPOSITORY_NAME_DEFAULT);
            bool isStatic = configuration.GetBoolean(REPOSITORY_STATIC, REPOSITORY_STATIC_DEFAULT);
            string encoding = configuration.GetString(REPOSITORY_ENCODING);

            // look for an existing repository of that name and isStatic setting
            if (isStatic)
            {
                this.repository = GetRepository(repoName);
                if (repository != null && log.DebugEnabled)
                {
                    log.Debug("Loaded repository '" + repoName + "' from static repo store");
                }
            }
            else
            {
                this.repository = (IStringResourceRepository)rsvc.GetApplicationAttribute(repoName);
                if (repository != null && log.DebugEnabled)
                {
                    log.Debug("Loaded repository '" + repoName + "' from application attributes");
                }
            }

            if (this.repository == null)
            {
                // since there's no repository under the repo name, create a new one
                this.repository = CceateRepository(repoClass, encoding);

                // and store it according to the isStatic setting
                if (isStatic)
                {
                    SetRepository(repoName, this.repository);
                }
                else
                {
                    rsvc.SetApplicationAttribute(repoName, this.repository);
                }
            }
            else
            {
                // ok, we already have a repo
                // Warn them if they are trying to change the class of the repository
                if (!this.repository.GetType().FullName.Equals(repoClass))
                {
                    log.Debug("Cannot change class of string repository '" + repoName + "' from " + this.repository.GetType().FullName + " to " + repoClass + ". The change will be ignored.");
                }

                // allow them to change the default encoding of the repo
                if (encoding != null && !this.repository.Encoding.Equals(encoding))
                {
                    if (log.DebugEnabled)
                    {
                        log.Debug("Changing the default encoding of string repository '" + repoName + "' from " + this.repository.Encoding + " to " + encoding);
                    }
                    this.repository.Encoding = encoding;
                }
            }

            log.Trace("StringResourceLoader : initialization complete.");
        }

        /// <since> 1.6
        /// </since>
        public virtual IStringResourceRepository CceateRepository(string className, string encoding)
        {
            if (log.DebugEnabled)
            {
                log.Debug("Creating string repository using class " + className + "...");
            }

            IStringResourceRepository repo;
            try
            {
                repo = (IStringResourceRepository)System.Activator.CreateInstance(Type.GetType(className));
            }
            catch (VelocityException cnfe)
            {
                throw new VelocityException("Could not find '" + className + "'", cnfe);
            }
            catch (System.UnauthorizedAccessException iae)
            {
                throw new VelocityException("Could not access '" + className + "'", iae);
            }
            catch (System.Exception ie)
            {
                throw new VelocityException("Could not instantiate '" + className + "'", ie);
            }

            if (encoding != null)
            {
                repo.Encoding = encoding;
            }
            else
            {
                repo.Encoding = REPOSITORY_ENCODING_DEFAULT;
            }

            if (log.DebugEnabled)
            {
                log.Debug("Default repository encoding is " + repo.Encoding);
            }
            return repo;
        }

        /// <summary> Overrides superclass for better performance.</summary>
        /// <since> 1.6
        /// </since>
        public override bool ResourceExists(string name)
        {
            if (name == null)
            {
                return false;
            }
            return (this.repository.GetStringResource(name) != null);
        }

        /// <summary> Get an InputStream so that the Runtime can build a
        /// template with it.
        /// 
        /// </summary>
        /// <param name="name">name of template to Get.
        /// </param>
        /// <returns> InputStream containing the template.
        /// </returns>
        /// <throws>  ResourceNotFoundException Ff template not found </throws>
        /// <summary>         in the RepositoryFactory.
        /// </summary>
        public override System.IO.Stream GetResourceStream(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ResourceNotFoundException("No template name provided");
            }

            StringResource resource = this.repository.GetStringResource(name);

            if (resource == null)
            {
                throw new ResourceNotFoundException("Could not locate resource '" + name + "'");
            }

            try
            {
                return new System.IO.MemoryStream(System.Text.Encoding.GetEncoding(resource.Encoding).GetBytes(resource.Body));
            }
            catch (System.IO.IOException ue)
            {
                throw new VelocityException("Could not convert String using encoding " + resource.Encoding, ue);
            }
        }

        /// <seealso cref="org.apache.velocity.runtime.resource.loader.ResourceLoader.isSourceModified(org.apache.velocity.runtime.resource.Resource)">
        /// </seealso>
        public override bool IsSourceModified(Resource resource)
        {
            StringResource original = null;
            bool result = true;

            original = this.repository.GetStringResource(resource.Name);

            if (original != null)
            {
                result = original.LastModified != resource.LastModified;
            }

            return result;
        }

        /// <seealso cref="org.apache.velocity.runtime.resource.loader.ResourceLoader.getLastModified(org.apache.velocity.runtime.resource.Resource)">
        /// </seealso>
        public override long GetLastModified(Resource resource)
        {
            StringResource original = null;

            original = this.repository.GetStringResource(resource.Name);

            return (original != null) ? original.LastModified : 0;
        }
        static StringResourceLoader()
        {
            REPOSITORY_CLASS_DEFAULT = typeof(StringResourceRepositoryImpl).FullName;
            REPOSITORY_NAME_DEFAULT = typeof(IStringResourceRepository).FullName;
        }
    }
}
