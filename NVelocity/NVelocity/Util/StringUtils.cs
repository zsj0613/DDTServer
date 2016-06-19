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
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary> This class provides some methods for dynamically
    /// invoking methods in objects, and some string
    /// manipulation methods used by torque. The string
    /// methods will soon be moved into the turbine
    /// string utilities class.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author>  <a href="mailto:dlr@finemaltcoding.com">Daniel Rall</a>
    /// </author>
    /// <version>  $Id: StringUtils.java 685685 2008-08-13 21:43:27Z nbubna $
    /// </version>
    public class StringUtils
    {
        /// <summary> Line separator for the OS we are operating on.</summary>
        private static readonly string EOL = System.Environment.NewLine;

        /// <summary> Chop i characters off the end of a string.
        /// A 2 character EOL will count as 1 character.
        /// 
        /// </summary>
        /// <param name="s">String to chop.
        /// </param>
        /// <param name="i">Number of characters to chop.
        /// </param>
        /// <param name="eol">A String representing the EOL (end of line).
        /// </param>
        /// <returns> String with processed answer.
        /// </returns>
        public static string Chop(string s, int i, string eol)
        {
            if (i == 0 || s == null || eol == null)
            {
                return s;
            }

            int length = s.Length;

            /*
            * if it is a 2 char EOL and the string ends with
            * it, nip it off.  The EOL in this case is treated like 1 character
            */
            if (eol.Length == 2 && s.EndsWith(eol))
            {
                length -= 2;
                i -= 1;
            }

            if (i > 0)
            {
                length -= i;
            }

            if (length < 0)
            {
                length = 0;
            }

            return s.Substring(0, (length) - (0));
        }

        /// <summary> Return a context-relative path, beginning with a "/", that represents
        /// the canonical version of the specified path after ".." and "." elements
        /// are resolved out.  If the specified path attempts to go outside the
        /// boundaries of the current context (i.e. too many ".." path elements
        /// are present), return <code>null</code> instead.
        /// 
        /// </summary>
        /// <param name="path">Path to be normalized
        /// </param>
        /// <returns> String normalized path
        /// </returns>
        public static string NormalizePath(string path)
        {
            // Normalize the slashes and Add leading slash if necessary
            string normalized = path;
            if (normalized.IndexOf(Path.AltDirectorySeparatorChar) >= 0)
            {
                normalized = normalized.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            if (!normalized.StartsWith(Path.DirectorySeparatorChar.ToString()))
            {
                normalized = Path.DirectorySeparatorChar + normalized;
            }

            // Resolve occurrences of "//" in the normalized path
            while (true)
            {
                int index = normalized.IndexOf("//");
                if (index < 0) break;
                normalized = normalized.Substring(0, (index) - (0)) + normalized.Substring(index + 1);
            }

            // Resolve occurrences of "%20" in the normalized path
            while (true)
            {
                int index = normalized.IndexOf("%20");
                if (index < 0) break;
                normalized = string.Format("{0} {1}", normalized.Substring(0, (index) - (0)), normalized.Substring(index + 3));
            }

            // Resolve occurrences of "/./" in the normalized path
            while (true)
            {
                int index = normalized.IndexOf("/./");
                if (index < 0)
                {
                    break;
                }
                normalized = normalized.Substring(0, (index) - (0)) + normalized.Substring(index + 2);
            }

            // Resolve occurrences of "/../" in the normalized path
            while (true)
            {
                int index = normalized.IndexOf("/../");
                if (index < 0)
                {
                    break;
                }
                if (index == 0)
                {
                    return (null);
                }
                // Trying to go outside our context
                int index2 = normalized.LastIndexOf((Char)'/', index - 1);
                normalized = normalized.Substring(0, (index2) - (0)) + normalized.Substring(index + 3);
            }

            // Return the normalized path that we have completed
            return (normalized);
        }

        /// <summary> Read the contents of a file and place them in
        /// a string object.
        /// 
        /// </summary>
        /// <param name="file">path to file.
        /// </param>
        /// <returns> String contents of the file.
        /// </returns>
        public static System.String FileContentsToString(System.String file, Encoding encoding)
        {
            System.String contents = "";

            System.IO.FileInfo f = null;
            try
            {
                f = new System.IO.FileInfo(file);

                bool tmpBool;
                if (System.IO.File.Exists(f.FullName))
                    tmpBool = true;
                else
                    tmpBool = System.IO.Directory.Exists(f.FullName);
                if (tmpBool)
                {
                    System.IO.StreamReader fr = null;
                    try
                    {
                        fr = new System.IO.StreamReader(f.FullName, encoding);
                        char[] template = new char[(int)SupportClass.FileLength(f)];
                        fr.Read((System.Char[])template, 0, template.Length);
                        contents = new System.String(template);
                    }
                    catch (System.Exception e)
                    {
                        SupportClass.WriteStackTrace(e, Console.Error);
                    }
                    finally
                    {
                        if (fr != null)
                        {
                            fr.Close();
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                SupportClass.WriteStackTrace(e, Console.Error);
            }
            return contents;
        }


        /// <summary> Trim all strings in a List.  Changes the strings in the existing list.</summary>
        /// <param name="list">
        /// </param>
        /// <returns> List of trimmed strings.
        /// </returns>
        /// <since> 1.5
        /// </since>
        public static IList<string> TrimStrings(IList<string> list)
        {
            if (list == null)
                return null;

            int sz = list.Count;
            for (int i = 0; i < sz; i++)
                list[i] = NullTrim((string)list[i]);
            return list;
        }

        public static ArrayList TrimStrings(ArrayList list)
        {
            if (list == null)
                return null;

            int sz = list.Count;
            for (int i = 0; i < sz; i++)
                list[i] = NullTrim((string)list[i]);
            return list;
        }

        /// <summary> Trim the string, but pass a null through.</summary>
        /// <param name="s">
        /// </param>
        /// <returns> List of trimmed Strings.
        /// </returns>
        /// <since> 1.5
        /// </since>
        public static string NullTrim(string s)
        {
            return s == null ? null : s.Trim();
        }
    }
}