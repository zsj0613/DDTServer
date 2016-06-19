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
    using System.Collections;

    using Directive;
    using Parser.Node;

    /// <summary> Manages VMs in namespaces.  Currently, two namespace modes are
    /// supported:
    /// 
    /// <ul>
    /// <li>flat - all allowable VMs are in the global namespace</li>
    /// <li>local - inline VMs are added to it's own template namespace</li>
    /// </ul>
    /// 
    /// Thanks to <a href="mailto:JFernandez@viquity.com">Jose Alberto Fernandez</a>
    /// for some ideas incorporated here.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  <a href="mailto:JFernandez@viquity.com">Jose Alberto Fernandez</a>
    /// </author>
    /// <version>  $Id: VelocimacroManager.java 698376 2008-09-23 22:15:49Z nbubna $
    /// </version>
    public class VelocimacroManager
    {
        private void InitBlock()
        {
            namespaceHash = new Hashtable();
        }
        /// <summary>  public switch to let external user of manager to control namespace
        /// usage indep of properties.  That way, for example, at startup the
        /// library files are loaded into global namespace
        /// 
        /// </summary>
        /// <param name="namespaceOn">True if namespaces should be used.
        /// </param>
        public bool NamespaceUsage
        {
            set
            {
                this.namespacesOn = value;
            }

        }
        /// <summary> Should macros registered from Libraries be marked special?</summary>
        /// <param name="registerFromLib">True if macros from Libs should be marked.
        /// </param>
        public bool RegisterFromLib
        {
            set
            {
                this.registerFromLib = value;
            }

        }
        /// <summary> Should macros from the same template be inlined?
        /// 
        /// </summary>
        /// <param name="inlineLocalMode">True if macros should be inlined on the same template.
        /// </param>
        public bool TemplateLocalInlineVM
        {
            set
            {
                this.inlineLocalMode = value;
            }

        }
        /// <since> 1.6
        /// </since>
        public bool InlineReplacesGlobal
        {
            set
            {
                inlineReplacesGlobal = value;
            }

        }
        private static string GLOBAL_NAMESPACE = "";

        private bool registerFromLib = false;

        /// <summary>Hash of namespace hashes. </summary>
        private IDictionary namespaceHash;

        /// <summary>reference to global namespace hash </summary>
        private IDictionary globalNamespace;

        /// <summary>set of names of library tempates/namespaces </summary>
        private Hashtable libraries = new Hashtable();

        /*
        * big switch for namespaces.  If true, then properties control
        * usage. If false, no.
        */
        private bool namespacesOn = true;
        private bool inlineLocalMode = false;
        private bool inlineReplacesGlobal = false;

        /// <summary> Adds the global namespace to the hash.</summary>
        internal VelocimacroManager(IRuntimeServices rsvc)
        {
            InitBlock();
            /*
            *  Add the global namespace to the namespace hash. We always have that.
            */

            globalNamespace = AddNamespace(GLOBAL_NAMESPACE);
        }

        /// <summary> Adds a VM definition to the cache.
        /// 
        /// Called by VelocimacroFactory.addVelociMacro (after parsing and discovery in Macro directive)
        /// 
        /// </summary>
        /// <param name="vmName">Name of the new VelociMacro.
        /// </param>
        /// <param name="macroBody">String representation of the macro body.
        /// </param>
        /// <param name="argArray">Array of macro parameters, first parameter is the macro name.
        /// </param>
        /// <param name="namespace">The namespace/template from which this macro has been loaded.
        /// </param>
        /// <returns> Whether everything went okay.
        /// </returns>
        public bool AddVM(string vmName, INode macroBody, string[] argArray, string namespace_Renamed, bool canReplaceGlobalMacro)
        {
            if (macroBody == null)
            {
                // happens only if someone uses this class without the Macro directive
                // and provides a null value as an argument
                throw new System.SystemException("Null AST for " + vmName + " in " + namespace_Renamed);
            }

            MacroEntry me = new MacroEntry(vmName, macroBody, argArray, namespace_Renamed);

            me.FromLibrary = registerFromLib;

            /*
            *  the client (VMFactory) will signal to us via
            *  registerFromLib that we are in startup mode registering
            *  new VMs from libraries.  Therefore, we want to
            *  addto the library map for subsequent auto reloads
            */

            bool isLib = true;

            MacroEntry exist = (MacroEntry)globalNamespace[vmName];

            if (registerFromLib)
            {
                libraries.Add(namespace_Renamed, namespace_Renamed);
            }
            else
            {
                /*
                *  now, we first want to check to see if this namespace (template)
                *  is actually a library - if so, we need to use the global namespace
                *  we don't have to do this when registering, as namespaces should
                *  be shut off. If not, the default value is true, so we still go
                *  global
                */

                isLib = libraries.Contains(namespace_Renamed);
            }

            if (!isLib && UsingNamespaces(namespace_Renamed))
            {
                /*
                *  first, do we have a namespace hash already for this namespace?
                *  if not, Add it to the namespaces, and Add the VM
                */

                IDictionary local = GetNamespace(namespace_Renamed, true);
                local[vmName] = me;

                return true;
            }
            else
            {
                /*
                *  otherwise, Add to global template.  First, check if we
                *  already have it to preserve some of the autoload information
                */


                if (exist != null)
                {
                    me.FromLibrary = exist.FromLibrary;
                }

                /*
                *  now Add it
                */

                globalNamespace[vmName] = me;

                return true;
            }
        }

        /// <summary> Gets a VelocimacroProxy object by the name / source template duple.
        /// 
        /// </summary>
        /// <param name="vmName">Name of the VelocityMacro to look up.
        /// </param>
        /// <param name="namespace">Namespace in which to look up the macro.
        /// </param>
        /// <returns> A proxy representing the Macro.
        /// </returns>
        public virtual VelocimacroProxy Get(string vmName, string namespace_Renamed)
        {
            return (Get(vmName, namespace_Renamed, null));
        }

        /// <summary> Gets a VelocimacroProxy object by the name / source template duple.
        /// 
        /// </summary>
        /// <param name="vmName">Name of the VelocityMacro to look up.
        /// </param>
        /// <param name="namespace">Namespace in which to look up the macro.
        /// </param>
        /// <param name="renderingTemplate">Name of the template we are currently rendering.
        /// </param>
        /// <returns> A proxy representing the Macro.
        /// </returns>
        /// <since> 1.6
        /// </since>
        public virtual VelocimacroProxy Get(string vmName, string namespace_Renamed, string renderingTemplate)
        {
            if (inlineReplacesGlobal && renderingTemplate != null)
            {
                /*
                * if VM_PERM_ALLOW_INLINE_REPLACE_GLOBAL is true (local macros can
                * override global macros) and we know which template we are rendering at the
                * moment, check if local namespace contains a macro we are looking for
                * if so, return it instead of the global one
                */
                IDictionary local = GetNamespace(renderingTemplate, false);
                if (local != null)
                {
                    MacroEntry me = (MacroEntry)local[vmName];

                    if (me != null)
                    {
                        return me.GetProxy(namespace_Renamed);
                    }
                }
            }

            if (UsingNamespaces(namespace_Renamed))
            {
                IDictionary local = GetNamespace(namespace_Renamed, false);

                /*
                *  if we have macros defined for this template
                */

                if (local != null)
                {
                    MacroEntry me = (MacroEntry)local[vmName];

                    if (me != null)
                    {
                        return me.GetProxy(namespace_Renamed);
                    }
                }
            }

            /*
            * if we didn't return from there, we need to simply see
            * if it's in the global namespace
            */

            MacroEntry me2 = (MacroEntry)globalNamespace[vmName];

            if (me2 != null)
            {
                return me2.GetProxy(namespace_Renamed);
            }

            return null;
        }

        /// <summary> Removes the VMs and the namespace from the manager.
        /// Used when a template is reloaded to avoid
        /// losing memory.
        /// 
        /// </summary>
        /// <param name="namespace">namespace to dump
        /// </param>
        /// <returns> boolean representing success
        /// </returns>
        public bool DumpNamespace(string namespace_Renamed)
        {
            lock (this)
            {
                if (UsingNamespaces(namespace_Renamed))
                {
                    object tempObject;
                    tempObject = namespaceHash[namespace_Renamed];
                    namespaceHash.Remove(namespace_Renamed);
                    IDictionary h = (IDictionary)tempObject;

                    if (h == null)
                    {
                        return false;
                    }

                    h.Clear();

                    return true;
                }

                return false;
            }
        }

        /// <summary>  returns the hash for the specified namespace, and if it doesn't exist
        /// will create a new one and Add it to the namespaces
        /// 
        /// </summary>
        /// <param name="namespace"> name of the namespace :)
        /// </param>
        /// <param name="addIfNew"> flag to Add a new namespace if it doesn't exist
        /// </param>
        /// <returns> namespace Map of VMs or null if doesn't exist
        /// </returns>
        private IDictionary GetNamespace(string namespace_Renamed, bool addIfNew)
        {
            IDictionary h = (IDictionary)namespaceHash[namespace_Renamed];

            if (h == null && addIfNew)
            {
                h = AddNamespace(namespace_Renamed);
            }

            return h;
        }

        /// <summary>   adds a namespace to the namespaces
        /// 
        /// </summary>
        /// <param name="namespace">name of namespace to Add
        /// </param>
        /// <returns> Hash added to namespaces, ready for use
        /// </returns>
        private IDictionary AddNamespace(string namespace_Renamed)
        {
            IDictionary h = new Hashtable(17, 0.5f);
            object oh;

            object tempObject;
            tempObject = namespaceHash[namespace_Renamed];
            namespaceHash[namespace_Renamed] = h;
            if ((oh = tempObject) != null)
            {
                /*
                * There was already an entry on the table, restore it!
                * This condition should never occur, given the code
                * and the fact that this method is private.
                * But just in case, this way of testing for it is much
                * more efficient than testing before hand using Get().
                */
                namespaceHash[namespace_Renamed] = oh;
                /*
                * Should't we be returning the old entry (oh)?
                * The previous code was just returning null in this case.
                */
                return null;
            }

            return h;
        }

        /// <summary>  determines if currently using namespaces.
        /// 
        /// </summary>
        /// <param name="namespace">currently ignored
        /// </param>
        /// <returns> true if using namespaces, false if not
        /// </returns>
        private bool UsingNamespaces(string namespace_Renamed)
        {
            /*
            *  if the big switch turns of namespaces, then ignore the rules
            */

            if (!namespacesOn)
            {
                return false;
            }

            /*
            *  currently, we only support the local template namespace idea
            */

            if (inlineLocalMode)
            {
                return true;
            }

            return false;
        }

        /// <summary> Return the library name for a given macro.</summary>
        /// <param name="vmName">Name of the Macro to look up.
        /// </param>
        /// <param name="namespace">Namespace to look the macro up.
        /// </param>
        /// <returns> The name of the library which registered this macro in a namespace.
        /// </returns>
        public virtual string GetLibraryName(string vmName, string namespace_Renamed)
        {
            if (UsingNamespaces(namespace_Renamed))
            {
                IDictionary local = GetNamespace(namespace_Renamed, false);

                /*
                *  if we have this macro defined in this namespace, then
                *  it is masking the global, library-based one, so
                *  just return null
                */

                if (local != null)
                {
                    MacroEntry me = (MacroEntry)local[vmName];

                    if (me != null)
                    {
                        return null;
                    }
                }
            }

            /*
            * if we didn't return from there, we need to simply see
            * if it's in the global namespace
            */

            MacroEntry me2 = (MacroEntry)globalNamespace[vmName];

            if (me2 != null)
            {
                return me2.SourceTemplate;
            }

            return null;
        }


        /// <summary>  wrapper class for holding VM information</summary>
        private class MacroEntry
        {
            /// <summary> Returns true if the macro was registered from a library.</summary>
            /// <returns> True if the macro was registered from a library.
            /// </returns>
            /// <summary> Has the macro been registered from a library.</summary>
            /// <param name="fromLibrary">True if the macro was registered from a Library.
            /// </param>
            public bool FromLibrary
            {
                get
                {
                    return fromLibrary;
                }

                set
                {
                    this.fromLibrary = value;
                }

            }
            /// <summary> Returns the node tree for this macro.</summary>
            /// <returns> The node tree for this macro.
            /// </returns>
            public SimpleNode NodeTree
            {
                get
                {
                    return nodeTree;
                }

            }
            /// <summary> Returns the source template name for this macro.</summary>
            /// <returns> The source template name for this macro.
            /// </returns>
            public string SourceTemplate
            {
                get
                {
                    return sourceTemplate;
                }

            }

            private string vmName;

            private string[] argArray;

            private string sourceTemplate;
            private SimpleNode nodeTree = null;
            private bool fromLibrary = false;
            private VelocimacroProxy vp;

            internal MacroEntry(string vmName, INode macro, string[] argArray, string sourceTemplate)
            {
                this.vmName = vmName;
                this.argArray = argArray;
                this.nodeTree = (SimpleNode)macro;
                this.sourceTemplate = sourceTemplate;

                vp = new VelocimacroProxy();
                vp.SetName(this.vmName);
                vp.ArgArray = this.argArray;
                vp.NodeTree = this.nodeTree;
            }

            internal VelocimacroProxy GetProxy(string namespace_Renamed)
            {
                /*
                * FIXME: namespace data is omitted, this probably 
                * breaks some Error reporting?
                */
                return vp;
            }
        }
    }
}