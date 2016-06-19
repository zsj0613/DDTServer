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
    using System;

    using Exception;

    /// <summary> <p>
    /// This class is responsible for instantiating the correct ILogChute
    /// </p>
    /// 
    /// <p>
    /// The approach is :
    /// </p>
    /// <ul>
    /// <li>
    /// First try to see if the user is passing in a living object
    /// that is a ILogChute, allowing the app to give its living
    /// custom loggers.
    /// </li>
    /// <li>
    /// Next, run through the (possible) list of classes specified
    /// specified as loggers, taking the first one that appears to
    /// work.  This is how we support finding logkit, log4j or
    /// jdk logging, whichever is in the classpath and found first,
    /// as all three are listed as defaults.
    /// </li>
    /// <li>
    /// Finally, we turn to the System.err stream and print Log messages
    /// to it if nothing else works.
    /// </li>
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:jon@latchkey.com">Jon S. Stevens</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  <a href="mailto:nbubna@apache.org">Nathan Bubna</a>
    /// </author>
    /// <version>  $Id: LogManager.java 699307 2008-09-26 13:16:05Z cbrisson $
    /// </version>
    public class LogManager
    {
        // Creates a new logging system or returns an existing one
        // specified by the application.
        private static ILogChute CreateLogChute(IRuntimeServices rsvc)
        {
            Log log = rsvc.Log;

            /* If a ILogChute or LogSystem instance was set as a configuration
            * value, use that.  This is any class the user specifies.
            */
            System.Object o = rsvc.GetProperty(RuntimeConstants.RUNTIME_LOG_LOGSYSTEM);
            if (o != null)
            {
                // first check for a ILogChute
                if (o is ILogChute)
                {
                    try
                    {
                        ((ILogChute)o).Init(rsvc);
                        return (ILogChute)o;
                    }
                    catch (System.Exception e)
                    {
                        System.String msg = "Could not init runtime.log.logsystem " + o;
                        log.Error(msg, e);
                        throw new VelocityException(msg, e);
                    }
                }
                else
                {
                    System.String msg = o.GetType().FullName + " object set as runtime.log.logsystem is not a valid log implementation.";
                    log.Error(msg);
                    throw new VelocityException(msg);
                }
            }

            /* otherwise, see if a class was specified.  You can Put multiple
            * classes, and we use the first one we find.
            *
            * Note that the default value of this property contains the
            * AvalonLogChute, the Log4JLogChute, CommonsLogLogChute,
            * ServletLogChute, and the JdkLogChute for
            * convenience - so we use whichever we works first.
            */
            System.Collections.IList classes = new System.Collections.ArrayList();
            System.Object obj = rsvc.GetProperty(RuntimeConstants.RUNTIME_LOG_LOGSYSTEM_CLASS);

            /*
            *  we might have a list, or not - so check
            */
            if (obj is System.Collections.IList)
            {
                classes = (System.Collections.IList)obj;
            }
            else if (obj is System.String)
            {
                classes.Add(obj);
            }

            /*
            *  now run through the list, trying each.  It's ok to
            *  fail with a class not found, as we do this to also
            *  search out a default simple file logger
            */
            for (System.Collections.IEnumerator ii = classes.GetEnumerator(); ii.MoveNext(); )
            {
                System.String claz = (System.String)ii.Current;
                if (claz != null && claz.Length > 0)
                {
                    log.Debug("Trying to use logger class " + claz);
                    try
                    {
                        o = Activator.CreateInstance(Type.GetType(claz.Replace(';', ',')));
                        if (o is ILogChute)
                        {
                            ((ILogChute)o).Init(rsvc);
                            log.Debug("Using logger class " + claz);
                            return (ILogChute)o;
                        }
                        else
                        {
                            System.String msg = "The specified logger class " + claz + " does not implement the " + typeof(ILogChute).FullName + " interface.";
                            log.Error(msg);
                            // be extra informative if it appears to be a classloader issue
                            // this should match all our provided LogChutes
                            if (IsProbablyProvidedLogChute(claz))
                            {
                                // if it's likely to be ours, tip them off about classloader stuff
                                log.Error("This appears to be a ClassLoader issue.  Check for multiple Velocity jars in your classpath.");
                            }
                            throw new VelocityException(msg);
                        }
                    }
                    catch (System.ApplicationException ncdfe)
                    {
                        // note these errors for anyone debugging the app
                        if (IsProbablyProvidedLogChute(claz))
                        {
                            log.Debug("Target log system for " + claz + " is not available (" + ncdfe.ToString() + ").  Falling back to next log system...");
                        }
                        else
                        {
                            log.Debug("Couldn't find class " + claz + " or necessary supporting classes in classpath.", ncdfe);
                        }
                    }
                    catch (System.Exception e)
                    {
                        System.String msg = "Failed to initialize an instance of " + claz + " with the current runtime configuration.";
                        // Log unexpected Init exception at higher priority
                        log.Error(msg, e);
                        throw new VelocityException(msg, e);
                    }
                }
            }

            /* If the above failed, that means either the user specified a
            * logging class that we can't find, there weren't the necessary
            * dependencies in the classpath for it, or there were the same
            * problems for the default loggers, log4j and Java1.4+.
            * Since we really don't know and we want to be sure the user knows
            * that something went wrong with the logging, let's fall back to the
            * surefire SystemLogChute. No panicking or failing to Log!!
            */
            ILogChute slc = new NullLogChute();
            slc.Init(rsvc);
            log.Debug("Using SystemLogChute.");
            return slc;
        }

        /// <summary> Simply tells whether the specified classname probably is provided
        /// by Velocity or is implemented by someone else.  Not surefire, but
        /// it'll probably always be right.  In any case, this method shouldn't
        /// be relied upon for anything important.
        /// </summary>
        private static bool IsProbablyProvidedLogChute(System.String claz)
        {
            if (claz == null)
            {
                return false;
            }
            else
            {
                return (claz.StartsWith("org.apache.velocity.runtime.log") && claz.EndsWith("ILogChute"));
            }
        }

        /// <summary> Update the Log instance with the appropriate ILogChute and other
        /// settings determined by the RuntimeServices.
        /// </summary>
        /// <param name="Log">
        /// </param>
        /// <param name="rsvc">
        /// </param>
        /// <throws>  Exception </throws>
        /// <since> 1.5
        /// </since>
        public static void UpdateLog(Log log, IRuntimeServices rsvc)
        {
            // create a new ILogChute using the RuntimeServices
            ILogChute newLogChute = CreateLogChute(rsvc);
            ILogChute oldLogChute = log.GetLogChute();

            // pass the new ILogChute to the Log first,
            // (if the old was a HoldingLogChute, we don't want it
            //  to accrue new messages during the transfer below)
            log.SetLogChute(newLogChute);

            // If the old ILogChute was the pre-Init logger,
            // dump its messages into the new system.
            if (oldLogChute is HoldingLogChute)
            {
                HoldingLogChute hlc = (HoldingLogChute)oldLogChute;
                hlc.TransferTo(newLogChute);
            }
        }
    }
}