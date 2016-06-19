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

namespace NVelocity.Runtime.Parser
{
    using System;

    using Exception;

    /// <summary> This is an extension of the ParseException, which also takes a
    /// template name.
    /// 
    /// </summary>
    /// <seealso cref="org.apache.velocity.runtime.parser.ParseException">
    /// 
    /// </seealso>
    /// <author>  <a href="hps@intermeta.de">Henning P. Schmiedehausen</a>
    /// </author>
    /// <version>  $Id: TemplateParseException.java 703544 2008-10-10 18:15:53Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    [Serializable]
    public class TemplateParseException : ParseException, IExtendedParseException
    {
        /// <summary> returns the Template name where this exception occured.</summary>
        /// <returns> The Template name where this exception occured.
        /// </returns>
        virtual public string TemplateName
        {
            get
            {
                return templateName;
            }

        }
        /// <summary> returns the line number where this exception occured.</summary>
        /// <returns> The line number where this exception occured.
        /// </returns>
        virtual public int LineNumber
        {
            get
            {
                if ((currentToken != null) && (currentToken.Next != null))
                {
                    return currentToken.Next.BeginLine;
                }
                else
                {
                    return -1;
                }
            }

        }
        /// <summary> returns the column number where this exception occured.</summary>
        /// <returns> The column number where this exception occured.
        /// </returns>
        virtual public int ColumnNumber
        {
            get
            {
                if ((currentToken != null) && (currentToken.Next != null))
                {
                    return currentToken.Next.BeginColumn;
                }
                else
                {
                    return -1;
                }
            }

        }
        /// <summary> This method has the standard behavior when this object has been
        /// created using the standard constructors.  Otherwise, it uses
        /// "currentToken" and "expectedTokenSequences" to generate a parse
        /// Error message and returns it.  If this object has been created
        /// due to a parse Error, and you do not catch it (it gets thrown
        /// from the parser), then this method is called during the printing
        /// of the final stack Trace, and hence the correct Error message
        /// gets displayed.
        /// </summary>
        /// <returns> The Error message.
        /// </returns>
        public override string Message
        {
            get
            {
                if (!specialConstructor)
                {
                    //UPGRADE_TODO: 在 .NET 中，方法“java.lang.Throwable.getMessage”的等效项可能返回不同的值。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(base.Message);
                    AppendTemplateInfo(sb);
                    return sb.ToString();
                }

                int maxSize = 0;

                System.Text.StringBuilder expected = new System.Text.StringBuilder();

                for (int i = 0; i < expectedTokenSequences.Length; i++)
                {
                    if (maxSize < expectedTokenSequences[i].Length)
                    {
                        maxSize = expectedTokenSequences[i].Length;
                    }

                    for (int j = 0; j < expectedTokenSequences[i].Length; j++)
                    {
                        expected.Append(tokenImage[expectedTokenSequences[i][j]]).Append(" ");
                    }

                    if (expectedTokenSequences[i][expectedTokenSequences[i].Length - 1] != 0)
                    {
                        expected.Append("...");
                    }

                    expected.Append(eol).Append("    ");
                }

                System.Text.StringBuilder retval = new System.Text.StringBuilder("Encountered \"");
                Token tok = currentToken.Next;

                for (int i = 0; i < maxSize; i++)
                {
                    if (i != 0)
                    {
                        retval.Append(" ");
                    }

                    if (tok.Kind == 0)
                    {
                        retval.Append(tokenImage[0]);
                        break;
                    }

                    retval.Append(AddEscapes(tok.Image));
                    tok = tok.Next;
                }

                retval.Append("\" at ");
                AppendTemplateInfo(retval);

                if (expectedTokenSequences.Length == 1)
                {
                    retval.Append("Was expecting:").Append(eol).Append("    ");
                }
                else
                {
                    retval.Append("Was expecting one of:").Append(eol).Append("    ");
                }

                // avoid JDK 1.3 StringBuffer.append(Object instance) vs 1.4 StringBuffer.append(StringBuffer sb) gotcha.
                retval.Append(expected.ToString());
                return retval.ToString();
            }

        }

        /// <summary> This is the name of the template which contains the parsing Error, or
        /// null if not defined.
        /// </summary>
        //UPGRADE_NOTE: Final 已从“templateName ”的声明中移除。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        private string templateName;

        /// <summary> This constructor is used to Add a template name
        /// to Info cribbed from a ParseException generated in the parser.
        /// </summary>
        /// <param name="currentTokenVal">
        /// </param>
        /// <param name="expectedTokenSequencesVal">
        /// </param>
        /// <param name="tokenImageVal">
        /// </param>
        /// <param name="templateNameVal">
        /// </param>
        public TemplateParseException(Token currentTokenVal, int[][] expectedTokenSequencesVal, string[] tokenImageVal, string templateNameVal)
            : base(currentTokenVal, expectedTokenSequencesVal, tokenImageVal)
        {
            this.templateName = templateNameVal;
        }

        /// <summary> This constructor is used by the method "generateParseException"
        /// in the generated parser.  Calling this constructor generates
        /// a new object of this type with the fields "currentToken",
        /// "expectedTokenSequences", and "tokenImage" set.  The boolean
        /// flag "specialConstructor" is also set to true to indicate that
        /// this constructor was used to create this object.
        /// This constructor calls its super class with the empty string
        /// to force the "toString" method of parent class "Throwable" to
        /// print the Error message in the form:
        /// ParseException: <result of getMessage>
        /// </summary>
        /// <param name="currentTokenVal">
        /// </param>
        /// <param name="expectedTokenSequencesVal">
        /// </param>
        /// <param name="tokenImageVal">
        /// </param>
        public TemplateParseException(Token currentTokenVal, int[][] expectedTokenSequencesVal, string[] tokenImageVal)
            : base(currentTokenVal, expectedTokenSequencesVal, tokenImageVal)
        {
            templateName = "*unset*";
        }

        /// <summary> The following constructors are for use by you for whatever
        /// purpose you can think of.  Constructing the exception in this
        /// manner makes the exception behave in the normal way - i.e., as
        /// documented in the class "Throwable".  The fields "errorToken",
        /// "expectedTokenSequences", and "tokenImage" do not contain
        /// relevant information.  The JavaCC generated code does not use
        /// these constructors.
        /// </summary>
        public TemplateParseException()
            : base()
        {
            templateName = "*unset*";
        }

        /// <summary> Creates a new TemplateParseException object.
        /// 
        /// </summary>
        /// <param name="message">TODO: DOCUMENT ME!
        /// </param>
        public TemplateParseException(string message)
            : base(message)
        {
            templateName = "*unset*";
        }

        /// <param name="sb">
        /// </param>
        protected internal virtual void AppendTemplateInfo(System.Text.StringBuilder sb)
        {
            //sb.Append(LogMessage.formatFileString(TemplateName, LineNumber, ColumnNumber));
            sb.Append(eol);
        }
    }
}
