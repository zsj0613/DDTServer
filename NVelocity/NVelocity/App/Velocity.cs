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

namespace NVelocity.App
{
    using System;
    using System.IO;
    using System.Text;

    using Commons.Collections;
    using Context;
    using Exception;
    using Runtime;
    using Runtime.Log;

    /// <summary> This class provides  services to the application
    /// developer, such as :
    /// <ul>
    /// <li> Simple Velocity Runtime engine initialization methods.
    /// <li> Functions to apply the template engine to streams and strings
    /// to allow embedding and dynamic template generation.
    /// <li> Methods to access Velocimacros directly.
    /// </ul>
    /// 
    /// <br><br>
    /// While the most common way to use Velocity is via templates, as
    /// Velocity is a general-purpose template engine, there are other
    /// uses that Velocity is well suited for, such as processing dynamically
    /// created templates, or processing content streams.
    /// 
    /// <br><br>
    /// The methods herein were developed to allow easy access to the Velocity
    /// facilities without direct spelunking of the internals.  If there is
    /// something you feel is necessary to Add here, please, send a patch.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  <a href="mailto:Christoph.Reck@dlr.de">Christoph Reck</a>
    /// </author>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <version>  $Id: Velocity.java 687177 2008-08-19 22:00:32Z nbubna $
    /// </version>
    public class Velocity
    {
        /// <summary> Set an entire configuration at once. This is
        /// useful in cases where the parent application uses
        /// the ExtendedProperties class and the velocity configuration
        /// is a subset of the parent application's configuration.
        /// 
        /// </summary>
        /// <param name="configuration">A configuration object.
        /// 
        /// </param>
        public static ExtendedProperties ExtendedProperties
        {
            set
            {
                RuntimeSingleton.Configuration = value;
            }

        }
        /// <summary> Returns a convenient LogMessage instance that wraps the current LogChute.
        /// Use this to Log Error messages. It has the usual methods.
        /// 
        /// </summary>
        /// <returns> A convenience LogMessage instance that wraps the current LogChute.
        /// </returns>
        /// <since> 1.5
        /// </since>
        public static Log Log
        {
            get
            {
                return RuntimeSingleton.Log;
            }

        }
        /// <summary>  Initialize the Velocity runtime engine, using the default
        /// properties of the Velocity distribution
        /// 
        /// </summary>
        /// <throws>  Exception When an Error during initialization occurs. </throws>
        public static void Init()
        {
            RuntimeSingleton.Init();
        }

        /// <summary>  Initialize the Velocity runtime engine, using default properties
        /// plus the properties in the properties file passed in as the arg
        /// 
        /// </summary>
        /// <param name="propsFilename">file containing properties to use to Initialize
        /// the Velocity runtime
        /// </param>
        /// <throws>  Exception When an Error during initialization occurs. </throws>
        public static void Init(string propsFilename)
        {
            RuntimeSingleton.Init(propsFilename);
        }

        /// <summary>  Initialize the Velocity runtime engine, using default properties
        /// plus the properties in the passed in java.util.Properties object
        /// 
        /// </summary>
        /// <param name="p"> Properties object containing initialization properties
        /// </param>
        /// <throws>  Exception When an Error during initialization occurs. </throws>
        /// <summary> 
        /// </summary>

        public static void Init(ExtendedProperties p)
        {
            RuntimeSingleton.Init(p);
        }

        /// <summary> Set a Velocity Runtime property.
        /// 
        /// </summary>
        /// <param name="key">The property key.
        /// </param>
        /// <param name="value">The property value.
        /// </param>
        public static void SetProperty(string key, object value)
        {
            RuntimeSingleton.SetProperty(key, value);
        }

        /// <summary> Add a Velocity Runtime property.
        /// 
        /// </summary>
        /// <param name="key">The property key.
        /// </param>
        /// <param name="value">The property value.
        /// </param>
        public static void AddProperty(string key, object value)
        {
            RuntimeSingleton.AddProperty(key, value);
        }

        /// <summary> Clear a Velocity Runtime property.
        /// 
        /// </summary>
        /// <param name="key">of property to clear
        /// </param>
        public static void ClearProperty(string key)
        {
            RuntimeSingleton.ClearProperty(key);
        }

