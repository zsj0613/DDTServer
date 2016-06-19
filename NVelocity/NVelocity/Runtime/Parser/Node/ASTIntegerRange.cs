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
    using System.Collections;

    using Context;
    using Log;

    /// <summary> handles the range 'operator'  [ n .. m ]
    /// 
    /// Please look at the Parser.jjt file which is
    /// what controls the generation of this class.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    public class ASTIntegerRange : SimpleNode
    {
        /// <param name="id">
        /// </param>
        public ASTIntegerRange(int id)
            : base(id)
        {
        }

        /// <param name="p">
        /// </param>
        /// <param name="id">
        /// </param>
        public ASTIntegerRange(Parser p, int id)
            : base(p, id)
        {
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.Accept(NVelocity.Runtime.Paser.Node.IParserVisitor, System.Object)">
        /// </seealso>
        public override object Accept(IParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }

        /// <summary>  does the real work.  Creates an Vector of Integers with the
        /// right value range
        /// 
        /// </summary>
        /// <param name="context"> app context used if Left or Right of .. is a ref
        /// </param>
        /// <returns> Object array of Integers
        /// </returns>
        /// <throws>  MethodInvocationException </throws>
        public override object Value(IInternalContextAdapter context)
        {
            /*
            *  Get the two range ends
            */

            object left = GetChild(0).Value(context);
            object right = GetChild(1).Value(context);

            /*
            *  if either is null, lets Log and bail
            */

            if (left == null || right == null)
            {
                log.Error((left == null ? "Left" : "Right") + " side of range operator [n..m] has null value." + " Operation not possible. " + Log.FormatFileString(this));
                return null;
            }

            /*
            *  if not a Number, not much we can do either
            */

            if (!(left is System.ValueType) || !(right is System.ValueType))
            {
                log.Error((!(left is System.ValueType) ? "Left" : "Right") + " side of range operator is not a valid type. " + "Currently only integers (1,2,3...) and the Number type are supported. " + Log.FormatFileString(this));


                return null;
            }


            /*
            *  Get the two integer values of the ends of the range
            */

            int l = System.Convert.ToInt32(((System.ValueType)left));
            int r = System.Convert.ToInt32(((System.ValueType)right));

            /*
            *  find out how many there are
            */

            int nbrElements = System.Math.Abs(l - r);
            nbrElements += 1;

            /*
            *  Determine whether the increment is positive or negative.
            */

            int delta = (l >= r) ? -1 : 1;

            /*
            * Fill the range with the appropriate values.
            */

            System.Collections.IList elements = new System.Collections.ArrayList(nbrElements);
            int value = l;

            for (int i = 0; i < nbrElements; i++)
            {
                // TODO: JDK 1.4+ -> valueOf()
                elements.Add((System.Int32)value);
                value += delta;
            }

            return elements;
        }
    }
}