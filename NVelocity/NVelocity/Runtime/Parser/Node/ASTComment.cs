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

    using Context;

    /// <summary>  Represents all comments...
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@apache.org">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: ASTComment.java 517553 2007-03-13 06:09:58Z wglass $
    /// </version>
    public class ASTComment : SimpleNode
    {
        private static readonly char[] ZILCH = "".ToCharArray();

        private char[] carr;

        /// <param name="id">
        /// </param>
        public ASTComment(int id)
            : base(id)
        {
        }

        /// <param name="p">
        /// </param>
        /// <param name="id">
        /// </param>
        public ASTComment(Parser p, int id)
            : base(p, id)
        {
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.Accept(NVelocity.Runtime.Paser.Node.IParserVisitor, System.Object)">
        /// </seealso>
        public override object Accept(IParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }

        /// <summary>  We need to make sure we catch any of the dreaded MORE tokens.</summary>
        /// <param name="context">
        /// </param>
        /// <param name="data">
        /// </param>
        /// <returns> The data object.
        /// </returns>
        public override object Init(IInternalContextAdapter context, object data)
        {
            Token t = FirstToken;

            int loc1 = t.Image.IndexOf("##");
            int loc2 = t.Image.IndexOf("#*");

            if (loc1 == -1 && loc2 == -1)
            {
                carr = ZILCH;
            }
            else
            {
                carr = t.Image.Substring(0, ((loc1 == -1) ? loc2 : loc1) - (0)).ToCharArray();
            }

            return data;
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.render(org.apache.velocity.context.InternalContextAdapter, java.io.Writer)">
        /// </seealso>
        public override bool Render(IInternalContextAdapter context, TextWriter writer)
        {
            if (context.AllowRendering)
            {
                writer.Write(carr);
            }

            return true;
        }
    }
}