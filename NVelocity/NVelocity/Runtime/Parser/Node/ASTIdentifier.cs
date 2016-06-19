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

namespace NVelocity.Runtime.Parser.Node
{
    using App.Event;
    using Context;
    using Exception;
    using Util.Introspection;

    /// <summary>  ASTIdentifier.java
    /// 
    /// Method support for identifiers :  $foo
    /// 
    /// mainly used by ASTRefrence
    /// 
    /// Introspection is now moved to 'just in time' or at render / execution
    /// time. There are many reasons why this has to be done, but the
    /// primary two are   thread safety, to remove any context-derived
    /// information from class member  variables.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: ASTIdentifier.java 720228 2008-11-24 16:58:33Z nbubna $
    /// </version>
    public class ASTIdentifier : SimpleNode
    {
        private string identifier = "";

        /// <summary>  This is really immutable after the Init, so keep one for this node</summary>
        protected internal Info uberInfo;

        /// <summary> Indicates if we are running in strict reference mode.</summary>
        protected internal bool strictRef = false;

        /// <param name="id">
        /// </param>
        public ASTIdentifier(int id)
            : base(id)
        {
        }

        /// <param name="p">
        /// </param>
        /// <param name="id">
        /// </param>
        public ASTIdentifier(Parser p, int id)
            : base(p, id)
        {
        }


        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.Accept(NVelocity.Runtime.Paser.Node.IParserVisitor, System.Object)">
        /// </seealso>
        public override object Accept(IParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }

        /// <summary>  simple Init - don't do anything that is context specific.
        /// just Get what we need from the AST, which is static.
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <param name="data">
        /// </param>
        /// <returns> The data object.
        /// </returns>
        /// <throws>  TemplateInitException </throws>
        public override object Init(IInternalContextAdapter context, object data)
        {
            base.Init(context, data);

            identifier = FirstToken.Image;

            uberInfo = new Info(TemplateName, Line, Column);

            strictRef = rsvc.GetBoolean(NVelocity.Runtime.RuntimeConstants.RUNTIME_REFERENCES_STRICT, false);

            return data;
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.Execute(java.lang.Object, org.apache.velocity.context.InternalContextAdapter)">
        /// </seealso>
        public override object Execute(object o, IInternalContextAdapter context)
        {

            IVelPropertyGet vg = null;

            try
            {
                /*
                *  first, see if we have this information cached.
                */

                IntrospectionCacheData icd = context.ICacheGet(this);

                /*
                * if we have the cache data and the class of the object we are
                * invoked with is the same as that in the cache, then we must
                * be allright.  The last 'variable' is the method name, and
                * that is fixed in the template :)
                */

                if (icd != null && (o != null) && (icd.ContextData == o.GetType()))
                {
                    vg = (IVelPropertyGet)icd.Thingy;
                }
                else
                {
                    /*
                    *  otherwise, do the introspection, and cache it.  Use the
                    *  uberspector
                    */

                    vg = rsvc.Uberspect.GetPropertyGet(o, identifier, uberInfo);

                    if (vg != null && vg.Cacheable && (o != null))
                    {
                        icd = new IntrospectionCacheData();
                        icd.ContextData = o.GetType();
                        icd.Thingy = vg;
                        context.ICachePut(this, icd);
                    }
                }
            }
            /**
            * pass through application level runtime exceptions
            */
            catch (System.SystemException e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                string msg = "ASTIdentifier.Execute() : identifier = " + identifier;
                log.Error(msg, e);
                throw new VelocityException(msg, e);
            }

            /*
            *  we have no getter... punt...
            */

            if (vg == null)
            {
                if (strictRef)
                {
                    throw new MethodInvocationException("Object '" + o.GetType().FullName + "' does not contain property '" + identifier + "'", null, identifier, uberInfo.TemplateName, uberInfo.Line, uberInfo.Column);
                }
                else
                {
                    return null;
                }
            }

            /*
            *  now try and Execute.  If we Get a MIE, throw that
            *  as the app wants to Get these.  If not, Log and punt.
            */
            try
            {
                return vg.Invoke(o);
            }
            catch (System.Reflection.TargetInvocationException ite)
            {
                /*
                *  if we have an event cartridge, see if it wants to veto
                *  also, let non-Exception Throwables go...
                */

                System.Exception t = ite.GetBaseException();
                if (t is System.Exception)
                {
                    try
                    {
                        return EventHandlerUtil.MethodException(rsvc, context, o.GetType(), vg.MethodName, (System.Exception)t);
                    }
                    /**
                    * If the event handler throws an exception, then wrap it
                    * in a MethodInvocationException.  Don't pass through RuntimeExceptions like other
                    * similar catchall code blocks.
                    */
                    catch (System.Exception)
                    {
                        throw new MethodInvocationException("Invocation of method '" + vg.MethodName + "'" + " in  " + o.GetType() + " threw exception " + ite.GetBaseException().ToString(), ite.GetBaseException(), vg.MethodName, TemplateName, this.Line, this.Column);
                    }
                }
                else
                {
                    /*
                    * no event cartridge to override. Just throw
                    */

                    throw new MethodInvocationException("Invocation of method '" + vg.MethodName + "'" + " in  " + o.GetType() + " threw exception " + ite.GetBaseException().ToString(), ite.GetBaseException(), vg.MethodName, TemplateName, this.Line, this.Column);
                }
            }
            catch (System.ArgumentException)
            {
                return null;
            }
            /**
            * pass through application level runtime exceptions
            */
            catch (System.SystemException e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                string msg = "ASTIdentifier() : exception invoking method " + "for identifier '" + identifier + "' in " + o.GetType();
                log.Error(msg, e);
                throw new VelocityException(msg, e);
            }
        }
    }
}