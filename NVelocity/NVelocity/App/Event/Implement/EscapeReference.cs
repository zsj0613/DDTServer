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
    using System;

    using Runtime;
    using Util;

    /// <summary> Base class for escaping references.  To use it, override the following methods:
    /// <DL>
    /// <DT><code>String escape(String text)</code></DT>
    /// <DD>escape the provided text</DD>
    /// <DT><code>String getMatchAttribute()</code></DT>
    /// <DD>retrieve the configuration attribute used to match references (see below)</DD>
    /// </DL>
    /// 
    /// <P>By default, all references are escaped.  However, by setting the match attribute
    /// in the configuration file to a regular expression, users can specify which references
    /// to escape.  For example the following configuration property tells the EscapeSqlReference
    /// event handler to only escape references that start with "sql".
    /// (e.g. <code>$sql</code>, <code>$sql.toString(),</code>, etc).
    /// 
    /// <PRE>
    /// <CODE>eventhandler.escape.sql.match = /sql.*<!-- -->/
    /// </CODE>
    /// </PRE>
    /// <!-- note: ignore empty HTML comment above - breaks up star slash avoiding javadoc end -->
    /// 
    /// Regular expressions should follow the "Perl5" format used by the ORO regular expression
    /// library.  More Info is at
    /// <a href="http://jakarta.apache.org/oro/api/org/apache/oro/text/perl/package-summary.html">http://jakarta.apache.org/oro/api/org/apache/oro/text/perl/package-summary.html</a>.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain </a>
    /// </author>
    /// <version>  $Id: EscapeReference.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public abstract class EscapeReference : IReferenceInsertionEventHandler, IRuntimeServicesAware
    {
        /// <summary> Specify the configuration attribute that specifies the
        /// regular expression.  Ideally should be in a form
        /// <pre><code>eventhandler.escape.XYZ.match</code></pre>
        /// 
        /// <p>where <code>XYZ</code> is the type of escaping being done.
        /// </summary>
        /// <returns> configuration attribute
        /// </returns>
        protected internal abstract string MatchAttribute { get; }

        private IRuntimeServices rs;

        private string matchRegExp = null;

        /// <summary> Escape the given text.  Override this in a subclass to do the parameters
        /// escaping.
        /// 
        /// </summary>
        /// <param name="text">the text to escape
        /// </param>
        /// <returns> the escaped text
        /// </returns>
        protected internal abstract string Escape(object text);

        /// <summary> Escape the provided text if it matches the configured regular expression.
        /// 
        /// </summary>
        /// <param name="reference">
        /// </param>
        /// <param name="value">
        /// </param>
        /// <returns> Escaped text.
        /// </returns>
        public virtual object ReferenceInsert(string reference, object value)
        {
            if (value == null)
            {
                return value;
            }

            if (matchRegExp == null)
            {
                return Escape(value);
            }
            else if (System.Text.RegularExpressions.Regex.IsMatch(reference, matchRegExp))
            {
                return Escape(value);
            }
            else
            {
                return value;
            }
        }

        /// <summary> Called automatically when event cartridge is initialized.
        /// 
        /// </summary>
        /// <param name="rs">instance of RuntimeServices
        /// </param>
        public virtual void SetRuntimeServices(IRuntimeServices rs)
        {
            this.rs = rs;

            /**
            * Get the regular expression pattern.
            */
            matchRegExp = StringUtils.NullTrim(rs.Configuration.GetString(MatchAttribute));
            if ((matchRegExp != null) && (matchRegExp.Length == 0))
            {
                matchRegExp = null;
            }

            /**
            * Test the regular expression for a well formed pattern
            */
            if (matchRegExp != null)
            {
                try
                {
                    System.Text.RegularExpressions.Regex.Match("", matchRegExp);
                }
                catch (ArgumentException E)
                {
                    rs.Log.Error("Invalid regular expression '" + matchRegExp + "'.  No escaping will be performed.", E);
                    matchRegExp = null;
                }
            }
        }

        /// <summary> Retrieve a reference to RuntimeServices.  Use this for checking additional
        /// configuration properties.
        /// 
        /// </summary>
        /// <returns> The current runtime services object.
        /// </returns>
        protected internal virtual IRuntimeServices GetRuntimeServices()
        {
            return rs;
        }
    }
}
