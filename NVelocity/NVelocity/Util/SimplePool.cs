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

namespace NVelocity.Util
{
    /// <summary> Simple object pool. Based on ThreadPool and few other classes
    /// 
    /// The pool will ignore overflow and return null if empty.
    /// 
    /// </summary>
    /// <author>  Gal Shachor
    /// </author>
    /// <author>  Costin
    /// </author>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: SimplePool.java 463298 2006-10-12 16:10:32Z henning $
    /// </version>
    public sealed class SimplePool<T>
        where T : class
    {
        /// <summary> Return the size of the pool</summary>
        /// <returns> The pool size.
        /// </returns>
        public int Max
        {
            get
            {
                return max;
            }

        }
        /// <summary>   for testing purposes, so we can examine the pool
        /// 
        /// </summary>
        /// <returns> Array of Objects in the pool.
        /// </returns>
        internal T[] Pool
        {
            get
            {
                return pool;
            }

        }
        /*
        * Where the objects are held.
        */
        private T[] pool;

        /// <summary>  max amount of objects to be managed
        /// set via CTOR
        /// </summary>
        private int max;

        /// <summary>  index of previous to next
        /// free slot
        /// </summary>
        private int current = -1;

        /// <param name="max">
        /// </param>
        public SimplePool(int max)
        {
            this.max = max;
            pool = new T[max];
        }

        /// <summary> Add the object to the pool, silent nothing if the pool is full</summary>
        /// <param name="o">
        /// </param>
        public void Put(T o)
        {
            int idx = -1;

            lock (this)
            {
                /*
                *  if we aren't full
                */

                if (current < max - 1)
                {
                    /*
                    *  then increment the
                    *  current index.
                    */
                    idx = ++current;
                }

                if (idx >= 0)
                {
                    pool[idx] = o;
                }
            }
        }

        /// <summary> Get an object from the pool, null if the pool is empty.</summary>
        /// <returns> The object from the pool.
        /// </returns>
        public T Get()
        {
            lock (this)
            {
                /*
                *  if we have any in the pool
                */
                if (current >= 0)
                {
                    /*
                    *  remove the current one
                    */

                    T o = pool[current];
                    pool[current] = null;

                    current--;

                    return o;
                }
            }

            return null;
        }
    }
}