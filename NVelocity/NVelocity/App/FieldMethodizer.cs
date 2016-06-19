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

namespace NVelocity.App
{
    using System;

    /// <summary>  <p>
    /// This is a small utility class allow easy access to static fields in a class,
    /// such as string constants.  Velocity will not introspect for class
    /// fields (and won't in the future :), but writing setter/getter methods to do
    /// this really is a pain,  so use this if you really have
    /// to access fields.
    /// 
    /// <p>
    /// The idea it so enable access to the fields just like you would in Java.
    /// For example, in Java, you would access a static field like
    /// <blockquote><pre>
    /// MyClass.STRING_CONSTANT
    /// </pre></blockquote>
    /// and that is the same thing we are trying to allow here.
    /// 
    /// <p>
    /// So to use in your Java code, do something like this :
    /// <blockquote><pre>
    /// context.put("runtime", new FieldMethodizer( "org.apache.velocity.runtime.Runtime" ));
    /// </pre></blockquote>
    /// and then in your template, you can access any of your static fields in this way :
    /// <blockquote><pre>
    /// $runtime.COUNTER_NAME
    /// </pre></blockquote>
    /// 
    /// <p>
    /// Right now, this class only methodizes <code>public static</code> fields.  It seems
    /// that anything else is too dangerous.  This class is for convenience accessing
    /// 'constants'.  If you have fields that aren't <code>static</code> it may be better
    /// to handle them by explicitly placing them into the context.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
    /// </author>
    /// <version>  $Id: FieldMethodizer.java 652755 2008-05-02 04:00:58Z nbubna $
    /// </version>
    public class FieldMethodizer
    {
        /// <summary>Hold the field objects by field name </summary>
      
        private System.Collections.Hashtable fieldHash = new System.Collections.Hashtable();

        /// <summary> Allow object to be initialized without any data. You would use
        /// AddObject() to Add data later.
        /// </summary>
        public FieldMethodizer()
        {
        }

        /// <summary>  Constructor that takes as it's arg the name of the class
        /// to methodize.
        /// 
        /// </summary>
        /// <param name="s">Name of class to methodize.
        /// </param>
        public FieldMethodizer(string s)
        {
            try
            {
                AddObject(s);
            }
            catch (System.Exception e)
            {
               
                System.Console.Error.WriteLine("Could not Add " + s + " for field methodizing: " + e.Message);
            }
        }

        /// <summary>  Constructor that takes as it's arg a living
        /// object to methodize.  Note that it will still
        /// only methodized the public static fields of
        /// the class.
        /// 
        /// </summary>
        /// <param name="instance">Name of class to methodize.
        /// </param>
        public FieldMethodizer(object o)
        {
            try
            {
                AddObject(o);
            }
            catch (System.Exception e)
            {
             
                System.Console.Error.WriteLine("Could not Add " + o + " for field methodizing: " + e.Message);
            }
        }

        /// <summary> Add the Name of the class to methodize</summary>
        /// <param name="s">
        /// </param>
        /// <throws>  Exception </throws>
        public virtual void AddObject(string s)
        {
            Inspect(Type.GetType(s));
        }

        /// <summary> Add an Object to methodize</summary>
        /// <param name="instance">
        /// </param>
        /// <throws>  Exception </throws>
        public virtual void AddObject(object o)
        {
            Inspect(o.GetType());
        }

        /// <summary>  Accessor method to get the fields by name.
        /// 
        /// </summary>
        /// <param name="fieldName">Name of static field to retrieve
        /// 
        /// </param>
        /// <returns> The value of the given field.
        /// </returns>
        public virtual object Get(string fieldName)
        {
            object value_Renamed = null;
            try
            {
               
                System.Reflection.FieldInfo f = (System.Reflection.FieldInfo)fieldHash[fieldName];
                if (f != null)
                {
                    value_Renamed = f.GetValue(null);
                }
            }
            catch (System.UnauthorizedAccessException e)
            {
              
                System.Console.Error.WriteLine("IllegalAccessException while trying to access " + fieldName + ": " + e.Message);
            }
            return value_Renamed;
        }

        /// <summary>  Method that retrieves all public static fields
        /// in the class we are methodizing.
        /// </summary>
        private void Inspect(System.Type clas)
        {
            System.Reflection.FieldInfo[] fields = clas.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                System.Reflection.FieldInfo fieldInfo = fields[i];

                if (fieldInfo.IsStatic && fieldInfo.IsPublic)
                {
                    fieldHash[fields[i].Name] = fields[i];
                }
            }
        }
    }
}