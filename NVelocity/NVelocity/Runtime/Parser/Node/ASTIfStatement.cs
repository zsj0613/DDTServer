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
    using System;
    using System.IO;

    using Context;

    /// <summary> </summary>
    public class ASTIfStatement : SimpleNode
    {
        /// <param name="id">
        /// </param>
        public ASTIfStatement(int id)
            : base(id)
        {
        }

        /// <param name="p">
        /// </param>
        /// <param name="id">
        /// </param>
        public ASTIfStatement(Parser p, int id)
            : base(p, id)
        {
        }


        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.Accept(NVelocity.Runtime.Paser.Node.IParserVisitor, System.Object)">
        /// </seealso>
        public override object Accept(IParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.render(org.apache.velocity.context.InternalContextAdapter, java.io.Writer)">
        /// </seealso>
        public override bool Render(IInternalContextAdapter context, System.IO.TextWriter writer)
        {
            /*
            * Check if the #if(expression) construct evaluates to true:
            * if so render and leave immediately because there
            * is nothing left to do!
            */
            if (GetChild(0).Evaluate(context))
            {
                GetChild(1).Render(context, writer);
                return true;
            }

            int totalNodes = GetNumChildren();

            /*
            * Now check the remaining nodes left in the
            * if construct. The nodes are either elseif
            *  nodes or else nodes. Each of these node
            * types knows how to Evaluate themselves. If
            * a node evaluates to true then the node will
            * render itself and this method will return
            * as there is nothing left to do.
            */
            for (int i = 2; i < totalNodes; i++)
            {
                if (GetChild(i).Evaluate(context))
                {
                    GetChild(i).Render(context, writer);
                    return true;
                }
            }

            /*
            * This is reached when an ASTIfStatement
            * consists of an if/elseif sequence where
            * none of the nodes Evaluate to true.
            */
            return true;
        }

        /// <param name="context">
        /// </param>
        /// <param name="visitor">
        /// </param>
        public virtual void Process(IInternalContextAdapter context, IParserVisitor visitor)
        {
        }
    }
}