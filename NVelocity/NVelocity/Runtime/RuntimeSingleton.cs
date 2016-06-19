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

namespace NVelocity.Runtime
{
    using App.Event;
    using Commons.Collections;
    using NVelocity.Util.Introspection;
    using Parser.Node;
    using Resource;

    /// <summary> This is the Runtime system for Velocity. It is the
    /// single access point for all functionality in Velocity.
    /// It adheres to the mediator pattern and is the only
    /// structure that developers need to be familiar with
    /// in order to Get Velocity to perform.
    /// 
    /// The Runtime will also cooperate with external
    /// systems like Turbine. Runtime properties can
    /// set and then the Runtime is initialized.
    /// 
    /// Turbine for example knows where the templates
    /// are to be loaded from, and where the velocity
    /// Log file should be placed.
    /// 
    /// So in the case of Velocity cooperating with Turbine
    /// the code might look something like the following:
    /// 
    /// <pre>
    /// RuntimeSingleton.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, templatePath);
    /// RuntimeSingleton.SetProperty(RuntimeConstants.RUNTIME_LOG, pathToVelocityLog);
    /// RuntimeSingleton.Init();
    /// </pre>
    /// 
    /// <pre>
    /// -----------------------------------------------------------------------
    /// N O T E S  O N  R U N T I M E  I N I T I A L I Z A T I O N
    /// -----------------------------------------------------------------------
    /// RuntimeSingleton.Init()
    /// 
    /// If Runtime.Init() is called by itself the Runtime will
    /// Initialize with a set of default values.
    /// -----------------------------------------------------------------------
    /// RuntimeSingleton.Init(String/Properties)
    /// 
    /// In this case the default velocity properties are layed down
    /// first to provide a solid base, then any properties provided
    /// in the given properties object will override the corresponding
    /// default property.
    /// -----------------------------------------------------------------------
    /// </pre>
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:jlb@houseofdistraction.com">Jeff Bowden</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magusson Jr.</a>
    /// </author>
    /// <author>  <a href="mailto:dlr@finemaltcoding.com">Daniel Rall</a>
    /// 
    /// </author>
    /// <seealso cref="org.apache.velocity.runtime.RuntimeInstance">
    /// 
    /// </seealso>
    /// <version>  $Id: RuntimeSingleton.java 685724 2008-08-13 23:12:12Z nbubna $
    /// </version>
    public class RuntimeSingleton
    {
        /// <summary> Returns true if the RuntimeInstance has been successfully initialized.</summary>
        /// <returns> True if the RuntimeInstance has been successfully initialized.
        /// </returns>
        /// <seealso cref="RuntimeInstance.isInitialized()">
        /// </seealso>
        /// <since> 1.5
        /// </since>
        public static bool Initialized
        {
            get
            {
                return ri.Initialized;
            }

        }
        /// <summary> Returns the RuntimeServices Instance used by this wrapper.
        /// 
        /// </summary>
        /// <returns> The RuntimeServices Instance used by this wrapper.
        /// </returns>
        public static IRuntimeServices RuntimeServices
        {
            get
            {
                return ri;
            }

        }

