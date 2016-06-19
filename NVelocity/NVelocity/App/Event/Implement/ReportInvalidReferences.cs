using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NVelocity.Util;
using NVelocity.Util.Introspection;
using NVelocity.Runtime;
using NVelocity.Exception;
using NVelocity.Context;

namespace NVelocity.App.Event.Implement
{
    /// <summary> Use this event handler to flag invalid references.  Since this 
    /// is intended to be used for a specific request, this should be
    /// used as a local event handler attached to a specific context
    /// instead of being globally defined in the Velocity properties file.
    /// 
    /// <p>
    /// Note that InvalidReferenceHandler can be used
    /// in two modes.  If the Velocity properties file contains the following:
    /// <pre>
    /// eventhandler.invalidreference.exception = true
    /// </pre>
    /// then the event handler will throw a ParseErrorRuntimeException upon 
    /// hitting the first invalid reference.  This stops processing and is 
    /// passed through to the application code.  The ParseErrorRuntimeException
    /// contain information about the template name, line number, column number,
    /// and invalid reference.
    /// 
    /// <p>
    /// If this configuration setting is false or omitted then the page 
    /// will be processed as normal, but all invalid references will be collected
    /// in a List of InvalidReferenceInfo objects.
    /// 
    /// <p>This feature should be regarded as experimental.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <version>  $Id: ReportInvalidReferences.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class ReportInvalidReferences : IInvalidReferenceEventHandler, IRuntimeServicesAware
    {
        /// <summary> All invalid references during the processing of this page.</summary>
        /// <returns> a List of InvalidReferenceInfo objects
        /// </returns>
        virtual public System.Collections.IList InvalidReferences
        {
            get
            {
                return invalidReferences;
            }

        }

        public const string EVENTHANDLER_INVALIDREFERENCE_EXCEPTION = "eventhandler.invalidreference.exception";

        /// <summary> List of InvalidReferenceInfo objects</summary>
        internal System.Collections.IList invalidReferences = new System.Collections.ArrayList();

        /// <summary> If true, stop at the first invalid reference and throw an exception.</summary>
        private bool stopOnFirstInvalidReference = false;


        /// <summary> Collect the Error and/or throw an exception, depending on configuration.
        /// 
        /// </summary>
        /// <param name="context">the context when the reference was found invalid
        /// </param>
        /// <param name="reference">string with complete invalid reference
        /// </param>
        /// <param name="object">the object referred to, or null if not found
        /// </param>
        /// <param name="property">the property name from the reference
        /// </param>
        /// <param name="Info">contains template, line, column details
        /// </param>
        /// <returns> always returns null
        /// </returns>
        /// <throws>  ParseErrorException </throws>
        public virtual object InvalidGetMethod(IContext context, string reference, object object_Renamed, string property, Info info)
        {
            reportInvalidReference(reference, info);
            return null;
        }

        /// <summary> Collect the Error and/or throw an exception, depending on configuration.
        /// 
        /// </summary>
        /// <param name="context">the context when the reference was found invalid
        /// </param>
        /// <param name="reference">complete invalid reference
        /// </param>
        /// <param name="object">the object referred to, or null if not found
        /// </param>
        /// <param name="method">the property name from the reference
        /// </param>
        /// <param name="Info">contains template, line, column details
        /// </param>
        /// <returns> always returns null
        /// </returns>
        /// <throws>  ParseErrorException </throws>
        public virtual object InvalidMethod(IContext context, string reference, object object_Renamed, string method, Info info)
        {
            if (reference == null)
            {
                //UPGRADE_TODO: 在 .NET 中，方法“java.lang.Class.getName”的等效项可能返回不同的值。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                reportInvalidReference(object_Renamed.GetType().FullName + "." + method, info);
            }
            else
            {
                reportInvalidReference(reference, info);
            }
            return null;
        }

        /// <summary> Collect the Error and/or throw an exception, depending on configuration.
        /// 
        /// </summary>
        /// <param name="context">the context when the reference was found invalid
        /// </param>
        /// <param name="leftreference">left reference being assigned to
        /// </param>
        /// <param name="rightreference">invalid reference on the right
        /// </param>
        /// <param name="Info">contains Info on template, line, col
        /// </param>
        /// <returns> loop to end -- always returns false
        /// </returns>
        public virtual bool InvalidSetMethod(IContext context, string leftreference, string rightreference, Info info)
        {
            reportInvalidReference(leftreference, info);
            return false;
        }


        /// <summary> Check for an invalid reference and collect the Error or throw an exception 
        /// (depending on configuration).
        /// 
        /// </summary>
        /// <param name="reference">the invalid reference
        /// </param>
        /// <param name="Info">line, column, template name
        /// </param>
        private void reportInvalidReference(string reference, Info info)
        {
            InvalidReferenceInfo invalidReferenceInfo = new InvalidReferenceInfo(reference, info);
            invalidReferences.Add(invalidReferenceInfo);

            if (stopOnFirstInvalidReference)
            {
                throw new ParseErrorException("Error in page - invalid reference.  ", info, invalidReferenceInfo.InvalidReference);
            }
        }


        /// <summary> Called automatically when event cartridge is initialized.</summary>
        /// <param name="rs">RuntimeServices object assigned during initialization
        /// </param>
        public virtual void SetRuntimeServices(IRuntimeServices rs)
        {
            stopOnFirstInvalidReference = rs.Configuration.GetBoolean(EVENTHANDLER_INVALIDREFERENCE_EXCEPTION, false);
        }
    }
}
