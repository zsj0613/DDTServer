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
    using Context;
    using Directive;
    using Exception;
    using Util;

    /// <summary> This class is responsible for handling the pluggable
    /// directives in VTL.
    /// 
    /// For example :  #foreach()
    /// 
    /// Please look at the Parser.jjt file which is
    /// what controls the generation of this class.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  <a href="mailto:kav@kav.dk">Kasper Nielsen</a>
    /// </author>
    /// <version>  $Id: ASTDirective.java 720228 2008-11-24 16:58:33Z nbubna $
    /// </version>
    public class ASTDirective : SimpleNode
    {
        /// <summary>  Gets the name of this directive.</summary>
        /// <returns> The name of this directive.
        /// </returns>
        /// <summary>   Sets the directive name.  Used by the parser.  This keeps us from having to
        /// dig it out of the token stream and gives the parse the change to override.
        /// </summary>
        /// <param name="str">
        /// </param>
        public virtual string DirectiveName
        {
            get
            {
                return directiveName;
            }

            set
            {
                directiveName = value;
            }

        }
        private Directive directive ;
        private string directiveName = "";
        private bool isDirective;
        private bool isInitialized;

        public Directive Directive
        {
            get
            {
                return directive;
            }
        }

        /// <param name="id">
        /// </param>
        public ASTDirective(int id)
            : base(id)
        {
        }

        /// <param name="p">
        /// </param>
        /// <param name="id">
        /// </param>
        public ASTDirective(Parser p, int id)
            : base(p, id)
        {
        }


        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.jjtAccept(NVelocity.Runtime.Paser.Node.ParserVisitor, java.lang.Object)">
        /// </seealso>
        public override object Accept(IParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.Init(org.apache.velocity.context.InternalContextAdapter, java.lang.Object)">
        /// </seealso>
        public override object Init(IInternalContextAdapter context, object data)
        {
            lock (this)
            {
                /** method is synchronized to avoid concurrent directive initialization **/

                if (!isInitialized)
                {
                    base.Init(context, data);

                    /*
                    *  only do things that are not context dependent
                    */

                    if (parser.IsDirective(directiveName))
                    {
                        isDirective = true;

                        try
                        {
                            directive = (Directive)System.Activator.CreateInstance(parser.GetDirective(directiveName).GetType());
                        }
                        catch (System.UnauthorizedAccessException e)
                        {
                            throw new RuntimeException("Couldn't initialize " + "directive of class " + parser.GetDirective(directiveName).GetType().FullName, e);
                        }
                        catch (System.Exception e)
                        {
                            throw new RuntimeException("Couldn't initialize " + "directive of class " + parser.GetDirective(directiveName).GetType().FullName, e);
                        }



                        directive.SetLocation(Line, Column);
                        directive.Init(rsvc, context, this);
                    }
                    else
                    {
                        /**
                        * Create a new RuntimeMacro
                        */
                        directive = new RuntimeMacro(directiveName, TemplateName);

                        directive.SetLocation(Line, Column);

                        /**
                        * Initialize it
                        */
                        try
                        {
                            directive.Init(rsvc, context, this);
                        }
                        /**
                        * correct the line/column number if an exception is caught
                        */
                        catch (TemplateInitException die)
                        {
                            throw new TemplateInitException(die.Message, (ParseException)die.InnerException, die.TemplateName, die.ColumnNumber + Column, die.LineNumber + Line);
                        }
                        isDirective = true;
                    }

                    isInitialized = true;
                }

                return data;
            }
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.render(org.apache.velocity.context.InternalContextAdapter, java.io.Writer)">
        /// </seealso>
        public override bool Render(IInternalContextAdapter context, System.IO.TextWriter writer)
        {
            /*
            *  normal processing
            */

            if (isDirective)
            {
                directive.Render(context, writer, this);
            }
            else
            {
                if (context.AllowRendering)
                {
                    writer.Write("#");
                    writer.Write(directiveName);
                }
            }

            return true;
        }

        /// <since> 1.5
        /// </since>
        public override string ToString()
        {
            return new System.Text.StringBuilder().Append(base.ToString()).AppendFormat("directiveName:{0}", DirectiveName).ToString();

        }
    }
}