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
    /// <summary>  This class is the abstract base class for all conventional
    /// Velocity Context  implementations.  Simply extend this class
    /// and implement the abstract routines that access your preferred
    /// storage method.
    /// 
    /// Takes care of context chaining.
    /// 
    /// Also handles / enforces policy on null keys and values :
    /// 
    /// <ul>
    /// <li> Null keys and values are accepted and basically dropped.
    /// <li> If you place an object into the context with a null key, it
    /// will be ignored and logged.
    /// <li> If you try to place a null into the context with any key, it
    /// will be dropped and logged.
    /// </ul>
    /// 
    /// The default implementation of this for application use is
    /// org.apache.velocity.VelocityContext.
    /// 
    /// All thanks to Fedor for the chaining idea.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <author>  <a href="mailto:fedor.karpelevitch@home.com">Fedor Karpelevitch</a>
    /// </author>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <version>  $Id: AbstractContext.java 702218 2008-10-06 18:15:18Z nbubna $
    /// </version>

    public abstract class AbstractContext : InternalContextBase, IContext
    {
        /// <summary>  Get all the keys for the values in the context</summary>
        /// <returns> Object[] of keys in the Context. Does not return
        /// keys in chained context.
        /// </returns>
        public virtual object[] Keys
        {
            get
            {
                return InternalGetKeys();
            }

        }
        /// <summary>  returns innerContext if one is chained
        /// 
        /// </summary>
        /// <returns> Context if chained, <code>null</code> if not
        /// </returns>
        public virtual IContext ChainedContext
        {
            get
            {
                return innerContext;
            }

        }
        /// <summary>  the chained Context if any</summary>
        private IContext innerContext = null;

        /// <summary>  Implement to return a value from the context storage.
        /// <br><br>
        /// The implementation of this method is required for proper
        /// operation of a Context implementation in general
        /// Velocity use.
        /// 
        /// </summary>
        /// <param name="key">key whose associated value is to be returned
        /// </param>
        /// <returns> object stored in the context
        /// </returns>
        public abstract object InternalGet(string key);

        /// <summary>  Implement to Put a value into the context storage.
        /// <br><br>
        /// The implementation of this method is required for
        /// proper operation of a Context implementation in
        /// general Velocity use.
        /// 
        /// </summary>
        /// <param name="key">key with which to associate the value
        /// </param>
        /// <param name="value">value to be associated with the key
        /// </param>
        /// <returns> previously stored value if exists, or null
        /// </returns>
        public abstract object InternalPut(string key, object value);

        /// <summary>  Implement to determine if a key is in the storage.
        /// <br><br>
        /// Currently, this method is not used internally by
        /// the Velocity engine.
        /// 
        /// </summary>
        /// <param name="key">key to test for existance
        /// </param>
        /// <returns> true if found, false if not
        /// </returns>
        public abstract bool InternalContainsKey(object key);

        /// <summary>  Implement to return an object array of key
        /// strings from your storage.
        /// <br><br>
        /// Currently, this method is not used internally by
        /// the Velocity engine.
        /// 
        /// </summary>
        /// <returns> array of keys
        /// </returns>
        public abstract object[] InternalGetKeys();

        /// <summary>  I mplement to remove an item from your storage.
        /// <br><br>
        /// Currently, this method is not used internally by
        /// the Velocity engine.
        /// 
        /// </summary>
        /// <param name="key">key to remove
        /// </param>
        /// <returns> object removed if exists, else null
        /// </returns>
        public abstract object InternalRemove(object key);

        /// <summary>  default CTOR</summary>
        public AbstractContext()
        {

        }

        /// <summary>  Chaining constructor accepts a Context argument.
        /// It will relay Get() operations into this Context
        /// in the even the 'local' Get() returns null.
        /// 
        /// </summary>
        /// <param name="inner">context to be chained
        /// </param>
        public AbstractContext(IContext inner)
        {
            innerContext = inner;

            /*
            *  now, do a 'forward pull' of event cartridge so
            *  it's accessable, bringing to the top level.
            */

            if (innerContext is IInternalEventContext)
            {
                AttachEventCartridge(((IInternalEventContext)innerContext).EventCartridge);
            }
        }

        /// <summary> Adds a name/value pair to the context.
        /// 
        /// </summary>
        /// <param name="key">  The name to key the provided value with.
        /// </param>
        /// <param name="value">The corresponding value.
        /// </param>
        /// <returns> Object that was replaced in the the Context if
        /// applicable or null if not.
        /// </returns>
        public virtual object Put(string key, object value)
        {
            /*
            * don't even continue if key is null
            */
            if (key == null)
            {
                return null;
            }

            return InternalPut(key, value);
        }

        /// <summary>  Gets the value corresponding to the provided key from the context.
        /// 
        /// Supports the chaining context mechanism.  If the 'local' context
        /// doesn't have the value, we try to Get it from the chained context.
        /// 
        /// </summary>
        /// <param name="key">The name of the desired value.
        /// </param>
        /// <returns>    The value corresponding to the provided key or null if
        /// the key param is null.
        /// </returns>
        public virtual object Get(string key)
        {
            /*
            *  punt if key is null
            */

            if (key == null)
            {
                return null;
            }

            /*
            *  Get the object for this key.  If null, and we are chaining another Context
            *  call the Get() on it.
            */

            object o = InternalGet(key);

            if (o == null && innerContext != null)
            {
                o = innerContext.Get(key);
            }

            return o;
        }

        /// <summary>  Indicates whether the specified key is in the context.  Provided for
        /// debugging purposes.
        /// 
        /// </summary>
        /// <param name="key">The key to look for.
        /// </param>
        /// <returns> true if the key is in the context, false if not.
        /// </returns>
        public virtual bool ContainsKey(object key)
        {
            if (key == null)
            {
                return false;
            }

            bool exists = InternalContainsKey(key);
            if (!exists && innerContext != null)
            {
                exists = innerContext.ContainsKey(key);
            }

            return exists;
        }

        /// <summary> Removes the value associated with the specified key from the context.
        /// 
        /// </summary>
        /// <param name="key">The name of the value to remove.
        /// </param>
        /// <returns>    The value that the key was mapped to, or <code>null</code>
        /// if unmapped.
        /// </returns>
        public virtual object Remove(object key)
        {
            if (key == null)
            {
                return null;
            }

            return InternalRemove(key);
        }
    }
}