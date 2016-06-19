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

namespace NVelocity.Runtime.Parser.Node
{
    using System.IO;
    using System.Text;

    using Context;
    using Exception;

    /// <summary> ASTStringLiteral support. Will Interpolate!
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <version>  $Id: ASTStringLiteral.java 705297 2008-10-16 17:59:24Z nbubna $
    /// </version>
    public class ASTStringLiteral : SimpleNode
    {
        /// <summary> Check to see if this is an interpolated string.</summary>
        /// <returns> true if this is constant (not an interpolated string)
        /// </returns>
        /// <since> 1.6
        /// </since>
        public bool Constant
        {
            get
            {
                return !interpolate;
            }

        }
        /* cache the value of the interpolation switch */
        private bool interpolate = true;

        private SimpleNode nodeTree = null;

        private string image = "";

        private string interpolateimage = "";

        /// <summary>true if the string contains a line comment (##) </summary>
        private bool containsLineComment;

        /// <param name="id">
        /// </param>
        public ASTStringLiteral(int id)
            : base(id)
        {
        }

        /// <param name="p">
        /// </param>
        /// <param name="id">
        /// </param>
        public ASTStringLiteral(Parser p, int id)
            : base(p, id)
        {
        }

        /// <summary> Init : we don't have to do much. Init the tree (there shouldn't be one)
        /// and then see if interpolation is turned on.
        /// 
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <param name="data">
        /// </param>
        /// <returns> Init result.
        /// </returns>
        /// <throws>  TemplateInitException </throws>
        public override object Init(IInternalContextAdapter context, object data)
        {
            /*
            * simple habit... we prollie don't have an AST beneath us
            */

            base.Init(context, data);

            /*
            * the stringlit is set at template parse time, so we can do this here
            * for now. if things change and we can somehow create stringlits at
            * runtime, this must move to the runtime execution path
            * 
            * so, only if interpolation is turned on AND it starts with a " AND it
            * has a directive or reference, then we can Interpolate. Otherwise,
            * don't bother.
            */

            interpolate = rsvc.GetBoolean(RuntimeConstants.INTERPOLATE_STRINGLITERALS, true) && FirstToken.Image.StartsWith("\"") && ((FirstToken.Image.IndexOf('$') != -1) || (FirstToken.Image.IndexOf('#') != -1));

            /*
            * Get the contents of the string, minus the '/" at each end
            */

            image = FirstToken.Image.Substring(1, (FirstToken.Image.Length - 1) - (1));
            if (FirstToken.Image.StartsWith("\""))
            {
                image = Unescape(image);
            }

            /**
            * note. A kludge on a kludge. The first part, Geir calls this the
            * dreaded <MORE> kludge. Basically, the use of the <MORE> token eats
            * the last character of an interpolated string. EXCEPT when a line
            * comment (##) is in the string this isn't an issue.
            * 
            * So, to solve this we look for a line comment. If it isn't found we
            * Add a space here and remove it later.
            */

            /**
            * Note - this should really use a regexp to look for [^\]## but
            * apparently escaping of line comments isn't working right now anyway.
            */
            containsLineComment = (image.IndexOf("##") != -1);

            /*
            * if appropriate, tack a space on the end (dreaded <MORE> kludge)
            */

            if (!containsLineComment)
            {
                interpolateimage = image + " ";
            }
            else
            {
                interpolateimage = image;
            }

            if (interpolate)
            {
                /*
                * now parse and Init the nodeTree
                */
                StringReader br = new StringReader(interpolateimage);

                /*
                * it's possible to not have an initialization context - or we don't
                * want to trust the caller - so have a fallback value if so
                * 
                * Also, do *not* dump the VM namespace for this template
                */

                string templateName = (context != null) ? context.CurrentTemplateName : "StringLiteral";
                try
                {
                    nodeTree = rsvc.Parse(br, templateName, false);
                }
                catch (ParseException e)
                {
                    string msg = "Failed to parse String literal at " + Log.Log.FormatFileString(templateName, Line, Column);
                    throw new TemplateInitException(msg, e, templateName, Column, Line);
                }

                AdjTokenLineNums(nodeTree);

                /*
                * Init with context. It won't modify anything
                */

                nodeTree.Init(context, rsvc);
            }

            return data;
        }

        /// <summary> Adjust all the line and column numbers that comprise a node so that they
        /// are corrected for the string literals position within the template file.
        /// This is neccessary if an exception is thrown while processing the node so
        /// that the line and column position reported reflects the Error position
        /// within the template and not just relative to the Error position within
        /// the string literal.
        /// </summary>
        public void AdjTokenLineNums(INode node)
        {
            Token tok = node.FirstToken;
            // Test against null is probably not neccessary, but just being safe
            while (tok != null && tok != node.LastToken)
            {
                // If tok is on the first line, then the parameters column is 
                // offset by the template column.

                if (tok.BeginLine == 1)
                    tok.BeginColumn += Column;

                if (tok.EndLine == 1)
                    tok.EndColumn += Column;

                tok.BeginLine += Line - 1;
                tok.EndLine += Line - 1;
                tok = tok.Next;
            }
        }


        /// <since> 1.6
        /// </since>
        public static string Unescape(string str)
        {
            int u = str.IndexOf("\\u");
            if (u < 0)
                return str;

            StringBuilder result = new StringBuilder();

            int lastCopied = 0;

            for (; ; )
            {
                result.Append(str.Substring(lastCopied, (u) - (lastCopied)));

                /* we don't worry about an exception here,
                * because the lexer checked that string is correct */
                char c = (char)System.Convert.ToInt32(str.Substring(u + 2, (u + 6) - (u + 2)), 16);
                result.Append(c);

                lastCopied = u + 6;

                u = str.IndexOf("\\u", lastCopied);
                if (u < 0)
                {
                    result.Append(str.Substring(lastCopied));
                    return result.ToString();
                }
            }
        }


        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.jjtAccept(NVelocity.Runtime.Paser.Node.ParserVisitor,">
        /// java.lang.Object)
        /// </seealso>
        public override object Accept(IParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }

        /// <summary> renders the value of the string literal If the properties allow, and the
        /// string literal contains a $ or a # the literal is rendered against the
        /// context Otherwise, the stringlit is returned.
        /// 
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <returns> result of the rendering.
        /// </returns>
        public override object Value(IInternalContextAdapter context)
        {
            if (interpolate)
            {
                try
                {
                    /*
                    * now render against the real context
                    */

                    StringWriter writer = new StringWriter();
                    nodeTree.Render(context, writer);

                    /*
                    * and return the result as a String
                    */

                    string ret = writer.ToString();

                    /*
                    * if appropriate, remove the space from the end (dreaded <MORE>
                    * kludge part deux)
                    */
                    if (!containsLineComment && ret.Length > 0)
                    {
                        return ret.Substring(0, (ret.Length - 1) - (0));
                    }
                    else
                    {
                        return ret;
                    }
                }
                /**
                * pass through application level runtime exceptions
                */
                catch (System.IO.IOException e)
                {
                    string msg = "Error in interpolating string literal";
                    log.Error(msg, e);
                    throw new VelocityException(msg, e);
                }
                catch (System.SystemException e)
                {
                    throw e;
                }
            }

            /*
            * ok, either not allowed to Interpolate, there wasn't a ref or
            * directive, or we failed, so just output the literal
            */

            return image;
        }
    }
}