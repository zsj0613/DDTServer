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
    using System.IO;

    using App.Event;
    using Commons.Collections;
    using Context;

    using Directive;
    using NVelocity.Util.Introspection;
    using Parser.Node;
    using Resource;

    /// <summary> Interface for internal runtime services that are needed by the
    /// various components w/in Velocity.  This was taken from the old
    /// Runtime singleton, and anything not necessary was removed.
    /// 
    /// Currently implemented by RuntimeInstance.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magusson Jr.</a>
    /// </author>
    /// <version>  $Id: RuntimeServices.java 685724 2008-08-13 23:12:12Z nbubna $
    /// </version>
    public interface IRuntimeServices
    {
        /// <summary> Return the velocity runtime configuration object.
        /// 
        /// </summary>
        /// <returns> ExtendedProperties configuration object which houses
        /// the velocity runtime properties.
        /// </returns>
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
        ExtendedProperties Configuration
        {
            get;

            set;

        }
        /// <summary> Returns the configured class introspection/reflection
        /// implementation.
        /// </summary>
        /// <returns> The current Uberspect object.
        /// </returns>
        IUberspect Uberspect
        {
            get;

        }
        /// <summary> Returns a convenient LogMessage instance that wraps the current LogChute.</summary>
        /// <returns> A Log object.
        /// </returns>
        Log.Log Log
        {
            get;

        }
        /// <summary> Returns the event handlers for the application.</summary>
        /// <returns> The event handlers for the application.
        /// </returns>
        EventCartridge ApplicationEventCartridge
        {
            get;

        }
        /// <summary> Returns the configured method introspection/reflection
        /// implementation.
        /// </summary>
        /// <returns> The configured method introspection/reflection
        /// implementation.
        /// </returns>
        Introspector Introspector
        {
            get;

        }
        /// <summary> Returns true if the RuntimeInstance has been successfully initialized.</summary>
        /// <returns> True if the RuntimeInstance has been successfully initialized.
        /// </returns>
        bool Initialized
        {
            get;

        }

        /// <summary> This is the primary initialization method in the Velocity
        /// Runtime. The systems that are setup/initialized here are
        /// as follows:
        /// 
        /// <ul>
        /// <li>Logging System</li>
        /// <li>ResourceManager</li>
        /// <li>Parser Pool</li>
        /// <li>Global Cache</li>
        /// <li>Static Content Include System</li>
        /// <li>Velocimacro System</li>
        /// </ul>
        /// </summary>
        /// <throws>  Exception </throws>
        void Init();

        /// <summary> Allows an external system to set a property in
        /// the Velocity Runtime.
        /// 
        /// </summary>
        /// <param name="key">property key
        /// </param>
        /// <param name="value">property value
        /// </param>
        void SetProperty(string key, object value);

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
        void AddProperty(string key, object value);

        /// <summary> Clear the values pertaining to a particular
        /// property.
        /// 
        /// </summary>
        /// <param name="key">of property to clear
        /// </param>
        void ClearProperty(string key);

        /// <summary>  Allows an external caller to Get a property.  The calling
        /// routine is required to know the type, as this routine
        /// will return an Object, as that is what properties can be.
        /// 
        /// </summary>
        /// <param name="key">property to return
        /// </param>
        /// <returns> The value.
        /// </returns>
        object GetProperty(string key);

        /// <summary> Initialize the Velocity Runtime with a Properties
        /// object.
        /// 
        /// </summary>
        /// <param name="p">
        /// </param>
        /// <throws>  Exception </throws>
    
        void Init(ExtendedProperties p);

        /// <summary> Initialize the Velocity Runtime with the name of
        /// ExtendedProperties object.
        /// 
        /// </summary>
        /// <param name="configurationFile">
        /// </param>
        /// <throws>  Exception </throws>
        void Init(string configurationFile);

        /// <summary> Wraps the String in a StringReader and passes it off to
        /// {@link #parse(Reader,String)}.
        /// </summary>
        /// <since> 1.6
        /// </since>
        SimpleNode Parse(string string_Renamed, string templateName);

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
        /// <param name="reader">inputstream retrieved by a resource loader
        /// </param>
        /// <param name="templateName">name of the template being parsed
        /// </param>
        /// <returns> The AST representing the template.
        /// </returns>
        /// <throws>  ParseException </throws>
        SimpleNode Parse(TextReader reader, string templateName);

        /// <summary>  Parse the input and return the root of the AST node structure.
        /// 
        /// </summary>
        /// <param name="reader">inputstream retrieved by a resource loader
        /// </param>
        /// <param name="templateName">name of the template being parsed
        /// </param>
        /// <param name="dumpNamespace">flag to dump the Velocimacro namespace for this template
        /// </param>
        /// <returns> The AST representing the template.
        /// </returns>
        /// <throws>  ParseException </throws>
        SimpleNode Parse(TextReader reader, string templateName, bool dumpNamespace);

        /// <summary> Renders the input string using the context into the output writer.
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
        /// <since> Velocity 1.6
        /// </since>
        bool Evaluate(IContext context, TextWriter out_Renamed, string logTag, string instring);

        /// <summary> Renders the input reader using the context into the output writer.
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
        /// <since> Velocity 1.6
        /// </since>
        bool Evaluate(IContext context, TextWriter writer, string logTag, TextReader reader);

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
        /// <since> 1.6
        /// </since>
        bool InvokeVelocimacro(string vmName, string logTag, string[] params_Renamed, IContext context, TextWriter writer);

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
        Template GetTemplate(string name);

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
        Template GetTemplate(string name, string encoding);

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
        /// <throws>  ParseErrorException </throws>
        /// <throws>  Exception </throws>
        ContentResource GetContent(string name);

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
        /// <throws>  ParseErrorException </throws>
        /// <throws>  Exception </throws>
        ContentResource GetContent(string name, string encoding);

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
        string GetLoaderNameForResource(string resourceName);

        /// <summary> String property accessor method with default to hide the
        /// configuration implementation.
        /// 
        /// </summary>
        /// <param name="key">property key
        /// </param>
        /// <param name="defaultValue"> default value to return if key not
        /// found in resource manager.
        /// </param>
        /// <returns> String  value of key or default
        /// </returns>
        string GetString(string key, string defaultValue);

        /// <summary> Returns the appropriate VelocimacroProxy object if strVMname
        /// is a valid current Velocimacro.
        /// 
        /// </summary>
        /// <param name="vmName"> Name of velocimacro requested
        /// </param>
        /// <param name="templateName">Name of the namespace.
        /// </param>
        /// <returns> VelocimacroProxy
        /// </returns>
        Directive.Directive GetVelocimacro(string vmName, string templateName);

        /// <summary> Returns the appropriate VelocimacroProxy object if strVMname
        /// is a valid current Velocimacro.
        /// 
        /// </summary>
        /// <param name="vmName"> Name of velocimacro requested
        /// </param>
        /// <param name="templateName">Name of the namespace.
        /// </param>
        /// <param name="renderingTemplate">Name of the template we are currently rendering. This
        /// information is needed when VM_PERM_ALLOW_INLINE_REPLACE_GLOBAL setting is true
        /// and template contains a macro with the same name as the global macro library.
        /// 
        /// </param>
        /// <since> Velocity 1.6
        /// 
        /// </since>
        /// <returns> VelocimacroProxy
        /// </returns>
        Directive.Directive GetVelocimacro(string vmName, string templateName, string renderingTemplate);


        /// <summary> Adds a new Velocimacro. Usually called by Macro only while parsing.
        /// 
        /// </summary>
        /// <param name="name"> Name of velocimacro
        /// </param>
        /// <param name="macro"> root AST node of the parsed macro
        /// </param>
        /// <param name="argArray"> Array of strings, containing the
        /// #macro() arguments.  the 0th is the name.
        /// </param>
        /// <param name="sourceTemplate">
        /// </param>
        /// <since> Velocity 1.6
        /// 
        /// </since>
        /// <returns> boolean  True if added, false if rejected for some
        /// reason (either parameters or permission settings)
        /// </returns>
        bool AddVelocimacro(string name, INode macro, string[] argArray, string sourceTemplate);


        /// <summary>  Checks to see if a VM exists
        /// 
        /// </summary>
        /// <param name="vmName"> Name of velocimacro
        /// </param>
        /// <param name="templateName">
        /// </param>
        /// <returns> boolean  True if VM by that name exists, false if not
        /// </returns>
        bool IsVelocimacro(string vmName, string templateName);

        /// <summary>  tells the vmFactory to dump the specified namespace.  This is to support
        /// clearing the VM list when in inline-VM-local-scope mode
        /// </summary>
        /// <param name="namespace">
        /// </param>
        /// <returns> True if the Namespace was dumped.
        /// </returns>
        bool DumpVMNamespace(string namespace_Renamed);

        /// <summary> String property accessor method to hide the configuration implementation</summary>
        /// <param name="key"> property key
        /// </param>
        /// <returns>   value of key or null
        /// </returns>
        string GetString(string key);

        /// <summary> Int property accessor method to hide the configuration implementation.
        /// 
        /// </summary>
        /// <param name="key">property key
        /// </param>
        /// <returns> int value
        /// </returns>
        int GetInt(string key);

        /// <summary> Int property accessor method to hide the configuration implementation.
        /// 
        /// </summary>
        /// <param name="key"> property key
        /// </param>
        /// <param name="defaultValue">default value
        /// </param>
        /// <returns> int  value
        /// </returns>
        int GetInt(string key, int defaultValue);

        /// <summary> Boolean property accessor method to hide the configuration implementation.
        /// 
        /// </summary>
        /// <param name="key"> property key
        /// </param>
        /// <param name="def">default default value if property not found
        /// </param>
        /// <returns> boolean  value of key or default value
        /// </returns>
        bool GetBoolean(string key, bool def);

        /// <summary> Return the specified application attribute.
        /// 
        /// </summary>
        /// <param name="key">The name of the attribute to retrieve.
        /// </param>
        /// <returns> The value of the attribute.
        /// </returns>
        object GetApplicationAttribute(object key);

        /// <summary> Set the specified application attribute.
        /// 
        /// </summary>
        /// <param name="key">The name of the attribute to set.
        /// </param>
        /// <param name="value">The attribute value to set.
        /// </param>
        /// <returns> the displaced attribute value
        /// </returns>
        object SetApplicationAttribute(object key, object value);

        /// <summary> Create a new parser instance.</summary>
        /// <returns> A new parser instance.
        /// </returns>
        Parser.Parser CreateNewParser();

        /// <summary> Retrieve a previously instantiated directive.</summary>
        /// <param name="name">name of the directive
        /// </param>
        /// <returns> the directive with that name, if any
        /// </returns>
        /// <since> 1.6
        /// </since>
        Directive.Directive GetDirective(string name);
    }
}