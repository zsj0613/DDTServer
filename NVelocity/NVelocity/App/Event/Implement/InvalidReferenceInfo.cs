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
    using Util.Introspection;

    /// <summary> Convenience class to use when reporting out invalid syntax 
    /// with line, column, and template name.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain </a>
    /// </author>
    /// <version>  $Id: InvalidReferenceInfo.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class InvalidReferenceInfo : Info
    {
        /// <summary> Get the specific invalid reference string.</summary>
        /// <returns> the invalid reference string
        /// </returns>
        virtual public string InvalidReference
        {
            get
            {
                return invalidReference;
            }

        }
        private string invalidReference;

        public InvalidReferenceInfo(string invalidReference, Info info)
            : base(info.TemplateName, info.Line, info.Column)
        {
            this.invalidReference = invalidReference;
        }



        /// <summary> Formats a textual representation of this object as <code>SOURCE
        /// [line X, column Y]: invalidReference</code>.
        /// 
        /// </summary>
        /// <returns> String representing this object.
        /// </returns>
        public override string ToString()
        {
            return TemplateName + " [line " + Line + ", column " + Column + "]: " + invalidReference;
        }
    }
}