        /// <summary> Return the velocity runtime configuration object.
        /// 
        /// </summary>
        /// <returns> ExtendedProperties configuration object which houses
        /// the velocity runtime properties.
        /// </returns>
        /// <seealso cref="RuntimeInstance.getConfiguration()">
        /// </seealso>
        /// <summary> Allow an external system to set an ExtendedProperties
        /// object to use. This is useful where the external
        /// system also uses the ExtendedProperties class and
        /// the velocity configuration is a subset of
        /// parent application's configuration. This is
        /// the case with Turbine.
        /// 
        /// </summary>
        /// <param name="configuration">
        /// </param>
        /// <seealso cref="RuntimeInstance.setConfiguration(ExtendedProperties)">
        /// </seealso>
        public static ExtendedProperties Configuration
        {
            get
            {
                return ri.Configuration;
            }

            set
            {
                ri.Configuration = value;
            }

        }
        /// <summary> Returns a convenient LogMessage instance that wraps the current LogChute.
        /// 
        /// </summary>
        /// <returns> A convenience LogMessage instance that wraps the current LogChute.
        /// </returns>
        /// <seealso cref="RuntimeInstance.getLog()">
        /// </seealso>
        /// <since> 1.5
        /// </since>
        public static Log.Log Log
        {
            get
            {
                return ri.Log;
            }

        }
        /// <summary>  Return the Introspector for this RuntimeInstance
        /// 
        /// </summary>
        /// <returns> Introspector object for this runtime instance
        /// </returns>
        /// <seealso cref="RuntimeInstance.getIntrospector()">
        /// </seealso>
        public static Introspector Introspector
        {
            get
            {
                return ri.Introspector;
            }

        }
        /// <summary> Returns the event handlers for the application.</summary>
        /// <returns> The event handlers for the application.
        /// </returns>
        /// <seealso cref="RuntimeInstance.getApplicationEventCartridge()">
        /// </seealso>
        /// <since> 1.5
        /// </since>
        public EventCartridge EventCartridge
        {
            get
            {
                return ri.ApplicationEventCartridge;
            }

        }
        /// <summary> Returns the Uberspect object for this Instance.
        /// 
        /// </summary>
        /// <returns> The Uberspect object for this Instance.
        /// </returns>
        /// <seealso cref="org.apache.velocity.runtime.RuntimeServices.getUberspect()">
        /// </seealso>
        /// <seealso cref="RuntimeInstance.getUberspect()">
        /// </seealso>
        public static IUberspect Uberspect
        {
            get
            {
                return ri.Uberspect;
            }

        }

        private static RuntimeInstance ri = new RuntimeInstance();

        /// <summary> This is the primary initialization method in the Velocity
        /// Runtime. The systems that are setup/initialized here are
        /// as follows:
        /// 
        /// <ul>
        /// <li>Logging System</li>
        /// <li>ResourceManager</li>
        /// <li>Event Handlers</li>
        /// <li>Parser Pool</li>
        /// <li>Global Cache</li>
        /// <li>Static Content Include System</li>
        /// <li>Velocimacro System</li>
        /// </ul>
        /// </summary>
        /// <throws>  Exception When an Error occured during initialization. </throws>
        /// <seealso cref="RuntimeInstance.Init()">
        /// </seealso>

        public static void Init()
        {
            lock (typeof(RuntimeSingleton))
            {
                ri.Init();
            }
        }


        /// <summary> Allows an external system to set a property in
        /// the Velocity Runtime.
        /// 
        /// </summary>
        /// <param name="key">property key
        /// </param>
        /// <param name="value">property value
        /// </param>
        /// <seealso cref="RuntimeInstance.SetProperty(String, Object)">
        /// </seealso>
        public static void SetProperty(string key, object value)
        {
            ri.SetProperty(key, value);
        }

        /// <summary> Add a property to the configuration. If it already
        /// exists then the value stated here will be added
        /// to the configuration entry. For example, if
        /// 
        /// resource.loader = file
        /// 
        /// is already present in the configuration and you
        /// 
        /// addProperty("resource.loader", "classpath")
        /// 
        /// Then you will end up with a Vector like the
        /// following:
        /// 
        /// ["file", "classpath"]
        /// 
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="value">
        /// </param>
        /// <seealso cref="RuntimeInstance.addProperty(String, Object)">
        /// </seealso>
        public static void AddProperty(string key, object value)
        {
            ri.AddProperty(key, value);
        }

        /// <summary> Clear the values pertaining to a particular
        /// property.
        /// 
        /// </summary>
        /// <param name="key">of property to clear
        /// </param>
        /// <seealso cref="RuntimeInstance.clearProperty(String)">
        /// </seealso>
        public static void ClearProperty(string key)
        {
            ri.ClearProperty(key);
        }

