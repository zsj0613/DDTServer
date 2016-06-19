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

namespace NVelocity.Runtime.Directive
{
    using Context;
    using Resource;

    /// <summary> Base class for directives which do input operations
    /// (e.g. <code>#include()</code>, <code>#parse()</code>, etc.).
    /// 
    /// </summary>
    /// <author>  <a href="mailto:dlr@finemaltcoding.com">Daniel Rall</a>
    /// </author>
    /// <since> 1.4
    /// </since>
    public abstract class InputBase : Directive
    {
        /// <summary> Decides the encoding used during input processing of this
        /// directive.
        /// 
        /// Get the resource, and assume that we use the encoding of the
        /// current template the 'current resource' can be
        /// <code>null</code> if we are processing a stream....
        /// 
        /// </summary>
        /// <param name="context">The context to derive the default input encoding
        /// from.
        /// </param>
        /// <returns> The encoding to use when processing this directive.
        /// </returns>
        protected internal virtual string GetInputEncoding(IInternalContextAdapter context)
        {
            Resource current = context.CurrentResource;

            if (current != null)
            {
                return current.Encoding;
            }
            else
            {
                return (string)rsvc.GetProperty(NVelocity.Runtime.RuntimeConstants.INPUT_ENCODING);
            }
        }
    }
}
