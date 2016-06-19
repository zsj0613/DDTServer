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
    using Parser.Node;

    /// <summary> Base class for all directives used in Velocity.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <version>  $Id: Directive.java 724825 2008-12-09 18:56:06Z nbubna $
    /// </version>
    public abstract class Directive
    {
        /// <summary> Get the directive type BLOCK/LINE.</summary>
        /// <returns> The directive type BLOCK/LINE.
        /// </returns>
        public abstract int Type { get; }
        /// <summary> for Log msg purposes</summary>
        /// <returns> The current line for Log msg purposes.
        /// </returns>
        virtual public int Line
        {
            get
            {
                return line;
            }

        }
        /// <summary> for Log msg purposes</summary>
        /// <returns> The current column for Log msg purposes.
        /// </returns>
        virtual public int Column
        {
            get
            {
                return column;
            }

        }

        /// <returns> The template file name this directive was defined in, or null if not 
        /// defined in a file.
        /// </returns>
        virtual public string TemplateName
        {
            get
            {
                return templateName;
            }

        }

        private int line = 0;
        private int column = 0;
        private string templateName;

        /// <summary> </summary>
        protected internal IRuntimeServices rsvc = null;

        /// <summary> Return the name of this directive.</summary>
        /// <returns> The name of this directive.
        /// </returns>
        public abstract string Name { get; }

        /// <summary> Allows the template location to be set.</summary>
        /// <param name="line">
        /// </param>
        /// <param name="column">
        /// </param>
        public virtual void SetLocation(int line, int column)
        {
            this.line = line;
            this.column = column;
        }

        /// <summary> Allows the template location to be set.</summary>
        /// <param name="line">
        /// </param>
        /// <param name="column">
        /// </param>
        public virtual void SetLocation(int line, int column, string templateName)
        {
            SetLocation(line, column);
            this.templateName = templateName;
        }

        /// <summary> How this directive is to be initialized.</summary>
        /// <param name="rs">
        /// </param>
        /// <param name="context">
        /// </param>
        /// <param name="node">
        /// </param>
        /// <throws>  TemplateInitException </throws>
        public virtual void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
        {
            rsvc = rs;

            //        int i, k = node.jjtGetNumChildren();

            //for (i = 0; i < k; i++)
            //    node.jjtGetChild(i).Init(context, rs);
        }

        /// <summary> How this directive is to be rendered</summary>
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
        public abstract bool Render(IInternalContextAdapter context, System.IO.TextWriter writer, INode node);
    }
}