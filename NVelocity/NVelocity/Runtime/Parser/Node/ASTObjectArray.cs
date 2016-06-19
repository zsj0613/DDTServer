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

    /// <summary> </summary>
    public class ASTObjectArray : SimpleNode
    {
        /// <param name="id">
        /// </param>
        public ASTObjectArray(int id)
            : base(id)
        {
        }

        /// <param name="p">
        /// </param>
        /// <param name="id">
        /// </param>
        public ASTObjectArray(Parser p, int id)
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

            // since we know the amount of elements, Initialize arraylist with proper size
            System.Collections.IList objectArray = new System.Collections.ArrayList(size);

            for (int i = 0; i < size; i++)
            {
                objectArray.Add(GetChild(i).Value(context));
            }

            return objectArray;
        }
    }
}