        /// <summary>  Allows an external caller to Get a property.  The calling
        /// routine is required to know the type, as this routine
        /// will return an Object, as that is what properties can be.
        /// 
        /// </summary>
        /// <param name="key">property to return
        /// </param>
        /// <returns> Value of the property or null if it does not exist.
        /// </returns>
        /// <seealso cref="RuntimeInstance.GetProperty(String)">
        /// </seealso>
        public static object GetProperty(string key)
        {
            return ri.GetProperty(key);
        }

        /// <summary> Initialize the Velocity Runtime with a Properties
        /// object.
        /// 
        /// </summary>
        /// <param name="p">
        /// </param>
        /// <throws>  Exception When an Error occurs during initialization. </throws>
        /// <seealso cref="RuntimeInstance.Init(Properties)">
        /// </seealso>
        public static void Init(ExtendedProperties p)
        {
            ri.Init(p);
        }

        /// <summary> Initialize the Velocity Runtime with the name of
        /// ExtendedProperties object.
        /// 
        /// </summary>
        /// <param name="configurationFile">
        /// </param>
        /// <throws>  Exception When an Error occurs during initialization. </throws>
        /// <seealso cref="RuntimeInstance.Init(String)">
        /// </seealso>
        public static void Init(string configurationFile)
        {
            ri.Init(configurationFile);
        }

        /// <summary> Parse the input and return the root of
        /// AST node structure.
        /// <br><br>
        /// In the event that it runs out of parsers in the
        /// pool, it will create and let them be GC'd
        /// dynamically, logging that it has to do that.  This
        /// is considered an exceptional condition.  It is
        /// expected that the user will set the
        /// PARSER_POOL_SIZE property appropriately for their
        /// application.  We will revisit this.
        /// 
        /// </summary>
        /// <param name="reader">Reader retrieved by a resource loader
        /// </param>
        /// <param name="templateName">name of the template being parsed
        /// </param>
        /// <returns> A root node representing the template as an AST tree.
        /// </returns>
        /// <throws>  ParseException When the template could not be parsed. </throws>
        /// <seealso cref="RuntimeInstance.parse(Reader, String)">
        /// </seealso>

        public static SimpleNode Parse(System.IO.TextReader reader, string templateName)
        {
            return ri.Parse(reader, templateName);
        }

        /// <summary>  Parse the input and return the root of the AST node structure.
        /// 
        /// </summary>
        /// <param name="reader">Reader retrieved by a resource loader
        /// </param>
        /// <param name="templateName">name of the template being parsed
        /// </param>
        /// <param name="dumpNamespace">flag to dump the Velocimacro namespace for this template
        /// </param>
        /// <returns> A root node representing the template as an AST tree.
        /// </returns>
        /// <throws>  ParseException When the template could not be parsed. </throws>
        /// <seealso cref="RuntimeInstance.parse(Reader, String, boolean)">
        /// </seealso>
        public static SimpleNode Parse(System.IO.TextReader reader, string templateName, bool dumpNamespace)
        {
            return ri.Parse(reader, templateName, dumpNamespace);
        }


        /// <summary> Returns a <code>Template</code> from the resource manager.
        /// This method assumes that the character encoding of the
        /// template is set by the <code>input.encoding</code>
        /// property.  The default is "ISO-8859-1"
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
        /// <seealso cref="RuntimeInstance.GetTemplate(String)">
        /// </seealso>
        public static Template GetTemplate(string name)
        {
            return ri.GetTemplate(name);
        }

        /// <summary> Returns a <code>Template</code> from the resource manager
        /// 
        /// </summary>
        /// <param name="name">The  name of the desired template.
        /// </param>
        /// <param name="encoding">Character encoding of the template
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
        /// <throws>  ParseErrorException When the template could not be parsed. </throws>
        /// <throws>  Exception Any other Error. </throws>
        /// <seealso cref="RuntimeInstance.GetTemplate(String, String)">
        /// </seealso>
        public static Template GetTemplate(string name, string encoding)
        {
            return ri.GetTemplate(name, encoding);
        }

