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
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    using Commons.Collections;
    using Exception;
    using NVelocity.Util;

    /// <summary> A loader for templates stored on the file system.  Treats the template
    /// as relative to the configured root path.  If the root path is empty
    /// treats the template name as an absolute path.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <author>  <a href="mailto:mailmur@yahoo.com">Aki Nieminen</a>
    /// </author>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <version>  $Id: FileResourceLoader.java 687518 2008-08-21 00:18:03Z nbubna $
    /// </version>
    public class FileResourceLoader : ResourceLoader
    {
        /// <summary> The paths to search for templates.</summary>
        private IList<string> paths = new List<string>();

        /// <summary> Used to map the path that a template was found on
        /// so that we can properly check the modification
        /// times of the files. This is synchronizedMap
        /// instance.
        /// </summary>
        private IDictionary templatePaths = new Hashtable();

        /// <summary>Shall we Inspect unicode files to see what encoding they contain?. </summary>
        private bool unicode = false;

        /// <seealso cref="org.apache.velocity.runtime.resource.loader.ResourceLoader.Init(org.apache.commons.collections.ExtendedProperties)">
        /// </seealso>
        public override void Init(ExtendedProperties configuration)
        {
            if (log.TraceEnabled)
            {
                log.Trace("FileResourceLoader : initialization starting.");
            }

            ArrayList vectors = configuration.GetVector("path");

            foreach (var o in vectors)
            {
                paths.Add(o.ToString());
            }

            // unicode files may have a BOM marker at the start, but Java
            // has problems recognizing the UTF-8 bom. Enabling unicode will
            // recognize all unicode boms.
            unicode = configuration.GetBoolean("unicode", false);

            if (log.DebugEnabled)
            {
                log.Debug("Do unicode file recognition:  " + unicode);
            }

            if (log.DebugEnabled)
            {
                // trim spaces from all paths
                StringUtils.TrimStrings(paths);

                // this section lets tell people what paths we will be using
                int sz = paths.Count;
                for (int i = 0; i < sz; i++)
                {
                    log.Debug("FileResourceLoader : adding path '" + ((string)paths[i]) + "'");
                }
                log.Trace("FileResourceLoader : initialization complete.");
            }
        }

        /// <summary> Get an InputStream so that the Runtime can build a
        /// template with it.
        /// 
        /// </summary>
        /// <param name="templateName">name of template to Get
        /// </param>
        /// <returns> InputStream containing the template
        /// </returns>
        /// <throws>  ResourceNotFoundException if template not found </throws>
        /// <summary>         in the file template path.
        /// </summary>
        public override Stream GetResourceStream(string templateName)
        {
            /*
            * Make sure we have a valid templateName.
            */
            if (string.IsNullOrEmpty(templateName))
            {
                /*
                * If we don't Get a properly formed templateName then
                * there's not much we can do. So we'll forget about
                * trying to search any more paths for the template.
                */
                throw new ResourceNotFoundException("Need to specify a file name or file path!");
            }

            string template = StringUtils.NormalizePath(templateName);
            if (template == null || template.Length == 0)
            {
                string msg = "File resource error : argument " + template + " contains .. and may be trying to access " + "content outside of template root.  Rejected.";

                log.Error("FileResourceLoader : " + msg);

                throw new ResourceNotFoundException(msg);
            }

            int size = paths.Count;
            for (int i = 0; i < size; i++)
            {
                string path = (string)paths[i];
                System.IO.Stream inputStream = null;

                try
                {
                    inputStream = FindTemplate(path, template);
                }
                catch (IOException ioe)
                {
                    string msg = "Exception while loading Template " + template;
                    log.Error(msg, ioe);
                    throw new VelocityException(msg, ioe);
                }

                if (inputStream != null)
                {
                    /*
                    * Store the path that this template came
                    * from so that we can check its modification
                    * time.
                    */
                    templatePaths[templateName] = path;
                    return inputStream;
                }
            }

            /*
            * We have now searched all the paths for
            * templates and we didn't find anything so
            * throw an exception.
            */
            throw new ResourceNotFoundException("FileResourceLoader : cannot find " + template);
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
            name = StringUtils.NormalizePath(name);
            if (name == null || name.Length == 0)
            {
                return false;
            }

            int size = paths.Count;
            for (int i = 0; i < size; i++)
            {
                string path = (string)paths[i];
                try
                {
                    FileInfo file = GetFile(path, name);

                    return file.Exists;
                }
                catch (System.Exception ioe)
                {
                    string msg = "Exception while checking for template " + name;
                    log.Debug(msg, ioe);
                }
            }
            return false;
        }

        /// <summary> Try to find a template given a normalized path.
        /// 
        /// </summary>
        /// <param name="path">a normalized path
        /// </param>
        /// <param name="template">name of template to find
        /// </param>
        /// <returns> InputStream input stream that will be parsed
        /// 
        /// </returns>
        private Stream FindTemplate(string path, string template)
        {
            try
            {
                FileInfo file = GetFile(path, template);

                if (file.Exists)
                {
                    return new BufferedStream(file.OpenRead());
                }
                else
                {
                    return null;
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                /*
                *  Log and convert to a general Velocity ResourceNotFoundException
                */
                return null;
            }
        }

        /// <summary> How to keep track of all the modified times
        /// across the paths.  Note that a file might have
        /// appeared in a directory which is earlier in the
        /// path; so we should search the path and see if
        /// the file we find that way is the same as the one
        /// that we have cached.
        /// </summary>
        /// <param name="resource">
        /// </param>
        /// <returns> True if the source has been modified.
        /// </returns>
        public override bool IsSourceModified(Resource resource)
        {
            /*
            * we assume that the file needs to be reloaded;
            * if we find the original file and it's unchanged,
            * then we'll flip this.
            */
            bool modified = true;

            string fileName = resource.Name;
            string path = (string)templatePaths[fileName];
            System.IO.FileInfo currentFile = null;

            for (int i = 0; currentFile == null && i < paths.Count; i++)
            {
                string testPath = (string)paths[i];
                System.IO.FileInfo testFile = GetFile(testPath, fileName);

                if (testFile.Exists)
                {
                    currentFile = testFile;
                }
            }
            System.IO.FileInfo file = GetFile(path, fileName);

            if (currentFile == null || !file.Exists)
            {
                /*
                * noop: if the file is missing now (either the cached
                * file is gone, or the file can no longer be found)
                * then we leave modified alone (it's set to true); a
                * reload attempt will be done, which will either use
                * a new template or fail with an appropriate message
                * about how the file couldn't be found.
                */
            }
            else
            {
                if (currentFile.Equals(file))
                {
                    /*
                    * if only if currentFile is the same as file and
                    * file.lastModified() is the same as
                    * resource.getLastModified(), then we should use the
                    * cached version.
                    */

                    modified = (((file.LastWriteTime.Ticks - 621355968000000000) / 10000) != resource.LastModified);
                }
            }

            /*
            * rsvc.Debug("isSourceModified for " + fileName + ": " + modified);
            */
            return modified;
        }

        /// <seealso cref="org.apache.velocity.runtime.resource.loader.ResourceLoader.getLastModified(org.apache.velocity.runtime.resource.Resource)">
        /// </seealso>
        public override long GetLastModified(Resource resource)
        {
            string path = (string)templatePaths[resource.Name];
            System.IO.FileInfo file = GetFile(path, resource.Name);

            try
            {
                return ((file.LastWriteTime.Ticks - 621355968000000000) / 10000);
            }
            catch
            {
                return 0;
            }
        }


        /// <summary> Create a File based on either a relative path if given, or absolute path otherwise</summary>
        private FileInfo GetFile(string path, string template)
        {
            FileInfo file = null;

            if (string.IsNullOrEmpty(path))
            {
                file = new FileInfo(template);
            }
            else
            {

                file = new FileInfo(path + Path.DirectorySeparatorChar + template);
            }

            return file;
        }
    }
}