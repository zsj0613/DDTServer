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

namespace NVelocity.Exception
{
    using System;

    using Runtime.Parser;

    /// <summary> Exception generated to indicate parse errors caught during
    /// directive initialization (e.g. wrong number of arguments)
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <version>  $Id: TemplateInitException.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    [Serializable]
    public class TemplateInitException : VelocityException, IExtendedParseException
    {
        /// <summary> Returns the Template name where this exception occured.</summary>
        /// <returns> the template name
        /// </returns>
        virtual public string TemplateName
        {
            get
            {
                return templateName;
            }

        }
        /// <summary> Returns the line number where this exception occured.</summary>
        /// <returns> the line number
        /// </returns>
        virtual public int LineNumber
        {
            get
            {
                return line;
            }

        }
        /// <summary> Returns the column number where this exception occured.</summary>
        /// <returns> the line number
        /// </returns>
        virtual public int ColumnNumber
        {
            get
            {
                return col;
            }

        }
        //UPGRADE_NOTE: Final 已从“templateName ”的声明中移除。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        private string templateName;
        //UPGRADE_NOTE: Final 已从“col ”的声明中移除。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        private int col;
        //UPGRADE_NOTE: Final 已从“line ”的声明中移除。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        private int line;


        public TemplateInitException(string msg, string templateName, int col, int line)
            : base(msg)
        {
            this.templateName = templateName;
            this.col = col;
            this.line = line;
        }

        public TemplateInitException(string msg, ParseException parseException, string templateName, int col, int line)
            : base(msg, parseException)
        {
            this.templateName = templateName;
            this.col = col;
            this.line = line;
        }
    }
}
