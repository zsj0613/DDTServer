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

namespace NVelocity.Context
{
    using System;
    using System.Linq;

    using Exception;
    using Runtime;


    /// <summary>  This is a special, internal-use-only context implementation to be
    /// used for the #Evaluate directive.
    /// 
    /// We use this context to chain the existing context, preventing any changes
    /// from impacting the parent context.  By separating this context into a 
    /// separate class it also allows for the future possibility of changing
    /// the context behavior for the #Evaluate directive.
    /// 
    /// Note that the context used to store values local to #Evaluate()
    /// is user defined but defaults to {@link VelocityContext}.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <version>  $Id: EvaluateContext.java 691519 2008-09-03 05:36:11Z nbubna $
    /// </version>
    /// <since> 1.6
    /// </since>
    public class EvaluateContext : ChainedInternalContextAdapter
    {
        /// <seealso cref="org.apache.velocity.context.Context.getKeys()">
        /// </seealso>
        override public object[] Keys
        {
            get
            {
                System.Collections.Generic.HashSet<object> keys = new System.Collections.Generic.HashSet<object>();
                object[] localKeys = localContext.Keys;
                for (int i = 0; i < localKeys.Length; i++)
                {
                    keys.Add(localKeys[i]);
                }

                object[] innerKeys = base.Keys;
                for (int i = 0; i < innerKeys.Length; i++)
                {
                    keys.Add(innerKeys[i]);
                }
                return keys.ToArray();
            }

        }

        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.getAllowRendering()">
        /// </seealso>
        /// <seealso cref="org.apache.velocity.context.InternalHousekeepingContext.setAllowRendering(boolean)">
        /// </seealso>
        override public bool AllowRendering
        {
            get
            {
                return allowRendering && innerContext.AllowRendering;
            }

            set
            {
                allowRendering = false;
            }

        }
        /// <summary>container for any local items </summary>
        internal IContext localContext;
        internal bool allowRendering = true;

        /// <summary>  CTOR, wraps an ICA</summary>
        /// <param name="inner">context for parent template
        /// </param>
        /// <param name="rsvc">
        /// </param>
        public EvaluateContext(IInternalContextAdapter inner, IRuntimeServices rsvc)
            : base(inner)
        {
            InitContext(rsvc);
        }

        /// <summary> Initialize the context based on user-configured class </summary>
        /// <param name="rsvc">
        /// </param>
        private void InitContext(IRuntimeServices rsvc)
        {
            string contextClass = rsvc.GetString(NVelocity.Runtime.RuntimeConstants.EVALUATE_CONTEXT_CLASS);

            if (contextClass != null && contextClass.Length > 0)
            {
                object o = null;

                try
                {
                    o = System.Activator.CreateInstance(Type.GetType(contextClass.Replace(';',',')));
                }
                catch (RuntimeException cnfe)
                {
                    string err = "The specified class for #evaluate() context (" + contextClass + ") does not exist or is not accessible to the current classloader.";
                    rsvc.Log.Error(err);
                    throw new RuntimeException(err, cnfe);
                }
                catch (System.Exception e)
                {
                    string err = "The specified class for #evaluate() context (" + contextClass + ") can not be loaded.";
                    rsvc.Log.Error(err, e);
                    throw new System.SystemException(err);
                }

                if (!(o is IContext))
                {
                    string err = "The specified class for #evaluate() context (" + contextClass + ") does not implement " + typeof(IContext).FullName + ".";
                    rsvc.Log.Error(err);
                    throw new System.SystemException(err);
                }

                localContext = (IContext)o;
            }
            else
            {
                string err = "No class specified for #evaluate() context.";
                rsvc.Log.Error(err);
                throw new System.SystemException(err);
            }
        }

        /// <summary>  Put method also stores values in local scope 
        /// 
        /// </summary>
        /// <param name="key">name of item to set
        /// </param>
        /// <param name="value">object to set to key
        /// </param>
        /// <returns> old stored object
        /// </returns>
        public override object Put(string key, object value)
        {
            /*
            *  just Put in the local context
            */
            return localContext.Put(key, value);
        }

        /// <summary>  Retrieves from local or global context.
        /// 
        /// </summary>
        /// <param name="key">name of item to Get
        /// </param>
        /// <returns>  stored object or null
        /// </returns>
        public override object Get(string key)
        {
            /*
            *  always try the local context then innerContext
            */

            object o = localContext.Get(key);

            if (o == null)
            {
                o = base.Get(key);
            }

            return o;
        }

        /// <seealso cref="org.apache.velocity.context.Context.containsKey(java.lang.Object)">
        /// </seealso>
        public override bool ContainsKey(object key)
        {
            return localContext.ContainsKey(key) || base.ContainsKey(key);
        }

        /// <seealso cref="org.apache.velocity.context.Context.remove(java.lang.Object)">
        /// </seealso>
        public override object Remove(object key)
        {
            return localContext.Remove(key);
        }

        /// <summary> Allows callers to explicitly Put objects in the local context.
        /// Objects added to the context through this method always end up
        /// in the top-level context of possible wrapped contexts.
        /// 
        /// </summary>
        /// <param name="key">name of item to set.
        /// </param>
        /// <param name="value">object to set to key.
        /// </param>
        /// <returns> old stored object
        /// </returns>
        public override object LocalPut(string key, object value)
        {
            return localContext.Put(key, value);
        }
    }
}
