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

    using Commons.Collections;
    using Context;
    using Exception;
    using Runtime;
    using Runtime.Log;

    /// <summary> <p>
    /// This class provides a separate new-able instance of the
    /// Velocity template engine.  The alternative model for use
    /// is using the Velocity class which employs the singleton
    /// model.
    /// </p>
    /// <p>Velocity will call
    /// the parameter-less Init() at the first use of this class
    /// if the Init() wasn't explicitly called.  While this will
    /// ensure that Velocity functions, it probably won't
    /// function in the way you intend, so it is strongly recommended that
    /// you call an Init() method yourself if you use the default constructor.
    /// </p>
    /// 
    /// </summary>
    /// <version>  $Id: VelocityEngine.java 687177 2008-08-19 22:00:32Z nbubna $
    /// </version>
    public class VelocityEngine
    {
        /// <summary> Set an entire configuration at once. This is
        /// useful in cases where the parent application uses
        /// the ExtendedProperties class and the velocity configuration
        /// is a subset of the parent application's configuration.
        /// 
        /// </summary>
        /// <param name="configuration">*
        /// </param>
        public virtual ExtendedProperties ExtendedProperties
        {
            set
            {
                ri.Configuration = value;
            }

        }
        /// <summary> Returns a convenient LogMessage instance that wraps the current LogChute.
        /// Use this to Log Error messages. It has the usual methods you'd expect.
        /// </summary>
        /// <returns> A Log object.
        /// </returns>
        /// <since> 1.5
        /// </since>
        virtual public Log Log
        {
            get
            {
                return ri.Log;
            }

        }
        private RuntimeInstance ri = new RuntimeInstance();


        /// <summary>  Init-less CTOR</summary>
        public VelocityEngine()
        {
            // do nothing
        }

        /// <summary>  CTOR that invokes an Init(String), initializing
        /// the engine using the properties file specified
        /// 
        /// </summary>
        /// <param name="propsFilename">name of properties file to Init with
        /// </param>
        /// <throws>  Exception </throws>
        /// <since> 1.5
        /// </since>
        public VelocityEngine(string propsFilename)
        {
            ri.Init(propsFilename);
        }

        /// <summary>  CTOR that invokes an Init(String), initializing
        /// the engine using the Properties specified
        /// 
        /// </summary>
        /// <param name="p">name of properties  to Init with
        /// </param>
        /// <throws>  Exception </throws>
        /// <since> 1.5
        /// </since>

        public VelocityEngine(ExtendedProperties p)
        {
            ri.Init(p);
        }

        /// <summary>  Initialize the Velocity runtime engine, using the default
        /// properties of the Velocity distribution
        /// </summary>
        /// <throws>  Exception </throws>
        public virtual void Init()
        {
            ri.Init();
        }

        /// <summary>  Initialize the Velocity runtime engine, using default properties
        /// plus the properties in the properties file passed in as the arg
        /// 
        /// </summary>
        /// <param name="propsFilename">file containing properties to use to Initialize
        /// the Velocity runtime
        /// </param>
        /// <throws>  Exception </throws>
        public virtual void Init(string propsFilename)
        {
            ri.Init(propsFilename);
        }

        /// <summary>  Initialize the Velocity runtime engine, using default properties
        /// plus the properties in the passed in java.util.Properties object
        /// 
        /// </summary>
        /// <param name="p"> Proprties object containing initialization properties
        /// </param>
        /// <throws>  Exception </throws>
        /// <summary> 
        /// </summary>

        public virtual void Init(ExtendedProperties p)
        {
            ri.Init(p);
        }

        /// <summary> Set a Velocity Runtime property.
        /// 
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="value">
        /// </param>
        public virtual void SetProperty(string key, object value)
        {
            ri.SetProperty(key, value);
        }

        /// <summary> Add a Velocity Runtime property.
        /// 
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="value">
        /// </param>
        public virtual void AddProperty(string key, object value)
        {
            ri.AddProperty(key, value);
        }

        /// <summary> Clear a Velocity Runtime property.
        /// 
        /// </summary>
        /// <param name="key">of property to clear
        /// </param>
        public virtual void ClearProperty(string key)
        {
            ri.ClearProperty(key);
        }

        /// <summary>  Get a Velocity Runtime property.
        /// 
        /// </summary>
        /// <param name="key">property to retrieve
        /// </param>
        /// <returns> property value or null if the property
        /// not currently set
        /// </returns>
        public virtual object GetProperty(string key)
        {
            return ri.GetProperty(key);
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

        public virtual bool Evaluate(IContext context, TextWriter writer, string logTag, string instring)
        {
            return ri.Evaluate(context, writer, logTag, instring);
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

        public virtual bool Evaluate(IContext context, TextWriter writer, string logTag, TextReader reader)
        {
            return ri.Evaluate(context, writer, logTag, reader);
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
        /// <throws>  IOException While rendering to the writer, an I/O problem occured. </throws>

        public virtual bool InvokeVelocimacro(string vmName, string logTag, string[] parameters, IContext context, TextWriter writer)
        {
            return ri.InvokeVelocimacro(vmName, logTag, parameters, context, writer);
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
        /// </returns>
        /// <throws>  ResourceNotFoundException </throws>
        /// <throws>  ParseErrorException </throws>
        /// <throws>  MethodInvocationException </throws>
        /// <throws>  Exception </throws>
        /// <summary> 
        /// </summary>
        /// <since> Velocity v1.1
        /// </since>

        public virtual bool MergeTemplate(string templateName, string encoding, IContext context, TextWriter writer)
        {
            Template template = ri.GetTemplate(templateName, encoding);

            if (template == null)
            {
                string msg = "VelocityEngine.mergeTemplate() was unable to load template '" + templateName + "'";
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
        public virtual Template GetTemplate(string name)
        {
            return ri.GetTemplate(name);
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
        public virtual Template GetTemplate(string name, string encoding)
        {
            return ri.GetTemplate(name, encoding);
        }

        /// <summary>   Determines if a resource is accessable via the currently
        /// configured resource loaders.
        /// <br><br>
        /// Note that the current implementation will <b>not</b>
        /// change the state of the system in any real way - so this
        /// cannot be used to pre-load the resource cache, as the
        /// previous implementation did as a side-effect.
        /// <br><br>
        /// The previous implementation exhibited extreme lazyness and
        /// sloth, and the author has been flogged.
        /// 
        /// </summary>
        /// <param name="resourceName"> name of the resource to search for
        /// </param>
        /// <returns> true if found, false otherwise
        /// </returns>
        /// <since> 1.5
        /// </since>
        public virtual bool ResourceExists(string resourceName)
        {
            return (ri.GetLoaderNameForResource(resourceName) != null);
        }

        /// <summary>  <p>
        /// Sets an application attribute (which can be any Object) that will be
        /// accessible from any component of the system that gets a
        /// RuntimeServices. This allows communication between the application
        /// environment and custom pluggable components of the Velocity engine,
        /// such as ResourceLoaders and LogChutes.
        /// </p>
        /// 
        /// <p>
        /// Note that there is no enforcement or rules for the key
        /// used - it is up to the application developer.  However, to
        /// help make the intermixing of components possible, using
        /// the target Class name (e.g. com.foo.bar ) as the key
        /// might help avoid collision.
        /// </p>
        /// 
        /// </summary>
        /// <param name="key">object 'name' under which the object is stored
        /// </param>
        /// <param name="value">object to store under this key
        /// </param>
        public virtual void SetApplicationAttribute(object key, object value)
        {
            ri.SetApplicationAttribute(key, value);
        }

        /// <summary>  <p>
        /// Return an application attribute (which can be any Object)
        /// that was set by the application in order to be accessible from
        /// any component of the system that gets a RuntimeServices.
        /// This allows communication between the application
        /// environment and custom pluggable components of the
        /// Velocity engine, such as ResourceLoaders and LogChutes.
        /// </p>
        /// 
        /// </summary>
        /// <param name="key">object 'name' under which the object is stored
        /// </param>
        /// <returns> value object to store under this key
        /// </returns>
        /// <since> 1.5
        /// </since>
        public virtual object GetApplicationAttribute(object key)
        {
            return ri.GetApplicationAttribute(key);
        }
    }
}