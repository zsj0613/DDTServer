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

namespace NVelocity.Runtime.Visitor
{
    using Parser;
    using Parser.Node;

    /// <summary> This class is simply a visitor implementation
    /// that traverses the AST, produced by the Velocity
    /// parsing process, and creates a visual structure
    /// of the AST. This is primarily used for
    /// debugging, but it useful for documentation
    /// as well.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <version>  $Id: NodeViewMode.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    public class NodeViewMode : BaseVisitor
    {
        private int indent = 0;
        private bool showTokens = true;

        /// <summary>Indent child nodes to help visually identify
        /// the structure of the AST.
        /// </summary>
        private string IndentString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < indent; ++i)
            {
                sb.Append("  ");
            }
            return sb.ToString();
        }

        /// <summary> Display the type of nodes and optionally the
        /// first token.
        /// </summary>
        private object ShowNode(INode node, object data)
        {
            string tokens = "";
            string special = "";
            Token t;

            if (showTokens)
            {
                t = node.FirstToken;

                if (t.SpecialToken != null && !t.SpecialToken.Image.StartsWith("##"))
                    special = t.SpecialToken.Image;

                tokens = " -> " + special + t.Image;
            }

            ++indent;
            data = node.ChildrenAccept(this, data);
            --indent;
            return data;
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.SimpleNode, java.lang.Object)">
        /// </seealso>
        public override object Visit(SimpleNode node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTprocess, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTprocess node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTExpression, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTExpression node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTAssignment, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTAssignment node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTOrNode, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTOrNode node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTAndNode, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTAndNode node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTEQNode, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTEQNode node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTNENode, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTNENode node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTLTNode, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTLTNode node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTGTNode, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTGTNode node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTLENode, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTLENode node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTGENode, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTGENode node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTAddNode, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTAddNode node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTSubtractNode, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTSubtractNode node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTMulNode, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTMulNode node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTDivNode, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTDivNode node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTModNode, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTModNode node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTNotNode, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTNotNode node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTFloatingPointLiteral, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTFloatingPointLiteral node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTIntegerLiteral, java.lang.Object)">
        /// </seealso>
        /// <since> 1.5
        /// </since>
        public override object Visit(ASTIntegerLiteral node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTStringLiteral, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTStringLiteral node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTIdentifier, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTIdentifier node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTMethod, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTMethod node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTReference, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTReference node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTTrue, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTTrue node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTFalse, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTFalse node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTBlock, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTBlock node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTText, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTText node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTIfStatement, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTIfStatement node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTElseStatement, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTElseStatement node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTElseIfStatement, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTElseIfStatement node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTObjectArray, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTObjectArray node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTDirective, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTDirective node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTWord, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTWord node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTSetDirective, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTSetDirective node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTEscapedDirective, java.lang.Object)">
        /// </seealso>
        /// <since> 1.5
        /// </since>
        public override object Visit(ASTEscapedDirective node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTEscape, java.lang.Object)">
        /// </seealso>
        /// <since> 1.5
        /// </since>
        public override object Visit(ASTEscape node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTMap, java.lang.Object)">
        /// </seealso>
        /// <since> 1.5
        /// </since>
        public override object Visit(ASTMap node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTIntegerRange, java.lang.Object)">
        /// </seealso>
        public override object Visit(ASTIntegerRange node, object data)
        {
            return ShowNode(node, data);
        }

        /// <seealso cref="org.apache.velocity.runtime.visitor.BaseVisitor.visit(NVelocity.Runtime.Paser.Node.ASTStop, java.lang.Object)">
        /// </seealso>
        /// <since> 1.5
        /// </since>
        public override object Visit(ASTStop node, object data)
        {
            return ShowNode(node, data);
        }
    }
}