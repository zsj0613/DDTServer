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
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    public class SupportClass
    {
        /// <summary> <p>Escapes and unescapes <code>String</code>s for
        /// Java, Java Script, HTML, XML, and SQL.</p>
        /// 
        /// </summary>
        /// <author>  Apache Jakarta Turbine
        /// </author>
        /// <author>  Purple Technology
        /// </author>
        /// <author>  <a href="mailto:alex@purpletech.com">Alexander Day Chaffee</a>
        /// </author>
        /// <author>  Antony Riley
        /// </author>
        /// <author>  Helge Tesgaard
        /// </author>
        /// <author>  <a href="sean@boohai.com">Sean Brown</a>
        /// </author>
        /// <author>  <a href="mailto:ggregory@seagullsw.com">Gary Gregory</a>
        /// </author>
        /// <author>  Phil Steitz
        /// </author>
        /// <author>  Pete Gieser
        /// </author>
        /// <since> 2.0
        /// </since>
        /// <version>  $Id: StringEscapeUtils.java 612880 2008-01-17 17:34:43Z ggregory $
        /// </version>
        public class StringEscapeUtils
        {
            /// <summary> <p>Escapes the characters in a <code>String</code> using JavaScript String rules.</p>
            /// <p>Escapes any values it finds into their JavaScript String form.
            /// Deals correctly with quotes and control-chars (tab, backslash, cr, ff, etc.) </p>
            /// 
            /// <p>So a tab becomes the characters <code>'\\'</code> and
            /// <code>'t'</code>.</p>
            /// 
            /// <p>The only difference between Java strings and JavaScript strings
            /// is that in JavaScript, a single quote must be escaped.</p>
            /// 
            /// <p>Example:
            /// <pre>
            /// input string: He didn't say, "Stop!"
            /// output string: He didn\'t say, \"Stop!\"
            /// </pre>
            /// </p>
            /// 
            /// </summary>
            /// <param name="str"> String to escape values in, may be null
            /// </param>
            /// <returns> String with escaped values, <code>null</code> if null string input
            /// </returns>
            public static string EscapeJavaScript(string str)
            {
                return escapeJavaStyleString(str, true);
            }

            /// <summary> <p>Worker method for the {@link #EscapeJavaScript(String)} method.</p>
            /// 
            /// </summary>
            /// <param name="str">String to escape values in, may be null
            /// </param>
            /// <param name="escapeSingleQuotes">escapes single quotes if <code>true</code>
            /// </param>
            /// <returns> the escaped string
            /// </returns>
            private static string escapeJavaStyleString(string str, bool escapeSingleQuotes)
            {
                if (str == null)
                {
                    return null;
                }
                try
                {
                    System.IO.StringWriter writer = new System.IO.StringWriter();
                    escapeJavaStyleString(writer, str, escapeSingleQuotes);
                    return writer.ToString();
                }
                catch (System.IO.IOException ioe)
                {
                    // this should never ever happen while writing to a StringWriter
                    SupportClass.WriteStackTrace(ioe, Console.Error);
                    return null;
                }
            }

            /// <summary> <p>Worker method for the {@link #EscapeJavaScript(String)} method.</p>
            /// 
            /// </summary>
            /// <param name="out">write to receieve the escaped string
            /// </param>
            /// <param name="str">String to escape values in, may be null
            /// </param>
            /// <param name="escapeSingleQuote">escapes single quotes if <code>true</code>
            /// </param>
            /// <throws>  IOException if an IOException occurs </throws>
            private static void escapeJavaStyleString(System.IO.TextWriter out_Renamed, string str, bool escapeSingleQuote)
            {
                if (out_Renamed == null)
                {
                    throw new System.ArgumentException("The Writer must not be null");
                }
                if (str == null)
                {
                    return;
                }
                int sz;
                sz = str.Length;
                for (int i = 0; i < sz; i++)
                {
                    char ch = str[i];

                    // handle unicode
                    if (ch > 0xfff)
                    {
                        out_Renamed.Write("\\u" + hex(ch));
                    }
                    else if (ch > 0xff)
                    {
                        out_Renamed.Write("\\u0" + hex(ch));
                    }
                    else if (ch > 0x7f)
                    {
                        out_Renamed.Write("\\u00" + hex(ch));
                    }
                    else if (ch < 32)
                    {
                        switch (ch)
                        {

                            case '\b':
                                out_Renamed.Write('\\');
                                out_Renamed.Write('b');
                                break;

                            case '\n':
                                out_Renamed.Write('\\');
                                out_Renamed.Write('n');
                                break;

                            case '\t':
                                out_Renamed.Write('\\');
                                out_Renamed.Write('t');
                                break;

                            case '\f':
                                out_Renamed.Write('\\');
                                out_Renamed.Write('f');
                                break;

                            case '\r':
                                out_Renamed.Write('\\');
                                out_Renamed.Write('r');
                                break;

                            default:
                                if (ch > 0xf)
                                {
                                    out_Renamed.Write("\\u00" + hex(ch));
                                }
                                else
                                {
                                    out_Renamed.Write("\\u000" + hex(ch));
                                }
                                break;

                        }
                    }
                    else
                    {
                        switch (ch)
                        {

                            case '\'':
                                if (escapeSingleQuote)
                                {
                                    out_Renamed.Write('\\');
                                }
                                out_Renamed.Write('\'');
                                break;

                            case '"':
                                out_Renamed.Write('\\');
                                out_Renamed.Write('"');
                                break;

                            case '\\':
                                out_Renamed.Write('\\');
                                out_Renamed.Write('\\');
                                break;

                            case '/':
                                out_Renamed.Write('\\');
                                out_Renamed.Write('/');
                                break;

                            default:
                                out_Renamed.Write(ch);
                                break;

                        }
                    }
                }
            }

            /// <summary> <p>Returns an upper case hexadecimal <code>String</code> for the given
            /// character.</p>
            /// 
            /// </summary>
            /// <param name="ch">The character to convert.
            /// </param>
            /// <returns> An upper case hexadecimal <code>String</code>
            /// </returns>
            private static string hex(char ch)
            {
                return System.Convert.ToString(ch, 16).ToUpper();
            }

           

            // HTML and XML
            //--------------------------------------------------------------------------
            /// <summary> <p>Escapes the characters in a <code>String</code> using HTML entities.</p>
            /// 
            /// <p>
            /// For example:
            /// </p> 
            /// <p><code>"bread" & "butter"</code></p>
            /// becomes:
            /// <p>
            /// <code>&amp;quot;bread&amp;quot; &amp;amp; &amp;quot;butter&amp;quot;</code>.
            /// </p>
            /// 
            /// <p>Supports all known HTML 4.0 entities, including funky accents.
            /// Note that the commonly used apostrophe escape character (&amp;apos;)
            /// is not a legal entity and so is not supported). </p>
            /// 
            /// </summary>
            /// <param name="str"> the <code>String</code> to escape, may be null
            /// </param>
            /// <returns> a new escaped <code>String</code>, <code>null</code> if null string input
            /// 
            /// </returns>
            /// <seealso cref="unescapeHtml(String)">
            /// </seealso>
            /// <seealso cref="<a href="http://hotwired.lycos.com/webmonkey/reference/special_characters/">ISO Entities</a>">
            /// </seealso>
            /// <seealso cref="<a href="http://www.w3.org/TR/REC-html32.latin1">HTML 3.2 Character Entities for ISO Latin-1</a>">
            /// </seealso>
            /// <seealso cref="<a href="http://www.w3.org/TR/REC-html40/sgml/entities.html">HTML 4.0 Character entity references</a>">
            /// </seealso>
            /// <seealso cref="<a href="http://www.w3.org/TR/html401/charset.html.h-5.3">HTML 4.01 Character References</a>">
            /// </seealso>
            /// <seealso cref="<a href="http://www.w3.org/TR/html401/charset.html.code-position">HTML 4.01 Code positions</a>">
            /// </seealso>
            public static string escapeHtml(string str)
            {
                if (str == null)
                {
                    return null;
                }
                try
                {
                    System.IO.StringWriter writer = new System.IO.StringWriter();
                    escapeHtml(writer, str);
                    return writer.ToString();
                }
                catch (System.IO.IOException e)
                {
                    //assert false;
                    //should be impossible
                    SupportClass.WriteStackTrace(e, Console.Error);
                    return null;
                }
            }

            /// <summary> <p>Escapes the characters in a <code>String</code> using HTML entities and writes
            /// them to a <code>Writer</code>.</p>
            /// 
            /// <p>
            /// For example:
            /// </p> 
            /// <code>"bread" & "butter"</code>
            /// <p>becomes:</p>
            /// <code>&amp;quot;bread&amp;quot; &amp;amp; &amp;quot;butter&amp;quot;</code>.
            /// 
            /// <p>Supports all known HTML 4.0 entities, including funky accents.
            /// Note that the commonly used apostrophe escape character (&amp;apos;)
            /// is not a legal entity and so is not supported). </p>
            /// 
            /// </summary>
            /// <param name="writer"> the writer receiving the escaped string, not null
            /// </param>
            /// <param name="string"> the <code>String</code> to escape, may be null
            /// </param>
            /// <throws>  IllegalArgumentException if the writer is null </throws>
            /// <throws>  IOException when <code>Writer</code> passed throws the exception from </throws>
            /// <summary>                                       calls to the {@link Writer#write(int)} methods.
            /// 
            /// </summary>
            /// <seealso cref="escapeHtml(String)">
            /// </seealso>
            /// <seealso cref="unescapeHtml(String)">
            /// </seealso>
            /// <seealso cref="<a href="http://hotwired.lycos.com/webmonkey/reference/special_characters/">ISO Entities</a>">
            /// </seealso>
            /// <seealso cref="<a href="http://www.w3.org/TR/REC-html32.latin1">HTML 3.2 Character Entities for ISO Latin-1</a>">
            /// </seealso>
            /// <seealso cref="<a href="http://www.w3.org/TR/REC-html40/sgml/entities.html">HTML 4.0 Character entity references</a>">
            /// </seealso>
            /// <seealso cref="<a href="http://www.w3.org/TR/html401/charset.html.h-5.3">HTML 4.01 Character References</a>">
            /// </seealso>
            /// <seealso cref="<a href="http://www.w3.org/TR/html401/charset.html.code-position">HTML 4.01 Code positions</a>">
            /// </seealso>
            public static void escapeHtml(System.IO.TextWriter writer, string string_Renamed)
            {
                if (writer == null)
                {
                    throw new System.ArgumentException("The Writer must not be null.");
                }
                if (string_Renamed == null)
                {
                    return;
                }
                Entities.HTML40.escape(writer, string_Renamed);
            }


            /// <summary> <p>Escapes the characters in a <code>String</code> using XML entities.</p>
            /// 
            /// <p>For example: <tt>"bread" & "butter"</tt> =>
            /// <tt>&amp;quot;bread&amp;quot; &amp;amp; &amp;quot;butter&amp;quot;</tt>.
            /// </p>
            /// 
            /// <p>Supports only the five basic XML entities (gt, lt, quot, amp, apos).
            /// Does not support DTDs or external entities.</p>
            /// 
            /// <p>Note that unicode characters greater than 0x7f are currently escaped to 
            /// their numerical \\u equivalent. This may change in future releases. </p>
            /// 
            /// </summary>
            /// <param name="str"> the <code>String</code> to escape, may be null
            /// </param>
            /// <returns> a new escaped <code>String</code>, <code>null</code> if null string input
            /// </returns>
            /// <seealso cref="unescapeXml(java.lang.String)">
            /// </seealso>
            public static string EscapeXml(string str)
            {
                if (str == null)
                {
                    return null;
                }
                return Entities.XML.escape(str);
            }

            //-----------------------------------------------------------------------
            /// <summary> <p>Escapes the characters in a <code>String</code> to be suitable to pass to
            /// an SQL query.</p>
            /// 
            /// <p>For example,
            /// <pre>statement.executeQuery("SELECT * FROM MOVIES WHERE TITLE='" + 
            /// StringEscapeUtils.EscapeSql("McHale's Navy") + 
            /// "'");</pre>
            /// </p>
            /// 
            /// <p>At present, this method only turns single-quotes into doubled single-quotes
            /// (<code>"McHale's Navy"</code> => <code>"McHale''s Navy"</code>). It does not
            /// handle the cases of percent (%) or underscore (_) for use in LIKE clauses.</p>
            /// 
            /// see http://www.jguru.com/faq/view.jsp?EID=8881
            /// </summary>
            /// <param name="str"> the string to escape, may be null
            /// </param>
            /// <returns> a new String, escaped for SQL, <code>null</code> if null string input
            /// </returns>
            public static string EscapeSql(string str)
            {
                if (str == null)
                {
                    return null;
                }
                return StringUtils.replace(str, "'", "''");
            }
        }

        /// <summary> <p>A hash map that uses primitive ints for the key rather than objects.</p>
        /// 
        /// <p>Note that this class is for internal optimization purposes only, and may
        /// not be supported in future releases of Apache Commons Lang.  Utilities of
        /// this sort may be included in future releases of Apache Commons Collections.</p>
        /// 
        /// </summary>
        /// <author>  Justin Couch
        /// </author>
        /// <author>  Alex Chaffee (alex@apache.org)
        /// </author>
        /// <author>  Stephen Colebourne
        /// </author>
        /// <since> 2.0
        /// </since>
        /// <version>  $Revision: 561230 $
        /// </version>
        /// <seealso cref="java.util.HashMap">
        /// </seealso>
        class IntHashMap
        {
            /// <summary> <p>Tests if this hashtable maps no keys to values.</p>
            /// 
            /// </summary>
            /// <returns>  <code>true</code> if this hashtable maps no keys to values;
            /// <code>false</code> otherwise.
            /// </returns>
            virtual public bool Empty
            {
                get
                {
                    return count == 0;
                }

            }

            /// <summary> The hash table data.</summary>
            [NonSerialized]
            private Entry[] table;

            /// <summary> The total number of entries in the hash table.</summary>
            [NonSerialized]
            private int count;

            /// <summary> The table is rehashed when its size exceeds this threshold.  (The
            /// value of this field is (int)(capacity * loadFactor).)
            /// 
            /// </summary>
            /// <serial>
            /// </serial>
            private int threshold;

            /// <summary> The load factor for the hashtable.
            /// 
            /// </summary>
            /// <serial>
            /// </serial>
            private float loadFactor;

            /// <summary> <p>Innerclass that acts as a datastructure to create a new entry in the
            /// table.</p>
            /// </summary>
            private class Entry
            {
                internal int hash;
                internal int key;
                internal object value;
                internal Entry next;

                /// <summary> <p>Create a new entry with the given values.</p>
                /// 
                /// </summary>
                /// <param name="hash">The code used to hash the object with
                /// </param>
                /// <param name="key">The key used to enter this in the table
                /// </param>
                /// <param name="value">The value for this key
                /// </param>
                /// <param name="next">A reference to the next entry in the table
                /// </param>
                protected internal Entry(int hash, int key, object value, Entry next)
                {
                    this.hash = hash;
                    this.key = key;
                    this.value = value;
                    this.next = next;
                }
            }

            /// <summary> <p>Constructs a new, empty hashtable with a default capacity and load
            /// factor, which is <code>20</code> and <code>0.75</code> respectively.</p>
            /// </summary>
            public IntHashMap()
                : this(20, 0.75f)
            {
            }

            /// <summary> <p>Constructs a new, empty hashtable with the specified initial capacity
            /// and default load factor, which is <code>0.75</code>.</p>
            /// 
            /// </summary>
            /// <param name="initialCapacity">the initial capacity of the hashtable.
            /// </param>
            /// <throws>  IllegalArgumentException if the initial capacity is less </throws>
            /// <summary>   than zero.
            /// </summary>
            public IntHashMap(int initialCapacity)
                : this(initialCapacity, 0.75f)
            {
            }

            /// <summary> <p>Constructs a new, empty hashtable with the specified initial
            /// capacity and the specified load factor.</p>
            /// 
            /// </summary>
            /// <param name="initialCapacity">the initial capacity of the hashtable.
            /// </param>
            /// <param name="loadFactor">the load factor of the hashtable.
            /// </param>
            /// <throws>  IllegalArgumentException  if the initial capacity is less </throws>
            /// <summary>             than zero, or if the load factor is nonpositive.
            /// </summary>
            public IntHashMap(int initialCapacity, float loadFactor)
                : base()
            {
                if (initialCapacity < 0)
                {
                    throw new System.ArgumentException("Illegal Capacity: " + initialCapacity);
                }
                if (loadFactor <= 0)
                {
                    throw new System.ArgumentException("Illegal Load: " + loadFactor);
                }
                if (initialCapacity == 0)
                {
                    initialCapacity = 1;
                }

                this.loadFactor = loadFactor;
                table = new Entry[initialCapacity];
                //UPGRADE_WARNING: 在 Visual C# 中，数据类型可能不同。请验证收缩转换的正确性。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                threshold = (int)(initialCapacity * loadFactor);
            }

            /// <summary> <p>Returns the number of keys in this hashtable.</p>
            /// 
            /// </summary>
            /// <returns>  the number of keys in this hashtable.
            /// </returns>
            public virtual int size()
            {
                return count;
            }

            /// <summary> <p>Tests if some key maps into the specified value in this hashtable.
            /// This operation is more expensive than the <code>containsKey</code>
            /// method.</p>
            /// 
            /// <p>Note that this method is identical in functionality to containsValue,
            /// (which is part of the Map interface in the collections framework).</p>
            /// 
            /// </summary>
            /// <param name="value">  a value to search for.
            /// </param>
            /// <returns>     <code>true</code> if and only if some key maps to the
            /// <code>value</code> argument in this hashtable as
            /// determined by the <tt>equals</tt> method;
            /// <code>false</code> otherwise.
            /// </returns>
            /// <throws>   NullPointerException  if the value is <code>null</code>. </throws>
            /// <seealso cref="containsKey(int)">
            /// </seealso>
            /// <seealso cref="containsValue(Object)">
            /// </seealso>
            /// <seealso cref="java.util.Map">
            /// </seealso>
            public virtual bool contains(object value)
            {
                if (value == null)
                {
                    throw new System.NullReferenceException();
                }

                Entry[] tab = table;
                for (int i = tab.Length; i-- > 0; )
                {
                    for (Entry e = tab[i]; e != null; e = e.next)
                    {
                        if (e.value.Equals(value))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            /// <summary> <p>Returns <code>true</code> if this HashMap maps one or more keys
            /// to this value.</p>
            /// 
            /// <p>Note that this method is identical in functionality to contains
            /// (which predates the Map interface).</p>
            /// 
            /// </summary>
            /// <param name="value">value whose presence in this HashMap is to be tested.
            /// </param>
            /// <returns> boolean <code>true</code> if the value is contained
            /// </returns>
            /// <seealso cref="java.util.Map">
            /// </seealso>
            /// <since> JDK1.2
            /// </since>
            public virtual bool containsValue(object value)
            {
                return contains(value);
            }

            /// <summary> <p>Tests if the specified object is a key in this hashtable.</p>
            /// 
            /// </summary>
            /// <param name="key"> possible key.
            /// </param>
            /// <returns> <code>true</code> if and only if the specified object is a
            /// key in this hashtable, as determined by the <tt>equals</tt>
            /// method; <code>false</code> otherwise.
            /// </returns>
            /// <seealso cref="contains(Object)">
            /// </seealso>
            public virtual bool containsKey(int key)
            {
                Entry[] tab = table;
                int hash = key;
                int index = (hash & 0x7FFFFFFF) % tab.Length;
                for (Entry e = tab[index]; e != null; e = e.next)
                {
                    if (e.hash == hash)
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary> <p>Returns the value to which the specified key is mapped in this map.</p>
            /// 
            /// </summary>
            /// <param name="key">  a key in the hashtable.
            /// </param>
            /// <returns>  the value to which the key is mapped in this hashtable;
            /// <code>null</code> if the key is not mapped to any value in
            /// this hashtable.
            /// </returns>
            /// <seealso cref="Put(int, Object)">
            /// </seealso>
            public virtual object get_Renamed(int key)
            {
                Entry[] tab = table;
                int hash = key;
                int index = (hash & 0x7FFFFFFF) % tab.Length;
                for (Entry e = tab[index]; e != null; e = e.next)
                {
                    if (e.hash == hash)
                    {
                        return e.value;
                    }
                }
                return null;
            }

            /// <summary> <p>Increases the capacity of and internally reorganizes this
            /// hashtable, in order to accommodate and access its entries more
            /// efficiently.</p>
            /// 
            /// <p>This method is called automatically when the number of keys
            /// in the hashtable exceeds this hashtable's capacity and load
            /// factor.</p>
            /// </summary>
            protected internal virtual void rehash()
            {
                int oldCapacity = table.Length;
                Entry[] oldMap = table;

                int newCapacity = oldCapacity * 2 + 1;
                Entry[] newMap = new Entry[newCapacity];

                //UPGRADE_WARNING: 在 Visual C# 中，数据类型可能不同。请验证收缩转换的正确性。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                threshold = (int)(newCapacity * loadFactor);
                table = newMap;

                for (int i = oldCapacity; i-- > 0; )
                {
                    for (Entry old = oldMap[i]; old != null; )
                    {
                        Entry e = old;
                        old = old.next;

                        int index = (e.hash & 0x7FFFFFFF) % newCapacity;
                        e.next = newMap[index];
                        newMap[index] = e;
                    }
                }
            }

            /// <summary> <p>Maps the specified <code>key</code> to the specified
            /// <code>value</code> in this hashtable. The key cannot be
            /// <code>null</code>. </p>
            /// 
            /// <p>The value can be retrieved by calling the <code>Get</code> method
            /// with a key that is equal to the original key.</p>
            /// 
            /// </summary>
            /// <param name="key">    the hashtable key.
            /// </param>
            /// <param name="value">  the value.
            /// </param>
            /// <returns> the previous value of the specified key in this hashtable,
            /// or <code>null</code> if it did not have one.
            /// </returns>
            /// <throws>   NullPointerException  if the key is <code>null</code>. </throws>
            /// <seealso cref="Get(int)">
            /// </seealso>
            public virtual object put(int key, object value)
            {
                // Makes sure the key is not already in the hashtable.
                Entry[] tab = table;
                int hash = key;
                int index = (hash & 0x7FFFFFFF) % tab.Length;
                for (Entry e = tab[index]; e != null; e = e.next)
                {
                    if (e.hash == hash)
                    {
                        object old = e.value;
                        e.value = value;
                        return old;
                    }
                }

                if (count >= threshold)
                {
                    // Rehash the table if the threshold is exceeded
                    rehash();

                    tab = table;
                    index = (hash & 0x7FFFFFFF) % tab.Length;
                }

                // Creates the new entry.
                Entry e2 = new Entry(hash, key, value, tab[index]);
                tab[index] = e2;
                count++;
                return null;
            }

            /// <summary> <p>Removes the key (and its corresponding value) from this
            /// hashtable.</p>
            /// 
            /// <p>This method does nothing if the key is not present in the
            /// hashtable.</p>
            /// 
            /// </summary>
            /// <param name="key">  the key that needs to be removed.
            /// </param>
            /// <returns>  the value to which the key had been mapped in this hashtable,
            /// or <code>null</code> if the key did not have a mapping.
            /// </returns>
            public virtual object remove(int key)
            {
                Entry[] tab = table;
                int hash = key;
                int index = (hash & 0x7FFFFFFF) % tab.Length;
                for (Entry e = tab[index], prev = null; e != null; prev = e, e = e.next)
                {
                    if (e.hash == hash)
                    {
                        if (prev != null)
                        {
                            prev.next = e.next;
                        }
                        else
                        {
                            tab[index] = e.next;
                        }
                        count--;
                        object oldValue = e.value;
                        e.value = null;
                        return oldValue;
                    }
                }
                return null;
            }

            /// <summary> <p>Clears this hashtable so that it contains no keys.</p></summary>
            //UPGRADE_NOTE: Synchronized 关键字已从方法“clear”中移除。添加了锁定表达式。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1027'"
            public virtual void clear()
            {
                lock (this)
                {
                    Entry[] tab = table;
                    for (int index = tab.Length; --index >= 0; )
                    {
                        tab[index] = null;
                    }
                    count = 0;
                }
            }
        }

        /// <summary> <p>
        /// Provides HTML and XML entity utilities.
        /// </p>
        /// 
        /// </summary>
        /// <seealso cref="<a href="http://hotwired.lycos.com/webmonkey/reference/special_characters/">ISO Entities</a>">
        /// </seealso>
        /// <seealso cref="<a href="http://www.w3.org/TR/REC-html32.latin1">HTML 3.2 Character Entities for ISO Latin-1</a>">
        /// </seealso>
        /// <seealso cref="<a href="http://www.w3.org/TR/REC-html40/sgml/entities.html">HTML 4.0 Character entity references</a>">
        /// </seealso>
        /// <seealso cref="<a href="http://www.w3.org/TR/html401/charset.html.h-5.3">HTML 4.01 Character References</a>">
        /// </seealso>
        /// <seealso cref="<a href="http://www.w3.org/TR/html401/charset.html.code-position">HTML 4.01 Code positions</a>">
        /// 
        /// </seealso>
        /// <author>  <a href="mailto:alex@purpletech.com">Alexander Day Chaffee</a>
        /// </author>
        /// <author>  <a href="mailto:ggregory@seagullsw.com">Gary Gregory</a>
        /// </author>
        /// <since> 2.0
        /// </since>
        /// <version>  $Id: Entities.java 636641 2008-03-13 06:11:30Z bayard $
        /// </version>
        class Entities
        {

            //UPGRADE_NOTE: Final 已从“BASIC_ARRAY”的声明中移除。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
            private static readonly string[][] BASIC_ARRAY = new string[][] { new string[] { "quot", "34" }, new string[] { "amp", "38" }, new string[] { "lt", "60" }, new string[] { "gt", "62" } };

            //UPGRADE_NOTE: Final 已从“APOS_ARRAY”的声明中移除。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
            private static readonly string[][] APOS_ARRAY = new string[][] { new string[] { "apos", "39" } };

            // package scoped for testing
            //UPGRADE_NOTE: Final 已从“ISO8859_1_ARRAY”的声明中移除。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
            internal static readonly string[][] ISO8859_1_ARRAY = new string[][]{new string[]{"nbsp", "160"}, new string[]{"iexcl", "161"}, new string[]{"cent", "162"}, new string[]{"pound", "163"}, new string[]{"curren", "164"}, new string[]{"yen", "165"}, new string[]{"brvbar", "166"}, new string[]{"sect", "167"}, new string[]{"uml", "168"}, new string[]{"copy", "169"}, new string[]{"ordf", "170"}, new string[]{"laquo", "171"}, new string[]{"not", "172"}, new string[]{"shy", "173"}, new string[]{"reg", "174"}, new string[]{"macr", "175"}, new string[]{"deg", "176"}, new string[]{"plusmn", "177"}, new string[]{"sup2", "178"}, new string[]{"sup3", "179"}, new string[]{"acute", "180"}, new string[]{"micro", "181"}, new string[]{"para", "182"}, new string[]{"middot", "183"}, new string[]{"cedil", "184"}, new string[]{"sup1", "185"}, new string[]{"ordm", "186"}, new string[]{"raquo", "187"}, new string[]{"frac14", "188"}, new string[]{"frac12", "189"}, new string[]{"frac34", "190"}, new string[]{"iquest", "191"}, new string[]{"Agrave", "192"}, new string[]{"Aacute", "193"}, new string[]{"Acirc", "194"}, new string[]{"Atilde", "195"}, new string[]{"Auml", "196"}, new string[]{"Aring", "197"}, new string[]{"AElig", "198"}, new string[]{"Ccedil", "199"}, new string[]{"Egrave", "200"}, new string[]{"Eacute", "201"}, new string[]{"Ecirc", "202"}, new string[]{"Euml", "203"}, new string[]{"Igrave", "204"}, new string[]{"Iacute", "205"}, new string[]{"Icirc", "206"}, new string[]{"Iuml", "207"}, new string[]{"ETH", "208"}, new string[]{"Ntilde", "209"}, new string[]{"Ograve", "210"}, new string[]{"Oacute", "211"}, new string[]{"Ocirc", "212"}, 
			new string[]{"Otilde", "213"}, new string[]{"Ouml", "214"}, new string[]{"times", "215"}, new string[]{"Oslash", "216"}, new string[]{"Ugrave", "217"}, new string[]{"Uacute", "218"}, new string[]{"Ucirc", "219"}, new string[]{"Uuml", "220"}, new string[]{"Yacute", "221"}, new string[]{"THORN", "222"}, new string[]{"szlig", "223"}, new string[]{"agrave", "224"}, new string[]{"aacute", "225"}, new string[]{"acirc", "226"}, new string[]{"atilde", "227"}, new string[]{"auml", "228"}, new string[]{"aring", "229"}, new string[]{"aelig", "230"}, new string[]{"ccedil", "231"}, new string[]{"egrave", "232"}, new string[]{"eacute", "233"}, new string[]{"ecirc", "234"}, new string[]{"euml", "235"}, new string[]{"igrave", "236"}, new string[]{"iacute", "237"}, new string[]{"icirc", "238"}, new string[]{"iuml", "239"}, new string[]{"eth", "240"}, new string[]{"ntilde", "241"}, new string[]{"ograve", "242"}, new string[]{"oacute", "243"}, new string[]{"ocirc", "244"}, new string[]{"otilde", "245"}, new string[]{"ouml", "246"}, new string[]{"Divide", "247"}, new string[]{"oslash", "248"}, new string[]{"ugrave", "249"}, new string[]{"uacute", "250"}, new string[]{"ucirc", "251"}, new string[]{"uuml", "252"}, new string[]{"yacute", "253"}, new string[]{"thorn", "254"}, new string[]{"yuml", "255"}};

            // http://www.w3.org/TR/REC-html40/sgml/entities.html
            // package scoped for testing
            //UPGRADE_NOTE: Final 已从“HTML40_ARRAY”的声明中移除。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
            internal static readonly string[][] HTML40_ARRAY = new string[][]{new string[]{"fnof", "402"}, new string[]{"Alpha", "913"}, new string[]{"Beta", "914"}, new string[]{"Gamma", "915"}, new string[]{"Delta", "916"}, new string[]{"Epsilon", "917"}, new string[]{"Zeta", "918"}, new string[]{"Eta", "919"}, new string[]{"Theta", "920"}, new string[]{"Iota", "921"}, new string[]{"Kappa", "922"}, new string[]{"Lambda", "923"}, new string[]{"Mu", "924"}, new string[]{"Nu", "925"}, new string[]{"Xi", "926"}, new string[]{"Omicron", "927"}, new string[]{"Pi", "928"}, new string[]{"Rho", "929"}, new string[]{"Sigma", "931"}, new string[]{"Tau", "932"}, new string[]{"Upsilon", "933"}, new string[]{"Phi", "934"}, new string[]{"Chi", "935"}, new string[]{"Psi", "936"}, new string[]{"Omega", "937"}, new string[]{"alpha", "945"}, new string[]{"beta", "946"}, new string[]{"gamma", "947"}, new string[]{"delta", "948"}, new string[]{"epsilon", "949"}, new string[]{"zeta", "950"}, new string[]{"eta", "951"}, new string[]{"theta", "952"}, new string[]{"iota", "953"}, new string[]{"kappa", "954"}, new string[]{"lambda", "955"}, new string[]{"mu", "956"}, new string[]{"nu", "957"}, new string[]{"xi", "958"}, new string[]{"omicron", "959"}, new string[]{"pi", "960"}, new string[]{"rho", "961"}, new string[]{"sigmaf", "962"}, new string[]{"sigma", "963"}, new string[]{"tau", "964"}, new string[]{"upsilon", "965"}, new string[]{"phi", "966"}, new string[]{"chi", "967"}, new string[]{"psi", "968"}, new string[]{"omega", "969"}, new string[]{"thetasym", "977"}, new string[]{"upsih", "978"}, new string[]{"piv", "982"}, new string[]{"bull", "8226"
			}, new string[]{"hellip", "8230"}, new string[]{"prime", "8242"}, new string[]{"Prime", "8243"}, new string[]{"oline", "8254"}, new string[]{"frasl", "8260"}, new string[]{"weierp", "8472"}, new string[]{"image", "8465"}, new string[]{"real", "8476"}, new string[]{"trade", "8482"}, new string[]{"alefsym", "8501"}, new string[]{"larr", "8592"}, new string[]{"uarr", "8593"}, new string[]{"rarr", "8594"}, new string[]{"darr", "8595"}, new string[]{"harr", "8596"}, new string[]{"crarr", "8629"}, new string[]{"lArr", "8656"}, new string[]{"uArr", "8657"}, new string[]{"rArr", "8658"}, new string[]{"dArr", "8659"}, new string[]{"hArr", "8660"}, new string[]{"forall", "8704"}, new string[]{"part", "8706"}, new string[]{"exist", "8707"}, new string[]{"empty", "8709"}, new string[]{"nabla", "8711"}, new string[]{"isin", "8712"}, new string[]{"notin", "8713"}, new string[]{"ni", "8715"}, new string[]{"prod", "8719"}, new string[]{"sum", "8721"}, new string[]{"minus", "8722"}, new string[]{"lowast", "8727"}, new string[]{"radic", "8730"}, new string[]{"prop", "8733"}, new string[]{"infin", "8734"}, new string[]{"ang", "8736"}, new string[]{"and", "8743"}, new string[]{"or", "8744"}, new string[]{"cap", "8745"}, new string[]{"cup", "8746"}, new string[]{"int", "8747"}, new string[]{"there4", "8756"}, new string[]{"sim", "8764"}, new string[]{"cong", "8773"}, new string[]{"asymp", "8776"}, new string[]{"ne", "8800"}, new string[]{"equiv", "8801"}, new string[]{"le", "8804"}, new string[]{"ge", "8805"}, new string[]{"sub", "8834"}, new string[]{"sup", "8835"}, new string[]{"sube", "8838"}, new string[]{"supe", "8839"}, new string[]{
			"oplus", "8853"}, new string[]{"otimes", "8855"}, new string[]{"perp", "8869"}, new string[]{"sdot", "8901"}, new string[]{"lceil", "8968"}, new string[]{"rceil", "8969"}, new string[]{"lfloor", "8970"}, new string[]{"rfloor", "8971"}, new string[]{"lang", "9001"}, new string[]{"rang", "9002"}, new string[]{"loz", "9674"}, new string[]{"spades", "9824"}, new string[]{"clubs", "9827"}, new string[]{"hearts", "9829"}, new string[]{"diams", "9830"}, new string[]{"OElig", "338"}, new string[]{"oelig", "339"}, new string[]{"Scaron", "352"}, new string[]{"scaron", "353"}, new string[]{"Yuml", "376"}, new string[]{"circ", "710"}, new string[]{"tilde", "732"}, new string[]{"ensp", "8194"}, new string[]{"emsp", "8195"}, new string[]{"thinsp", "8201"}, new string[]{"zwnj", "8204"}, new string[]{"zwj", "8205"}, new string[]{"lrm", "8206"}, new string[]{"rlm", "8207"}, new string[]{"ndash", "8211"}, new string[]{"mdash", "8212"}, new string[]{"lsquo", "8216"}, new string[]{"rsquo", "8217"}, new string[]{"sbquo", "8218"}, new string[]{"ldquo", "8220"}, new string[]{"rdquo", "8221"}, new string[]{"bdquo", "8222"}, new string[]{"dagger", "8224"}, new string[]{"Dagger", "8225"}, new string[]{"permil", "8240"}, new string[]{"lsaquo", "8249"}, new string[]{"rsaquo", "8250"}, new string[]{"euro", "8364"}};

            /// <summary> <p>
            /// The set of entities supported by standard XML.
            /// </p>
            /// </summary>
            public static Entities XML;

            /// <summary> <p>
            /// The set of entities supported by HTML 3.2.
            /// </p>
            /// </summary>
            public static Entities HTML32;

            /// <summary> <p>
            /// The set of entities supported by HTML 4.0.
            /// </p>
            /// </summary>
            public static Entities HTML40;

            /// <summary> <p>
            /// Fills the specified entities instance with HTML 40 entities.
            /// </p>
            /// 
            /// </summary>
            /// <param name="entities">the instance to be filled.
            /// </param>
            internal static void fillWithHtml40Entities(Entities entities)
            {
                entities.addEntities(BASIC_ARRAY);
                entities.addEntities(ISO8859_1_ARRAY);
                entities.addEntities(HTML40_ARRAY);
            }

            internal interface EntityMap
            {
                /// <summary> <p>
                /// Add an entry to this entity map.
                /// </p>
                /// 
                /// </summary>
                /// <param name="name">the entity name
                /// </param>
                /// <param name="value">the entity value
                /// </param>
                void add(string name, int value);

                /// <summary> <p>
                /// Returns the name of the entity identified by the specified value.
                /// </p>
                /// 
                /// </summary>
                /// <param name="value">the value to locate
                /// </param>
                /// <returns> entity name associated with the specified value
                /// </returns>
                string name(int value);

                /// <summary> <p>
                /// Returns the value of the entity identified by the specified name.
                /// </p>
                /// 
                /// </summary>
                /// <param name="name">the name to locate
                /// </param>
                /// <returns> entity value associated with the specified name
                /// </returns>
                int value(string name);
            }

            internal class PrimitiveEntityMap : Entities.EntityMap
            {
                //UPGRADE_TODO: Class“java.util.HashMap”被转换为具有不同行为的 'System.Collections.Hashtable'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMap'"
                private System.Collections.IDictionary mapNameToValue = new System.Collections.Hashtable();

                private IntHashMap mapValueToName = new IntHashMap();

                /// <summary> {@inheritDoc}</summary>
                public virtual void add(string name, int value)
                {
                    mapNameToValue[name] = (System.Int32)value;
                    mapValueToName.put(value, name);
                }

                /// <summary> {@inheritDoc}</summary>
                public virtual string name(int value)
                {
                    return (string)mapValueToName.get_Renamed(value);
                }

                /// <summary> {@inheritDoc}</summary>
                public virtual int value(string name)
                {
                    object value = mapNameToValue[name];
                    if (value == null)
                    {
                        return -1;
                    }
                    return ((System.Int32)value);
                }
            }

            internal abstract class MapIntMap : Entities.EntityMap
            {
                protected internal System.Collections.IDictionary mapNameToValue;

                protected internal System.Collections.IDictionary mapValueToName;

                /// <summary> {@inheritDoc}</summary>
                public virtual void add(string name, int value)
                {
                    mapNameToValue[name] = (System.Int32)value;
                    mapValueToName[(System.Int32)value] = name;
                }

                /// <summary> {@inheritDoc}</summary>
                public virtual string name(int value)
                {
                    return (string)mapValueToName[(System.Int32)value];
                }

                /// <summary> {@inheritDoc}</summary>
                public virtual int value(string name)
                {
                    object value = mapNameToValue[name];
                    if (value == null)
                    {
                        return -1;
                    }
                    return ((System.Int32)value);
                }
            }

            internal class HashEntityMap : MapIntMap
            {
                /// <summary> Constructs a new instance of <code>HashEntityMap</code>.</summary>
                public HashEntityMap()
                {
                    //UPGRADE_TODO: Class“java.util.HashMap”被转换为具有不同行为的 'System.Collections.Hashtable'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMap'"
                    mapNameToValue = new System.Collections.Hashtable();
                    //UPGRADE_TODO: Class“java.util.HashMap”被转换为具有不同行为的 'System.Collections.Hashtable'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMap'"
                    mapValueToName = new System.Collections.Hashtable();
                }
            }

            internal class TreeEntityMap : MapIntMap
            {
                /// <summary> Constructs a new instance of <code>TreeEntityMap</code>.</summary>
                public TreeEntityMap()
                {
                    //UPGRADE_TODO: 构造函数“java.util.TreeMap.TreeMap”被转换为具有不同行为的 'System.Collections.SortedList'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilTreeMapTreeMap'"
                    //UPGRADE_ISSUE: “java.util.TreeMap”和“System.Collections.SortedList”之间的类层次结构差异可能导致编译错误。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1186'"
                    mapNameToValue = new System.Collections.SortedList();
                    //UPGRADE_TODO: 构造函数“java.util.TreeMap.TreeMap”被转换为具有不同行为的 'System.Collections.SortedList'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilTreeMapTreeMap'"
                    //UPGRADE_ISSUE: “java.util.TreeMap”和“System.Collections.SortedList”之间的类层次结构差异可能导致编译错误。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1186'"
                    mapValueToName = new System.Collections.SortedList();
                }
            }

            internal class LookupEntityMap : PrimitiveEntityMap
            {
                private string[] lookupTable_Renamed_Field;

                private int LOOKUP_TABLE_SIZE = 256;

                /// <summary> {@inheritDoc}</summary>
                public override string name(int value)
                {
                    if (value < LOOKUP_TABLE_SIZE)
                    {
                        return lookupTable()[value];
                    }
                    return base.name(value);
                }

                /// <summary> <p>
                /// Returns the lookup table for this entity map. The lookup table is created if it has not been previously.
                /// </p>
                /// 
                /// </summary>
                /// <returns> the lookup table
                /// </returns>
                private string[] lookupTable()
                {
                    if (lookupTable_Renamed_Field == null)
                    {
                        createLookupTable();
                    }
                    return lookupTable_Renamed_Field;
                }

                /// <summary> <p>
                /// Creates an entity lookup table of LOOKUP_TABLE_SIZE elements, initialized with entity names.
                /// </p>
                /// </summary>
                private void createLookupTable()
                {
                    lookupTable_Renamed_Field = new string[LOOKUP_TABLE_SIZE];
                    for (int i = 0; i < LOOKUP_TABLE_SIZE; ++i)
                    {
                        lookupTable_Renamed_Field[i] = base.name(i);
                    }
                }
            }

            internal class ArrayEntityMap : Entities.EntityMap
            {
                protected internal int growBy = 100;

                protected internal int size = 0;

                protected internal string[] names;

                protected internal int[] values;

                /// <summary> Constructs a new instance of <code>ArrayEntityMap</code>.</summary>
                public ArrayEntityMap()
                {
                    names = new string[growBy];
                    values = new int[growBy];
                }

                /// <summary> Constructs a new instance of <code>ArrayEntityMap</code> specifying the size by which the array should
                /// grow.
                /// 
                /// </summary>
                /// <param name="growBy">array will be initialized to and will grow by this amount
                /// </param>
                public ArrayEntityMap(int growBy)
                {
                    this.growBy = growBy;
                    names = new string[growBy];
                    values = new int[growBy];
                }

                /// <summary> {@inheritDoc}</summary>
                public virtual void add(string name, int value)
                {
                    ensureCapacity(size + 1);
                    names[size] = name;
                    values[size] = value;
                    size++;
                }

                /// <summary> Verifies the capacity of the entity array, adjusting the size if necessary.
                /// 
                /// </summary>
                /// <param name="capacity">size the array should be
                /// </param>
                protected internal virtual void ensureCapacity(int capacity)
                {
                    if (capacity > names.Length)
                    {
                        int newSize = System.Math.Max(capacity, size + growBy);
                        string[] newNames = new string[newSize];
                        Array.Copy(names, 0, newNames, 0, size);
                        names = newNames;
                        int[] newValues = new int[newSize];
                        Array.Copy(values, 0, newValues, 0, size);
                        values = newValues;
                    }
                }

                /// <summary> {@inheritDoc}</summary>
                public virtual string name(int value)
                {
                    for (int i = 0; i < size; ++i)
                    {
                        if (values[i] == value)
                        {
                            return names[i];
                        }
                    }
                    return null;
                }

                /// <summary> {@inheritDoc}</summary>
                public virtual int value(string name)
                {
                    for (int i = 0; i < size; ++i)
                    {
                        if (names[i].Equals(name))
                        {
                            return values[i];
                        }
                    }
                    return -1;
                }
            }

            internal class BinaryEntityMap : ArrayEntityMap
            {

                /// <summary> Constructs a new instance of <code>BinaryEntityMap</code>.</summary>
                public BinaryEntityMap()
                    : base()
                {
                }

                /// <summary> Constructs a new instance of <code>ArrayEntityMap</code> specifying the size by which the underlying array
                /// should grow.
                /// 
                /// </summary>
                /// <param name="growBy">array will be initialized to and will grow by this amount
                /// </param>
                public BinaryEntityMap(int growBy)
                    : base(growBy)
                {
                }

                /// <summary> Performs a binary search of the entity array for the specified key. This method is based on code in
                /// {@link java.util.Arrays}.
                /// 
                /// </summary>
                /// <param name="key">the key to be found
                /// </param>
                /// <returns> the index of the entity array matching the specified key
                /// </returns>
                private int binarySearch(int key)
                {
                    int low = 0;
                    int high = size - 1;

                    while (low <= high)
                    {
                        int mid = SupportClass.URShift((low + high), 1);
                        int midVal = values[mid];

                        if (midVal < key)
                        {
                            low = mid + 1;
                        }
                        else if (midVal > key)
                        {
                            high = mid - 1;
                        }
                        else
                        {
                            return mid; // key found
                        }
                    }
                    return -(low + 1); // key not found.
                }

                /// <summary> {@inheritDoc}</summary>
                public override void add(string name, int value)
                {
                    ensureCapacity(size + 1);
                    int insertAt = binarySearch(value);
                    if (insertAt > 0)
                    {
                        return; // note: this means you can't insert the same value twice
                    }
                    insertAt = -(insertAt + 1); // binarySearch returns it negative and off-by-one
                    Array.Copy(values, insertAt, values, insertAt + 1, size - insertAt);
                    values[insertAt] = value;
                    Array.Copy(names, insertAt, names, insertAt + 1, size - insertAt);
                    names[insertAt] = name;
                    size++;
                }

                /// <summary> {@inheritDoc}</summary>
                public override string name(int value)
                {
                    int index = binarySearch(value);
                    if (index < 0)
                    {
                        return null;
                    }
                    return names[index];
                }
            }

            // package scoped for testing
            internal Entities.EntityMap map = new Entities.LookupEntityMap();

            /// <summary> <p>
            /// Adds entities to this entity.
            /// </p>
            /// 
            /// </summary>
            /// <param name="entityArray">array of entities to be added
            /// </param>
            public virtual void addEntities(string[][] entityArray)
            {
                for (int i = 0; i < entityArray.Length; ++i)
                {
                    addEntity(entityArray[i][0], System.Int32.Parse(entityArray[i][1]));
                }
            }

            /// <summary> <p>
            /// Add an entity to this entity.
            /// </p>
            /// 
            /// </summary>
            /// <param name="name">name of the entity
            /// </param>
            /// <param name="value">vale of the entity
            /// </param>
            public virtual void addEntity(string name, int value)
            {
                map.add(name, value);
            }

            /// <summary> <p>
            /// Returns the name of the entity identified by the specified value.
            /// </p>
            /// 
            /// </summary>
            /// <param name="value">the value to locate
            /// </param>
            /// <returns> entity name associated with the specified value
            /// </returns>
            public virtual string entityName(int value)
            {
                return map.name(value);
            }

            /// <summary> <p>
            /// Returns the value of the entity identified by the specified name.
            /// </p>
            /// 
            /// </summary>
            /// <param name="name">the name to locate
            /// </param>
            /// <returns> entity value associated with the specified name
            /// </returns>
            public virtual int entityValue(string name)
            {
                return map.value(name);
            }

            /// <summary> <p>
            /// Escapes the characters in a <code>String</code>.
            /// </p>
            /// 
            /// <p>
            /// For example, if you have called addEntity(&quot;foo&quot;, 0xA1), escape(&quot;\u00A1&quot;) will return
            /// &quot;&amp;foo;&quot;
            /// </p>
            /// 
            /// </summary>
            /// <param name="str">The <code>String</code> to escape.
            /// </param>
            /// <returns> A new escaped <code>String</code>.
            /// </returns>
            public virtual string escape(string str)
            {
                System.IO.StringWriter stringWriter = createStringWriter(str);
                try
                {
                    this.escape(stringWriter, str);
                }
                catch (System.IO.IOException e)
                {
                    // This should never happen because ALL the StringWriter methods called by #escape(Writer, String) do not
                    // throw IOExceptions.
                    throw e;
                }
                return stringWriter.ToString();
            }

            /// <summary> <p>
            /// Escapes the characters in the <code>String</code> passed and writes the result to the <code>Writer</code>
            /// passed.
            /// </p>
            /// 
            /// </summary>
            /// <param name="writer">The <code>Writer</code> to write the results of the escaping to. Assumed to be a non-null value.
            /// </param>
            /// <param name="str">The <code>String</code> to escape. Assumed to be a non-null value.
            /// </param>
            /// <throws>  IOException </throws>
            /// <summary>             when <code>Writer</code> passed throws the exception from calls to the {@link Writer#write(int)}
            /// methods.
            /// 
            /// </summary>
            /// <seealso cref="escape(String)">
            /// </seealso>
            /// <seealso cref="Writer">
            /// </seealso>
            //UPGRADE_ISSUE: “java.io.Writer”和“System.IO.StreamWriter”之间的类层次结构差异可能导致编译错误。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1186'"
            public virtual void escape(System.IO.TextWriter writer, string str)
            {
                int len = str.Length;
                for (int i = 0; i < len; i++)
                {
                    char c = str[i];
                    string entityName = this.entityName(c);
                    if (entityName == null)
                    {
                        if (c > 0x7F)
                        {
                            writer.Write("&#");
                            writer.Write(System.Convert.ToString(c, 10));
                            writer.Write(';');
                        }
                        else
                        {
                            writer.Write(c);
                        }
                    }
                    else
                    {
                        writer.Write('&');
                        writer.Write(entityName);
                        writer.Write(';');
                    }
                }
            }

            /// <summary> <p>
            /// Unescapes the entities in a <code>String</code>.
            /// </p>
            /// 
            /// <p>
            /// For example, if you have called addEntity(&quot;foo&quot;, 0xA1), unescape(&quot;&amp;foo;&quot;) will return
            /// &quot;\u00A1&quot;
            /// </p>
            /// 
            /// </summary>
            /// <param name="str">The <code>String</code> to escape.
            /// </param>
            /// <returns> A new escaped <code>String</code>.
            /// </returns>
            public virtual string unescape(string str)
            {
                int firstAmp = str.IndexOf('&');
                if (firstAmp < 0)
                {
                    return str;
                }
                else
                {
                    System.IO.StringWriter stringWriter = createStringWriter(str);
                    try
                    {
                        this.doUnescape(stringWriter, str, firstAmp);
                    }
                    catch (System.IO.IOException e)
                    {
                        // This should never happen because ALL the StringWriter methods called by #escape(Writer, String) 
                        // do not throw IOExceptions.
                        throw e;
                    }
                    return stringWriter.ToString();
                }
            }

            /// <summary> Make the StringWriter 10% larger than the source String to avoid growing the writer
            /// 
            /// </summary>
            /// <param name="str">The source string
            /// </param>
            /// <returns> A newly created StringWriter
            /// </returns>
            private System.IO.StringWriter createStringWriter(string str)
            {
                return new System.IO.StringWriter();
            }

            /// <summary> <p>
            /// Unescapes the escaped entities in the <code>String</code> passed and writes the result to the
            /// <code>Writer</code> passed.
            /// </p>
            /// 
            /// </summary>
            /// <param name="writer">The <code>Writer</code> to write the results to; assumed to be non-null.
            /// </param>
            /// <param name="str">The source <code>String</code> to unescape; assumed to be non-null.
            /// </param>
            /// <throws>  IOException </throws>
            /// <summary>             when <code>Writer</code> passed throws the exception from calls to the {@link Writer#write(int)}
            /// methods.
            /// 
            /// </summary>
            /// <seealso cref="escape(String)">
            /// </seealso>
            /// <seealso cref="Writer">
            /// </seealso>
            //UPGRADE_ISSUE: “java.io.Writer”和“System.IO.StreamWriter”之间的类层次结构差异可能导致编译错误。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1186'"
            public virtual void unescape(System.IO.TextWriter writer, string str)
            {
                int firstAmp = str.IndexOf('&');
                if (firstAmp < 0)
                {
                    writer.Write(str);
                    return;
                }
                else
                {
                    doUnescape(writer, str, firstAmp);
                }
            }

            /// <summary> Underlying unescape method that allows the optimisation of not starting from the 0 index again.
            /// 
            /// </summary>
            /// <param name="writer">The <code>Writer</code> to write the results to; assumed to be non-null.
            /// </param>
            /// <param name="str">The source <code>String</code> to unescape; assumed to be non-null.
            /// </param>
            /// <param name="firstAmp">The <code>int</code> index of the first ampersand in the source String.
            /// </param>
            /// <throws>  IOException </throws>
            /// <summary>             when <code>Writer</code> passed throws the exception from calls to the {@link Writer#write(int)}
            /// methods.
            /// </summary>
            //UPGRADE_ISSUE: “java.io.Writer”和“System.IO.StreamWriter”之间的类层次结构差异可能导致编译错误。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1186'"
            private void doUnescape(System.IO.TextWriter writer, string str, int firstAmp)
            {
                writer.Write(str.ToCharArray(), 0, firstAmp);
                int len = str.Length;
                for (int i = firstAmp; i < len; i++)
                {
                    char c = str[i];
                    if (c == '&')
                    {
                        int nextIdx = i + 1;
                        //UPGRADE_WARNING: 方法 'java.lang.String.indexOf' 已转换为 'string.IndexOf'，后者可能引发异常。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1101'"
                        int semiColonIdx = str.IndexOf(';', nextIdx);
                        if (semiColonIdx == -1)
                        {
                            writer.Write(c);
                            continue;
                        }
                        //UPGRADE_WARNING: 方法 'java.lang.String.indexOf' 已转换为 'string.IndexOf'，后者可能引发异常。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1101'"
                        int amphersandIdx = str.IndexOf('&', i + 1);
                        if (amphersandIdx != -1 && amphersandIdx < semiColonIdx)
                        {
                            // Then the text looks like &...&...;
                            writer.Write(c);
                            continue;
                        }
                        string entityContent = str.Substring(nextIdx, (semiColonIdx) - (nextIdx));
                        int entityValue = -1;
                        int entityContentLen = entityContent.Length;
                        if (entityContentLen > 0)
                        {
                            if (entityContent[0] == '#')
                            {
                                // escaped value content is an integer (decimal or
                                // hexidecimal)
                                if (entityContentLen > 1)
                                {
                                    char isHexChar = entityContent[1];
                                    try
                                    {
                                        switch (isHexChar)
                                        {

                                            case 'X':
                                            case 'x':
                                                {
                                                    //UPGRADE_TODO: 方法“java.lang.Integer.parseInt”被转换为具有不同行为的 'System.Convert.ToInt32'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
                                                    entityValue = System.Convert.ToInt32(entityContent.Substring(2), 16);
                                                    break;
                                                }

                                            default:
                                                {
                                                    //UPGRADE_TODO: 方法“java.lang.Integer.parseInt”被转换为具有不同行为的 'System.Convert.ToInt32'。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
                                                    entityValue = System.Convert.ToInt32(entityContent.Substring(1), 10);
                                                }
                                                break;

                                        }
                                        if (entityValue > 0xFFFF)
                                        {
                                            entityValue = -1;
                                        }
                                    }
                                    catch (System.FormatException)
                                    {
                                        entityValue = -1;
                                    }
                                }
                            }
                            else
                            {
                                // escaped value content is an entity name
                                entityValue = this.entityValue(entityContent);
                            }
                        }

                        if (entityValue == -1)
                        {
                            writer.Write('&');
                            writer.Write(entityContent);
                            writer.Write(';');
                        }
                        else
                        {
                            writer.Write(entityValue);
                        }
                        i = semiColonIdx; // move index up to the semi-colon
                    }
                    else
                    {
                        writer.Write(c);
                    }
                }
            }
            static Entities()
            {
                {
                    XML = new Entities();
                    XML.addEntities(BASIC_ARRAY);
                    XML.addEntities(APOS_ARRAY);
                }
                {
                    HTML32 = new Entities();
                    HTML32.addEntities(BASIC_ARRAY);
                    HTML32.addEntities(ISO8859_1_ARRAY);
                }
                {
                    HTML40 = new Entities();
                    fillWithHtml40Entities(HTML40);
                }
            }
        }

        /// <summary> <p>Operations on char primitives and Character objects.</p>
        /// 
        /// <p>This class tries to handle <code>null</code> input gracefully.
        /// An exception will not be thrown for a <code>null</code> input.
        /// Each method documents its behaviour in more detail.</p>
        /// 
        /// </summary>
        /// <author>  Stephen Colebourne
        /// </author>
        /// <since> 2.1
        /// </since>
        /// <version>  $Id: CharUtils.java 437554 2006-08-28 06:21:41Z bayard $
        /// </version>
        public class CharUtils
        {

            /// <summary> <code>
            /// </code> linefeed LF ('\n').
            /// 
            /// </summary>
            /// <seealso cref="<a href="http://java.sun.com/docs/books/jls/third_edition/html/lexical.html.101089">JLF: Escape Sequences">
            /// for Character and String Literals</a>
            /// </seealso>
            /// <since> 2.2
            /// </since>
            public const char LF = '\n';

            /// <summary> <code>
            /// </code> carriage return CR ('\r').
            /// 
            /// </summary>
            /// <seealso cref="<a href="http://java.sun.com/docs/books/jls/third_edition/html/lexical.html.101089">JLF: Escape Sequences">
            /// for Character and String Literals</a>
            /// </seealso>
            /// <since> 2.2
            /// </since>
            public const char CR = '\r';


            //--------------------------------------------------------------------------
            /// <summary> <p>Converts the string to the unicode format '\u0020'.</p>
            /// 
            /// <p>This format is the Java source code format.</p>
            /// 
            /// <pre>
            /// CharUtils.unicodeEscaped(' ') = "\u0020"
            /// CharUtils.unicodeEscaped('A') = "\u0041"
            /// </pre>
            /// 
            /// </summary>
            /// <param name="ch"> the character to convert
            /// </param>
            /// <returns> the escaped unicode string
            /// </returns>
            public static string unicodeEscaped(char ch)
            {
                if (ch < 0x10)
                {
                    return "\\u000" + System.Convert.ToString(ch, 16);
                }
                else if (ch < 0x100)
                {
                    return "\\u00" + System.Convert.ToString(ch, 16);
                }
                else if (ch < 0x1000)
                {
                    return "\\u0" + System.Convert.ToString(ch, 16);
                }
                return "\\u" + System.Convert.ToString(ch, 16);
            }
        }


        /// <summary> <p>Operations on {@link java.lang.String} that are
        /// <code>null</code> safe.</p>
        /// 
        /// <ul>
        /// <li><b>IsEmpty/IsBlank</b>
        /// - checks if a String contains text</li>
        /// <li><b>Trim/Strip</b>
        /// - removes leading and trailing whitespace</li>
        /// <li><b>Equals</b>
        /// - compares two strings null-safe</li>
        /// <li><b>startsWith</b>
        /// - check if a String starts with a prefix null-safe</li>
        /// <li><b>endsWith</b>
        /// - check if a String ends with a suffix null-safe</li>
        /// <li><b>IndexOf/LastIndexOf/Contains</b>
        /// - null-safe index-of checks
        /// <li><b>IndexOfAny/LastIndexOfAny/IndexOfAnyBut/LastIndexOfAnyBut</b>
        /// - index-of any of a set of Strings</li>
        /// <li><b>ContainsOnly/ContainsNone/ContainsAny</b>
        /// - does String contains only/none/any of these characters</li>
        /// <li><b>Substring/Left/Right/Mid</b>
        /// - null-safe substring extractions</li>
        /// <li><b>SubstringBefore/SubstringAfter/SubstringBetween</b>
        /// - substring extraction relative to other strings</li>
        /// <li><b>Split/Join</b>
        /// - splits a String into an array of substrings and vice versa</li>
        /// <li><b>Remove/Delete</b>
        /// - removes part of a String</li>
        /// <li><b>Replace/Overlay</b>
        /// - Searches a String and replaces one String with another</li>
        /// <li><b>Chomp/Chop</b>
        /// - removes the last part of a String</li>
        /// <li><b>LeftPad/RightPad/Center/Repeat</b>
        /// - pads a String</li>
        /// <li><b>UpperCase/LowerCase/SwapCase/Capitalize/Uncapitalize</b>
        /// - changes the case of a String</li>
        /// <li><b>CountMatches</b>
        /// - counts the number of occurrences of one String in another</li>
        /// <li><b>IsAlpha/IsNumeric/IsWhitespace/IsAsciiPrintable</b>
        /// - checks the characters in a String</li>
        /// <li><b>DefaultString</b>
        /// - protects against a null input String</li>
        /// <li><b>Reverse/ReverseDelimited</b>
        /// - reverses a String</li>
        /// <li><b>Abbreviate</b>
        /// - abbreviates a string using ellipsis</li>
        /// <li><b>Difference</b>
        /// - compares Strings and reports on their differences</li>
        /// <li><b>LevensteinDistance</b>
        /// - the number of changes needed to change one String into another</li>
        /// </ul>
        /// 
        /// <p>The <code>StringUtils</code> class defines certain words related to
        /// String handling.</p>
        /// 
        /// <ul>
        /// <li>null - <code>null</code></li>
        /// <li>empty - a zero-length string (<code>""</code>)</li>
        /// <li>space - the space character (<code>' '</code>, char 32)</li>
        /// <li>whitespace - the characters defined by {@link Character#isWhitespace(char)}</li>
        /// <li>trim - the characters &lt;= 32 as in {@link String#trim()}</li>
        /// </ul>
        /// 
        /// <p><code>StringUtils</code> handles <code>null</code> input Strings quietly.
        /// That is to say that a <code>null</code> input will return <code>null</code>.
        /// Where a <code>boolean</code> or <code>int</code> is being returned
        /// details vary by method.</p>
        /// 
        /// <p>A side effect of the <code>null</code> handling is that a
        /// <code>NullPointerException</code> should be considered a bug in
        /// <code>StringUtils</code> (except for deprecated methods).</p>
        /// 
        /// <p>Methods in this class give sample code to explain their operation.
        /// The symbol <code>*</code> is used to indicate any input including <code>null</code>.</p>
        /// 
        /// </summary>
        /// <seealso cref="java.lang.String">
        /// </seealso>
        /// <author>  <a href="http://jakarta.apache.org/turbine/">Apache Jakarta Turbine</a>
        /// </author>
        /// <author>  <a href="mailto:jon@latchkey.com">Jon S. Stevens</a>
        /// </author>
        /// <author>  Daniel L. Rall
        /// </author>
        /// <author>  <a href="mailto:gcoladonato@yahoo.com">Greg Coladonato</a>
        /// </author>
        /// <author>  <a href="mailto:ed@apache.org">Ed Korthof</a>
        /// </author>
        /// <author>  <a href="mailto:rand_mcneely@yahoo.com">Rand McNeely</a>
        /// </author>
        /// <author>  Stephen Colebourne
        /// </author>
        /// <author>  <a href="mailto:fredrik@westermarck.com">Fredrik Westermarck</a>
        /// </author>
        /// <author>  Holger Krauth
        /// </author>
        /// <author>  <a href="mailto:alex@purpletech.com">Alexander Day Chaffee</a>
        /// </author>
        /// <author>  <a href="mailto:hps@intermeta.de">Henning P. Schmiedehausen</a>
        /// </author>
        /// <author>  Arun Mammen Thomas
        /// </author>
        /// <author>  Gary Gregory
        /// </author>
        /// <author>  Phil Steitz
        /// </author>
        /// <author>  Al Chou
        /// </author>
        /// <author>  Michael Davey
        /// </author>
        /// <author>  Reuben Sivan
        /// </author>
        /// <author>  Chris Hyzer
        /// </author>
        /// <author>  Scott Johnson
        /// </author>
        /// <since> 1.0
        /// </since>
        /// <version>  $Id: StringUtils.java 635447 2008-03-10 06:27:09Z bayard $
        /// </version>
        public class StringUtils
        {
            // Performance testing notes (JDK 1.4, Jul03, scolebourne)
            // Whitespace:
            // Character.isWhitespace() is faster than WHITESPACE.indexOf()
            // where WHITESPACE is a string of all whitespace characters
            //
            // Character access:
            // String.charAt(n) versus toCharArray(), then array[n]
            // String.charAt(n) is about 15% worse for a 10K string
            // They are about equal for a length 50 string
            // String.charAt(n) is about 4 times better for a length 3 string
            // String.charAt(n) is best bet overall
            //
            // Append:
            // String.concat about twice as fast as StringBuffer.append
            // (not sure who tested this)

            /// <summary> The empty String <code>""</code>.</summary>
            /// <since> 2.0
            /// </since>
            public const string EMPTY = "";

            /// <summary> Represents a failed index search.</summary>
            /// <since> 2.1
            /// </since>
            public const int INDEX_NOT_FOUND = -1;

            /// <summary> <p>The maximum size to which the padding constant(s) can expand.</p></summary>
            private const int PAD_LIMIT = 8192;

            // Empty checks
            //-----------------------------------------------------------------------
            /// <summary> <p>Checks if a String is empty ("") or null.</p>
            /// 
            /// <pre>
            /// StringUtils.isEmpty(null)      = true
            /// StringUtils.isEmpty("")        = true
            /// StringUtils.isEmpty(" ")       = false
            /// StringUtils.isEmpty("bob")     = false
            /// StringUtils.isEmpty("  bob  ") = false
            /// </pre>
            /// 
            /// <p>NOTE: This method changed in Lang version 2.0.
            /// It no longer trims the String.
            /// That functionality is available in isBlank().</p>
            /// 
            /// </summary>
            /// <param name="str"> the String to check, may be null
            /// </param>
            /// <returns> <code>true</code> if the String is empty or null
            /// </returns>
            public static bool isEmpty(string str)
            {
                return str == null || str.Length == 0;
            }

            /// <summary> <p>Strips any of a set of characters from the start and end of a String.
            /// This is similar to {@link String#trim()} but allows the characters
            /// to be stripped to be controlled.</p>
            /// 
            /// <p>A <code>null</code> input String returns <code>null</code>.
            /// An empty string ("") input returns the empty string.</p>
            /// 
            /// <p>If the stripChars String is <code>null</code>, whitespace is
            /// stripped as defined by {@link Character#isWhitespace(char)}.
            /// Alternatively use {@link #strip(String)}.</p>
            /// 
            /// <pre>
            /// StringUtils.strip(null, *)          = null
            /// StringUtils.strip("", *)            = ""
            /// StringUtils.strip("abc", null)      = "abc"
            /// StringUtils.strip("  abc", null)    = "abc"
            /// StringUtils.strip("abc  ", null)    = "abc"
            /// StringUtils.strip(" abc ", null)    = "abc"
            /// StringUtils.strip("  abcyx", "xyz") = "  abc"
            /// </pre>
            /// 
            /// </summary>
            /// <param name="str"> the String to remove characters from, may be null
            /// </param>
            /// <param name="stripChars"> the characters to remove, null treated as whitespace
            /// </param>
            /// <returns> the stripped String, <code>null</code> if null String input
            /// </returns>
            public static string strip(string str, string stripChars)
            {
                if (isEmpty(str))
                {
                    return str;
                }
                str = stripStart(str, stripChars);
                return stripEnd(str, stripChars);
            }

            /// <summary> <p>Strips any of a set of characters from the start of a String.</p>
            /// 
            /// <p>A <code>null</code> input String returns <code>null</code>.
            /// An empty string ("") input returns the empty string.</p>
            /// 
            /// <p>If the stripChars String is <code>null</code>, whitespace is
            /// stripped as defined by {@link Character#isWhitespace(char)}.</p>
            /// 
            /// <pre>
            /// StringUtils.stripStart(null, *)          = null
            /// StringUtils.stripStart("", *)            = ""
            /// StringUtils.stripStart("abc", "")        = "abc"
            /// StringUtils.stripStart("abc", null)      = "abc"
            /// StringUtils.stripStart("  abc", null)    = "abc"
            /// StringUtils.stripStart("abc  ", null)    = "abc  "
            /// StringUtils.stripStart(" abc ", null)    = "abc "
            /// StringUtils.stripStart("yxabc  ", "xyz") = "abc  "
            /// </pre>
            /// 
            /// </summary>
            /// <param name="str"> the String to remove characters from, may be null
            /// </param>
            /// <param name="stripChars"> the characters to remove, null treated as whitespace
            /// </param>
            /// <returns> the stripped String, <code>null</code> if null String input
            /// </returns>
            public static string stripStart(string str, string stripChars)
            {
                int strLen;
                if (str == null || (strLen = str.Length) == 0)
                {
                    return str;
                }
                int start = 0;
                if (stripChars == null)
                {
                    while ((start != strLen) && System.Char.IsWhiteSpace(str[start]))
                    {
                        start++;
                    }
                }
                else if (stripChars.Length == 0)
                {
                    return str;
                }
                else
                {
                    while ((start != strLen) && (stripChars.IndexOf((System.Char)str[start]) != -1))
                    {
                        start++;
                    }
                }
                return str.Substring(start);
            }

            /// <summary> <p>Strips any of a set of characters from the end of a String.</p>
            /// 
            /// <p>A <code>null</code> input String returns <code>null</code>.
            /// An empty string ("") input returns the empty string.</p>
            /// 
            /// <p>If the stripChars String is <code>null</code>, whitespace is
            /// stripped as defined by {@link Character#isWhitespace(char)}.</p>
            /// 
            /// <pre>
            /// StringUtils.stripEnd(null, *)          = null
            /// StringUtils.stripEnd("", *)            = ""
            /// StringUtils.stripEnd("abc", "")        = "abc"
            /// StringUtils.stripEnd("abc", null)      = "abc"
            /// StringUtils.stripEnd("  abc", null)    = "  abc"
            /// StringUtils.stripEnd("abc  ", null)    = "abc"
            /// StringUtils.stripEnd(" abc ", null)    = " abc"
            /// StringUtils.stripEnd("  abcyx", "xyz") = "  abc"
            /// </pre>
            /// 
            /// </summary>
            /// <param name="str"> the String to remove characters from, may be null
            /// </param>
            /// <param name="stripChars"> the characters to remove, null treated as whitespace
            /// </param>
            /// <returns> the stripped String, <code>null</code> if null String input
            /// </returns>
            public static string stripEnd(string str, string stripChars)
            {
                int end;
                if (str == null || (end = str.Length) == 0)
                {
                    return str;
                }

                if (stripChars == null)
                {
                    while ((end != 0) && System.Char.IsWhiteSpace(str[end - 1]))
                    {
                        end--;
                    }
                }
                else if (stripChars.Length == 0)
                {
                    return str;
                }
                else
                {
                    while ((end != 0) && (stripChars.IndexOf((System.Char)str[end - 1]) != -1))
                    {
                        end--;
                    }
                }
                return str.Substring(0, (end) - (0));
            }

            // ContainsAny
            //-----------------------------------------------------------------------
            /// <summary> <p>Checks if the String contains any character in the given
            /// set of characters.</p>
            /// 
            /// <p>A <code>null</code> String will return <code>false</code>.
            /// A <code>null</code> or zero length search array will return <code>false</code>.</p>
            /// 
            /// <pre>
            /// StringUtils.containsAny(null, *)                = false
            /// StringUtils.containsAny("", *)                  = false
            /// StringUtils.containsAny(*, null)                = false
            /// StringUtils.containsAny(*, [])                  = false
            /// StringUtils.containsAny("zzabyycdxx",['z','a']) = true
            /// StringUtils.containsAny("zzabyycdxx",['b','y']) = true
            /// StringUtils.containsAny("aba", ['z'])           = false
            /// </pre>
            /// 
            /// </summary>
            /// <param name="str"> the String to check, may be null
            /// </param>
            /// <param name="searchChars"> the chars to search for, may be null
            /// </param>
            /// <returns> the <code>true</code> if any of the chars are found,
            /// <code>false</code> if no match or null input
            /// </returns>
            /// <since> 2.4
            /// </since>
            public static bool containsAny(string str, char[] searchChars)
            {
                if (str == null || str.Length == 0 || searchChars == null || searchChars.Length == 0)
                {
                    return false;
                }
                for (int i = 0; i < str.Length; i++)
                {
                    char ch = str[i];
                    for (int j = 0; j < searchChars.Length; j++)
                    {
                        if (searchChars[j] == ch)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            // ContainsNone
            //-----------------------------------------------------------------------
            /// <summary> <p>Checks that the String does not contain certain characters.</p>
            /// 
            /// <p>A <code>null</code> String will return <code>true</code>.
            /// A <code>null</code> invalid character array will return <code>true</code>.
            /// An empty String ("") always returns true.</p>
            /// 
            /// <pre>
            /// StringUtils.containsNone(null, *)       = true
            /// StringUtils.containsNone(*, null)       = true
            /// StringUtils.containsNone("", *)         = true
            /// StringUtils.containsNone("ab", '')      = true
            /// StringUtils.containsNone("abab", 'xyz') = true
            /// StringUtils.containsNone("ab1", 'xyz')  = true
            /// StringUtils.containsNone("abz", 'xyz')  = false
            /// </pre>
            /// 
            /// </summary>
            /// <param name="str"> the String to check, may be null
            /// </param>
            /// <param name="invalidChars"> an array of invalid chars, may be null
            /// </param>
            /// <returns> true if it contains none of the invalid chars, or is null
            /// </returns>
            /// <since> 2.0
            /// </since>
            public static bool containsNone(string str, char[] invalidChars)
            {
                if (str == null || invalidChars == null)
                {
                    return true;
                }
                int strSize = str.Length;
                int validSize = invalidChars.Length;
                for (int i = 0; i < strSize; i++)
                {
                    char ch = str[i];
                    for (int j = 0; j < validSize; j++)
                    {
                        if (invalidChars[j] == ch)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            /// <summary> <p>Replaces all occurrences of a String within another String.</p>
            /// 
            /// <p>A <code>null</code> reference passed to this method is a no-op.</p>
            /// 
            /// <pre>
            /// StringUtils.replace(null, *, *)        = null
            /// StringUtils.replace("", *, *)          = ""
            /// StringUtils.replace("any", null, *)    = "any"
            /// StringUtils.replace("any", *, null)    = "any"
            /// StringUtils.replace("any", "", *)      = "any"
            /// StringUtils.replace("aba", "a", null)  = "aba"
            /// StringUtils.replace("aba", "a", "")    = "b"
            /// StringUtils.replace("aba", "a", "z")   = "zbz"
            /// </pre>
            /// 
            /// </summary>
            /// <seealso cref="replace(String text, String searchString, String replacement, int max)">
            /// </seealso>
            /// <param name="text"> text to search and replace in, may be null
            /// </param>
            /// <param name="searchString"> the String to search for, may be null
            /// </param>
            /// <param name="replacement"> the String to replace it with, may be null
            /// </param>
            /// <returns> the text with any replacements processed,
            /// <code>null</code> if null String input
            /// </returns>
            public static string replace(string text, string searchString, string replacement)
            {
                return replace(text, searchString, replacement, -1);
            }

            /// <summary> <p>Replaces a String with another String inside a larger String,
            /// for the first <code>max</code> values of the search String.</p>
            /// 
            /// <p>A <code>null</code> reference passed to this method is a no-op.</p>
            /// 
            /// <pre>
            /// StringUtils.replace(null, *, *, *)         = null
            /// StringUtils.replace("", *, *, *)           = ""
            /// StringUtils.replace("any", null, *, *)     = "any"
            /// StringUtils.replace("any", *, null, *)     = "any"
            /// StringUtils.replace("any", "", *, *)       = "any"
            /// StringUtils.replace("any", *, *, 0)        = "any"
            /// StringUtils.replace("abaa", "a", null, -1) = "abaa"
            /// StringUtils.replace("abaa", "a", "", -1)   = "b"
            /// StringUtils.replace("abaa", "a", "z", 0)   = "abaa"
            /// StringUtils.replace("abaa", "a", "z", 1)   = "zbaa"
            /// StringUtils.replace("abaa", "a", "z", 2)   = "zbza"
            /// StringUtils.replace("abaa", "a", "z", -1)  = "zbzz"
            /// </pre>
            /// 
            /// </summary>
            /// <param name="text"> text to search and replace in, may be null
            /// </param>
            /// <param name="searchString"> the String to search for, may be null
            /// </param>
            /// <param name="replacement"> the String to replace it with, may be null
            /// </param>
            /// <param name="max"> maximum number of values to replace, or <code>-1</code> if no maximum
            /// </param>
            /// <returns> the text with any replacements processed,
            /// <code>null</code> if null String input
            /// </returns>
            public static string replace(string text, string searchString, string replacement, int max)
            {
                if (isEmpty(text) || isEmpty(searchString) || replacement == null || max == 0)
                {
                    return text;
                }
                int start = 0;
                //UPGRADE_WARNING: 方法 'java.lang.String.indexOf' 已转换为 'string.IndexOf'，后者可能引发异常。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1101'"
                int end = text.IndexOf(searchString, start);
                if (end == -1)
                {
                    return text;
                }
                int replLength = searchString.Length;
                int increase = replacement.Length - replLength;
                increase = (increase < 0 ? 0 : increase);
                increase *= (max < 0 ? 16 : (max > 64 ? 64 : max));
                System.Text.StringBuilder buf = new System.Text.StringBuilder(text.Length + increase);
                while (end != -1)
                {
                    buf.Append(text.Substring(start, (end) - (start))).Append(replacement);
                    start = end + replLength;
                    if (--max == 0)
                    {
                        break;
                    }
                    //UPGRADE_WARNING: 方法 'java.lang.String.indexOf' 已转换为 'string.IndexOf'，后者可能引发异常。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1101'"
                    end = text.IndexOf(searchString, start);
                }
                buf.Append(text.Substring(start));
                return buf.ToString();
            }


            /// <summary> <p>
            /// Replaces all occurrences of Strings within another String.
            /// </p>
            /// 
            /// <p>
            /// A <code>null</code> reference passed to this method is a no-op, or if
            /// any "search string" or "string to replace" is null, that replace will be
            /// ignored. 
            /// </p>
            /// 
            /// <pre>
            /// StringUtils.replaceEach(null, *, *, *) = null
            /// StringUtils.replaceEach("", *, *, *) = ""
            /// StringUtils.replaceEach("aba", null, null, *) = "aba"
            /// StringUtils.replaceEach("aba", new String[0], null, *) = "aba"
            /// StringUtils.replaceEach("aba", null, new String[0], *) = "aba"
            /// StringUtils.replaceEach("aba", new String[]{"a"}, null, *) = "aba"
            /// StringUtils.replaceEach("aba", new String[]{"a"}, new String[]{""}, *) = "b"
            /// StringUtils.replaceEach("aba", new String[]{null}, new String[]{"a"}, *) = "aba"
            /// StringUtils.replaceEach("abcde", new String[]{"ab", "d"}, new String[]{"w", "t"}, *) = "wcte"
            /// (example of how it repeats)
            /// StringUtils.replaceEach("abcde", new String[]{"ab", "d"}, new String[]{"d", "t"}, false) = "dcte"
            /// StringUtils.replaceEach("abcde", new String[]{"ab", "d"}, new String[]{"d", "t"}, true) = "tcte"
            /// StringUtils.replaceEach("abcde", new String[]{"ab", "d"}, new String[]{"d", "ab"}, *) = IllegalArgumentException
            /// </pre>
            /// 
            /// </summary>
            /// <param name="text">text to search and replace in, no-op if null
            /// </param>
            /// <param name="searchList">the Strings to search for, no-op if null
            /// </param>
            /// <param name="replacementList">the Strings to replace them with, no-op if null
            /// </param>
            /// <param name="repeat">if true, then replace repeatedly 
            /// until there are no more possible replacements or timeToLive < 0
            /// </param>
            /// <param name="timeToLive">if less than 0 then there is a circular reference and endless
            /// loop
            /// </param>
            /// <returns> the text with any replacements processed, <code>null</code> if
            /// null String input
            /// </returns>
            /// <throws>  IllegalArgumentException </throws>
            /// <summary>             if the search is repeating and there is an endless loop due
            /// to outputs of one being inputs to another
            /// </summary>
            /// <throws>  IndexOutOfBoundsException </throws>
            /// <summary>             if the lengths of the arrays are not the same (null is ok,
            /// and/or size 0)
            /// </summary>
            /// <since> 2.4
            /// </since>
            private static string replaceEach(string text, string[] searchList, string[] replacementList, bool repeat, int timeToLive)
            {

                // mchyzer Performance note: This creates very few new objects (one major goal)
                // let me know if there are performance requests, we can create a harness to measure

                if (text == null || text.Length == 0 || searchList == null || searchList.Length == 0 || replacementList == null || replacementList.Length == 0)
                {
                    return text;
                }

                // if recursing, this shouldnt be less than 0
                if (timeToLive < 0)
                {
                    throw new System.SystemException("TimeToLive of " + timeToLive + " is less than 0: " + text);
                }

                int searchLength = searchList.Length;
                int replacementLength = replacementList.Length;

                // make sure lengths are ok, these need to be equal
                if (searchLength != replacementLength)
                {
                    throw new System.ArgumentException("Search and Replace array lengths don't match: " + searchLength + " vs " + replacementLength);
                }

                // keep track of which still have matches
                bool[] noMoreMatchesForReplIndex = new bool[searchLength];

                // index on index that the match was found
                int textIndex = -1;
                int replaceIndex = -1;
                int tempIndex = -1;

                // index of replace array that will replace the search string found
                // NOTE: logic duplicated below START
                for (int i = 0; i < searchLength; i++)
                {
                    if (noMoreMatchesForReplIndex[i] || searchList[i] == null || searchList[i].Length == 0 || replacementList[i] == null)
                    {
                        continue;
                    }
                    tempIndex = text.IndexOf(searchList[i]);

                    // see if we need to keep searching for this
                    if (tempIndex == -1)
                    {
                        noMoreMatchesForReplIndex[i] = true;
                    }
                    else
                    {
                        if (textIndex == -1 || tempIndex < textIndex)
                        {
                            textIndex = tempIndex;
                            replaceIndex = i;
                        }
                    }
                }
                // NOTE: logic mostly below END

                // no search strings found, we are done
                if (textIndex == -1)
                {
                    return text;
                }

                int start = 0;

                // Get a good guess on the size of the result buffer so it doesnt have to double if it goes over a bit
                int increase = 0;

                // count the replacement text elements that are larger than their corresponding text being replaced
                for (int i = 0; i < searchList.Length; i++)
                {
                    int greater = replacementList[i].Length - searchList[i].Length;
                    if (greater > 0)
                    {
                        increase += 3 * greater; // assume 3 matches
                    }
                }
                // have upper-bound at 20% increase, then let Java take over
                increase = System.Math.Min(increase, text.Length / 5);

                System.Text.StringBuilder buf = new System.Text.StringBuilder(text.Length + increase);

                while (textIndex != -1)
                {

                    for (int i = start; i < textIndex; i++)
                    {
                        buf.Append(text[i]);
                    }
                    buf.Append(replacementList[replaceIndex]);

                    start = textIndex + searchList[replaceIndex].Length;

                    textIndex = -1;
                    replaceIndex = -1;
                    tempIndex = -1;
                    // find the next earliest match
                    // NOTE: logic mostly duplicated above START
                    for (int i = 0; i < searchLength; i++)
                    {
                        if (noMoreMatchesForReplIndex[i] || searchList[i] == null || searchList[i].Length == 0 || replacementList[i] == null)
                        {
                            continue;
                        }
                        //UPGRADE_WARNING: 方法 'java.lang.String.indexOf' 已转换为 'string.IndexOf'，后者可能引发异常。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1101'"
                        tempIndex = text.IndexOf(searchList[i], start);

                        // see if we need to keep searching for this
                        if (tempIndex == -1)
                        {
                            noMoreMatchesForReplIndex[i] = true;
                        }
                        else
                        {
                            if (textIndex == -1 || tempIndex < textIndex)
                            {
                                textIndex = tempIndex;
                                replaceIndex = i;
                            }
                        }
                    }
                    // NOTE: logic duplicated above END
                }
                int textLength = text.Length;
                for (int i = start; i < textLength; i++)
                {
                    buf.Append(text[i]);
                }
                string result = buf.ToString();
                if (!repeat)
                {
                    return result;
                }

                return replaceEach(result, searchList, replacementList, repeat, timeToLive - 1);
            }

            /// <summary> <p>Returns padding using the specified delimiter repeated
            /// to a given length.</p>
            /// 
            /// <pre>
            /// StringUtils.padding(0, 'e')  = ""
            /// StringUtils.padding(3, 'e')  = "eee"
            /// StringUtils.padding(-2, 'e') = IndexOutOfBoundsException
            /// </pre>
            /// 
            /// <p>Note: this method doesn't not support padding with
            /// <a href="http://www.unicode.org/glossary/#supplementary_character">Unicode Supplementary Characters</a>
            /// as they require a pair of <code>char</code>s to be represented.
            /// If you are needing to support full I18N of your applications
            /// consider using {@link #repeat(String, int)} instead. 
            /// </p>
            /// 
            /// </summary>
            /// <param name="repeat"> number of times to repeat delim
            /// </param>
            /// <param name="padChar"> character to repeat
            /// </param>
            /// <returns> String with repeated character
            /// </returns>
            /// <throws>  IndexOutOfBoundsException if <code>repeat &lt; 0</code> </throws>
            /// <seealso cref="repeat(String, int)">
            /// </seealso>
            private static string padding(int repeat, char padChar)
            {
                if (repeat < 0)
                {
                    throw new System.IndexOutOfRangeException("Cannot pad a negative amount: " + repeat);
                }
                //UPGRADE_NOTE: Final 已从“buf ”的声明中移除。 "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
                char[] buf = new char[repeat];
                for (int i = 0; i < buf.Length; i++)
                {
                    buf[i] = padChar;
                }
                return new string(buf);
            }

            /// <summary> <p>Right pad a String with a specified character.</p>
            /// 
            /// <p>The String is padded to the size of <code>size</code>.</p>
            /// 
            /// <pre>
            /// StringUtils.rightPad(null, *, *)     = null
            /// StringUtils.rightPad("", 3, 'z')     = "zzz"
            /// StringUtils.rightPad("bat", 3, 'z')  = "bat"
            /// StringUtils.rightPad("bat", 5, 'z')  = "batzz"
            /// StringUtils.rightPad("bat", 1, 'z')  = "bat"
            /// StringUtils.rightPad("bat", -1, 'z') = "bat"
            /// </pre>
            /// 
            /// </summary>
            /// <param name="str"> the String to pad out, may be null
            /// </param>
            /// <param name="size"> the size to pad to
            /// </param>
            /// <param name="padChar"> the character to pad with
            /// </param>
            /// <returns> right padded String or original String if no padding is necessary,
            /// <code>null</code> if null String input
            /// </returns>
            /// <since> 2.0
            /// </since>
            public static string rightPad(string str, int size, char padChar)
            {
                if (str == null)
                {
                    return null;
                }
                int pads = size - str.Length;
                if (pads <= 0)
                {
                    return str; // returns original String when possible
                }
                if (pads > PAD_LIMIT)
                {
                    return rightPad(str, size, System.Convert.ToString(padChar));
                }
                return string.Concat(str, padding(pads, padChar));
            }

            /// <summary> <p>Right pad a String with a specified String.</p>
            /// 
            /// <p>The String is padded to the size of <code>size</code>.</p>
            /// 
            /// <pre>
            /// StringUtils.rightPad(null, *, *)      = null
            /// StringUtils.rightPad("", 3, "z")      = "zzz"
            /// StringUtils.rightPad("bat", 3, "yz")  = "bat"
            /// StringUtils.rightPad("bat", 5, "yz")  = "batyz"
            /// StringUtils.rightPad("bat", 8, "yz")  = "batyzyzy"
            /// StringUtils.rightPad("bat", 1, "yz")  = "bat"
            /// StringUtils.rightPad("bat", -1, "yz") = "bat"
            /// StringUtils.rightPad("bat", 5, null)  = "bat  "
            /// StringUtils.rightPad("bat", 5, "")    = "bat  "
            /// </pre>
            /// 
            /// </summary>
            /// <param name="str"> the String to pad out, may be null
            /// </param>
            /// <param name="size"> the size to pad to
            /// </param>
            /// <param name="padStr"> the String to pad with, null or empty treated as single space
            /// </param>
            /// <returns> right padded String or original String if no padding is necessary,
            /// <code>null</code> if null String input
            /// </returns>
            public static string rightPad(string str, int size, string padStr)
            {
                if (str == null)
                {
                    return null;
                }
                if (isEmpty(padStr))
                {
                    padStr = " ";
                }
                int padLen = padStr.Length;
                int strLen = str.Length;
                int pads = size - strLen;
                if (pads <= 0)
                {
                    return str; // returns original String when possible
                }
                if (padLen == 1 && pads <= PAD_LIMIT)
                {
                    return rightPad(str, size, padStr[0]);
                }

                if (pads == padLen)
                {
                    return string.Concat(str, padStr);
                }
                else if (pads < padLen)
                {
                    return string.Concat(str, padStr.Substring(0, (pads) - (0)));
                }
                else
                {
                    char[] padding = new char[pads];
                    char[] padChars = padStr.ToCharArray();
                    for (int i = 0; i < pads; i++)
                    {
                        padding[i] = padChars[i % padLen];
                    }
                    return string.Concat(str, new string(padding));
                }
            }

            /// <summary> <p>Left pad a String with a specified character.</p>
            /// 
            /// <p>Pad to a size of <code>size</code>.</p>
            /// 
            /// <pre>
            /// StringUtils.leftPad(null, *, *)     = null
            /// StringUtils.leftPad("", 3, 'z')     = "zzz"
            /// StringUtils.leftPad("bat", 3, 'z')  = "bat"
            /// StringUtils.leftPad("bat", 5, 'z')  = "zzbat"
            /// StringUtils.leftPad("bat", 1, 'z')  = "bat"
            /// StringUtils.leftPad("bat", -1, 'z') = "bat"
            /// </pre>
            /// 
            /// </summary>
            /// <param name="str"> the String to pad out, may be null
            /// </param>
            /// <param name="size"> the size to pad to
            /// </param>
            /// <param name="padChar"> the character to pad with
            /// </param>
            /// <returns> left padded String or original String if no padding is necessary,
            /// <code>null</code> if null String input
            /// </returns>
            /// <since> 2.0
            /// </since>
            public static string leftPad(string str, int size, char padChar)
            {
                if (str == null)
                {
                    return null;
                }
                int pads = size - str.Length;
                if (pads <= 0)
                {
                    return str; // returns original String when possible
                }
                if (pads > PAD_LIMIT)
                {
                    return leftPad(str, size, System.Convert.ToString(padChar));
                }
                return string.Concat(padding(pads, padChar), str);
            }

            /// <summary> <p>Left pad a String with a specified String.</p>
            /// 
            /// <p>Pad to a size of <code>size</code>.</p>
            /// 
            /// <pre>
            /// StringUtils.leftPad(null, *, *)      = null
            /// StringUtils.leftPad("", 3, "z")      = "zzz"
            /// StringUtils.leftPad("bat", 3, "yz")  = "bat"
            /// StringUtils.leftPad("bat", 5, "yz")  = "yzbat"
            /// StringUtils.leftPad("bat", 8, "yz")  = "yzyzybat"
            /// StringUtils.leftPad("bat", 1, "yz")  = "bat"
            /// StringUtils.leftPad("bat", -1, "yz") = "bat"
            /// StringUtils.leftPad("bat", 5, null)  = "  bat"
            /// StringUtils.leftPad("bat", 5, "")    = "  bat"
            /// </pre>
            /// 
            /// </summary>
            /// <param name="str"> the String to pad out, may be null
            /// </param>
            /// <param name="size"> the size to pad to
            /// </param>
            /// <param name="padStr"> the String to pad with, null or empty treated as single space
            /// </param>
            /// <returns> left padded String or original String if no padding is necessary,
            /// <code>null</code> if null String input
            /// </returns>
            public static string leftPad(string str, int size, string padStr)
            {
                if (str == null)
                {
                    return null;
                }
                if (isEmpty(padStr))
                {
                    padStr = " ";
                }
                int padLen = padStr.Length;
                int strLen = str.Length;
                int pads = size - strLen;
                if (pads <= 0)
                {
                    return str; // returns original String when possible
                }
                if (padLen == 1 && pads <= PAD_LIMIT)
                {
                    return leftPad(str, size, padStr[0]);
                }

                if (pads == padLen)
                {
                    return string.Concat(padStr, str);
                }
                else if (pads < padLen)
                {
                    return string.Concat(padStr.Substring(0, (pads) - (0)), str);
                }
                else
                {
                    char[] padding = new char[pads];
                    char[] padChars = padStr.ToCharArray();
                    for (int i = 0; i < pads; i++)
                    {
                        padding[i] = padChars[i % padLen];
                    }
                    return string.Concat(new string(padding), str);
                }
            }
        }

        /*******************************/
        /// <summary>
        /// Performs an unsigned bitwise right shift with the specified number
        /// </summary>
        /// <param name="number">Number to operate on</param>
        /// <param name="bits">Ammount of bits to shift</param>
        /// <returns>The resulting number from the shift operation</returns>
        public static int URShift(int number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            else
                return (number >> bits) + (2 << ~bits);
        }

        /// <summary>
        /// Performs an unsigned bitwise right shift with the specified number
        /// </summary>
        /// <param name="number">Number to operate on</param>
        /// <param name="bits">Ammount of bits to shift</param>
        /// <returns>The resulting number from the shift operation</returns>
        public static int URShift(int number, long bits)
        {
            return URShift(number, (int)bits);
        }

        /// <summary>
        /// Performs an unsigned bitwise right shift with the specified number
        /// </summary>
        /// <param name="number">Number to operate on</param>
        /// <param name="bits">Ammount of bits to shift</param>
        /// <returns>The resulting number from the shift operation</returns>
        public static long URShift(long number, int bits)
        {
            if (number >= 0)
                return number >> bits;
            else
                return (number >> bits) + (2L << ~bits);
        }

        /// <summary>
        /// Performs an unsigned bitwise right shift with the specified number
        /// </summary>
        /// <param name="number">Number to operate on</param>
        /// <param name="bits">Ammount of bits to shift</param>
        /// <returns>The resulting number from the shift operation</returns>
        public static long URShift(long number, long bits)
        {
            return URShift(number, (int)bits);
        }

        /*******************************/

        /*******************************/
        /*******************************/
        /// <summary>
        /// Gives support functions to Http internet connections.
        /// </summary>
        public class URLConnectionSupport
        {
            /// <summary>
            /// Sets the request property for the specified key
            /// </summary>
            /// <param name="connection">Connection used to assign the property value</param>
            /// <param name="key">Property name to obtain the property value</param>
            /// <param name="keyValue">The value to associate with the specified property</param>
            public static void SetRequestProperty(System.Net.HttpWebRequest connection, string key, string keyValue)
            {
                connection.Headers.Set(key, keyValue);
            }

            /// <summary>
            /// Gets the request property for the specified key
            /// </summary>
            /// <param name="connection">Connection used to obtain the property value</param>
            /// <param name="key">Property name to return it's property value</param>
            /// <returns>The value associated with the specified property</returns>
            public static string GetRequestProperty(System.Net.HttpWebRequest connection, string key)
            {
                try
                {
                    return connection.Headers.Get(key);
                }
                catch (System.Exception)
                { }
                return "";
            }

            /// <summary>
            /// Receives a key and returns it's default property value
            /// </summary>
            /// <param name="key">Key name to obtain the default request value</param>
            /// <returns>The default value associated with the property</returns>
            public static string GetDefaultRequestProperty(string key)
            {
                return null;
            }

            /// <summary> 
            /// Gets the value of the "Content-Encoding" property from the collection of headers associated with the specified HttpWebRequest
            /// </summary>
            /// <param name="request">Instance of HttpWebRequest to Get the headers from</param>
            /// <returns>The value of the "Content-Encoding" property if found, otherwise returns null</returns>
            public static string GetContentEncoding(System.Net.HttpWebRequest request)
            {
                try
                {
                    return request.GetResponse().Headers.Get("Content-Encoding");
                }
                catch (System.Exception)
                { }
                return null;
            }

            /// <summary>
            /// Gets the sending date of the resource referenced by the HttpRequest
            /// </summary>
            /// <param name="request">Instance of HttpWebRequest to Get the date from</param>
            /// <returns>The sending date of the resource if found, otherwise 0</returns>
            public static long GetSendingDate(System.Net.HttpWebRequest request)
            {
                long headerDate;
                try
                {
                    headerDate = System.DateTime.Parse(request.GetResponse().Headers.Get("Date")).Ticks;
                }
                catch (System.Exception)
                {
                    headerDate = 0;
                }
                return headerDate;
            }

            /// <summary>
            /// Gets the key for the specified index from the KeysCollection of the specified HttpWebRequest's Headers property
            /// </summary>
            /// <param name="request">Instance HttpWebRequest to Get the key from</param>
            /// <param name="indexField">Index of the field to Get the corresponding key</param>
            /// <returns>The key for the specified index if found, otherwise null</returns>
            public static string GetHeaderFieldKey(System.Net.HttpWebRequest request, int indexField)
            {
                try
                {
                    return request.GetResponse().Headers.Keys.Get(indexField);
                }
                catch (System.Exception)
                { }
                return null;
            }

            /// <summary>
            /// Gets the value of the "Last-Modified" property from the collection of headers associated with the specified HttWebRequest
            /// </summary>
            /// <param name="request">Instance of HttpWebRequest to Get the headers from</param>
            /// <returns>The value of the "Last-Modified" property if found, otherwise returns null</returns>
            public static long GetLastModifiedHeaderField(System.Net.HttpWebRequest request)
            {
                long fieldHeaderDate;
                try
                {
                    fieldHeaderDate = System.DateTime.Parse(request.GetResponse().Headers.Get("Last-Modified")).Ticks;
                }
                catch (System.Exception)
                {
                    fieldHeaderDate = 0;
                }
                return fieldHeaderDate;
            }

            /// <summary>
            /// Gets the value of the named field parsed as date in milliseconds
            /// </summary>
            /// <param name="request">Instance of System.Net.HttpWebRequest to Get the headers from</param>
            /// <param name="fieldName">Name of the header field</param>
            /// <param name="defaultValue">A default value to return if the value does not exist in the headers</param>
            /// <returns></returns>
            public static long GetHeaderFieldDate(System.Net.HttpWebRequest request, string fieldName, long defaultValue)
            {
                long fieldHeaderDate;
                try
                {
                    fieldHeaderDate = System.DateTime.Parse(request.GetResponse().Headers.Get(fieldName)).Ticks;
                }
                catch (System.Exception)
                {
                    fieldHeaderDate = defaultValue;
                }
                return fieldHeaderDate;
            }
        }



        public static long FileLength(FileInfo file)
        {
            if (Directory.Exists(file.FullName))
                return 0;
            else
                return file.Length;
        }

        /*******************************/

        public static void WriteStackTrace(System.Exception throwable, TextWriter stream)
        {
            stream.Write(throwable.StackTrace);
            stream.Flush();
        }


        public class DateTimeFormatManager
        {
            public static DateTimeFormatHashTable manager = new DateTimeFormatHashTable();

            public class DateTimeFormatHashTable : Hashtable
            {
                public void SetDateFormatPattern(DateTimeFormatInfo format, String newPattern)
                {
                    if (this[format] != null)
                        ((DateTimeFormatProperties)this[format]).DateFormatPattern = newPattern;
                    else
                    {
                        DateTimeFormatProperties tempProps = new DateTimeFormatProperties();
                        tempProps.DateFormatPattern = newPattern;
                        Add(format, tempProps);
                    }
                }

                public string GetDateFormatPattern(DateTimeFormatInfo format)
                {
                    if (this[format] == null)
                        return "d-MMM-yy";
                    else
                        return ((DateTimeFormatProperties)this[format]).DateFormatPattern;
                }

                public void SetTimeFormatPattern(DateTimeFormatInfo format, String newPattern)
                {
                    if (this[format] != null)
                        ((DateTimeFormatProperties)this[format]).TimeFormatPattern = newPattern;
                    else
                    {
                        DateTimeFormatProperties tempProps = new DateTimeFormatProperties();
                        tempProps.TimeFormatPattern = newPattern;
                        Add(format, tempProps);
                    }
                }

                public string GetTimeFormatPattern(DateTimeFormatInfo format)
                {
                    if (this[format] == null)
                        return "h:mm:ss tt";
                    else
                        return ((DateTimeFormatProperties)this[format]).TimeFormatPattern;
                }

                private class DateTimeFormatProperties
                {
                    public string DateFormatPattern = "d-MMM-yy";
                    public string TimeFormatPattern = "h:mm:ss tt";
                }
            }
        }

        /*******************************/

        public static string FormatDateTime(DateTimeFormatInfo format, DateTime date)
        {
            string timePattern = DateTimeFormatManager.manager.GetTimeFormatPattern(format);
            string datePattern = DateTimeFormatManager.manager.GetDateFormatPattern(format);
            return date.ToString(string.Format("{0} {1}", datePattern, timePattern), format);
        }

        /*******************************/

        public static DateTimeFormatInfo GetDateTimeFormatInstance(int dateStyle, int timeStyle, CultureInfo culture)
        {
            DateTimeFormatInfo format = culture.DateTimeFormat;

            switch (timeStyle)
            {
                case -1:
                    DateTimeFormatManager.manager.SetTimeFormatPattern(format, string.Empty);
                    break;

                case 0:
                    DateTimeFormatManager.manager.SetTimeFormatPattern(format, "h:mm:ss 'o clock' tt zzz");
                    break;

                case 1:
                    DateTimeFormatManager.manager.SetTimeFormatPattern(format, "h:mm:ss tt zzz");
                    break;

                case 2:
                    DateTimeFormatManager.manager.SetTimeFormatPattern(format, "h:mm:ss tt");
                    break;

                case 3:
                    DateTimeFormatManager.manager.SetTimeFormatPattern(format, "h:mm tt");
                    break;
            }

            switch (dateStyle)
            {
                case -1:
                    DateTimeFormatManager.manager.SetDateFormatPattern(format, string.Empty);
                    break;

                case 0:
                    DateTimeFormatManager.manager.SetDateFormatPattern(format, "dddd, MMMM dd%, yyy");
                    break;

                case 1:
                    DateTimeFormatManager.manager.SetDateFormatPattern(format, "MMMM dd%, yyy");
                    break;

                case 2:
                    DateTimeFormatManager.manager.SetDateFormatPattern(format, "d-MMM-yy");
                    break;

                case 3:
                    DateTimeFormatManager.manager.SetDateFormatPattern(format, "M/dd/yy");
                    break;
            }

            return format;
        }
    }
}
