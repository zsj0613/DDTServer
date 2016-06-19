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
    using App.Event;
    using Context;
    using Exception;
    using Parser.Node;

    /// <summary> Pluggable directive that handles the <code>#parse()</code>
    /// statement in VTL.
    /// 
    /// <pre>
    /// Notes:
    /// -----
    /// 1) The parsed source material can only come from somewhere in
    /// the TemplateRoot tree for security reasons. There is no way
    /// around this.  If you want to include content from elsewhere on
    /// your disk, use a link from somwhere under Template Root to that
    /// content.
    /// 
    /// 2) There is a limited parse depth.  It is set as a property
    /// "directive.parse.max.depth = 10" by default.  This 10 deep
    /// limit is a safety feature to prevent infinite loops.
    /// </pre>
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:Christoph.Reck@dlr.de">Christoph Reck</a>
    /// </author>
    /// <version>  $Id: Parse.java 724825 2008-12-09 18:56:06Z nbubna $
    /// </version>
    public class Parse : InputBase
    {
        /// <summary> Return type of this directive.</summary>
        /// <returns> The type of this directive.
        /// </returns>
        override public int Type
        {
            get
            {
                return NVelocity.Runtime.Directive.DirectiveType.LINE;
            }

        }
        private int maxDepth;

        /// <summary> Return name of this directive.</summary>
        /// <returns> The name of this directive.
        /// </returns>
        public override string Name
        {
            get
            {
                return "parse";
            }
        }

        /// <summary> Init's the #parse directive.</summary>
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

            this.maxDepth = rsvc.GetInt(NVelocity.Runtime.RuntimeConstants.PARSE_DIRECTIVE_MAXDEPTH, 10);
        }

        /// <summary>  iterates through the argument list and renders every
        /// argument that is appropriate.  Any non appropriate
        /// arguments are logged, but render() continues.
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
        /// <throws>  ParseErrorException </throws>
        /// <throws>  MethodInvocationException </throws>
        public override bool Render(IInternalContextAdapter context, System.IO.TextWriter writer, INode node)
        {
            /*
            * if rendering is no longer allowed (after a stop), we can safely
            * skip execution of all the parse directives.
            */
            if (!context.AllowRendering)
            {
                return true;
            }

            /*
            *  did we Get an argument?
            */
            if (node.GetChild(0) == null)
            {
                rsvc.Log.Error("#parse() null argument");
                return false;
            }

            /*
            *  does it have a value?  If you have a null reference, then no.
            */
            object value = node.GetChild(0).Value(context);

            if (value == null)
            {
                rsvc.Log.Error("#parse() null argument");
                return false;
            }

            /*
            *  Get the path
            */
           
            string sourcearg = value.ToString();

            /*
            *  check to see if the argument will be changed by the event cartridge
            */


            string arg = EventHandlerUtil.IncludeEvent(rsvc, context, sourcearg, context.CurrentTemplateName, Name);

            /*
            *   a null return value from the event cartridge indicates we should not
            *   input a resource.
            */
            bool blockinput = false;
            if (arg == null)
                blockinput = true;


            if (maxDepth > 0)
            {
                /* 
                * see if we have exceeded the configured depth.
                */
                object[] templateStack = context.TemplateNameStack;
                if (templateStack.Length >= maxDepth)
                {
                    System.Text.StringBuilder path = new System.Text.StringBuilder();
                    for (int i = 0; i < templateStack.Length; ++i)
                    {
                        path.Append(" > " + templateStack[i]);
                    }
                    rsvc.Log.Error("Max recursion depth reached (" + templateStack.Length + ')' + " File stack:" + path);
                    return false;
                }
            }

            /*
            *  now use the Runtime resource loader to Get the template
            */

            Template t = null;

            try
            {

                if (!blockinput)
                    t = rsvc.GetTemplate(arg, GetInputEncoding(context));
            }
            catch (ResourceNotFoundException rnfe)
            {
                /*
                * the arg wasn't found.  Note it and throw
                */
                rsvc.Log.Error("#parse(): cannot find template '" + arg + "', called at " + Log.Log.FormatFileString(this));
                throw rnfe;
            }
            catch (ParseErrorException pee)
            {
                /*
                * the arg was found, but didn't parse - syntax Error
                *  note it and throw
                */
                rsvc.Log.Error("#parse(): syntax error in #parse()-ed template '" + arg + "', called at " + Log.Log.FormatFileString(this));

                throw pee;
            }
            /**
            * pass through application level runtime exceptions
            */
            catch (System.SystemException e)
            {
                rsvc.Log.Error("Exception rendering #parse(" + arg + ") at " + Log.Log.FormatFileString(this));
                throw e;
            }
            catch (System.Exception e)
            {
                string msg = "Exception rendering #parse(" + arg + ") at " + Log.Log.FormatFileString(this);
                rsvc.Log.Error(msg, e);
                throw new VelocityException(msg, e);
            }

            /**
            * Add the template name to the macro libraries list
            */
            System.Collections.IList macroLibraries = context.MacroLibraries;

            /**
            * if macroLibraries are not set create a new one
            */
            if (macroLibraries == null)
            {
                macroLibraries = new System.Collections.ArrayList();
            }

            context.MacroLibraries = macroLibraries;

            macroLibraries.Add(arg);

            /*
            *  and render it
            */
            try
            {
                if (!blockinput)
                {
                    context.PushCurrentTemplateName(arg);
                    ((SimpleNode)t.Data).Render(context, writer);
                }
            }

            /**
            * pass through application level runtime exceptions
            */
            catch (System.SystemException e)
            {
                /**
                * LogMessage #parse errors so the user can track which file called which.
                */
                rsvc.Log.Error("Exception rendering #parse(" + arg + ") at " + Log.Log.FormatFileString(this));
                throw e;
            }
            catch (System.Exception e)
            {
                string msg = "Exception rendering #parse(" + arg + ") at " + Log.Log.FormatFileString(this);
                rsvc.Log.Error(msg, e);
                throw new VelocityException(msg, e);
            }
            finally
            {
                if (!blockinput)
                    context.PopCurrentTemplateName();
            }

            /*
            *    note - a blocked input is still a successful operation as this is
            *    expected behavior.
            */

            return true;
        }
    }
}