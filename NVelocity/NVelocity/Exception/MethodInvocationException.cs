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

    using Runtime.Log;

    /// <summary>  Application-level exception thrown when a reference method is
    /// invoked and an exception is thrown.
    /// <br>
    /// When this exception is thrown, a best effort will be made to have
    /// useful information in the exception's message.  For complete
    /// information, consult the runtime Log.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: MethodInvocationException.java 703544 2008-10-10 18:15:53Z nbubna $
    /// </version>
    [Serializable]
    public class MethodInvocationException : VelocityException, IExtendedParseException
    {
        /// <summary>  Returns the name of the method that threw the
        /// exception.
        /// 
        /// </summary>
        /// <returns> String name of method
        /// </returns>
        virtual public string MethodName
        {
            get
            {
                return methodName;
            }

        }
       
        /// <summary>  Retrieves the name of the reference that caused the
        /// exception.
        /// 
        /// </summary>
        /// <returns> name of reference.
        /// </returns>
        /// <summary>  Sets the reference name that threw this exception.
        /// 
        /// </summary>
        /// <param name="ref">name of reference
        /// </param>
        virtual public string ReferenceName
        {
            get
            {
                return referenceName;
            }

            set
            {
                referenceName = value;
            }

        }
        /// <seealso cref="ExtendedParseException.getColumnNumber()">
        /// </seealso>
        /// <since> 1.5
        /// </since>
        virtual public int ColumnNumber
        {
            get
            {
                return columnNumber;
            }

        }
        /// <seealso cref="ExtendedParseException.getLineNumber()">
        /// </seealso>
        /// <since> 1.5
        /// </since>
        virtual public int LineNumber
        {
            get
            {
                return lineNumber;
            }

        }
        /// <seealso cref="ExtendedParseException.getTemplateName()">
        /// </seealso>
        /// <since> 1.5
        /// </since>
        virtual public string TemplateName
        {
            get
            {
                return templateName;
            }

        }
        /// <seealso cref="Exception.getMessage()">
        /// </seealso>
        /// <since> 1.5
        /// </since>
        public override string Message
        {
            get
            {
                System.Text.StringBuilder message = new System.Text.StringBuilder();
               
                message.Append(base.Message);
                message.Append(" at ");
                message.Append(Log.FormatFileString(templateName, lineNumber, columnNumber));
                return message.ToString();
            }

        }

        private string referenceName = "";

       
        private string methodName;

       
        private int lineNumber;
       
        private int columnNumber;
       
        private string templateName;

        /// <summary>  CTOR - wraps the passed in exception for
        /// examination later
        /// 
        /// </summary>
        /// <param name="message">
        /// </param>
        /// <param name="e">Throwable that we are wrapping
        /// </param>
        /// <param name="methodName">name of method that threw the exception
        /// </param>
        /// <param name="templateName">The name of the template where the exception occured.
        /// </param>
        public MethodInvocationException(string message, System.Exception e, string methodName, string templateName, int lineNumber, int columnNumber)
            : base(message, e)
        {

            this.methodName = methodName;
            this.templateName = templateName;
            this.lineNumber = lineNumber;
            this.columnNumber = columnNumber;
        }
    }
}