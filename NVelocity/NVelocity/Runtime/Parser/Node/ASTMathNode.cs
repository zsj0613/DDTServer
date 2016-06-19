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
    using Exception;
    using Util;

    /// <summary> Helps handle math<br><br>
    /// 
    /// Please look at the Parser.jjt file which is
    /// what controls the generation of this class.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <author>  <a href="mailto:pero@antaramusic.de">Peter Romianowski</a>
    /// </author>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  Nathan Bubna
    /// </author>
    /// <version>  $Id: ASTMathNode.java 517553 2007-03-13 06:09:58Z wglass $
    /// </version>
    public abstract class ASTMathNode : SimpleNode
    {
        protected internal bool strictMode = false;

        public ASTMathNode(int id)
            : base(id)
        {
        }

        public ASTMathNode(Parser p, int id)
            : base(p, id)
        {
        }

        /// <summary> {@inheritDoc}</summary>
        public override object Init(IInternalContextAdapter context, object data)
        {
            base.Init(context, data);
            strictMode = rsvc.GetBoolean(RuntimeConstants.STRICT_MATH, false);
            return data;
        }

        /// <summary> {@inheritDoc}</summary>
        public override object Accept(IParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }

        /// <summary> gets the two args and performs the operation on them
        /// 
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <returns> result or null
        /// </returns>
        /// <throws>  MethodInvocationException </throws>
        public override Object Value(IInternalContextAdapter context)
        {
            object left = GetChild(0).Value(context);
            object right = GetChild(1).Value(context);

            /*
            * should we do anything special here?
            */
            object special = HandleSpecial(left, right, context);
            if (special != null)
            {
                return special;
            }

            /*
            * convert to Number if applicable
            */
            if (left is ITemplateNumber)
            {
                left = ((ITemplateNumber)left).AsNumber;
            }
            if (right is ITemplateNumber)
            {
                right = ((ITemplateNumber)right).AsNumber;
            }

            /*
            * if not a Number, not much we can do
            */
            if (!(left is System.ValueType) || !(right is System.ValueType))
            {
                bool wrongright = (left is System.ValueType);
                bool wrongtype = wrongright ? right != null : left != null;
                string msg = (wrongright ? "Right" : "Left") + " side of math operation (" + GetChild(wrongright ? 1 : 0).Literal + ") " + (wrongtype ? "is not a Number. " : "has a null value. ") + GetLocation(context);
                if (strictMode)
                {
                    log.Error(msg);
                    throw new MathException(msg);
                }
                else
                {
                    log.Debug(msg);
                    return null;
                }
            }

            return Perform(left, right, context);
        }

        /// <summary> Extension hook to allow special behavior by subclasses
        /// If this method returns a non-null value, that is returned,
        /// rather than the result of the math operation.
        /// </summary>
        /// <seealso cref="ASTAddNode.handleSpecial">
        /// </seealso>
        protected internal virtual object HandleSpecial(object left, object right, IInternalContextAdapter context)
        {
            // do nothing, this is an extension hook
            return null;
        }

        /// <summary> Performs the math operation represented by this node.</summary>
        public abstract Object Perform(Object left, Object right, IInternalContextAdapter context);
    }
}
