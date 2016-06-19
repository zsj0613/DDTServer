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

    /// <summary> This class is responsible for handling the ElseIf VTL control statement.
    /// 
    /// Please look at the Parser.jjt file which is
    /// what controls the generation of this class.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: ASTElseIfStatement.java 517553 2007-03-13 06:09:58Z wglass $
    /// </version>
    public class ASTElseIfStatement : SimpleNode
    {
        /// <param name="id">
        /// </param>
        public ASTElseIfStatement(int id)
            : base(id)
        {
        }

        /// <param name="p">
        /// </param>
        /// <param name="id">
        /// </param>
        public ASTElseIfStatement(Parser p, int id)
            : base(p, id)
        {
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.Accept(NVelocity.Runtime.Paser.Node.IParserVisitor, System.Object)">
        /// </seealso>
        public override object Accept(IParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }

        /// <summary> An ASTElseStatement is true if the expression
        /// it contains evaluates to true. Expressions know
        /// how to Evaluate themselves, so we do that
        /// here and return the value back to ASTIfStatement
        /// where this node was originally asked to Evaluate
        /// itself.
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <returns> True if all childs are true.
        /// </returns>
        /// <throws>  MethodInvocationException </throws>
        public override bool Evaluate(IInternalContextAdapter context)
        {
            return GetChild(0).Evaluate(context);
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.render(org.apache.velocity.context.InternalContextAdapter, java.io.Writer)">
        /// </seealso>
        public override bool Render(IInternalContextAdapter context, System.IO.TextWriter writer)
        {
            return GetChild(1).Render(context, writer);
        }
    }
}