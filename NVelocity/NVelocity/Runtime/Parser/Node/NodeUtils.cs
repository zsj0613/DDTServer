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
    using System.Text;

    using Context;

    /// <summary> Utilities for dealing with the AST node structure.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: NodeUtils.java 687386 2008-08-20 16:57:07Z nbubna $
    /// </version>
    public static class NodeUtils
    {
        /// <summary> Collect all the <SPECIAL_TOKEN>s that
        /// are carried along with a token. Special
        /// tokens do not participate in parsing but
        /// can still trigger certain lexical actions.
        /// In some cases you may want to retrieve these
        /// special tokens, this is simply a way to
        /// extract them.
        /// </summary>
        /// <param name="t">the Token
        /// </param>
        /// <returns> StrBuilder with the special tokens.
        /// </returns>
        public static StringBuilder GetSpecialText(Token t)
        {
            StringBuilder sb = new StringBuilder();

            Token tmp_t = t.SpecialToken;

            while (tmp_t.SpecialToken != null)
            {
                tmp_t = tmp_t.SpecialToken;
            }

            while (tmp_t != null)
            {
                string st = tmp_t.Image;

                for (int i = 0, is_Renamed = st.Length; i < is_Renamed; i++)
                {
                    char c = st[i];

                    if (c == '#' || c == '$')
                    {
                        sb.Append(c);
                    }

                    /*
                    *  more dreaded MORE hack :)
                    *
                    *  looking for ("\\")*"$" sequences
                    */

                    if (c == '\\')
                    {
                        bool ok = true;
                        bool term = false;

                        int j = i;
                        for (ok = true; ok && j < is_Renamed; j++)
                        {
                            char cc = st[j];

                            if (cc == '\\')
                            {
                                /*
                                *  if we see a \, keep going
                                */
                                continue;
                            }
                            else if (cc == '$')
                            {
                                /*
                                *  a $ ends it correctly
                                */
                                term = true;
                                ok = false;
                            }
                            else
                            {
                                /*
                                *  nah...
                                */
                                ok = false;
                            }
                        }

                        if (term)
                        {
                            string foo = st.Substring(i, (j) - (i));
                            sb.Append(foo);
                            i = j;
                        }
                    }
                }

                tmp_t = tmp_t.Next;
            }
            return sb;
        }

        /// <summary>  complete node literal</summary>
        /// <param name="t">
        /// </param>
        /// <returns> A node literal.
        /// </returns>
        public static string TokenLiteral(Token t)
        {
            // Look at kind of token and return "" when it's a multiline comment
            if (t.Kind == NVelocity.Runtime.Parser.ParserConstants.MULTI_LINE_COMMENT)
            {
                return "";
            }
            else if (t.SpecialToken == null || t.SpecialToken.Image.StartsWith("##"))
            {
                return t.Image;
            }
            else
            {
                System.Text.StringBuilder special = GetSpecialText(t);
                if (special.Length > 0)
                {
                    return special.Append(t.Image).ToString();
                }
                return t.Image;
            }
        }
    }
}