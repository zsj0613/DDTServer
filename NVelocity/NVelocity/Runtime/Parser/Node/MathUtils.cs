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
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Util;

    /// <summary> Utility-class for all arithmetic-operations.<br><br>
    /// 
    /// All operations (+ - / *) return a Number which type is the type of the bigger argument.<br>
    /// Example:<br>
    /// <code>Add ( new Integer(10), new Integer(1))</code> will return an <code>Integer</code>-Object with the value 11<br>
    /// <code>Add ( new Long(10), new Integer(1))</code> will return an <code>Long</code>-Object with the value 11<br>
    /// <code>Add ( new Integer(10), new Float(1))</code> will return an <code>Float</code>-Object with the value 11<br><br>
    /// 
    /// Overflow checking:<br>
    /// For integral values (byte, short, int) there is an implicit overflow correction (the next "bigger"
    /// type will be returned). For example, if you call <code>Add (new Integer (Integer.MAX_VALUE), 1)</code> a
    /// <code>Long</code>-object will be returned with the correct value of <code>Integer.MAX_VALUE+1</code>.<br>
    /// In addition to that the methods <code>Multiply</code>,<code>Add</code> and <code>substract</code> implement overflow
    /// checks for <code>long</code>-values. That means that if an overflow occurs while working with long values a BigInteger
    /// will be returned.<br>
    /// For all other operations and types (such as Float and Double) there is no overflow checking.
    /// 
    /// </summary>
    /// <author>  <a href="mailto:pero@antaramusic.de">Peter Romianowski</a>
    /// </author>
    /// <since> 1.5
    /// </since>
    public class MathUtils
    {
        /// <summary> Add two numbers and return the correct value / type.
        /// Overflow detection is done for integer values (byte, short, int, long) only!
        /// </summary>
        /// <param name="op1">
        /// </param>
        /// <param name="op2">
        /// </param>
        /// <returns> Addition result.
        /// </returns>
        public static Object Add(Type maxType, Object op1, Object op2)
        {
            if (maxType == typeof(Double))
            {
                return Convert.ToDouble(op1) + Convert.ToDouble(op2);
            }
            else if (maxType == typeof(Single))
            {
                return Convert.ToSingle(op1) + Convert.ToSingle(op2);
            }
            else if (maxType == typeof(Decimal))
            {
                return Convert.ToDecimal(op1) + Convert.ToDecimal(op2);
            }
            else if (maxType == typeof(Int64))
            {
                return Convert.ToInt64(op1) + Convert.ToInt64(op2);
            }
            else if (maxType == typeof(Int32))
            {
                return Convert.ToInt32(op1) + Convert.ToInt32(op2);
            }
            else if (maxType == typeof(Int16))
            {
                return Convert.ToInt16(op1) + Convert.ToInt16(op2);
            }
            else if (maxType == typeof(SByte))
            {
                return Convert.ToSByte(op1) + Convert.ToSByte(op2);
            }
            else if (maxType == typeof(Byte))
            {
                return Convert.ToByte(op1) + Convert.ToByte(op2);
            }

            return 0;
        }

        /// <summary> Subtract two numbers and return the correct value / type.
        /// Overflow detection is done for integer values (byte, short, int, long) only!
        /// </summary>
        /// <param name="op1">
        /// </param>
        /// <param name="op2">
        /// </param>
        /// <returns> Subtraction result.
        /// </returns>
        public static Object Subtract(Type maxType, Object op1, Object op2)
        {
            if (maxType == typeof(Double))
            {
                return Convert.ToDouble(op1) - Convert.ToDouble(op2);
            }
            else if (maxType == typeof(Single))
            {
                return Convert.ToSingle(op1) - Convert.ToSingle(op2);
            }
            else if (maxType == typeof(Decimal))
            {
                return Convert.ToDecimal(op1) - Convert.ToDecimal(op2);
            }
            else if (maxType == typeof(Int64))
            {
                return Convert.ToInt64(op1) - Convert.ToInt64(op2);
            }
            else if (maxType == typeof(Int32))
            {
                return Convert.ToInt32(op1) - Convert.ToInt32(op2);
            }
            else if (maxType == typeof(Int16))
            {
                return Convert.ToInt16(op1) - Convert.ToInt16(op2);
            }
            else if (maxType == typeof(SByte))
            {
                return Convert.ToSByte(op1) - Convert.ToSByte(op2);
            }
            else if (maxType == typeof(Byte))
            {
                return Convert.ToByte(op1) - Convert.ToByte(op2);
            }

            return 0;
        }

        /// <summary> Multiply two numbers and return the correct value / type.
        /// Overflow detection is done for integer values (byte, short, int, long) only!
        /// </summary>
        /// <param name="op1">
        /// </param>
        /// <param name="op2">
        /// </param>
        /// <returns> Multiplication result.
        /// </returns>
        public static Object Multiply(Type maxType, Object op1, Object op2)
        {
            if (maxType == typeof(Double))
            {
                return Convert.ToDouble(op1) * Convert.ToDouble(op2);
            }
            else if (maxType == typeof(Single))
            {
                return Convert.ToSingle(op1) * Convert.ToSingle(op2);
            }
            else if (maxType == typeof(Decimal))
            {
                return Convert.ToDecimal(op1) * Convert.ToDecimal(op2);
            }
            else if (maxType == typeof(Int64))
            {
                return Convert.ToInt64(op1) * Convert.ToInt64(op2);
            }
            else if (maxType == typeof(Int32))
            {
                return Convert.ToInt32(op1) * Convert.ToInt32(op2);
            }
            else if (maxType == typeof(Int16))
            {
                return Convert.ToInt16(op1) * Convert.ToInt16(op2);
            }
            else if (maxType == typeof(SByte))
            {
                return Convert.ToSByte(op1) * Convert.ToSByte(op2);
            }
            else if (maxType == typeof(Byte))
            {
                return Convert.ToByte(op1) * Convert.ToByte(op2);
            }

            return 0;
        }

        /// <summary> Divide two numbers. The result will be returned as Integer-type if and only if
        /// both sides of the division operator are Integer-types. Otherwise a Float, Double,
        /// or BigDecimal will be returned.
        /// </summary>
        /// <param name="op1">
        /// </param>
        /// <param name="op2">
        /// </param>
        /// <returns> Division result.
        /// </returns>
        public static Object Divide(Type maxType, Object op1, Object op2)
        {

            if (maxType == typeof(Double))
            {
                return Convert.ToDouble(op1) / Convert.ToDouble(op2);
            }
            else if (maxType == typeof(Single))
            {
                return Convert.ToSingle(op1) / Convert.ToSingle(op2);
            }
            else if (maxType == typeof(Decimal))
            {
                return Convert.ToDecimal(op1) / Convert.ToDecimal(op2);
            }
            else if (maxType == typeof(Int64))
            {
                return Convert.ToInt64(op1) / Convert.ToInt64(op2);
            }
            else if (maxType == typeof(Int32))
            {
                return Convert.ToInt32(op1) / Convert.ToInt32(op2);
            }
            else if (maxType == typeof(Int16))
            {
                return Convert.ToInt16(op1) / Convert.ToInt16(op2);
            }
            else if (maxType == typeof(SByte))
            {
                return Convert.ToSByte(op1) / Convert.ToSByte(op2);
            }
            else if (maxType == typeof(Byte))
            {
                return Convert.ToByte(op1) / Convert.ToByte(op2);
            }

            return 0;
        }

        /// <summary> Modulo two numbers.</summary>
        /// <param name="op1">
        /// </param>
        /// <param name="op2">
        /// </param>
        /// <returns> Modulo result.
        /// 
        /// </returns>
        /// <throws>  ArithmeticException If at least one parameter is a BigDecimal </throws>
        public static Object Modulo(Type maxType, Object op1, Object op2)
        {
            if (maxType == typeof(Double))
            {
                return Convert.ToDouble(op1) % Convert.ToDouble(op2);
            }
            else if (maxType == typeof(Single))
            {
                return Convert.ToSingle(op1) % Convert.ToSingle(op2);
            }
            else if (maxType == typeof(Decimal))
            {
                return Convert.ToDecimal(op1) % Convert.ToDecimal(op2);
            }
            else if (maxType == typeof(Int64))
            {
                return Convert.ToInt64(op1) % Convert.ToInt64(op2);
            }
            else if (maxType == typeof(Int32))
            {
                return Convert.ToInt32(op1) % Convert.ToInt32(op2);
            }
            else if (maxType == typeof(Int16))
            {
                return Convert.ToInt16(op1) % Convert.ToInt16(op2);
            }
            else if (maxType == typeof(SByte))
            {
                return Convert.ToSByte(op1) % Convert.ToSByte(op2);
            }
            else if (maxType == typeof(Byte))
            {
                return Convert.ToByte(op1) % Convert.ToByte(op2);
            }

            return 0;
        }


        public static Type ToMaxType(Type leftType, Type rightType)
        {
            if (leftType == rightType) return leftType;

            if (leftType == typeof(Double) || rightType == typeof(Double))
            {
                return typeof(Double);
            }
            else if (leftType == typeof(Single) || rightType == typeof(Single))
            {
                return typeof(Single);
            }
            else if (leftType == typeof(Decimal) || rightType == typeof(Decimal))
            {
                return typeof(Decimal);
            }
            else if (leftType == typeof(Int64) || rightType == typeof(Int64))
            {
                return typeof(Int64);
            }
            else if (leftType == typeof(Int32) || rightType == typeof(Int32))
            {
                return typeof(Int32);
            }
            else if (leftType == typeof(Int16) || rightType == typeof(Int16))
            {
                return typeof(Int16);
            }
            else if (leftType == typeof(SByte) || rightType == typeof(SByte))
            {
                return typeof(SByte);
            }
            else if (leftType == typeof(Byte) || rightType == typeof(Byte))
            {
                return typeof(Byte);
            }

            return null;
        }
    }
}
