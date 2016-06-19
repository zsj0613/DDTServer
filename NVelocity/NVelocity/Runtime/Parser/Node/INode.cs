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

    /// <summary>  This file describes the interface between the Velocity code
    /// and the JavaCC generated code.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:hps@intermeta.de">Henning P. Schmiedehausen</a>
    /// </author>
    /// <version>  $Id: Node.java 720228 2008-11-24 16:58:33Z nbubna $
    /// </version>

    public interface INode
    {
        /// <returns> The last token.
        /// </returns>
        Token LastToken
        {
            get;

        }
        /// <returns> The NodeType.
        /// </returns>
        int Type
        {
            get;

        }

        /// <returns> The current node Info.
        /// </returns>
        /// <param name="Info">
        /// </param>
        int Info
        {
            get;

            set;

        }
        /// <returns> The current line position.
        /// </returns>
        int Line
        {
            get;

        }
        /// <returns> The current column position.
        /// </returns>
        int Column
        {
            get;

        }

        /// <returns> the file name of the template
        /// </returns>
        string TemplateName
        {
            get;

        }

        /// <summary>This method is called after the node has been made the current
        /// node.  It indicates that child nodes can now be added to it. 
        /// </summary>
        void Open();

        /// <summary>This method is called after all the child nodes have been
        /// added.
        /// </summary>
        void Close();

        /// <summary> This pair of methods are used to inform the node of its
        /// parent.
        /// </summary>
        /// <param name="n">*
        /// </param>
        /// <returns> The node parent.
        /// </returns>
        INode Parent { get; set; }

        /// <summary> This method tells the node to Add its argument to the node's
        /// list of children.
        /// </summary>
        /// <param name="n">
        /// </param>
        /// <param name="i">
        /// </param>
        void AddChild(INode n, int i);

        /// <summary> This method returns a child node.  The children are numbered
        /// from zero, left to right.
        /// </summary>
        /// <param name="i">
        /// </param>
        /// <returns> A child node.
        /// </returns>
        INode GetChild(int i);

        /// <summary> Return the number of children the node has.</summary>
        /// <returns> The number of children of this node.
        /// </returns>
        int GetNumChildren();

        /// <param name="visitor">
        /// </param>
        /// <param name="data">
        /// </param>
        /// <returns> The Node execution result object.
        /// </returns>
        object Accept(IParserVisitor visitor, object data);

        /*
        * ========================================================================
        *
        * The following methods are not generated automatically be the Parser but
        * added manually to be used by Velocity.
        *
        * ========================================================================
        */

        /// <seealso cref="jjtAccept(ParserVisitor, Object)">
        /// </seealso>
        /// <param name="visitor">
        /// </param>
        /// <param name="data">
        /// </param>
        /// <returns> The node execution result.
        /// </returns>
        object ChildrenAccept(IParserVisitor visitor, object data);

        /// <returns> The first token.
        /// </returns>
        Token FirstToken { get; }

        /// <param name="context">
        /// </param>
        /// <param name="data">
        /// </param>
        /// <returns> The Init result.
        /// </returns>
        /// <throws>  TemplateInitException </throws>
        object Init(IInternalContextAdapter context, object data);

        /// <param name="context">
        /// </param>
        /// <returns> The evaluation result.
        /// </returns>
        /// <throws>  MethodInvocationException </throws>
        bool Evaluate(IInternalContextAdapter context);

        /// <param name="context">
        /// </param>
        /// <returns> The node value.
        /// </returns>
        /// <throws>  MethodInvocationException </throws>
        object Value(IInternalContextAdapter context);

        /// <param name="context">
        /// </param>
        /// <param name="writer">
        /// </param>
        /// <returns> True if the node rendered successfully.
        /// </returns>
        /// <throws>  IOException </throws>
        /// <throws>  MethodInvocationException </throws>
        /// <throws>  ParseErrorException </throws>
        /// <throws>  ResourceNotFoundException </throws>
        //UPGRADE_ISSUE: “java.io.Writer”和“System.IO.StreamWriter”之间的类层次结构差异可能导致编译错误。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1186'"
        bool Render(IInternalContextAdapter context, System.IO.TextWriter writer);

        /// <param name="instance">
        /// </param>
        /// <param name="context">
        /// </param>
        /// <returns> The execution result.
        /// </returns>
        /// <throws>  MethodInvocationException </throws>
        object Execute(object o, IInternalContextAdapter context);

        /// <returns> A literal.
        /// </returns>
        string Literal { get; }

        /// <returns>
        /// Mark the node as invalid.
        /// True if the node is invalid.
        /// </returns>
        bool IsInvalid { get; set; }
    }
}