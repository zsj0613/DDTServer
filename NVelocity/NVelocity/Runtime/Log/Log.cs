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

namespace NVelocity.Runtime.Log
{
    using Directive;
    using Parser.Node;
    using Util.Introspection;

    /// <summary> Convenient wrapper for LogChute functions. This implements
    /// the RuntimeLogger methods (and then some).  It is hoped that
    /// use of this will fully replace use of the RuntimeLogger.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:nbubna@apache.org">Nathan Bubna</a>
    /// </author>
    /// <version>  $Id: LogMessage.java 724825 2008-12-09 18:56:06Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class Log
    {
        /// <summary> Returns true if Trace level messages will be printed by the LogChute.</summary>
        /// <returns> If Trace level messages will be printed by the LogChute.
        /// </returns>
        virtual public bool TraceEnabled
        {
            get
            {
                return GetLogChute().IsLevelEnabled(LogChute_Fields.TRACE_ID);
            }

        }
        /// <summary> Returns true if Debug level messages will be printed by the LogChute.</summary>
        /// <returns> True if Debug level messages will be printed by the LogChute.
        /// </returns>
        virtual public bool DebugEnabled
        {
            get
            {
                return GetLogChute().IsLevelEnabled(LogChute_Fields.DEBUG_ID);
            }

        }
        /// <summary> Returns true if Info level messages will be printed by the LogChute.</summary>
        /// <returns> True if Info level messages will be printed by the LogChute.
        /// </returns>
        virtual public bool InfoEnabled
        {
            get
            {
                return GetLogChute().IsLevelEnabled(LogChute_Fields.INFO_ID);
            }

        }
        /// <summary> Returns true if Warn level messages will be printed by the LogChute.</summary>
        /// <returns> True if Warn level messages will be printed by the LogChute.
        /// </returns>
        virtual public bool WarnEnabled
        {
            get
            {
                return GetLogChute().IsLevelEnabled(LogChute_Fields.WARN_ID);
            }

        }
        /// <summary> Returns true if Error level messages will be printed by the LogChute.</summary>
        /// <returns> True if Error level messages will be printed by the LogChute.
        /// </returns>
        virtual public bool ErrorEnabled
        {
            get
            {
                return GetLogChute().IsLevelEnabled(LogChute_Fields.ERROR_ID);
            }

        }

        private ILogChute chute;

        /// <summary> Creates a new LogMessage that wraps a HoldingLogChute.</summary>
        public Log()
        {
            SetLogChute(new HoldingLogChute());
        }

        /// <summary> Creates a new LogMessage that wraps the specified LogChute.</summary>
        /// <param name="chute">
        /// </param>
        public Log(ILogChute chute)
        {
            SetLogChute(chute);
        }

        /// <summary> Updates the LogChute wrapped by this LogMessage instance.</summary>
        /// <param name="chute">The new value for the Log chute.
        /// </param>
        protected internal virtual void SetLogChute(ILogChute chute)
        {
            if (chute == null)
            {
                throw new System.NullReferenceException("The LogChute cannot be set to null!");
            }
            this.chute = chute;
        }

        /// <summary> Returns the LogChute wrapped by this LogMessage instance.</summary>
        /// <returns> The LogChute wrapped by this LogMessage instance.
        /// </returns>
        protected internal virtual ILogChute GetLogChute()
        {
            return this.chute;
        }

        protected internal virtual void LogMessage(int level, System.Object message)
        {
            GetLogChute().Log(level, System.Convert.ToString(message));
        }

        protected internal virtual void LogMessage(int level, System.Object message, System.Exception t)
        {
            GetLogChute().Log(level, System.Convert.ToString(message), t);
        }

        /// <summary> LogMessage a Trace message.</summary>
        /// <param name="message">
        /// </param>
        public virtual void Trace(System.Object message)
        {
            LogMessage(LogChute_Fields.TRACE_ID, message);
        }

        /// <summary> LogMessage a Trace message and accompanying Throwable.</summary>
        /// <param name="message">
        /// </param>
        /// <param name="t">
        /// </param>
        public virtual void Trace(System.Object message, System.Exception t)
        {
            LogMessage(LogChute_Fields.TRACE_ID, message, t);
        }

        /// <summary> LogMessage a Debug message.</summary>
        /// <param name="message">
        /// </param>
        public virtual void Debug(System.Object message)
        {
            LogMessage(LogChute_Fields.DEBUG_ID, message);
        }

        /// <summary> LogMessage a Debug message and accompanying Throwable.</summary>
        /// <param name="message">
        /// </param>
        /// <param name="t">
        /// </param>
        public virtual void Debug(System.Object message, System.Exception t)
        {
            LogMessage(LogChute_Fields.DEBUG_ID, message, t);
        }

        /// <summary> LogMessage an Info message.</summary>
        /// <param name="message">
        /// </param>
        public virtual void Info(System.Object message)
        {
            LogMessage(LogChute_Fields.INFO_ID, message);
        }

        /// <summary> LogMessage an Info message and accompanying Throwable.</summary>
        /// <param name="message">
        /// </param>
        /// <param name="t">
        /// </param>
        public virtual void Info(System.Object message, System.Exception t)
        {
            LogMessage(LogChute_Fields.INFO_ID, message, t);
        }

        /// <summary> LogMessage a warning message.</summary>
        /// <param name="message">
        /// </param>
        public virtual void Warn(System.Object message)
        {
            LogMessage(LogChute_Fields.WARN_ID, message);
        }

        /// <summary> LogMessage a warning message and accompanying Throwable.</summary>
        /// <param name="message">
        /// </param>
        /// <param name="t">
        /// </param>
        public virtual void Warn(System.Object message, System.Exception t)
        {
            LogMessage(LogChute_Fields.WARN_ID, message, t);
        }

        /// <summary> LogMessage an Error message.</summary>
        /// <param name="message">
        /// </param>
        public virtual void Error(System.Object message)
        {
            LogMessage(LogChute_Fields.ERROR_ID, message);
        }

        /// <summary> LogMessage an Error message and accompanying Throwable.</summary>
        /// <param name="message">
        /// </param>
        /// <param name="t">
        /// </param>
        public virtual void Error(System.Object message, System.Exception t)
        {
            LogMessage(LogChute_Fields.ERROR_ID, message, t);
        }

        /// <summary> Creates a string that formats the template filename with line number
        /// and column of the given Directive. We use this routine to provide a cosistent format for displaying 
        /// file errors.
        /// </summary>
        public static System.String FormatFileString(Directive directive)
        {
            return FormatFileString(directive.TemplateName, directive.Line, directive.Column);
        }

        /// <summary> Creates a string that formats the template filename with line number
        /// and column of the given Node. We use this routine to provide a cosistent format for displaying 
        /// file errors.
        /// </summary>
        public static System.String FormatFileString(INode node)
        {
            return FormatFileString(node.TemplateName, node.Line, node.Column);
        }

        /// <summary> Simply creates a string that formats the template filename with line number
        /// and column. We use this routine to provide a cosistent format for displaying 
        /// file errors.
        /// </summary>
        public static System.String FormatFileString(Info info)
        {
            return FormatFileString(info.TemplateName, info.Line, info.Column);
        }

        /// <summary> Simply creates a string that formats the template filename with line number
        /// and column. We use this routine to provide a cosistent format for displaying 
        /// file errors.
        /// </summary>
        /// <param name="template">File name of template, can be null
        /// </param>
        /// <param name="linenum">Line number within the file
        /// </param>
        /// <param name="colnum">Column number withing the file at linenum
        /// </param>
        public static System.String FormatFileString(System.String template, int linenum, int colnum)
        {
            if (template == null || template.Equals(""))
            {
                template = "<unknown template>";
            }
            return template + "[line " + linenum + ", column " + colnum + "]";
        }
    }
}
