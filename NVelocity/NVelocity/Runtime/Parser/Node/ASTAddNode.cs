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
    using Util;

    /// <summary> Handles number addition of nodes.<br><br>
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
    /// <version>  $Id: ASTAddNode.java 712887 2008-11-11 00:27:50Z nbubna $
    /// </version>
    public class ASTAddNode : ASTMathNode
    {
        /// <param name="id">
        /// </param>
        public ASTAddNode(int id)
            : base(id)
        {
        }

        /// <param name="p">
        /// </param>
        /// <param name="id">
        /// </param>
        public ASTAddNode(Parser p, int id)
            : base(p, id)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected internal override object HandleSpecial(object left, object right, IInternalContextAdapter context)
        {
            /*
            * shall we try for strings?
            */
            if (left is string || right is string)
            {
                if (left == null)
                {
                    left = GetChild(0).Literal;
                }
                else if (right == null)
                {
                    right = GetChild(1).Literal;
                }

                return string.Concat(left.ToString(), right.ToString());
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Object Perform(Object left, Object right, IInternalContextAdapter context)
        {
            Type maxType = MathUtils.ToMaxType(left.GetType(), right.GetType());

            if (maxType == null)
            {
                return null;
            }

            return MathUtils.Add(maxType, left, right);
        }
    }
}