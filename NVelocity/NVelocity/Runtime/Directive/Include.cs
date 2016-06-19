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
    using Resource;

    /// <summary> <p>Pluggable directive that handles the #include() statement in VTL.
    /// This #include() can take multiple arguments of either
    /// StringLiteral or Reference.</p>
    /// 
    /// <p>Notes:</p>
    /// <ol>
    /// <li>For security reasons, the included source material can only come
    /// from somewhere within the template root tree.  If you want to include
    /// content from elsewhere on your disk, Add extra template roots, or use
    /// a link from somwhere under template root to that content.</li>
    /// 
    /// <li>By default, there is no output to the render stream in the event of
    /// a problem.  You can override this behavior with two property values :
    /// include.output.errormsg.start
    /// include.output.errormsg.end
    /// If both are defined in velocity.properties, they will be used to
    /// in the render output to bracket the arg string that caused the
    /// problem.
    /// Ex. : if you are working in html then
    /// include.output.errormsg.start=&lt;!-- #include Error :
    /// include.output.errormsg.end= --&gt;
    /// might be an excellent way to start...</li>
    /// 
    /// <li>As noted above, #include() can take multiple arguments.
    /// Ex : #include('foo.vm' 'bar.vm' $foo)
    /// will include all three if valid to output without any
    /// special separator.</li>
    /// </ol>
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:kav@kav.dk">Kasper Nielsen</a>
    /// </author>
    /// <version>  $Id: Include.java 724825 2008-12-09 18:56:06Z nbubna $
    /// </version>
    public class Include : InputBase
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
        private string outputMsgStart = "";
        private string outputMsgEnd = "";

        /// <summary> Return name of this directive.</summary>
        /// <returns> The name of this directive.
        /// </returns>
        public override string Name
        {
            get
            {
                return "include";
            }
        }

        /// <summary>  simple Init - Init the tree and Get the elementKey from
        /// the AST
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
            base.Init(rs, context, node);

            /*
            *  Get the msg, and Add the space so we don't have to
            *  do it each time
            */
            outputMsgStart = rsvc.GetString(NVelocity.Runtime.RuntimeConstants.ERRORMSG_START);
            outputMsgStart = outputMsgStart + " ";

            outputMsgEnd = rsvc.GetString(NVelocity.Runtime.RuntimeConstants.ERRORMSG_END);
            outputMsgEnd = " " + outputMsgEnd;
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
        /// <throws>  MethodInvocationException </throws>
        /// <throws>  ResourceNotFoundException </throws>
        public override bool Render(IInternalContextAdapter context, System.IO.TextWriter writer, INode node)
        {
            /*
            *  Get our arguments and check them
            */

            int argCount = node.GetNumChildren();

            for (int i = 0; i < argCount; i++)
            {
                /*
                *  we only handle StringLiterals and References right now
                */

                INode n = node.GetChild(i);

                if (n.Type == NVelocity.Runtime.Parser.ParserTreeConstants.JJTSTRINGLITERAL || n.Type == NVelocity.Runtime.Parser.ParserTreeConstants.JJTREFERENCE)
                {
                    if (!RenderOutput(n, context, writer))
                        OutputErrorToStream(writer, "error with arg " + i + " please see log.");
                }
                else
                {
                    string msg = "invalid #include() argument '" + n.ToString() + "' at " + Log.Log.FormatFileString(this);
                    rsvc.Log.Error(msg);
                    OutputErrorToStream(writer, "error with arg " + i + " please see log.");
                    throw new VelocityException(msg);
                }
            }

            return true;
        }

        /// <summary>  does the parameters rendering of the included file
        /// 
        /// </summary>
        /// <param name="node">AST argument of type StringLiteral or Reference
        /// </param>
        /// <param name="context">valid context so we can render References
        /// </param>
        /// <param name="writer">output Writer
        /// </param>
        /// <returns> boolean success or failure.  failures are logged
        /// </returns>
        /// <exception cref="IOException">
        /// </exception>
        /// <exception cref="MethodInvocationException">
        /// </exception>
        /// <exception cref="ResourceNotFoundException">
        /// </exception>
        private bool RenderOutput(INode node, IInternalContextAdapter context, System.IO.TextWriter writer)
        {
            if (node == null)
            {
                rsvc.Log.Error("#include() null argument");
                return false;
            }

            /*
            *  does it have a value?  If you have a null reference, then no.
            */
            object value = node.Value(context);
            if (value == null)
            {
                rsvc.Log.Error("#include() null argument");
                return false;
            }

            /*
            *  Get the path
            */
            string sourcearg = value.ToString();

            /*
            *  check to see if the argument will be changed by the event handler
            */

            string arg = EventHandlerUtil.IncludeEvent(rsvc, context, sourcearg, context.CurrentTemplateName, Name);

            /*
            *   a null return value from the event cartridge indicates we should not
            *   input a resource.
            */
            bool blockinput = false;
            if (arg == null)
                blockinput = true;

            Resource resource = null;

            try
            {
                if (!blockinput)
                    resource = rsvc.GetContent(arg, GetInputEncoding(context));
            }
            catch (ResourceNotFoundException rnfe)
            {
                /*
                * the arg wasn't found.  Note it and throw
                */
                rsvc.Log.Error("#include(): cannot find resource '" + arg + "', called at " + Log.Log.FormatFileString(this));
                throw rnfe;
            }
            /**
            * pass through application level runtime exceptions
            */
            catch (System.SystemException e)
            {
                rsvc.Log.Error("#include(): arg = '" + arg + "', called at " + Log.Log.FormatFileString(this));
                throw e;
            }
            catch (System.Exception e)
            {
                string msg = "#include(): arg = '" + arg + "', called at " + Log.Log.FormatFileString(this);
                rsvc.Log.Error(msg, e);
                throw new VelocityException(msg, e);
            }


            /*
            *    note - a blocked input is still a successful operation as this is
            *    expected behavior.
            */

            if (blockinput)
                return true;
            else if (resource == null)
                return false;

            writer.Write((string)resource.Data);
            return true;
        }

        /// <summary>  Puts a message to the render output stream if ERRORMSG_START / END
        /// are valid property strings.  Mainly used for end-user template
        /// debugging.
        /// </summary>
        /// <param name="writer">
        /// </param>
        /// <param name="msg">
        /// </param>
        /// <throws>  IOException </throws>
        private void OutputErrorToStream(System.IO.TextWriter writer, string msg)
        {
            if (outputMsgStart != null && outputMsgEnd != null)
            {
                writer.Write(outputMsgStart);
                writer.Write(msg);
                writer.Write(outputMsgEnd);
            }
        }
    }
}