        /// <summary>  Get a Velocity Runtime property.
        /// 
        /// </summary>
        /// <param name="key">property to retrieve
        /// </param>
        /// <returns> property value or null if the property
        /// not currently set
        /// </returns>
        public static object GetProperty(string key)
        {
            return RuntimeSingleton.GetProperty(key);
        }

        /// <summary>  renders the input string using the context into the output writer.
        /// To be used when a template is dynamically constructed, or want to use
        /// Velocity as a token replacer.
        /// 
        /// </summary>
        /// <param name="context">context to use in rendering input string
        /// </param>
        /// <param name="out"> Writer in which to render the output
        /// </param>
        /// <param name="logTag"> string to be used as the template name for Log
        /// messages in case of Error
        /// </param>
        /// <param name="instring">input string containing the VTL to be rendered
        /// 
        /// </param>
        /// <returns> true if successful, false otherwise.  If false, see
        /// Velocity runtime Log
        /// </returns>
        /// <throws>  ParseErrorException The template could not be parsed. </throws>
        /// <throws>  MethodInvocationException A method on a context object could not be invoked. </throws>
        /// <throws>  ResourceNotFoundException A referenced resource could not be loaded. </throws>
        /// <throws>  IOException While rendering to the writer, an I/O problem occured. </throws>

        public static bool Evaluate(IContext context, TextWriter writer, string logTag, string instring)
        {
            return RuntimeSingleton.RuntimeServices.Evaluate(context, writer, logTag, instring);
        }

        /// <summary>  Renders the input reader using the context into the output writer.
        /// To be used when a template is dynamically constructed, or want to
        /// use Velocity as a token replacer.
        /// 
        /// </summary>
        /// <param name="context">context to use in rendering input string
        /// </param>
        /// <param name="writer"> Writer in which to render the output
        /// </param>
        /// <param name="logTag"> string to be used as the template name for Log messages
        /// in case of Error
        /// </param>
        /// <param name="reader">Reader containing the VTL to be rendered
        /// 
        /// </param>
        /// <returns> true if successful, false otherwise.  If false, see
        /// Velocity runtime Log
        /// </returns>
        /// <throws>  ParseErrorException The template could not be parsed. </throws>
        /// <throws>  MethodInvocationException A method on a context object could not be invoked. </throws>
        /// <throws>  ResourceNotFoundException A referenced resource could not be loaded. </throws>
        /// <throws>  IOException While reading from the reader or rendering to the writer, </throws>
        /// <summary>                     an I/O problem occured.
        /// </summary>
        /// <since> Velocity v1.1
        /// </since>

        public static bool Evaluate(IContext context, TextWriter writer, string logTag, TextReader reader)
        {
            return RuntimeSingleton.RuntimeServices.Evaluate(context, writer, logTag, reader);
        }

        /// <summary> Invokes a currently registered Velocimacro with the params provided
        /// and places the rendered stream into the writer.
        /// <br>
        /// Note : currently only accepts args to the VM if they are in the context.
        /// 
        /// </summary>
        /// <param name="vmName">name of Velocimacro to call
        /// </param>
        /// <param name="logTag">string to be used for template name in case of Error. if null,
        /// the vmName will be used
        /// </param>
        /// <param name="params">keys for args used to invoke Velocimacro, in java format
        /// rather than VTL (eg  "foo" or "bar" rather than "$foo" or "$bar")
        /// </param>
        /// <param name="context">Context object containing data/objects used for rendering.
        /// </param>
        /// <param name="writer"> Writer for output stream
        /// </param>
        /// <returns> true if Velocimacro exists and successfully invoked, false otherwise.
        /// </returns>

        public static bool InvokeVelocimacro(string vmName, string logTag, string[] parameters, IContext context, TextWriter writer)
        {
            try
            {
                return RuntimeSingleton.RuntimeServices.InvokeVelocimacro(vmName, logTag, parameters, context, writer);
            }
            catch (IOException ioe)
            {
                string msg = "Velocity.InvokeVelocimacro(" + vmName + ") failed";
                Log.Error(msg, ioe);
                throw new VelocityException(msg, ioe);
            }
        }

