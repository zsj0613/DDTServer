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

    /// <summary> AST Node for creating a map / dictionary.
    /// 
    /// This class was originally generated from Parset.jjt.
    /// 
    /// </summary>
    /// <version>  $Id: ASTMap.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class ASTMap : SimpleNode
    {
        /// <param name="id">
        /// </param>
        public ASTMap(int id)
            : base(id)
        {
        }

        /// <param name="p">
        /// </param>
        /// <param name="id">
        /// </param>
        public ASTMap(Parser p, int id)
            : base(p, id)
        {
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.Accept(NVelocity.Runtime.Paser.Node.IParserVisitor, System.Object)">
        /// </seealso>
        public override object Accept(IParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.Value(NVelocity.Context.IInternalContextAdapter)">
        /// </seealso>
        public override object Value(IInternalContextAdapter context)
        {
            int size = GetNumChildren();

            System.Collections.IDictionary objectMap = new System.Collections.Hashtable();

            for (int i = 0; i < size; i += 2)
            {
                SimpleNode keyNode = (SimpleNode)GetChild(i);
                SimpleNode valueNode = (SimpleNode)GetChild(i + 1);

                object key = (keyNode == null ? null : keyNode.Value(context));
                object value = (valueNode == null ? null : valueNode.Value(context));

                objectMap[key] = value;
            }

            return objectMap;
        }
    }
}