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
    using System;

    using Context;
    using App.Event;
    using Exception;
    using Util.Introspection;

    /// <summary>  ASTMethod.java
    /// 
    /// Method support for references :  $foo.method()
    /// 
    /// NOTE :
    /// 
    /// introspection is now done at render time.
    /// 
    /// Please look at the Parser.jjt file which is
    /// what controls the generation of this class.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: ASTMethod.java 720228 2008-11-24 16:58:33Z nbubna $
    /// </version>
    public class ASTMethod : SimpleNode
    {
        /// <returns> Returns the methodName.
        /// </returns>
        /// <since> 1.5
        /// </since>
        virtual public string MethodName
        {
            get
            {
                return methodName;
            }

        }
        private string methodName = "";
        private int paramCount = 0;

        protected internal Info uberInfo;

        /// <summary> Indicates if we are running in strict reference mode.</summary>
        protected internal bool strictRef = false;

        /// <param name="id">
        /// </param>
        public ASTMethod(int id)
            : base(id)
        {
        }

        /// <param name="p">
        /// </param>
        /// <param name="id">
        /// </param>
        public ASTMethod(Parser p, int id)
            : base(p, id)
        {
        }

        /// <seealso cref="NVelocity.Runtime.Paser.Node.SimpleNode.Accept(NVelocity.Runtime.Paser.Node.IParserVisitor, System.Object)">
        /// </seealso>
        public override object Accept(IParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }

        /// <summary>  simple Init - Init our subtree and Get what we can from
        /// the AST
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <param name="data">
        /// </param>
        /// <returns> The Init result
        /// </returns>
        /// <throws>  TemplateInitException </throws>
        public override object Init(IInternalContextAdapter context, object data)
        {
            base.Init(context, data);

            /*
            * make an uberinfo - saves new's later on
            */

            uberInfo = new Info(TemplateName, Line, Column);
            /*
            *  this is about all we can do
            */

            methodName = FirstToken.Image;
            paramCount = GetNumChildren() - 1;

            strictRef = rsvc.GetBoolean(NVelocity.Runtime.RuntimeConstants.RUNTIME_REFERENCES_STRICT, false);

            return data;
        }

        /// <summary>  invokes the method.  Returns null if a problem, the
        /// parameters return if the method returns something, or
        /// an empty string "" if the method returns void
        /// </summary>
        /// <param name="instance">
        /// </param>
        /// <param name="context">
        /// </param>
        /// <returns> Result or null.
        /// </returns>
        /// <throws>  MethodInvocationException </throws>
        public override object Execute(object o, IInternalContextAdapter context)
        {
            /*
            *  new strategy (strategery!) for introspection. Since we want
            *  to be thread- as well as context-safe, we *must* do it now,
            *  at execution time.  There can be no in-node caching,
            *  but if we are careful, we can do it in the context.
            */

            IVelMethod method = null;

            object[] params_Renamed = new object[paramCount];

            try
            {
                /*
                * sadly, we do need recalc the values of the args, as this can
                * change from visit to visit
                */

                System.Type[] paramClasses = paramCount > 0 ? new System.Type[paramCount] : new System.Collections.Generic.List<Type>().ToArray();

                for (int j = 0; j < paramCount; j++)
                {
                    params_Renamed[j] = GetChild(j + 1).Value(context);

                    if (params_Renamed[j] != null)
                    {
                        paramClasses[j] = params_Renamed[j].GetType();
                    }
                }

                /*
                *   check the cache
                */

                MethodCacheKey mck = new MethodCacheKey(methodName, paramClasses);
                IntrospectionCacheData icd = context.ICacheGet(mck);

                /*
                *  like ASTIdentifier, if we have cache information, and the
                *  Class of Object instance is the same as that in the cache, we are
                *  safe.
                */

                if (icd != null && (o != null && icd.ContextData == o.GetType()))
                {

                    /*
                    * Get the method from the cache
                    */

                    method = (IVelMethod)icd.Thingy;
                }
                else
                {
                    /*
                    *  otherwise, do the introspection, and then
                    *  cache it
                    */

                    method = rsvc.Uberspect.GetMethod(o, methodName, params_Renamed, new Info(TemplateName, Line, Column));

                    if ((method != null) && (o != null))
                    {
                        icd = new IntrospectionCacheData();
                        icd.ContextData = o.GetType();
                        icd.Thingy = method;

                        context.ICachePut(mck, icd);
                    }
                }

                /*
                *  if we still haven't gotten the method, either we are calling
                *  a method that doesn't exist (which is fine...)  or I screwed
                *  it up.
                */

                if (method == null)
                {
                    if (strictRef)
                    {
                        // Create a parameter list for the exception Error message
                        System.Text.StringBuilder plist = new System.Text.StringBuilder();
                        for (int i = 0; i < params_Renamed.Length; i++)
                        {
                            System.Type param = paramClasses[i];
                          
                            plist.Append(param == null ? "null" : param.FullName);
                            if (i < params_Renamed.Length - 1)
                                plist.Append(", ");
                        }
                       
                        throw new MethodInvocationException("Object '" + o.GetType().FullName + "' does not contain method " + methodName + "(" + plist + ")", null, methodName, uberInfo.TemplateName, uberInfo.Line, uberInfo.Column);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (MethodInvocationException mie)
            {
                /*
                *  this can come from the doIntrospection(), as the arg values
                *  are evaluated to find the right method signature.  We just
                *  want to propogate it here, not do anything fancy
                */

                throw mie;
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
                /*
                *  can come from the doIntropection() also, from Introspector
                */
                string msg = "ASTMethod.Execute() : exception from introspection";
                log.Error(msg, e);
                throw new VelocityException(msg, e);
            }

            try
            {
                /*
                *  Get the returned object.  It may be null, and that is
                *  valid for something declared with a void return type.
                *  Since the caller is expecting something to be returned,
                *  as long as things are peachy, we can return an empty
                *  String so ASTReference() correctly figures out that
                *  all is well.
                */

                object obj = method.Invoke(o, params_Renamed);

                if (obj == null)
                {
                    if (method.ReturnType == System.Type.GetType("System.Void"))
                    {
                        return "";
                    }
                }

                return obj;
            }
            catch (System.Reflection.TargetInvocationException ite)
            {
                return HandleInvocationException(o, context, ite.GetBaseException());
            }
            /** Can also be thrown by method invocation **/
            catch (System.ArgumentException t)
            {
                return HandleInvocationException(o, context, t);
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
                string msg = "ASTMethod.Execute() : exception invoking method '" + methodName + "' in " + o.GetType();
                log.Error(msg, e);
                throw new VelocityException(msg, e);
            }
        }

        
        private object HandleInvocationException(object o, IInternalContextAdapter context, System.Exception t)
        {
            /*
            *  In the event that the invocation of the method
            *  itself throws an exception, we want to catch that
            *  wrap it, and throw.  We don't Log here as we want to figure
            *  out which reference threw the exception, so do that
            *  above
            */

            /*
            *  let non-Exception Throwables go...
            */

            if (t is System.Exception)
            {
                try
                {
                    return EventHandlerUtil.MethodException(rsvc, context, o.GetType(), methodName, (System.Exception)t);
                }
                /**
                * If the event handler throws an exception, then wrap it
                * in a MethodInvocationException.  Don't pass through RuntimeExceptions like other
                * similar catchall code blocks.
                */
                catch (System.Exception e)
                {
                    throw new MethodInvocationException("Invocation of method '" + methodName + "' in  " + o.GetType() + " threw exception " + e.ToString(), e, methodName, TemplateName, this.Line, this.Column);
                }
            }
            else
            {
                /*
                * no event cartridge to override. Just throw
                */

                throw new MethodInvocationException("Invocation of method '" + methodName + "' in  " + o.GetType() + " threw exception " + t.ToString(), t, methodName, TemplateName, this.Line, this.Column);
            }
        }

        /// <summary> Internal class used as key for method cache.  Combines
        /// ASTMethod fields with array of parameter classes.  Has
        /// public access (and complete constructor) for unit test 
        /// purposes.
        /// </summary>
        /// <since> 1.5
        /// </since>
        public class MethodCacheKey
        {
            
            private string methodName;
          
            private System.Type[] params_Renamed;

            public MethodCacheKey(string methodName, System.Type[] params_Renamed)
            {
                /// <summary> Should never be initialized with nulls, but to be safe we refuse 
                /// to accept them.
                /// </summary>
                this.methodName = (methodName != null) ? methodName : string.Empty;
                this.params_Renamed = (params_Renamed != null) ? params_Renamed : new System.Collections.Generic.List<Type>().ToArray();
            }

            /// <seealso cref="java.lang.Object.equals(java.lang.Object)">
            /// </seealso>
            public override bool Equals(object o)
            {
                /** 
                * note we skip the null test for methodName and params
                * due to the earlier test in the constructor
                */
                if (o is MethodCacheKey)
                {
                   
                    MethodCacheKey other = (MethodCacheKey)o;
                    if (params_Renamed.Length == other.params_Renamed.Length && methodName.Equals(other.methodName))
                    {
                        for (int i = 0; i < params_Renamed.Length; ++i)
                        {
                            if (params_Renamed[i] == null)
                            {
                                if (params_Renamed[i] != other.params_Renamed[i])
                                {
                                    return false;
                                }
                            }
                            else if (!params_Renamed[i].Equals(other.params_Renamed[i]))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
                return false;
            }


            /// <seealso cref="java.lang.Object.hashCode()">
            /// </seealso>
            public override int GetHashCode()
            {
                int result = 17;

                /** 
                * note we skip the null test for methodName and params
                * due to the earlier test in the constructor
                */
                for (int i = 0; i < params_Renamed.Length; ++i)
                {
                   
                    System.Type param = params_Renamed[i];
                    if (param != null)
                    {
                        result = result * 37 + param.GetHashCode();
                    }
                }

                result = result * 37 + methodName.GetHashCode();

                return result;
            }
        }
    }
}