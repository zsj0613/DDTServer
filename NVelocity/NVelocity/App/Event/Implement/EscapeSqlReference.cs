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

namespace NVelocity.App.Event.Implement
{
    /// <summary> Escapes the characters in a String to be suitable to pass to an SQL query.</summary>
    /// <seealso cref="<a href="http://jakarta.apache.org/commons/lang/api/org/apache/commons/lang/StringEscapeUtils.html.EscapeSql(java.lang.String)">StringEscapeUtils</a>">
    /// </seealso>
    /// <author>  wglass
    /// </author>
    /// <since> 1.5
    /// </since>
    public class EscapeSqlReference : EscapeReference
    {
        /// <returns> attribute "eventhandler.escape.sql.match"
        /// </returns>
        override protected internal string MatchAttribute
        {
            get
            {
                return "eventhandler.escape.sql.match";
            }

        }

        /// <summary> Escapes the characters in a String to be suitable to pass to an SQL query.
        /// 
        /// </summary>
        /// <param name="text">
        /// </param>
        /// <returns> An escaped string.
        /// </returns>
        /// <seealso cref="<a href="http://jakarta.apache.org/commons/lang/api/org/apache/commons/lang/StringEscapeUtils.html.EscapeSql(java.lang.String)">StringEscapeUtils</a>">
        /// </seealso>
        protected internal override string Escape(object text)
        {
            return SupportClass.StringEscapeUtils.EscapeSql(text.ToString());
        }
    }
}
