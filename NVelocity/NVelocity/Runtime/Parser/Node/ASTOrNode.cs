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

    /// <summary> Please look at the Parser.jjt file which is
    /// what controls the generation of this class.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: ASTOrNode.java 685370 2008-08-12 23:36:35Z nbubna $
    /// </version>
    public class ASTOrNode : SimpleNode
    {
        /// <param name="id">
        /// </param>
        public ASTOrNode(int id)
            : base(id)
        {
        }

        /// <param name="p">
        /// </param>
        /// <param name="id">
        /// </param>
        public ASTOrNode(Parser p, int id)
            : base(p, id)
        {
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.Accept(NVelocity.Runtime.Paser.Node.IParserVisitor, System.Object)">
        /// </seealso>
        public override object Accept(IParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }

        /// <summary>  Returns the value of the expression.
        /// Since the value of the expression is simply the boolean
        /// result of Evaluate(), lets return that.
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <returns> The Expression value.
        /// </returns>
        /// <throws>  MethodInvocationException </throws>
        public override object Value(IInternalContextAdapter context)
        {
            // TODO: JDK 1.4+ -> valueOf()
            // return new Boolean(Evaluate(context));
            return Evaluate(context) ? true : false;
        }

        /// <summary>  the logical or :
        /// the rule :
        /// left || null -> left
        /// null || right -> right
        /// null || null -> false
        /// left || right ->  left || right
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <returns> The evaluation result.
        /// </returns>
        /// <throws>  MethodInvocationException </throws>
        public override bool Evaluate(IInternalContextAdapter context)
        {
            INode left = GetChild(0);
            INode right = GetChild(1);

            /*
            *  if the left is not null and true, then true
            */

            if (left != null && left.Evaluate(context))
                return true;

            /*
            *  same for right
            */

            if (right != null && right.Evaluate(context))
                return true;

            return false;
        }
    }
}