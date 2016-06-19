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

namespace NVelocity
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;

    using Context;
    using Exception;
    using Runtime.Parser;
    using Runtime.Parser.Node;
    using Runtime.Resource;

    /// <summary> This class is used for controlling all template
    /// operations. This class uses a parser created
    /// by JavaCC to create an AST that is subsequently
    /// traversed by a Visitor.
    /// 
    /// <pre>
    /// // set up and Initialize Velocity before this code block
    /// 
    /// Template template = Velocity.GetTemplate("test.wm");
    /// Context context = new VelocityContext();
    /// 
    /// context.Put("foo", "bar");
    /// context.Put("customer", new Customer());
    /// 
    /// template.merge(context, writer);
    /// </pre>
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: Template.java 685724 2008-08-13 23:12:12Z nbubna $
    /// </version>
    public class Template : Resource
    {
        private VelocityException errorCondition = null;

        /// <summary>Default constructor </summary>
        public Template()
            : base()
        {

            Type = ResourceManagerConstants.RESOURCE_TEMPLATE;
        }

        /// <summary>  gets the named resource as a stream, parses and inits
        /// 
        /// </summary>
        /// <returns> true if successful
        /// </returns>
        /// <throws>  ResourceNotFoundException if template not found </throws>
        /// <summary>          from any available source.
        /// </summary>
        /// <throws>  ParseErrorException if template cannot be parsed due </throws>
        /// <summary>          to syntax (or other) Error.
        /// </summary>
        /// <throws>  IOException problem reading input stream </throws>
        public override bool Process()
        {
            data = null;
            Stream stream = null;
            errorCondition = null;

            /*
            *  first, try to Get the stream from the loader
            */
            try
            {
                stream = resourceLoader.GetResourceStream(name);
            }
            catch (ResourceNotFoundException rnfe)
            {
                /*
                *  remember and re-throw
                */

                errorCondition = rnfe;
                throw rnfe;
            }

            /*
            *  if that worked, lets protect in case a loader impl
            *  forgets to throw a proper exception
            */

            if (stream != null)
            {
                /*
                *  now parse the template
                */

                try
                {
                    using (StreamReader br = new StreamReader(stream, System.Text.Encoding.GetEncoding(encoding)))
                    {
                        data = rsvc.Parse(br, name);
                        InitDocument();
                        return true;
                    }
                }
                catch (IOException)
                {
                    string msg = "Template.process : Unsupported input encoding : " + encoding + " for template " + name;

                    errorCondition = new ParseErrorException(msg);
                    throw errorCondition;
                }
                catch (ParseException pex)
                {
                    /*
                    *  remember the Error and convert
                    */
                    errorCondition = new ParseErrorException(pex);
                    throw errorCondition;
                }
                catch (TemplateInitException pex)
                {
                    errorCondition = new ParseErrorException(pex);
                    throw errorCondition;
                }
                /**
                * pass through runtime exceptions
                */
                catch (SystemException e)
                {
                    throw new RuntimeException("Exception thrown processing Template " + Name, e);
                }
                finally
                {
                    /*
                    *  Make sure to close the inputstream when we are done.
                    */
                    stream.Close();
                }
            }
            else
            {
                /*
                *  is == null, therefore we have some kind of file issue
                */

                errorCondition = new ResourceNotFoundException("Unknown resource error for resource " + name);
                throw errorCondition;
            }
        }

        /// <summary>  initializes the document.  Init() is not longer
        /// dependant upon context, but we need to let the
        /// Init() carry the template name down throught for VM
        /// namespace features
        /// </summary>
        /// <throws>  TemplateInitException When a problem occurs during the document initialization. </throws>
        public virtual void InitDocument()
        {
            /*
            *  send an empty InternalContextAdapter down into the AST to Initialize it
            */

            InternalContextAdapterImpl ica = new InternalContextAdapterImpl(new VelocityContext());

            try
            {
                /*
                *  Put the current template name on the stack
                */

                ica.PushCurrentTemplateName(name);
                ica.CurrentResource = this;

                /*
                *  Init the AST
                */

                ((SimpleNode)data).Init(ica, rsvc);
            }
            finally
            {
                /*
                *  in case something blows up...
                *  pull it off for completeness
                */

                ica.PopCurrentTemplateName();
                ica.CurrentResource = null;
            }
        }

        /// <summary> The AST node structure is merged with the
        /// context to produce the final output.
        /// 
        /// </summary>
        /// <param name="context">Conext with data elements accessed by template
        /// </param>
        /// <param name="writer">output writer for rendered template
        /// </param>
        /// <throws>  ResourceNotFoundException if template not found </throws>
        /// <summary>          from any available source.
        /// </summary>
        /// <throws>  ParseErrorException if template cannot be parsed due </throws>
        /// <summary>          to syntax (or other) Error.
        /// </summary>
        /// <throws>  MethodInvocationException When a method on a referenced object in the context could not invoked. </throws>
        /// <throws>  IOException  Might be thrown while rendering. </throws>
        public virtual void Merge(IContext context, TextWriter writer)
        {
            Merge(context, writer, null);
        }


        /// <summary> The AST node structure is merged with the
        /// context to produce the final output.
        /// 
        /// </summary>
        /// <param name="context">Conext with data elements accessed by template
        /// </param>
        /// <param name="writer">output writer for rendered template
        /// </param>
        /// <param name="macroLibraries">a list of template files containing macros to be used when merging
        /// </param>
        /// <throws>  ResourceNotFoundException if template not found </throws>
        /// <summary>          from any available source.
        /// </summary>
        /// <throws>  ParseErrorException if template cannot be parsed due </throws>
        /// <summary>          to syntax (or other) Error.
        /// </summary>
        /// <throws>  MethodInvocationException When a method on a referenced object in the context could not invoked. </throws>
        /// <throws>  IOException  Might be thrown while rendering. </throws>
        /// <since> 1.6
        /// </since>
        public virtual void Merge(IContext context, TextWriter writer, IList macroLibraries)
        {
            /*
            *  we shouldn't have to do this, as if there is an Error condition,
            *  the application code should never Get a reference to the
            *  Template
            */

            if (errorCondition != null)
            {
                throw errorCondition;
            }

            if (data != null)
            {
                /*
                *  create an InternalContextAdapter to carry the user Context down
                *  into the rendering engine.  Set the template name and render()
                */

                InternalContextAdapterImpl ica = new InternalContextAdapterImpl(context);

                /**
                * Set the macro libraries
                */
                ica.MacroLibraries = macroLibraries;

                if (macroLibraries != null)
                {
                    for (int i = 0; i < macroLibraries.Count; i++)
                    {
                        /**
                        * Build the macro library
                        */
                        try
                        {
                            rsvc.GetTemplate((string)macroLibraries[i]);
                        }
                        catch (ResourceNotFoundException re)
                        {
                            /*
                            * the macro lib wasn't found.  Note it and throw
                            */
                            rsvc.Log.Error("template.merge(): " + "cannot find template " + ((string)macroLibraries[i]));
                            throw re;
                        }
                        catch (ParseErrorException pe)
                        {
                            /*
                            * the macro lib was found, but didn't parse - syntax Error
                            *  note it and throw
                            */
                            rsvc.Log.Error("template.merge(): " + "syntax error in template " + ((string)macroLibraries[i]) + ".");
                            throw pe;
                        }
                        catch (System.Exception e)
                        {
                            throw new RuntimeException("Template.merge(): parse failed in template  " + ((string)macroLibraries[i]) + ".", e);
                        }
                    }
                }

                try
                {
                    ica.PushCurrentTemplateName(name);
                    ica.CurrentResource = this;

                    ((SimpleNode)data).Render(ica, writer);
                }
                finally
                {
                    /*
                    *  lets make sure that we always clean up the context
                    */
                    ica.PopCurrentTemplateName();
                    ica.CurrentResource = null;
                }
            }
            else
            {
                /*
                * this shouldn't happen either, but just in case.
                */

                string msg = "Template.merge() failure. The document is null, " + "most likely due to parsing error.";

                throw new SystemException(msg);
            }
        }
    }
}