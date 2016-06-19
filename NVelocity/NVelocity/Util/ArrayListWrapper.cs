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
    using System;
    using System.Collections;

    /// <summary> A class that wraps an array with a List interface.
    /// 
    /// </summary>
    /// <author>  Chris Schultz &lt;chris@christopherschultz.net$gt;
    /// </author>
    /// <version>  $Revision: 685685 $ $Date: 2006-04-14 19:40:41 $
    /// </version>
    /// <since> 1.6
    /// </since>
    public class ArrayListWrapper : IList
    {
        public object this[int index]
        {
            get
            {
                return ((Array)array).GetValue(index);
            }

            set
            {
            }

        }

        public int Count
        {
            get
            {
                return ((Array)array).Length;
            }

        }

        private object array;

        public ArrayListWrapper(object array)
        {
            this.array = array;
        }

        public object Set(int index, object element)
        {
            object old = this[index];
            ((Array)array).SetValue(element, index);
            return old;
        }

        public int Add(object value)
        {
            return 0;
        }

        public bool Contains(object value)
        {
            return false;
        }

        public void Clear()
        {
        }

        public int IndexOf(object value)
        {
            return 0;
        }

        public void Insert(int index, object value)
        {
        }

        public void Remove(object value)
        {
        }

        public void RemoveAt(int index)
        {
        }

        public void CopyTo(Array array, System.Int32 index)
        {
            for (int i = index; i < this.Count; i++)
                array.SetValue(this[i], i);
        }

        public IEnumerator GetEnumerator()
        {
            return null;
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }

        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }

        }

        public object SyncRoot
        {
            get
            {
                return null;
            }

        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }

        }
    }
}
