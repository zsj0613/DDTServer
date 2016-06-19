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
    using System;

    using Context;
    using Parser.Node;

    /// <summary> Break directive used for interrupting foreach loops.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wyla@removethis.sci.fi">Jarkko Viinamaki</a>
    /// </author>
    /// <version>  $Id$
    /// </version>
    public class Break : Directive
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
        /// <summary> Return name of this directive.</summary>
        /// <returns> The name of this directive.
        /// </returns>
        public override string Name
        {
            get
            {
                return "break";
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
        }

        /// <summary> Break directive does not actually do any rendering. 
        /// 
        /// This directive throws a BreakException (RuntimeException) which
        /// signals foreach directive to break out of the loop. Note that this
        /// directive does not verify that it is being called inside a foreach
        /// loop.
        /// 
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <param name="writer">
        /// </param>
        /// <param name="node">
        /// </param>
        /// <returns> true if the directive rendered successfully.
        /// </returns>
        /// <throws>  IOException </throws>
        /// <throws>  MethodInvocationException </throws>
        /// <throws>  ResourceNotFoundException </throws>
        /// <throws>  ParseErrorException </throws>
        public override bool Render(IInternalContextAdapter context, System.IO.TextWriter writer, INode node)
        {
            throw new BreakException();
        }

        [Serializable]
        public class BreakException : System.SystemException
        {

        }
    }
}
