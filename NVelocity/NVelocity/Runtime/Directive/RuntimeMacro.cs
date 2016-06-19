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
    using System.Text;

    using Context;
    using Exception;
    using Parser;
    using Parser.Node;
    using Util.Introspection;

    /// <summary> This class acts as a proxy for potential macros.  When the AST is built
    /// this class is inserted as a placeholder for the macro (whether or not
    /// the macro is actually defined).  At render time we check whether there is
    /// a implementation for the macro call. If an implementation cannot be
    /// found the literal text is rendered.
    /// </summary>
    /// <since> 1.6
    /// </since>
    public class RuntimeMacro : Directive
    {
        /// <summary> Velocimacros are always LINE
        /// type directives.
        /// 
        /// </summary>
        /// <returns> The type of this directive.
        /// </returns>
        override public int Type
        {
            get
            {
                return NVelocity.Runtime.Directive.DirectiveType.LINE;
            }

        }
        /// <summary> It is probably quite rare that we need to render the macro literal
        /// so do it only on-demand and then cache the value. This tactic helps to
        /// reduce memory usage a bit.
        /// </summary>
        private string Literal
        {
            get
            {
                if (literal == null)
                {
                    StringBuilder buffer = new StringBuilder();
                    Token t = node.FirstToken;

                    while (t != null && t != node.LastToken)
                    {
                        buffer.Append(t.Image);
                        t = t.Next;
                    }

                    if (t != null)
                    {
                        buffer.Append(t.Image);
                    }

                    literal = buffer.ToString();
                }
                return literal;
            }

        }
        /// <summary> Name of the macro</summary>
        private string macroName;

        /// <summary> source template name</summary>
        private string sourceTemplate;

        /// <summary> Literal text of the macro</summary>
        private string literal = null;

        /// <summary> Node of the macro call</summary>
        private INode node = null;

        /// <summary> Indicates if we are running in strict reference mode.</summary>
        protected internal bool strictRef = false;

        /// <summary> Create a RuntimeMacro instance. Macro name and source
        /// template stored for later use.
        /// 
        /// </summary>
        /// <param name="macroName">name of the macro
        /// </param>
        /// <param name="sourceTemplate">template where macro call is made
        /// </param>
        public RuntimeMacro(string macroName, string sourceTemplate)
        {
            if (macroName == null || sourceTemplate == null)
            {
                throw new System.ArgumentException("Null arguments");
            }

            this.macroName = macroName;
            this.sourceTemplate = sourceTemplate;
        }

        /// <summary> Return name of this Velocimacro.
        /// 
        /// </summary>
        /// <returns> The name of this Velocimacro.
        /// </returns>
        public override string Name
        {
            get
            {
                return macroName;
            }
        }


        /// <summary> Intialize the Runtime macro. At the Init time no implementation so we
        /// just save the values to use at the render time.
        /// 
        /// </summary>
        /// <param name="rs">runtime services
        /// </param>
        /// <param name="context">InternalContextAdapter
        /// </param>
        /// <param name="node">node containing the macro call
        /// </param>
        public override void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
        {
            base.Init(rs, context, node);
            rsvc = rs;
            this.node = node;

            /**
            * Only check for strictRef setting if this really looks like a macro,
            * so strict mode doesn't balk at things like #E0E0E0 in a template.
            */
            Token t = node.LastToken;
            if (t.Image[0] == ')')
            {
                strictRef = rsvc.GetBoolean(NVelocity.Runtime.RuntimeConstants.RUNTIME_REFERENCES_STRICT, false);
            }
        }


        /// <summary> Velocimacro implementation is not known at the Init time. So look for
        /// a implementation in the macro libaries and if finds one renders it. The
        /// parameters rendering is delegated to the VelocimacroProxy object. When
        /// looking for a macro we first loot at the template with has the
        /// macro call then we look at the macro lbraries in the order they appear
        /// in the list. If a macro has many definitions above look up will
        /// determine the precedence.
        /// 
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <param name="writer">
        /// </param>
        /// <param name="node">
        /// </param>
        /// <returns> true if the rendering is successfull
        /// </returns>
        /// <throws>  IOException </throws>
        /// <throws>  ResourceNotFoundException </throws>
        /// <throws>  ParseErrorException </throws>
        /// <throws>  MethodInvocationException </throws>
        public override bool Render(IInternalContextAdapter context, System.IO.TextWriter writer, INode node)
        {
            VelocimacroProxy vmProxy = null;
            string renderingTemplate = context.CurrentTemplateName;

            /**
            * first look in the source template
            */
            object o = rsvc.GetVelocimacro(macroName, sourceTemplate, renderingTemplate);

            if (o != null)
            {
                // getVelocimacro can only return a VelocimacroProxy so we don't need the
                // costly instanceof check
                vmProxy = (VelocimacroProxy)o;
            }

            /**
            * if not found, look in the macro libraries.
            */
            if (vmProxy == null)
            {
                System.Collections.IList macroLibraries = context.MacroLibraries;
                if (macroLibraries != null)
                {
                    for (int i = macroLibraries.Count - 1; i >= 0; i--)
                    {
                        o = rsvc.GetVelocimacro(macroName, (string)macroLibraries[i], renderingTemplate);

                        // Get the first matching macro
                        if (o != null)
                        {
                            vmProxy = (VelocimacroProxy)o;
                            break;
                        }
                    }
                }
            }

            if (vmProxy != null)
            {
                try
                {
                    // mainly check the number of arguments
                    vmProxy.Init(rsvc, context, node);
                }
                catch (TemplateInitException die)
                {
                    Info info = new Info(sourceTemplate, node.Line, node.Column);
                   
                    throw new ParseErrorException(die.Message + " at " + Log.Log.FormatFileString(info), info);
                }

                try
                {
                    return vmProxy.Render(context, writer, node);
                }
                catch (System.IO.IOException e)
                {
                    rsvc.Log.Error("Exception in macro #" + macroName + " at " + Log.Log.FormatFileString(sourceTemplate, Line, Column));
                    throw e;
                }
                catch (System.SystemException e)
                {
                    /**
                    * We catch, the exception here so that we can record in
                    * the logs the template and line number of the macro call
                    * which generate the exception.  This information is
                    * especially important for multiple macro call levels.
                    * this is also true for the following catch blocks.
                    */
                    rsvc.Log.Error("Exception in macro #" + macroName + " at " + Log.Log.FormatFileString(sourceTemplate, Line, Column));
                    throw e;
                }
               
            }
            else if (strictRef)
            {
                Info info = new Info(sourceTemplate, node.Line, node.Column);
                throw new ParseErrorException("Macro '#" + macroName + "' is not defined at " + Log.Log.FormatFileString(info), info);
            }

            /**
            * If we cannot find an implementation write the literal text
            */
            writer.Write(Literal);
            return true;
        }
    }
}
