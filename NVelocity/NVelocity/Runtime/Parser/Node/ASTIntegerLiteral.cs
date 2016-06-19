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

    using Context;

    /// <summary> Handles integer numbers.  The value will be either an Integer, a Long, or a BigInteger.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <since> 1.5
    /// </since>
    public class ASTIntegerLiteral : SimpleNode
    {

        // This may be of type Integer, Long or BigInteger
        private System.ValueType value = null;

        /// <param name="id">
        /// </param>
        public ASTIntegerLiteral(int id)
            : base(id)
        {
        }

        /// <param name="p">
        /// </param>
        /// <param name="id">
        /// </param>
        public ASTIntegerLiteral(Parser p, int id)
            : base(p, id)
        {
        }


        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.Accept(NVelocity.Runtime.Paser.Node.IParserVisitor, System.Object)">
        /// </seealso>
        public override object Accept(IParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.Init(org.apache.velocity.context.InternalContextAdapter, java.lang.Object)">
        /// </seealso>
        public override object Init(IInternalContextAdapter context, object data)
        {
            /*
            *  Init the tree correctly
            */

            base.Init(context, data);

            /**
            * Determine the size of the item and make it an Integer, Long, or BigInteger as appropriate.
            */
            string str = FirstToken.Image;
            try
            {
                value = System.Int32.Parse(str);
            }
            catch (FormatException)
            {
                try
                {
                    value = System.Int64.Parse(str);
                }
                catch (FormatException)
                {

                    value = System.Decimal.Parse(str, System.Globalization.NumberStyles.Any);
                }
            }

            return data;
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.Value(NVelocity.Context.IInternalContextAdapter)">
        /// </seealso>
        public override object Value(IInternalContextAdapter context)
        {
            return value;
        }
    }
}
