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
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using App.Event;
    using Commons.Collections;
    using Context;
    using Exception;
    using Log;
    using NVelocity.Util.Introspection;
    using Parser;
    using Parser.Node;
    using Resource;
    using Util;

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
    /// Turbine, for example, knows where the templates
    /// are to be loaded from, and where the Velocity
    /// Log file should be placed.
    /// 
    /// So in the case of Velocity cooperating with Turbine
    /// the code might look something like the following:
    /// 
    /// <blockquote><code><pre>
    /// ri.SetProperty(Runtime.FILE_RESOURCE_LOADER_PATH, templatePath);
    /// ri.SetProperty(Runtime.RUNTIME_LOG, pathToVelocityLog);
    /// ri.Init();
    /// </pre></code></blockquote>
    /// 
    /// <pre>
    /// -----------------------------------------------------------------------
    /// N O T E S  O N  R U N T I M E  I N I T I A L I Z A T I O N
    /// -----------------------------------------------------------------------
    /// Init()
    /// 
    /// If Init() is called by itself the RuntimeInstance will Initialize
    /// with a set of default values.
    /// -----------------------------------------------------------------------
    /// Init(String/Properties)
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
    /// <version>  $Id: RuntimeInstance.java 703049 2008-10-09 03:18:58Z nbubna $
    /// </version>
    public class RuntimeInstance : IRuntimeServices
    {
        private static readonly object syncObj = new object();

        /// <summary> Returns true if the RuntimeInstance has been successfully initialized.</summary>
        /// <returns> True if the RuntimeInstance has been successfully initialized.
        /// </returns>
        /// <since> 1.5
        /// </since>
        public bool Initialized
        {
            get
            {
                return initialized;
            }

        }

        /// <summary> Return the velocity runtime configuration object.
        /// 
        /// </summary>
        /// <returns> Configuration object which houses the Velocity runtime
        /// properties.
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
        public ExtendedProperties Configuration
        {
            get
            {
                return configuration;
            }

            set
            {
                if (overridingProperties == null)
                {
                    overridingProperties = value;
                }
                else
                {
                    // Avoid possible ConcurrentModificationException
                    if (overridingProperties != value)
                    {
                        overridingProperties.Combine(value);
                    }
                }
            }

        }
        private ExtendedProperties Properties
        {
            set
            {
                if (overridingProperties == null)
                {
                    overridingProperties = value;
                }
                else
                {
                    overridingProperties.Combine(value);
                }
            }

        }
        /// <summary> Retrieves and caches the configured default encoding
        /// for better performance. (VELOCITY-606)
        /// </summary>
        private string DefaultEncoding
        {
            get
            {
                if (encoding == null)
                {
                    encoding = GetString(RuntimeConstants.INPUT_ENCODING, RuntimeConstants.ENCODING_DEFAULT);
                }
                return encoding;
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
        public Log.Log Log
        {
            get
            {
                return log;
            }

        }
        /// <summary>  Return the Introspector for this instance</summary>
        /// <returns> The Introspector for this instance
        /// </returns>
        public Introspector Introspector
        {
            get
            {
                return introspector;
            }

        }
        /// <summary> Returns the event handlers for the application.</summary>
        /// <returns> The event handlers for the application.
        /// </returns>
        /// <since> 1.5
        /// </since>
        public EventCartridge ApplicationEventCartridge
        {
            get
            {
                return eventCartridge;
            }

        }
        /// <summary> Returns the Uberspect object for this Instance.
        /// 
        /// </summary>
        /// <returns> The Uberspect object for this Instance.
        /// </returns>
        public IUberspect Uberspect
        {
            get
            {
                return uberSpect;
            }

        }
        /// <summary>  VelocimacroFactory object to manage VMs</summary>
        private VelocimacroFactory vmFactory = null;

        /// <summary> The Runtime logger.  We start with an instance of
        /// a 'primordial logger', which just collects Log messages
        /// then, when the Log system is initialized, all the
        /// messages Get dumpted out of the primordial one into the real one.
        /// </summary>
        private Log.Log log = new Log.Log();

        /// <summary> The Runtime parser pool</summary>
        private IParserPool parserPool;

        /// <summary> Indicate whether the Runtime is in the midst of initialization.</summary>
        private bool initializing = false;

        /// <summary> Indicate whether the Runtime has been fully initialized.</summary>
        private bool initialized = false;

        /// <summary> These are the properties that are laid down over top
        /// of the default properties when requested.
        /// </summary>
        private ExtendedProperties overridingProperties = null;

        /// <summary> This is a hashtable of initialized directives.
        /// The directives that populate this hashtable are
        /// taken from the RUNTIME_DEFAULT_DIRECTIVES
        /// property file. This hashtable is passed
        /// to each parser that is created.
        /// </summary>
        private IDictionary<string, Directive.Directive> runtimeDirectives;

        /// <summary> Object that houses the configuration options for
        /// the velocity runtime. The ExtendedProperties object allows
        /// the convenient retrieval of a subset of properties.
        /// For example all the properties for a resource loader
        /// can be retrieved from the main ExtendedProperties object
        /// using something like the following:
        /// 
        /// ExtendedProperties loaderConfiguration =
        /// configuration.subset(loaderID);
        /// 
        /// And a configuration is a lot more convenient to deal
        /// with then conventional properties objects, or Maps.
        /// </summary>
        private ExtendedProperties configuration = new ExtendedProperties();

        private IResourceManager resourceManager = null;

        /// <summary> This stores the engine-wide set of event handlers.  Event handlers for
        /// each specific merge are stored in the context.
        /// </summary>
        private EventCartridge eventCartridge = null;

        /*
        *  Each runtime instance has it's own introspector
        *  to ensure that each instance is completely separate.
        */
        private Introspector introspector = null;


        /*
        *  Opaque reference to something specificed by the
        *  application for use in application supplied/specified
        *  pluggable components
        */
        private IDictionary applicationAttributes = null;
        private IUberspect uberSpect;
        private string encoding;

        /// <summary> Creates a new RuntimeInstance object.</summary>
        public RuntimeInstance()
        {
            /*
            *  create a VM factory, introspector, and application attributes
            */
            vmFactory = new VelocimacroFactory(this);

            /*
            *  make a new introspector and Initialize it
            */
            introspector = new Introspector(Log);

            /*
            * and a store for the application attributes
            */

            applicationAttributes = new Hashtable();
        }

        /// <summary> This is the primary initialization method in the Velocity
        /// Runtime. The systems that are setup/initialized here are
        /// as follows:
        /// 
        /// <ul>
        /// <li>Logging System</li>
        /// <li>ResourceManager</li>
        /// <li>EventHandler</li>
        /// <li>Parser Pool</li>
        /// <li>Global Cache</li>
        /// <li>Static Content Include System</li>
        /// <li>Velocimacro System</li>
        /// </ul>
        /// </summary>
        /// <throws>  Exception When an Error occured during initialization. </throws>

        public virtual void Init()
        {
            lock (syncObj)
            {
                if (!initialized && !initializing)
                {
                    initializing = true;

                    log.Trace("*******************************************************************");
                    log.Debug("Starting Apache Velocity v@build.version@ (compiled: @build.time@)");
                    log.Trace("RuntimeInstance initializing.");

                    InitializeProperties();
                    InitializeLog();
                    InitializeResourceManager();
                    InitializeDirectives();
                    InitializeEventHandlers();
                    InitializeParserPool();

                    InitializeIntrospection();
                    /*
                    *  Initialize the VM Factory.  It will use the properties
                    * accessable from Runtime, so keep this here at the end.
                    */
                    vmFactory.InitVelocimacro();

                    log.Trace("RuntimeInstance successfully initialized.");

                    initialized = true;
                    initializing = false;
                }
            }
        }

        /// <summary> Init or die! (with some Log help, of course)</summary>
        private void RequireInitialization()
        {
            if (!initialized && !initializing)
            {
                log.Debug("Velocity was not initialized! Calling init()...");
                try
                {
                    Init();
                }
                catch (System.Exception e)
                {
                    Log.Error("Could not auto-initialize Velocity", e);
                    throw new RuntimeException("Velocity could not be initialized!", e);
                }
            }
        }

        /// <summary>  Gets the classname for the Uberspect introspection package and
        /// instantiates an instance.
        /// </summary>
        private void InitializeIntrospection()
        {
            string[] uberspectors = configuration.GetStringArray(RuntimeConstants.UBERSPECT_CLASSNAME);
            for (int i = 0; i < uberspectors.Length; i++)
            {
                string rm = uberspectors[i];
                object o = null;

                try
                {
                    o = System.Activator.CreateInstance(Type.GetType(rm.Replace(';', ',')));
                }
                catch (System.Exception)
                {
                    string err = "The specified class for Uberspect (" + rm + ") does not exist or is not accessible to the current classloader.";
                    log.Error(err);
                    throw new System.Exception(err);
                }

                if (!(o is IUberspect))
                {
                    string err = "The specified class for Uberspect (" + rm + ") does not implement " + typeof(IUberspect).FullName + "; Velocity is not initialized correctly.";

                    log.Error(err);
                    throw new System.Exception(err);
                }

                IUberspect u = (IUberspect)o;

                if (u is IUberspectLoggable)
                {
                    ((IUberspectLoggable)u).Log = Log;
                }

                if (u is IRuntimeServicesAware)
                {
                    ((IRuntimeServicesAware)u).SetRuntimeServices(this);
                }

                if (uberSpect == null)
                {
                    uberSpect = u;
                }
                else
                {
                    if (u is IChainableUberspector)
                    {
                        ((IChainableUberspector)u).Wrap(uberSpect);
                        uberSpect = u;
                    }
                    else
                    {
                        uberSpect = new LinkingUberspector(uberSpect, u);
                    }
                }
            }

            if (uberSpect != null)
            {
                uberSpect.Init();
            }
            else
            {
                /*
                *  someone screwed up.  Lets not fool around...
                */

                string err = "It appears that no class was specified as the" + " Uberspect.  Please ensure that all configuration" + " information is correct.";

                log.Error(err);
                throw new System.Exception(err);
            }
        }

        /// <summary> Initializes the Velocity Runtime with properties file.
        /// The properties file may be in the file system proper,
        /// or the properties file may be in the classpath.
        /// </summary>
        private void SetDefaultProperties()
        {
            try
            {

                using (Stream inputStream = GetType().Assembly.GetManifestResourceStream(RuntimeConstants.DEFAULT_RUNTIME_PROPERTIES))
                {
                    configuration.Load(inputStream);

                    if (log.DebugEnabled)
                    {
                        log.Debug("Default Properties File: " + new FileInfo(RuntimeConstants.DEFAULT_RUNTIME_PROPERTIES).FullName);
                    }
                }
            }
            catch (IOException ioe)
            {
                string msg = "Cannot Get Velocity Runtime default properties!";
                log.Error(msg, ioe);
                throw new RuntimeException(msg, ioe);
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
        public void SetProperty(string key, object value)
        {
            if (overridingProperties == null)
            {
                overridingProperties = new ExtendedProperties();
            }

            overridingProperties.SetProperty(key, value);
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
        public void AddProperty(string key, object value)
        {
            if (overridingProperties == null)
            {
                overridingProperties = new ExtendedProperties();
            }

            overridingProperties.AddProperty(key, value);
        }

        /// <summary> Clear the values pertaining to a particular
        /// property.
        /// 
        /// </summary>
        /// <param name="key">of property to clear
        /// </param>
        public void ClearProperty(string key)
        {
            if (overridingProperties != null)
            {
                overridingProperties.ClearProperty(key);
            }
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
        public object GetProperty(string key)
        {
            object o = null;

            /**
            * Before initialization, check the user-entered properties first.
            */
            if (!initialized && !initializing && overridingProperties != null)
            {
                o = overridingProperties[key];
            }

            /**
            * After initialization, configuration will hold all properties.
            */
            if (o == null)
            {
                o = configuration.GetProperty(key);
            }
            if (o is string)
            {
                return StringUtils.NullTrim((string)o);
            }
            else
            {
                return o;
            }
        }

        /// <summary> Initialize Velocity properties, if the default
        /// properties have not been laid down first then
        /// do so. Then proceed to process any overriding
        /// properties. Laying down the default properties
        /// gives a much greater chance of having a
        /// working system.
        /// </summary>
        private void InitializeProperties()
        {
            /*
            * Always lay down the default properties first as
            * to provide a solid base.
            */
            if (configuration.isInitialized == false)
            {
                SetDefaultProperties();
            }

            if (overridingProperties != null)
            {
                configuration.Combine(overridingProperties);
            }
        }

        /// <summary> Initialize the Velocity Runtime with a Properties
        /// object.
        /// 
        /// </summary>
        /// <param name="p">
        /// </param>
        /// <throws>  Exception When an Error occurs during initialization. </throws>
        public void Init(ExtendedProperties p)
        {
            Properties = ExtendedProperties.ConvertProperties(p);
            Init();
        }

        /// <summary> Initialize the Velocity Runtime with the name of
        /// ExtendedProperties object.
        /// 
        /// </summary>
        /// <param name="configurationFile">
        /// </param>
        /// <throws>  Exception When an Error occurs during initialization. </throws>
        public void Init(string configurationFile)
        {
            Properties = new ExtendedProperties(configurationFile);
            Init();
        }

        private void InitializeResourceManager()
        {
            /*
            * Which resource manager?
            */

            string rm = GetString(RuntimeConstants.RESOURCE_MANAGER_CLASS);

            if (rm != null && rm.Length > 0)
            {
                /*
                *  if something was specified, then make one.
                *  if that isn't a ResourceManager, consider
                *  this a huge Error and throw
                */

                object o = null;

                try
                {
                    o = Activator.CreateInstance(Type.GetType(rm.Replace(';', ',')));
                }

                catch (System.Exception)
                {
                    string err = "The specified class for ResourceManager (" + rm + ") does not exist or is not accessible to the current classloader.";
                    log.Error(err);
                    throw new System.Exception(err);
                }

                if (!(o is IResourceManager))
                {
                    string err = "The specified class for ResourceManager (" + rm + ") does not implement " + typeof(IResourceManager).FullName + "; Velocity is not initialized correctly.";

                    log.Error(err);
                    throw new System.Exception(err);
                }

                resourceManager = (IResourceManager)o;

                resourceManager.Initialize(this);
            }
            else
            {
                /*
                *  someone screwed up.  Lets not fool around...
                */

                string err = "It appears that no class was specified as the" + " ResourceManager.  Please ensure that all configuration" + " information is correct.";

                log.Error(err);
                throw new System.Exception(err);
            }
        }

        private void InitializeEventHandlers()
        {

            eventCartridge = new EventCartridge();

            /**
            * For each type of event handler, Get the class name, instantiate it, and store it.
            */

            string[] referenceinsertion = configuration.GetStringArray(NVelocity.Runtime.RuntimeConstants.EVENTHANDLER_REFERENCEINSERTION);
            if (referenceinsertion != null)
            {
                for (int i = 0; i < referenceinsertion.Length; i++)
                {
                    IEventHandler ev = InitializeSpecificEventHandler(referenceinsertion[i], NVelocity.Runtime.RuntimeConstants.EVENTHANDLER_REFERENCEINSERTION, typeof(IReferenceInsertionEventHandler));
                    if (ev != null)
                        eventCartridge.AddReferenceInsertionEventHandler((IReferenceInsertionEventHandler)ev);
                }
            }

            string[] nullset = configuration.GetStringArray(NVelocity.Runtime.RuntimeConstants.EVENTHANDLER_NULLSET);
            if (nullset != null)
            {
                for (int i = 0; i < nullset.Length; i++)
                {
                    IEventHandler ev = InitializeSpecificEventHandler(nullset[i], NVelocity.Runtime.RuntimeConstants.EVENTHANDLER_NULLSET, typeof(INullSetEventHandler));
                    if (ev != null)
                        eventCartridge.AddNullSetEventHandler((INullSetEventHandler)ev);
                }
            }

            string[] methodexception = configuration.GetStringArray(NVelocity.Runtime.RuntimeConstants.EVENTHANDLER_METHODEXCEPTION);
            if (methodexception != null)
            {
                for (int i = 0; i < methodexception.Length; i++)
                {
                    IEventHandler ev = InitializeSpecificEventHandler(methodexception[i], NVelocity.Runtime.RuntimeConstants.EVENTHANDLER_METHODEXCEPTION, typeof(IMethodExceptionEventHandler));
                    if (ev != null)
                        eventCartridge.AddMethodExceptionHandler((IMethodExceptionEventHandler)ev);
                }
            }

            string[] includeHandler = configuration.GetStringArray(NVelocity.Runtime.RuntimeConstants.EVENTHANDLER_INCLUDE);
            if (includeHandler != null)
            {
                for (int i = 0; i < includeHandler.Length; i++)
                {
                    IEventHandler ev = InitializeSpecificEventHandler(includeHandler[i], NVelocity.Runtime.RuntimeConstants.EVENTHANDLER_INCLUDE, typeof(IIncludeEventHandler));
                    if (ev != null)
                        eventCartridge.AddIncludeEventHandler((IIncludeEventHandler)ev);
                }
            }

            string[] invalidReferenceSet = configuration.GetStringArray(NVelocity.Runtime.RuntimeConstants.EVENTHANDLER_INVALIDREFERENCES);
            if (invalidReferenceSet != null)
            {
                for (int i = 0; i < invalidReferenceSet.Length; i++)
                {
                    IEventHandler ev = InitializeSpecificEventHandler(invalidReferenceSet[i], NVelocity.Runtime.RuntimeConstants.EVENTHANDLER_INVALIDREFERENCES, typeof(IInvalidReferenceEventHandler));
                    if (ev != null)
                    {
                        eventCartridge.AddInvalidReferenceEventHandler((IInvalidReferenceEventHandler)ev);
                    }
                }
            }
        }

        private App.Event.IEventHandler InitializeSpecificEventHandler(string classname, string paramName, System.Type EventHandlerInterface)
        {
            if (classname != null && classname.Length > 0)
            {
                object o = null;
                try
                {
                    o = System.Activator.CreateInstance(Type.GetType(classname.Replace(';', ',')));
                }
                catch (System.Exception)
                {
                    string err = "The specified class for " + paramName + " (" + classname + ") does not exist or is not accessible to the current classloader.";
                    log.Error(err);
                    throw new System.Exception(err);
                }

                if (!EventHandlerInterface.IsAssignableFrom(EventHandlerInterface))
                {
                    string err = "The specified class for " + paramName + " (" + classname + ") does not implement " + EventHandlerInterface.FullName + "; Velocity is not initialized correctly.";

                    log.Error(err);
                    throw new System.Exception(err);
                }

                App.Event.IEventHandler ev = (App.Event.IEventHandler)o;
                if (ev is IRuntimeServicesAware)
                    ((IRuntimeServicesAware)ev).SetRuntimeServices(this);
                return ev;
            }
            else
                return null;
        }

        /// <summary> Initialize the Velocity logging system.
        /// 
        /// </summary>
        /// <throws>  Exception </throws>
        private void InitializeLog()
        {
            // since the LogMessage we started with was just placeholding,
            // let's update it with the real LogChute settings.
            LogManager.UpdateLog(this.Log, this);
        }


        /// <summary> This methods initializes all the directives
        /// that are used by the Velocity Runtime. The
        /// directives to be initialized are listed in
        /// the RUNTIME_DEFAULT_DIRECTIVES properties
        /// file.
        /// 
        /// </summary>
        /// <throws>  Exception </throws>
        private void InitializeDirectives()
        {
            /*
            * Initialize the runtime directive table.
            * This will be used for creating parsers.
            */
            runtimeDirectives = new Dictionary<string, Directive.Directive>();

            ExtendedProperties directiveProperties = new ExtendedProperties();

            /*
            * Grab the properties file with the list of directives
            * that we should Initialize.
            */

            try
            {
                using (Stream inputStream = GetType().Assembly.GetManifestResourceStream(RuntimeConstants.DEFAULT_RUNTIME_DIRECTIVES))
                {

                    if (inputStream == null)
                    {
                        throw new System.Exception("Error loading directive.properties! " + "Something is very wrong if these properties " + "aren't being located. Either your Velocity " + "distribution is incomplete or your Velocity " + "jar file is corrupted!");
                    }

                    directiveProperties.Load(inputStream);
                }
            }
            catch (IOException ioe)
            {
                string msg = "Error while loading directive properties!";
                log.Error(msg, ioe);
                throw new RuntimeException(msg, ioe);
            }


            /*
            * Grab all the values of the properties. These
            * are all class names for example:
            *
            * org.apache.velocity.runtime.directive.Foreach
            */
            System.Collections.IEnumerator directiveClasses = directiveProperties.Values.GetEnumerator();

            while (directiveClasses.MoveNext())
            {
                string directiveClass = (string)directiveClasses.Current;
                LoadDirective(directiveClass);
                log.Debug("Loaded System Directive: " + directiveClass);
            }

            /*
            *  now the user's directives
            */

            string[] userdirective = configuration.GetStringArray("userdirective");

            for (int i = 0; i < userdirective.Length; i++)
            {
                LoadDirective(userdirective[i]);
                if (log.DebugEnabled)
                {
                    log.Debug("Loaded User Directive: " + userdirective[i]);
                }
            }
        }

        /// <summary> Programatically Add a directive.</summary>
        /// <param name="directive">
        /// </param>
        public virtual void AddDirective(Directive.Directive directive)
        {
            runtimeDirectives[directive.Name] = directive;
        }

        /// <summary> Retrieve a previously instantiated directive.</summary>
        /// <param name="name">name of the directive
        /// </param>
        /// <returns> the {@link Directive} for that name
        /// </returns>
        public virtual Directive.Directive GetDirective(string name)
        {
            return (Directive.Directive)runtimeDirectives[name];
        }

        /// <summary> Remove a directive.</summary>
        /// <param name="name">name of the directive.
        /// </param>
        public void RemoveDirective(string name)
        {
            runtimeDirectives.Remove(name);
        }

        /// <summary>  instantiates and loads the directive with some basic checks
        /// 
        /// </summary>
        /// <param name="directiveClass">classname of directive to load
        /// </param>
        private void LoadDirective(string directiveClass)
        {
            try
            {
                object o = System.Activator.CreateInstance(Type.GetType(directiveClass.Replace(';', ',')));

                if (o is Directive.Directive)
                {
                    Directive.Directive directive = (Directive.Directive)o;
                    AddDirective(directive);
                }
                else
                {
                    string msg = directiveClass + " does not implement " + typeof(Directive.Directive).FullName + "; it cannot be loaded.";
                    log.Error(msg);
                    throw new VelocityException(msg);
                }
            }
            // The ugly threesome:  ClassNotFoundException,
            // IllegalAccessException, InstantiationException.
            // Ignore Findbugs complaint for now.
            catch (System.Exception e)
            {
                string msg = "Failed to load Directive: " + directiveClass;
                log.Error(msg, e);
                throw new VelocityException(msg, e);
            }
        }


        /// <summary> Initializes the Velocity parser pool.</summary>
        private void InitializeParserPool()
        {
            /*
            * Which parser pool?
            */
            string pp = GetString(NVelocity.Runtime.RuntimeConstants.PARSER_POOL_CLASS);

            if (pp != null && pp.Length > 0)
            {
                /*
                *  if something was specified, then make one.
                *  if that isn't a ParserPool, consider
                *  this a huge Error and throw
                */

                object o = null;

                try
                {
                    o = System.Activator.CreateInstance(Type.GetType(pp));
                }
                catch (System.Exception)
                {
                    string err = "The specified class for ParserPool (" + pp + ") does not exist (or is not accessible to the current classloader.";
                    log.Error(err);
                    throw new System.Exception(err);
                }

                if (!(o is IParserPool))
                {
                    string err = "The specified class for ParserPool (" + pp + ") does not implement " + typeof(IParserPool) + " Velocity not initialized correctly.";

                    log.Error(err);
                    throw new System.Exception(err);
                }

                parserPool = (IParserPool)o;

                parserPool.Initialize(this);
            }
            else
            {
                /*
                *  someone screwed up.  Lets not fool around...
                */

                string err = "It appears that no class was specified as the" + " ParserPool.  Please ensure that all configuration" + " information is correct.";

                log.Error(err);
                throw new System.Exception(err);
            }
        }

        /// <summary> Returns a JavaCC generated Parser.
        /// 
        /// </summary>
        /// <returns> Parser javacc generated parser
        /// </returns>
        public virtual Parser.Parser CreateNewParser()
        {
            RequireInitialization();

            Parser.Parser parser = new Parser.Parser(this);
            parser.Directives = runtimeDirectives;
            return parser;
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
        /// <param name="string">String to be parsed
        /// </param>
        /// <param name="templateName">name of the template being parsed
        /// </param>
        /// <returns> A root node representing the template as an AST tree.
        /// </returns>
        /// <throws>  ParseException When the string could not be parsed as a template. </throws>
        /// <since> 1.6
        /// </since>
        public virtual SimpleNode Parse(string string_Renamed, string templateName)
        {
            return Parse(new System.IO.StringReader(string_Renamed), templateName);
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
        public virtual SimpleNode Parse(System.IO.TextReader reader, string templateName)
        {
            /*
            *  do it and dump the VM namespace for this template
            */
            return Parse(reader, templateName, true);
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
        public virtual SimpleNode Parse(System.IO.TextReader reader, string templateName, bool dumpNamespace)
        {
            RequireInitialization();

            Parser.Parser parser = (Parser.Parser)parserPool.Get();
            bool keepParser = true;
            if (parser == null)
            {
                /*
                *  if we couldn't Get a parser from the pool make one and Log it.
                */
                if (log.InfoEnabled)
                {
                    log.Info("Runtime : ran out of parsers. Creating a new one. " + " Please increment the parser.pool.size property." + " The current value is too small.");
                }
                parser = CreateNewParser();
                keepParser = false;
            }

            try
            {
                /*
                *  dump namespace if we are told to.  Generally, you want to
                *  do this - you don't in special circumstances, such as
                *  when a VM is getting Init()-ed & parsed
                */
                if (dumpNamespace)
                {
                    DumpVMNamespace(templateName);
                }
                return parser.Parse(reader, templateName);
            }
            finally
            {
                if (keepParser)
                {
                    parserPool.Put(parser);
                }
            }
        }

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
        public virtual bool Evaluate(IContext context, TextWriter textWriter, string logTag, string instring)
        {
            return Evaluate(context, textWriter, logTag, new StringReader(instring));
        }

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
        public bool Evaluate(IContext context,TextWriter writer, string logTag, TextReader reader)
        {
            if (logTag == null)
            {
                throw new System.NullReferenceException("logTag (i.e. template name) cannot be null, you must provide an identifier for the content being evaluated");
            }

            SimpleNode nodeTree = null;
            try
            {
                nodeTree = Parse(reader, logTag);
            }
            catch (ParseException pex)
            {
                throw new ParseErrorException(pex);
            }
            catch (TemplateInitException pex)
            {
                throw new ParseErrorException(pex);
            }

            if (nodeTree == null)
            {
                return false;
            }
            else
            {
                return Render(context, writer, logTag, nodeTree);
            }
        }


        /// <summary> Initializes and renders the AST {@link SimpleNode} using the context
        /// into the output writer.
        /// 
        /// </summary>
        /// <param name="context">context to use in rendering input string
        /// </param>
        /// <param name="writer"> Writer in which to render the output
        /// </param>
        /// <param name="logTag"> string to be used as the template name for Log messages
        /// in case of Error
        /// </param>
        /// <param name="nodeTree">SimpleNode which is the root of the AST to be rendered
        /// 
        /// </param>
        /// <returns> true if successful, false otherwise.  If false, see
        /// Velocity runtime Log for errors
        /// </returns>
        /// <throws>  ParseErrorException The template could not be parsed. </throws>
        /// <throws>  MethodInvocationException A method on a context object could not be invoked. </throws>
        /// <throws>  ResourceNotFoundException A referenced resource could not be loaded. </throws>
        /// <throws>  IOException While rendering to the writer, an I/O problem occured. </throws>
        /// <since> Velocity 1.6
        /// </since>
        public bool Render(IContext context, TextWriter writer, string logTag, SimpleNode nodeTree)
        {
            /*
            * we want to Init then render
            */
            InternalContextAdapterImpl ica = new InternalContextAdapterImpl(context);

            ica.PushCurrentTemplateName(logTag);

            try
            {
                try
                {
                    nodeTree.Init(ica, this);
                }
                catch (TemplateInitException pex)
                {
                    throw new ParseErrorException(pex);
                }
                /**
                * pass through application level runtime exceptions
                */
                catch (System.SystemException e)
                {
                    throw e;
                }
                catch (System.Exception e)
                {
                    string msg = "RuntimeInstance.render(): init exception for tag = " + logTag;
                    Log.Error(msg, e);
                    throw new VelocityException(msg, e);
                }

                /*
                *  now render, and let any exceptions fly
                */
                nodeTree.Render(ica, writer);
            }
            finally
            {
                ica.PopCurrentTemplateName();
            }

            return true;
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
        /// <since> 1.6
        /// </since>
        public bool InvokeVelocimacro(string vmName, string logTag, string[] params_Renamed, IContext context, System.IO.TextWriter writer)
        {
            /* check necessary parameters */
            if (vmName == null || context == null || writer == null)
            {
                string msg = "RuntimeInstance.InvokeVelocimacro() : invalid call : vmName, context, and writer must not be null";
                Log.Error(msg);
                throw new System.NullReferenceException(msg);
            }

            /* handle easily corrected parameters */
            if (logTag == null)
            {
                logTag = vmName;
            }
            if (params_Renamed == null)
            {
                params_Renamed = new string[0];
            }

            /* does the VM exist? */
            if (!IsVelocimacro(vmName, logTag))
            {
                string msg = "RuntimeInstance.InvokeVelocimacro() : VM '" + vmName + "' is not registered.";
                Log.Error(msg);
                throw new VelocityException(msg);
            }

            /* now just create the VM call, and use Evaluate */
            StringBuilder template = new StringBuilder("#");
            template.Append(vmName);
            template.Append("(");
            for (int i = 0; i < params_Renamed.Length; i++)
            {
                template.Append(" $");
                template.Append(params_Renamed[i]);
            }
            template.Append(" )");

            return Evaluate(context, writer, logTag, template.ToString());
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
        public Template GetTemplate(string name)
        {
            return GetTemplate(name, DefaultEncoding);
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
        public Template GetTemplate(string name, string encoding)
        {
            RequireInitialization();

            return (Template)resourceManager.GetResource(name, ResourceManagerConstants.RESOURCE_TEMPLATE, encoding);
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
        public ContentResource GetContent(string name)
        {
            /*
            *  the encoding is irrelvant as we don't do any converstion
            *  the bytestream should be dumped to the output stream
            */

            return GetContent(name, DefaultEncoding);
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
        public ContentResource GetContent(string name, string encoding)
        {
            RequireInitialization();

            return (ContentResource)resourceManager.GetResource(name, ResourceManagerConstants.RESOURCE_CONTENT, encoding);
        }


        /// <summary>  Determines if a template exists and returns name of the loader that
        /// provides it.  This is a slightly less hokey way to support
        /// the Velocity.ResourceExists() utility method, which was broken
        /// when per-template encoding was introduced.  We can revisit this.
        /// 
        /// </summary>
        /// <param name="resourceName">Name of template or content resource
        /// </param>
        /// <returns> class name of loader than can provide it
        /// </returns>
        public string GetLoaderNameForResource(string resourceName)
        {
            RequireInitialization();

            return resourceManager.GetLoaderNameForResource(resourceName);
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
        public string GetString(string key, string defaultValue)
        {
            return configuration.GetString(key, defaultValue);
        }

        /// <summary> Returns the appropriate VelocimacroProxy object if vmName
        /// is a valid current Velocimacro.
        /// 
        /// </summary>
        /// <param name="vmName">Name of velocimacro requested
        /// </param>
        /// <param name="templateName">Name of the template that contains the velocimacro.
        /// </param>
        /// <returns> The requested VelocimacroProxy.
        /// </returns>
        /// <since> 1.6
        /// </since>
        public Directive.Directive GetVelocimacro(string vmName, string templateName)
        {
            return vmFactory.GetVelocimacro(vmName, templateName);
        }

        /// <summary> Returns the appropriate VelocimacroProxy object if vmName
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
        public Directive.Directive GetVelocimacro(string vmName, string templateName, string renderingTemplate)
        {
            return vmFactory.GetVelocimacro(vmName, templateName, renderingTemplate);
        }

        /// <summary> Adds a new Velocimacro. Usually called by Macro only while parsing.
        /// 
        /// Called by org.apache.velocity.runtime.directive.processAndRegister
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
        public bool AddVelocimacro(string name, INode macro, string[] argArray, string sourceTemplate)
        {
            return vmFactory.AddVelocimacro(name, macro, argArray, sourceTemplate);
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
        public bool IsVelocimacro(string vmName, string templateName)
        {
            return vmFactory.IsVelocimacro(vmName, templateName);
        }

        /// <summary> tells the vmFactory to dump the specified namespace.  This is to support
        /// clearing the VM list when in inline-VM-local-scope mode
        /// </summary>
        /// <param name="namespace">Namespace to dump.
        /// </param>
        /// <returns> True if namespace was dumped successfully.
        /// </returns>
        public bool DumpVMNamespace(string namespaces)
        {
            return vmFactory.DumpVMNamespace(namespaces);
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
        public string GetString(string key)
        {
            return StringUtils.NullTrim(configuration.GetString(key));
        }

        /// <summary> Int property accessor method to hide the configuration implementation.
        /// 
        /// </summary>
        /// <param name="key">Property key
        /// </param>
        /// <returns> value
        /// </returns>
        public int GetInt(string key)
        {
            return configuration.GetInt(key);
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
        public int GetInt(string key, int defaultValue)
        {
            return configuration.GetInt(key, defaultValue);
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
        public bool GetBoolean(string key, bool def)
        {
            return configuration.GetBoolean(key, def);
        }


        /// <summary>  Gets the application attribute for the given key
        /// 
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <returns> The application attribute for the given key.
        /// </returns>
        public object GetApplicationAttribute(object key)
        {
            return applicationAttributes[key];
        }

        /// <summary>   Sets the application attribute for the given key
        /// 
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="o">The new application attribute.
        /// </param>
        /// <returns> The old value of this attribute or null if it hasn't been set before.
        /// </returns>
        public virtual object SetApplicationAttribute(object key, object o)
        {
            return applicationAttributes[key] = o;
        }
    }
}