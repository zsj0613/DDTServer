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
    using Util;

    /// <summary>  Handles <code>arg1  == arg2</code>
    /// 
    /// This operator requires that the LHS and RHS are both of the
    /// same Class OR both are subclasses of java.lang.Number
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <author>  <a href="mailto:pero@antaramusic.de">Peter Romianowski</a>
    /// </author>
    /// <version>  $Id: ASTEQNode.java 691048 2008-09-01 20:26:11Z nbubna $
    /// </version>
    public class ASTEQNode : SimpleNode
    {
        /// <param name="id">
        /// </param>
        public ASTEQNode(int id)
            : base(id)
        {
        }

        /// <param name="p">
        /// </param>
        /// <param name="id">
        /// </param>
        public ASTEQNode(Parser p, int id)
            : base(p, id)
        {
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.Accept(NVelocity.Runtime.Paser.Node.IParserVisitor, System.Object)">
        /// </seealso>
        public override object Accept(IParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }

        /// <summary>   Calculates the value of the logical expression
        /// 
        /// arg1 == arg2
        /// 
        /// All class types are supported.   Uses equals() to
        /// determine equivalence.  This should work as we represent
        /// with the types we already support, and anything else that
        /// implements equals() to mean more than identical references.
        /// 
        /// 
        /// </summary>
        /// <param name="context"> internal context used to Evaluate the LHS and RHS
        /// </param>
        /// <returns> true if equivalent, false if not equivalent,
        /// false if not compatible arguments, or false
        /// if either LHS or RHS is null
        /// </returns>
        /// <throws>  MethodInvocationException </throws>
        public override bool Evaluate(IInternalContextAdapter context)
        {
            object left = GetChild(0).Value(context);
            object right = GetChild(1).Value(context);

            // for equality, they are allowed to be null references 
            try
            {
                if (ObjectComparer.CompareObjects(left, right) == 0)
                    return true;
            }
            catch
            {
                // Ignore, we can't compare decently by value, but we honestly don't give a sh*t
            }

            // They are not equal by value, try a reference comparison
            // reference equal => definitely equal objects ;)
            // For operator overloaded types, this will not really be a reference comp, but that's ok.
            return left == right;
        }

        private string GetLiteral(bool left)
        {
            return GetChild(left ? 0 : 1).Literal;
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.Value(NVelocity.Context.IInternalContextAdapter)">
        /// </seealso>
        public override object Value(IInternalContextAdapter context)
        {
            return Evaluate(context) ? true : false;
        }
    }
}