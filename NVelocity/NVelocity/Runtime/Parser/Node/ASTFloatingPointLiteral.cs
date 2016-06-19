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

    /// <summary> Handles floating point numbers.  The value will be either a Double
    /// or a BigDecimal.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <since> 1.5
    /// </since>
    public class ASTFloatingPointLiteral : SimpleNode
    {

        // This may be of type Double or BigDecimal
        private System.ValueType value = null;

        /// <param name="id">
        /// </param>
        public ASTFloatingPointLiteral(int id)
            : base(id)
        {
        }

        /// <param name="p">
        /// </param>
        /// <param name="id">
        /// </param>
        public ASTFloatingPointLiteral(Parser p, int id)
            : base(p, id)
        {
        }


        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.Accept(NVelocity.Runtime.Paser.Node.IParserVisitor, System.Object)">
        /// </seealso>
        public override object Accept(IParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }

        /// <summary>  Initialization method - doesn't do much but do the object
        /// creation.  We only need to do it once.
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <param name="data">
        /// </param>
        /// <returns> The data object.
        /// </returns>
        /// <throws>  TemplateInitException </throws>
        public override object Init(IInternalContextAdapter context, object data)
        {
            /*
            *  Init the tree correctly
            */

            base.Init(context, data);

            /**
            * Determine the size of the item and make it a Double or BigDecimal as appropriate.
            */
            string str = FirstToken.Image;
            try
            {
                value = double.Parse(str);
            }
            catch (System.FormatException)
            {

                // if there's still an Exception it will propogate out
                value = decimal.Parse(str, System.Globalization.NumberStyles.Any);
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
