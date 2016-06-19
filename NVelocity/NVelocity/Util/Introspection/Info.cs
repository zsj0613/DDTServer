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

namespace NVelocity.Util.Introspection
{
    using Runtime.Log;

    /// <summary>  Little class to carry in Info such as template name, line and column
    /// for information Error reporting from the uberspector implementations
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: Info.java 704179 2008-10-13 17:42:11Z nbubna $
    /// </version>
    public class Info
    {
        /// <returns> The template name.
        /// </returns>
        public string TemplateName
        {
            get
            {
                return templateName;
            }

        }
        /// <returns> The line number.
        /// </returns>
        public int Line
        {
            get
            {
                return line;
            }

        }
        /// <returns> The column number.
        /// </returns>
        public int Column
        {
            get
            {
                return column;
            }

        }
        private int line;
        private int column;
        private string templateName;

        /// <param name="source">Usually a template name.
        /// </param>
        /// <param name="line">The line number from <code>source</code>.
        /// </param>
        /// <param name="column">The column number from <code>source</code>.
        /// </param>
        public Info(string source, int line, int column)
        {
            this.templateName = source;
            this.line = line;
            this.column = column;
        }

        /// <summary> Force callers to set the location information.</summary>
        private Info()
        {
        }

        /// <summary> Formats a textual representation of this object as <code>SOURCE
        /// [line X, column Y]</code>.
        /// 
        /// </summary>
        /// <returns> String representing this object.
        /// </returns>
        /// <since> 1.5
        /// </since>
        public override string ToString()
        {
            return Log.FormatFileString(TemplateName, Line, Column);
        }
    }
}