        /// <summary> Returns a static content resource from the
        /// resource manager.  Uses the current value
        /// if INPUT_ENCODING as the character encoding.
        /// 
        /// </summary>
        /// <param name="name">Name of content resource to Get
        /// </param>
        /// <returns> parsed ContentResource object ready for use
        /// </returns>
        /// <throws>  ResourceNotFoundException if template not found </throws>
        /// <summary>          from any available source.
        /// </summary>
        /// <throws>  ParseErrorException When the template could not be parsed. </throws>
        /// <throws>  Exception Any other Error. </throws>
        /// <seealso cref="RuntimeInstance.getContent(String)">
        /// </seealso>
        public static ContentResource GetContent(string name)
        {
            return ri.GetContent(name);
        }

        /// <summary> Returns a static content resource from the
        /// resource manager.
        /// 
        /// </summary>
        /// <param name="name">Name of content resource to Get
        /// </param>
        /// <param name="encoding">Character encoding to use
        /// </param>
        /// <returns> parsed ContentResource object ready for use
        /// </returns>
        /// <throws>  ResourceNotFoundException if template not found </throws>
        /// <summary>          from any available source.
        /// </summary>
        /// <throws>  ParseErrorException When the template could not be parsed. </throws>
        /// <throws>  Exception Any other Error. </throws>
        /// <seealso cref="RuntimeInstance.getContent(String, String)">
        /// </seealso>
        public static ContentResource GetContent(string name, string encoding)
        {
            return ri.GetContent(name, encoding);
        }


        /// <summary>  Determines is a template exists, and returns name of the loader that
        /// provides it.  This is a slightly less hokey way to support
        /// the Velocity.TemplateExists() utility method, which was broken
        /// when per-template encoding was introduced.  We can revisit this.
        /// 
        /// </summary>
        /// <param name="resourceName">Name of template or content resource
        /// </param>
        /// <returns> class name of loader than can provide it
        /// </returns>
        /// <seealso cref="RuntimeInstance.getLoaderNameForResource(String)">
        /// </seealso>
        public static string GetLoaderNameForResource(string resourceName)
        {
            return ri.GetLoaderNameForResource(resourceName);
        }

        /// <summary> String property accessor method with default to hide the
        /// configuration implementation.
        /// 
        /// </summary>
        /// <param name="key">property key
        /// </param>
        /// <param name="defaultValue"> default value to return if key not
        /// found in resource manager.
        /// </param>
        /// <returns> value of key or default
        /// </returns>
        /// <seealso cref="RuntimeInstance.getString(String, String)">
        /// </seealso>
        public static string GetString(string key, string defaultValue)
        {
            return ri.GetString(key, defaultValue);
        }

        /// <summary> Returns the appropriate VelocimacroProxy object if strVMname
        /// is a valid current Velocimacro.
        /// 
        /// </summary>
        /// <param name="vmName">Name of velocimacro requested
        /// </param>
        /// <param name="templateName">Name of the template that contains the velocimacro.
        /// </param>
        /// <returns> The requested VelocimacroProxy.
        /// </returns>
        /// <seealso cref="RuntimeInstance.getVelocimacro(String, String)">
        /// </seealso>
        public static Directive.Directive GetVelocimacro(string vmName, string templateName)
        {
            return ri.GetVelocimacro(vmName, templateName);
        }

        /// <summary> Adds a new Velocimacro. Usually called by Macro only while parsing.
        /// 
        /// </summary>
        /// <param name="name"> Name of a new velocimacro.
        /// </param>
        /// <param name="macro"> root AST node of the parsed macro
        /// </param>
        /// <param name="argArray"> Array of strings, containing the
        /// #macro() arguments.  the 0th argument is the name.
        /// </param>
        /// <param name="sourceTemplate">The template from which the macro is requested.
        /// </param>
        /// <returns> boolean  True if added, false if rejected for some
        /// reason (either parameters or permission settings)
        /// </returns>
        /// <seealso cref="RuntimeInstance.addVelocimacro(String, Node, String[], String)">
        /// </seealso>
        /// <since> 1.6
        /// </since>
        public static bool AddVelocimacro(string name, INode macro, string[] argArray, string sourceTemplate)
        {
            return ri.AddVelocimacro(name, macro, argArray, sourceTemplate);
        }

