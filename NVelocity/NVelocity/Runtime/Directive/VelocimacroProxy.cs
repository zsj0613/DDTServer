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

namespace NVelocity.Runtime.Directive
{
    using Context;
    using Exception;
    using Parser.Node;

    /// <summary>  VelocimacroProxy.java
    /// 
    /// a proxy Directive-derived object to fit with the current directive system
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: VelocimacroProxy.java 704172 2008-10-13 17:29:25Z nbubna $
    /// </version>
    public class VelocimacroProxy : Directive
    {
        /// <summary> Velocimacros are always LINE type directives.</summary>
        /// <returns> The type of this directive.
        /// </returns>
        override public int Type
        {
            get
            {
                return NVelocity.Runtime.Directive.DirectiveType.LINE;
            }

        }
        /// <summary> sets the array of arguments specified in the macro definition
        /// 
        /// </summary>
        /// <param name="arr">
        /// </param>
        virtual public string[] ArgArray
        {
            set
            {
                argArray = value;

                // for performance reasons we precache these strings - they are needed in
                // "render literal if null" functionality
                literalArgArray = new string[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    literalArgArray[i] = ".literal.$" + argArray[i];
                }

                /*
                * Get the arg count from the arg array. remember that the arg array has the macro name as
                * it's 0th element
                */

                numMacroArgs = argArray.Length - 1;
            }

        }
        /// <param name="tree">
        /// </param>
        virtual public SimpleNode NodeTree
        {
            set
            {
                nodeTree = value;
            }

        }
        /// <summary> returns the number of ars needed for this VM
        /// 
        /// </summary>
        /// <returns> The number of ars needed for this VM
        /// </returns>
        virtual public int NumArgs
        {
            get
            {
                return numMacroArgs;
            }

        }
        private string macroName;
        private string[] argArray = null;
        private string[] literalArgArray = null;
        private SimpleNode nodeTree = null;
        private int numMacroArgs = 0;
        private bool preInit = false;
        private bool strictArguments;
        private bool localContextScope = false;
        private int maxCallDepth;

        /// <summary> Return name of this Velocimacro.</summary>
        /// <returns> The name of this Velocimacro.
        /// </returns>
        public override string Name
        {
            get
            {
                return macroName;
            }
        }

        /// <summary> sets the directive name of this VM
        /// 
        /// </summary>
        /// <param name="name">
        /// </param>
        public virtual void SetName(string name)
        {
            macroName = name;
        }

        /// <summary> Renders the macro using the context.
        /// 
        /// </summary>
        /// <param name="context">Current rendering context
        /// </param>
        /// <param name="writer">Writer for output
        /// </param>
        /// <param name="node">AST that calls the macro
        /// </param>
        /// <returns> True if the directive rendered successfully.
        /// </returns>
        /// <throws>  IOException </throws>
        /// <throws>  MethodInvocationException </throws>
        /// <throws>  MacroOverflowException </throws>
        public override bool Render(IInternalContextAdapter context, System.IO.TextWriter writer, INode node)
        {
            // wrap the current context and Add the macro arguments

            // the creation of this context is a major bottleneck (incl 2x HashMap)
            //UPGRADE_NOTE: Final 已从“vmc ”的声明中移除。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
            ProxyVMContext vmc = new ProxyVMContext(context, rsvc, localContextScope);

            int callArguments = node.GetNumChildren();

            if (callArguments > 0)
            {
                // the 0th element is the macro name
                for (int i = 1; i < argArray.Length && i <= callArguments; i++)
                {
                    INode macroCallArgument = node.GetChild(i - 1);

                    /*
                    * literalArgArray[i] is needed for "render literal if null" functionality.
                    * The value is used in ASTReference render-method.
                    * 
                    * The idea is to avoid generating the literal until absolutely necessary.
                    * 
                    * This makes VMReferenceMungeVisitor obsolete and it would not work anyway 
                    * when the macro AST is shared
                    */
                    vmc.AddVMProxyArg(context, argArray[i], literalArgArray[i], macroCallArgument);
                }
            }

            /*
            * check that we aren't already at the max call depth
            */
            if (maxCallDepth > 0 && maxCallDepth == vmc.CurrentMacroCallDepth)
            {
                string templateName = vmc.CurrentTemplateName;
                object[] stack = vmc.MacroNameStack;

                System.Text.StringBuilder out_Renamed = new System.Text.StringBuilder(100).Append("Max calling depth of ").Append(maxCallDepth).Append(" was exceeded in Template:").Append(templateName).Append(" and Macro:").Append(macroName).Append(" with Call Stack:");
                for (int i = 0; i < stack.Length; i++)
                {
                    if (i != 0)
                    {
                        out_Renamed.Append("->");
                    }

                    out_Renamed.Append(stack[i]);
                }
                rsvc.Log.Error(out_Renamed);

                try
                {
                    throw new MacroOverflowException(out_Renamed.ToString());
                }
                finally
                {
                    // clean out the macro stack, since we just broke it
                    while (vmc.CurrentMacroCallDepth > 0)
                    {
                        vmc.PopCurrentMacroName();
                    }
                }
            }

            try
            {
                // render the velocity macro
                vmc.PushCurrentMacroName(macroName);
                nodeTree.Render(vmc, writer);
                vmc.PopCurrentMacroName();
                return true;
            }
            catch (System.SystemException e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                string msg = "VelocimacroProxy.render() : exception VM = #" + macroName + "()";
                rsvc.Log.Error(msg, e);
                throw new VelocityException(msg, e);
            }
        }