        /// <summary>  merges a template and puts the rendered stream into the writer
        /// 
        /// </summary>
        /// <param name="templateName">name of template to be used in merge
        /// </param>
        /// <param name="encoding">encoding used in template
        /// </param>
        /// <param name="context"> filled context to be used in merge
        /// </param>
        /// <param name="writer"> writer to write template into
        /// 
        /// </param>
        /// <returns> true if successful, false otherwise.  Errors
        /// logged to velocity Log
        /// 
        /// </returns>
        /// <throws>  ParseErrorException The template could not be parsed. </throws>
        /// <throws>  MethodInvocationException A method on a context object could not be invoked. </throws>
        /// <throws>  ResourceNotFoundException A referenced resource could not be loaded. </throws>
        /// <throws>  Exception Any other exception. </throws>
        /// <summary> 
        /// </summary>
        /// <since> Velocity v1.1
        /// </since>

        public static bool MergeTemplate(string templateName, string encoding, IContext context, TextWriter writer)
        {
            Template template = RuntimeSingleton.GetTemplate(templateName, encoding);

            if (template == null)
            {
                string msg = "Velocity.mergeTemplate() was unable to load template '" + templateName + "'";
                Log.Error(msg);
                throw new ResourceNotFoundException(msg);
            }
            else
            {
                template.Merge(context, writer);

                return true;
            }
        }

        /// <summary>  Returns a <code>Template</code> from the Velocity
        /// resource management system.
        /// 
        /// </summary>
        /// <param name="name">The file name of the desired template.
        /// </param>
        /// <returns>     The template.
        /// </returns>
        /// <throws>  ResourceNotFoundException if template not found </throws>
        /// <summary>          from any available source.
        /// </summary>
        /// <throws>  ParseErrorException if template cannot be parsed due </throws>
        /// <summary>          to syntax (or other) Error.
        /// </summary>
        /// <throws>  Exception if an Error occurs in template initialization </throws>
        public static Template GetTemplate(string name)
        {
            return RuntimeSingleton.GetTemplate(name);
        }

        /// <summary>  Returns a <code>Template</code> from the Velocity
        /// resource management system.
        /// 
        /// </summary>
        /// <param name="name">The file name of the desired template.
        /// </param>
        /// <param name="encoding">The character encoding to use for the template.
        /// </param>
        /// <returns>     The template.
        /// </returns>
        /// <throws>  ResourceNotFoundException if template not found </throws>
        /// <summary>          from any available source.
        /// </summary>
        /// <throws>  ParseErrorException if template cannot be parsed due </throws>
        /// <summary>          to syntax (or other) Error.
        /// </summary>
        /// <throws>  Exception if an Error occurs in template initialization </throws>
        /// <summary> 
        /// </summary>
        /// <since> Velocity v1.1
        /// </since>
        public static Template GetTemplate(string name, string encoding)
        {
            return RuntimeSingleton.GetTemplate(name, encoding);
        }

        /// <summary> <p>Determines whether a resource is accessable via the
        /// currently configured resource loaders.  {@link
        /// org.apache.velocity.runtime.resource.Resource} is the generic
        /// description of templates, static content, etc.</p>
        /// 
        /// <p>Note that the current implementation will <b>not</b> change
        /// the state of the system in any real way - so this cannot be
        /// used to pre-load the resource cache, as the previous
        /// implementation did as a side-effect.</p>
        /// 
        /// </summary>
        /// <param name="resourceName">The name of the resource to search for.
        /// </param>
        /// <returns> Whether the resource was located.
        /// </returns>
        public static bool ResourceExists(string resourceName)
        {
            return (RuntimeSingleton.GetLoaderNameForResource(resourceName) != null);
        }

        /// <summary>  <p>
        /// Set the an ApplicationAttribue, which is an Object
        /// set by the application which is accessable from
        /// any component of the system that gets a RuntimeServices.
        /// This allows communication between the application
        /// environment and custom pluggable components of the
        /// Velocity engine, such as loaders and loggers.
        /// </p>
        /// 
        /// <p>
        /// Note that there is no enfocement or rules for the key
        /// used - it is up to the application developer.  However, to
        /// help make the intermixing of components possible, using
        /// the target Class name (e.g.  com.foo.bar ) as the key
        /// might help avoid collision.
        /// </p>
        /// 
        /// </summary>
        /// <param name="key">object 'name' under which the object is stored
        /// </param>
        /// <param name="value">object to store under this key
        /// </param>
        public static void SetApplicationAttribute(object key, object value)
        {
            RuntimeSingleton.RuntimeServices.SetApplicationAttribute(key, value);
        }
    }
}