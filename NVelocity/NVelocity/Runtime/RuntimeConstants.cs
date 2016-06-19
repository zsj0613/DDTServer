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
    using System;
    using Resource;

    /// <summary> This class defines the keys that are used in the velocity.properties file so that they can be referenced as a constant within
    /// Java code.
    /// 
    /// </summary>
    /// <author>   <a href="mailto:jon@latchkey.com">Jon S. Stevens</a>
    /// </author>
    /// <author>   <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>   <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <version>   $Id: RuntimeConstants.java 702218 2008-10-06 18:15:18Z nbubna $
    /// </version>
    public struct RuntimeConstants
    {
        /// <summary>Location of the velocity Log file. </summary>
        public const string RUNTIME_LOG = "runtime.log";
        /// <summary>externally provided logger. </summary>
        public const string RUNTIME_LOG_LOGSYSTEM = "runtime.log.logsystem";
        /// <summary>class of Log system to use. </summary>
        public const string RUNTIME_LOG_LOGSYSTEM_CLASS = "runtime.log.logsystem.class";
        /// <summary> Properties referenced in the template are required to exist the object</summary>
        public const string RUNTIME_REFERENCES_STRICT = "runtime.references.strict";

        /// <summary>Logging of invalid references. </summary>
        public const string RUNTIME_LOG_REFERENCE_LOG_INVALID = "runtime.log.invalid.references";

        /// <summary>Counter reference name in #foreach directives. </summary>
        public const string COUNTER_NAME = "directive.foreach.counter.name";
        /// <summary>Initial counter value in #foreach directives. </summary>
        public const string COUNTER_INITIAL_VALUE = "directive.foreach.counter.initial.value";
        /// <summary>Maximum allowed number of loops. </summary>
        public const string MAX_NUMBER_LOOPS = "directive.foreach.maxloops";
        /// <summary> Whether to throw an exception or just skip bad iterables. Default is true.</summary>
        /// <since> 1.6
        /// </since>
        public const string SKIP_INVALID_ITERATOR = "directive.foreach.skip.invalid";
        /// <summary>if set to true then allows #set to accept null values in the right hand side. </summary>
        public const string SET_NULL_ALLOWED = "directive.set.null.allowed";
        /// <summary> Starting tag for Error messages triggered by passing a parameter not allowed in the #include directive. Only string literals,
        /// and references are allowed.
        /// </summary>
        public const string ERRORMSG_START = "directive.include.output.errormsg.start";
        /// <summary> Ending tag for Error messages triggered by passing a parameter not allowed in the #include directive. Only string literals,
        /// and references are allowed.
        /// </summary>
        public const string ERRORMSG_END = "directive.include.output.errormsg.end";
        /// <summary>Maximum recursion depth allowed for the #parse directive. </summary>
        public const string PARSE_DIRECTIVE_MAXDEPTH = "directive.parse.max.depth";
        /// <summary>Maximum recursion depth allowed for the #define directive. </summary>
        public const string DEFINE_DIRECTIVE_MAXDEPTH = "directive.define.max.depth";
        /// <summary> class to use for local context with #Evaluate()</summary>
        /// <since> 1.6
        /// </since>
        public const string EVALUATE_CONTEXT_CLASS = "directive.evaluate.context.class";

        public const string RESOURCE_MANAGER_CLASS = "resource.manager.class";
        /// <summary> The <code>resource.manager.cache.class</code> property specifies the name of the
        /// {@link org.apache.velocity.runtime.resource.ResourceCache} implementation to use.
        /// </summary>
        public const string RESOURCE_MANAGER_CACHE_CLASS = "resource.manager.cache.class";
        /// <summary>The <code>resource.manager.cache.size</code> property specifies the cache upper bound (if relevant). </summary>
        public const string RESOURCE_MANAGER_DEFAULTCACHE_SIZE = "resource.manager.defaultcache.size";
        /// <summary>controls if the finding of a resource is logged. </summary>
        public const string RESOURCE_MANAGER_LOGWHENFOUND = "resource.manager.logwhenfound";
        /// <summary> Key used to retrieve the names of the resource loaders to be used. In a properties file they may appear as the following:
        /// 
        /// <p>resource.loader = file,classpath</p>
        /// </summary>
        public const string RESOURCE_LOADER = "resource.loader";
        /// <summary>The public handle for setting a path in the FileResourceLoader. </summary>
        public const string FILE_RESOURCE_LOADER_PATH = "file.resource.loader.path";
        /// <summary>The public handle for turning the caching on in the FileResourceLoader. </summary>
        public const string FILE_RESOURCE_LOADER_CACHE = "file.resource.loader.cache";
        /// <summary> The <code>eventhandler.referenceinsertion.class</code> property specifies a list of the
        /// {@link org.apache.velocity.app.event.IReferenceInsertionEventHandler} implementations to use.
        /// </summary>
        public const string EVENTHANDLER_REFERENCEINSERTION = "eventhandler.referenceinsertion.class";
        /// <summary> The <code>eventhandler.nullset.class</code> property specifies a list of the
        /// {@link org.apache.velocity.app.event.NullSetEventHandler} implementations to use.
        /// </summary>
        public const string EVENTHANDLER_NULLSET = "eventhandler.nullset.class";
        /// <summary> The <code>eventhandler.methodexception.class</code> property specifies a list of the
        /// {@link org.apache.velocity.app.event.IMethodExceptionEventHandler} implementations to use.
        /// </summary>
        public const string EVENTHANDLER_METHODEXCEPTION = "eventhandler.methodexception.class";
        /// <summary> The <code>eventhandler.include.class</code> property specifies a list of the
        /// {@link org.apache.velocity.app.event.IIncludeEventHandler} implementations to use.
        /// </summary>
        public const string EVENTHANDLER_INCLUDE = "eventhandler.include.class";
        /// <summary> The <code>eventhandler.invalidreferences.class</code> property specifies a list of the
        /// {@link org.apache.velocity.app.event.IInvalidReferenceEventHandler} implementations to use.
        /// </summary>
        public const string EVENTHANDLER_INVALIDREFERENCES = "eventhandler.invalidreferences.class";
        /// <summary>Name of local Velocimacro library template. </summary>
        public const string VM_LIBRARY = "velocimacro.library";
        /// <summary>Default Velocimacro library template. </summary>
        public const string VM_LIBRARY_DEFAULT = "VM_global_library.vm";
        /// <summary>switch for autoloading library-sourced VMs (for development). </summary>
        public const string VM_LIBRARY_AUTORELOAD = "velocimacro.library.autoreload";
        /// <summary>boolean (true/false) default true : allow inline (in-template) macro definitions. </summary>
        public const string VM_PERM_ALLOW_INLINE = "velocimacro.permissions.allow.inline";
        /// <summary>boolean (true/false) default false : allow inline (in-template) macro definitions to replace existing. </summary>
        public const string VM_PERM_ALLOW_INLINE_REPLACE_GLOBAL = "velocimacro.permissions.allow.inline.to.replace.global";
        /// <summary>Switch for forcing inline macros to be local : default false. </summary>
        public const string VM_PERM_INLINE_LOCAL = "velocimacro.permissions.allow.inline.local.scope";
        /// <summary>Switch for VM blather : default true. </summary>
        public const string VM_MESSAGES_ON = "velocimacro.messages.on";
        /// <summary>switch for local context in VM : default false. </summary>
        public const string VM_CONTEXT_LOCALSCOPE = "velocimacro.context.localscope";
        /// <summary>if true, throw an exception for wrong number of arguments *</summary>
        public const string VM_ARGUMENTS_STRICT = "velocimacro.arguments.strict";
        /// <summary> Specify the maximum depth for macro calls</summary>
        /// <since> 1.6
        /// </since>
        public const string VM_MAX_DEPTH = "velocimacro.max.depth";
        /// <summary>Switch for the interpolation facility for string literals. </summary>
        public const string INTERPOLATE_STRINGLITERALS = "runtime.Interpolate.string.literals";
        /// <summary>The character encoding for the templates. Used by the parser in processing the input streams. </summary>
        public const string INPUT_ENCODING = "input.encoding";
        /// <summary>Encoding for the output stream. Currently used by Anakia and VelocityServlet </summary>
        public const string OUTPUT_ENCODING = "output.encoding";
        /// <summary>Default Encoding is ISO-8859-1. </summary>
        public const string ENCODING_DEFAULT = "ISO-8859-1";
        /// <summary>key name for uberspector. Multiple classnames can be specified,in which case uberspectors will be chained. </summary>
        public const string UBERSPECT_CLASSNAME = "runtime.introspector.uberspect";
        /// <summary>A comma separated list of packages to restrict access to in the SecureIntrospector. </summary>
        public const string INTROSPECTOR_RESTRICT_PACKAGES = "introspector.restrict.packages";
        /// <summary>A comma separated list of classes to restrict access to in the SecureIntrospector. </summary>
        public const string INTROSPECTOR_RESTRICT_CLASSES = "introspector.restrict.classes";
        /// <summary>Switch for ignoring nulls in math equations vs throwing exceptions. </summary>
        public const string STRICT_MATH = "runtime.strict.math";
        /// <summary> The <code>parser.pool.class</code> property specifies the name of the {@link org.apache.velocity.util.SimplePool}
        /// implementation to use.
        /// </summary>
        public const string PARSER_POOL_CLASS = "parser.pool.class";
        /// <seealso cref="NUMBER_OF_PARSERS">
        /// </seealso>
        public const string PARSER_POOL_SIZE = "parser.pool.size";
        /// <summary>Default Runtime properties. </summary>
        public const string DEFAULT_RUNTIME_PROPERTIES = "NVelocity.Runtime.Defaults.nvelocity.properties";
        /// <summary>Default Runtime properties. </summary>
        public const string DEFAULT_RUNTIME_DIRECTIVES = "NVelocity.Runtime.Defaults.directive.properties";
        /// <summary> The default number of parser instances to create. Configurable via the parameter named by the {@link #PARSER_POOL_SIZE}
        /// constant.
        /// </summary>
        public const int NUMBER_OF_PARSERS = 20;
    }
}