        /// <summary> The major meat of VelocimacroProxy, Init() checks the # of arguments.
        /// 
        /// </summary>
        /// <param name="rs">
        /// </param>
        /// <param name="context">
        /// </param>
        /// <param name="node">
        /// </param>
        /// <throws>  TemplateInitException </throws>
        public override void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
        {
            // there can be multiple threads here so avoid double inits
            lock (this)
            {
                if (!preInit)
                {
                    base.Init(rs, context, node);

                    // this is a very expensive call (ExtendedProperties is very slow)
                    strictArguments = rs.Configuration.GetBoolean(NVelocity.Runtime.RuntimeConstants.VM_ARGUMENTS_STRICT, false);

                    // support for local context scope feature, where all references are local
                    // we do not have to check this at every invocation of ProxyVMContext
                    localContextScope = rsvc.GetBoolean(NVelocity.Runtime.RuntimeConstants.VM_CONTEXT_LOCALSCOPE, false);

                    // Get the macro call depth limit
                    maxCallDepth = rsvc.GetInt(NVelocity.Runtime.RuntimeConstants.VM_MAX_DEPTH);

                    // Initialize the parsed AST
                    // since this is context independent we need to do this only once so
                    // do it here instead of the render method
                    nodeTree.Init(context, rs);

                    preInit = true;
                }
            }

            // check how many arguments we got
            int i = node.GetNumChildren();

            // Throw exception for invalid number of arguments?
            if (NumArgs != i)
            {
                // If we have a not-yet defined macro, we do Get no arguments because
                // the syntax tree looks different than with a already defined macro.
                // But we do know that we must be in a macro definition context somewhere up the
                // syntax tree.
                // Check for that, if it is true, suppress the Error message.
                // Fixes VELOCITY-71.

                for (INode parent = node.Parent; parent != null; )
                {
                    if ((parent is ASTDirective) && string.Equals(((ASTDirective)parent).DirectiveName, "macro"))
                    {
                        return;
                    }
                    parent = parent.Parent;
                }

                string msg = "VM #" + macroName + ": too " + ((NumArgs > i) ? "few" : "many") + " arguments to macro. Wanted " + NumArgs + " got " + i;

                if (strictArguments)
                {
                    /**
                    * indicate col/line assuming it starts at 0 - this will be corrected one call up
                    */
                    throw new TemplateInitException(msg, context.CurrentTemplateName, 0, 0);
                }
                else
                {
                    rsvc.Log.Debug(msg);
                    return;
                }
            }

            /* now validate that none of the arguments are plain words, (VELOCITY-614)
            * they should be string literals, references, inline maps, or inline lists */
            for (int n = 0; n < i; n++)
            {
                INode child = node.GetChild(n);
                if (child.Type == NVelocity.Runtime.Parser.ParserTreeConstants.JJTWORD)
                {
                    /* indicate col/line assuming it starts at 0
                    * this will be corrected one call up  */
                    throw new TemplateInitException("Invalid arg #" + n + " in VM #" + macroName, context.CurrentTemplateName, 0, 0);
                }
            }
        }
    }
}