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
    using System.IO;

    using Context;
    using Exception;
    using Parser;
    using Parser.Node;
    using Util.Introspection;

    /// <summary> Evaluates the directive argument as a VTL string, using the existing
    /// context.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@apache.org">Will Glass-Husain</a>
    /// </author>
    /// <version>  $Id: Evaluate.java 737544 2009-01-25 17:42:08Z nbubna $
    /// </version>
    /// <since> 1.6
    /// </since>
    public class Evaluate : Directive
    {
        /// <summary> Return type of this directive.</summary>
        /// <returns> The type of this directive.
        /// </returns>
        public override int Type
        {
            get
            {
                return DirectiveType.LINE;
            }

        }

        /// <summary> Return name of this directive.</summary>
        /// <returns> The name of this directive.
        /// </returns>
        public override string Name
        {
            get
            {
                return "evaluate";
            }
        }

        /// <summary> Initialize and check arguments.</summary>
        /// <param name="rs">
        /// </param>
        /// <param name="context">
        /// </param>
        /// <param name="node">
        /// </param>
        /// <throws>  TemplateInitException </throws>
        public override void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
        {
            base.Init(rs, context, node);

            /**
            * Check that there is exactly one argument and it is a string or reference.
            */

            int argCount = node.GetNumChildren();
            if (argCount == 0)
            {
                throw new TemplateInitException("#" + Name + "() requires exactly one argument", context.CurrentTemplateName, node.Column, node.Line);
            }
            if (argCount > 1)
            {
                /* 
                * use line/col of second argument
                */

                throw new TemplateInitException("#" + Name + "() requires exactly one argument", context.CurrentTemplateName, node.GetChild(1).Column, node.GetChild(1).Line);
            }

            INode childNode = node.GetChild(0);
            if (childNode.Type != NVelocity.Runtime.Parser.ParserTreeConstants.JJTSTRINGLITERAL && childNode.Type != NVelocity.Runtime.Parser.ParserTreeConstants.JJTREFERENCE)
            {
                throw new TemplateInitException("#" + Name + "()  argument must be a string literal or reference", context.CurrentTemplateName, childNode.Column, childNode.Line);
            }
        }

        /// <summary> Evaluate the argument, convert to a String, and Evaluate again 
        /// (with the same context).
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <param name="writer">
        /// </param>
        /// <param name="node">
        /// </param>
        /// <returns> True if the directive rendered successfully.
        /// </returns>
        /// <throws>  IOException </throws>
        /// <throws>  ResourceNotFoundException </throws>
        /// <throws>  ParseErrorException  </throws>
        /// <throws>  MethodInvocationException </throws>
        public override bool Render(IInternalContextAdapter context, System.IO.TextWriter writer, INode node)
        {

            /*
            * Evaluate the string with the current context.  We know there is
            * exactly one argument and it is a string or reference.
            */

            object value = node.GetChild(0).Value(context);
            string sourceText;
            if (value != null)
            {
                sourceText = value.ToString();
            }
            else
            {
                sourceText = "";
            }

            /*
            * The new string needs to be parsed since the text has been dynamically generated.
            */
            string templateName = context.CurrentTemplateName;
            SimpleNode nodeTree = null;

            try
            {
                nodeTree = rsvc.Parse(new StringReader(sourceText), templateName, false);
            }
            catch (ParseException pex)
            {
                // use the line/column from the template
                Info info = new Info(templateName, node.Line, node.Column);

                throw new ParseErrorException(pex.Message, info);
            }
            catch (TemplateInitException pex)
            {
                Info info = new Info(templateName, node.Line, node.Column);

                throw new ParseErrorException(pex.Message, info);
            }

            /*
            * now we want to Init and render.  Chain the context
            * to prevent any changes to the current context.
            */

            if (nodeTree != null)
            {
                IInternalContextAdapter ica = new EvaluateContext(context, rsvc);

                ica.PushCurrentTemplateName(templateName);

                try
                {
                    try
                    {
                        nodeTree.Init(ica, rsvc);
                    }
                    catch (TemplateInitException pex)
                    {
                        Info info = new Info(templateName, node.Line, node.Column);

                        throw new ParseErrorException(pex.Message, info);
                    }

                    try
                    {
                        /*
                        *  now render, and let any exceptions fly
                        */
                        nodeTree.Render(ica, writer);
                    }
                    catch (ParseErrorException pex)
                    {
                        // convert any parsing errors to the correct line/col
                        Info info = new Info(templateName, node.Line, node.Column);

                        throw new ParseErrorException(pex.Message, info);
                    }
                }
                finally
                {
                    ica.PopCurrentTemplateName();
                }

                return true;
            }


            return false;
        }
    }
}