        /// <summary>  Checks to see if a VM exists
        /// 
        /// </summary>
        /// <param name="vmName">Name of the Velocimacro.
        /// </param>
        /// <param name="templateName">Template on which to look for the Macro.
        /// </param>
        /// <returns> True if VM by that name exists, false if not
        /// </returns>
        /// <seealso cref="RuntimeInstance.isVelocimacro(String, String)">
        /// </seealso>
        public static bool IsVelocimacro(string vmName, string templateName)
        {
            return ri.IsVelocimacro(vmName, templateName);
        }

        /// <summary> tells the vmFactory to dump the specified namespace.  This is to support
        /// clearing the VM list when in inline-VM-local-scope mode
        /// </summary>
        /// <param name="namespace">Namespace to dump.
        /// </param>
        /// <returns> True if namespace was dumped successfully.
        /// </returns>
        /// <seealso cref="RuntimeInstance.dumpVMNamespace(String)">
        /// </seealso>
        public static bool DumpVMNamespace(string namespace_Renamed)
        {
            return ri.DumpVMNamespace(namespace_Renamed);
        }

        /* --------------------------------------------------------------------
        * R U N T I M E  A C C E S S O R  M E T H O D S
        * --------------------------------------------------------------------
        * These are the getXXX() methods that are a simple wrapper
        * around the configuration object. This is an attempt
        * to make a the Velocity Runtime the single access point
        * for all things Velocity, and allow the Runtime to
        * adhere as closely as possible the the Mediator pattern
        * which is the ultimate goal.
        * --------------------------------------------------------------------
        */

        /// <summary> String property accessor method to hide the configuration implementation</summary>
        /// <param name="key"> property key
        /// </param>
        /// <returns>   value of key or null
        /// </returns>
        /// <seealso cref="RuntimeInstance.getString(String)">
        /// </seealso>
        public static string GetString(string key)
        {
            return ri.GetString(key);
        }

        /// <summary> Int property accessor method to hide the configuration implementation.
        /// 
        /// </summary>
        /// <param name="key">Property key
        /// </param>
        /// <returns> value
        /// </returns>
        /// <seealso cref="RuntimeInstance.getInt(String)">
        /// </seealso>
        public static int GetInt(string key)
        {
            return ri.GetInt(key);
        }

        /// <summary> Int property accessor method to hide the configuration implementation.
        /// 
        /// </summary>
        /// <param name="key"> property key
        /// </param>
        /// <param name="defaultValue">The default value.
        /// </param>
        /// <returns> value
        /// </returns>
        /// <seealso cref="RuntimeInstance.getInt(String, int)">
        /// </seealso>
        public static int GetInt(string key, int defaultValue)
        {
            return ri.GetInt(key, defaultValue);
        }

        /// <summary> Boolean property accessor method to hide the configuration implementation.
        /// 
        /// </summary>
        /// <param name="key">property key
        /// </param>
        /// <param name="def">The default value if property not found.
        /// </param>
        /// <returns> value of key or default value
        /// </returns>
        /// <seealso cref="RuntimeInstance.getBoolean(String, boolean)">
        /// </seealso>
        public static bool GetBoolean(string key, bool def)
        {
            return ri.GetBoolean(key, def);
        }

        /// <summary>  Gets the application attribute for the given key
        /// 
        /// </summary>
        /// <seealso cref="org.apache.velocity.runtime.RuntimeServices.getApplicationAttribute(Object)">
        /// </seealso>
        /// <param name="key">
        /// </param>
        /// <returns> The application attribute for the given key.
        /// </returns>
        /// <seealso cref="RuntimeInstance.getApplicationAttribute(Object)">
        /// </seealso>
        public static object GetApplicationAttribute(object key)
        {
            return ri.GetApplicationAttribute(key);
        }
    }
}