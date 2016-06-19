namespace NVelocity.Util
{
    using System;
    using System.Linq.Expressions;

    public static class Operator
    {

        public static bool HasValue<T>(T value)
        {
            return Operator<T>.NullOp.HasValue(value);
        }

        public static bool AddIfNotNull<T>(ref T accumulator, T value)
        {
            return Operator<T>.NullOp.AddIfNotNull(ref accumulator, value);
        }

        public static T Negate<T>(T value)
        {
            return Operator<T>.Negate(value);
        }

        public static T Not<T>(T value)
        {
            return Operator<T>.Not(value);
        }

        public static T Or<T>(T value1, T value2)
        {
            return Operator<T>.Or(value1, value2);
        }

        public static T And<T>(T value1, T value2)
        {
            return Operator<T>.And(value1, value2);
        }

        public static T Xor<T>(T value1, T value2)
        {
            return Operator<T>.Xor(value1, value2);
        }

        public static TTo Convert<TFrom, TTo>(TFrom value)
        {
            return Operator<TFrom, TTo>.Convert(value);
        }

        public static T Add<T>(T value1, T value2)
        {
            return Operator<T>.Add(value1, value2);
        }

        public static TArg1 AddAlternative<TArg1, TArg2>(TArg1 value1, TArg2 value2)
        {
            return Operator<TArg2, TArg1>.Add(value1, value2);
        }

        public static T Subtract<T>(T value1, T value2)
        {
            return Operator<T>.Subtract(value1, value2);
        }

        public static TArg1 SubtractAlternative<TArg1, TArg2>(TArg1 value1, TArg2 value2)
        {
            return Operator<TArg2, TArg1>.Subtract(value1, value2);
        }

        public static T Multiply<T>(T value1, T value2)
        {
            return Operator<T>.Multiply(value1, value2);
        }

        public static TArg1 MultiplyAlternative<TArg1, TArg2>(TArg1 value1, TArg2 value2)
        {
            return Operator<TArg2, TArg1>.Multiply(value1, value2);
        }

        public static T Divide<T>(T value1, T value2)
        {
            return Operator<T>.Divide(value1, value2);
        }

        public static TArg1 DivideAlternative<TArg1, TArg2>(TArg1 value1, TArg2 value2)
        {
            return Operator<TArg2, TArg1>.Divide(value1, value2);
        }

        public static bool Equal<T>(T value1, T value2)
        {
            return Operator<T>.Equal(value1, value2);
        }

        public static bool NotEqual<T>(T value1, T value2)
        {
            return Operator<T>.NotEqual(value1, value2);
        }

        public static bool GreaterThan<T>(T value1, T value2)
        {
            return Operator<T>.GreaterThan(value1, value2);
        }

        public static bool LessThan<T>(T value1, T value2)
        {
            return Operator<T>.LessThan(value1, value2);
        }

        public static bool GreaterThanOrEqual<T>(T value1, T value2)
        {
            return Operator<T>.GreaterThanOrEqual(value1, value2);
        }

        public static bool LessThanOrEqual<T>(T value1, T value2)
        {
            return Operator<T>.LessThanOrEqual(value1, value2);
        }

        public static T DivideInt32<T>(T value, int divisor)
        {
            return Operator<int, T>.Divide(value, divisor);
        }
    }

    public static class Operator<T>
    {
        static readonly INullOp<T> nullOp;
        internal static INullOp<T> NullOp { get { return nullOp; } }

        static readonly T zero;

        public static T Zero { get { return zero; } }

        static readonly Func<T, T> negate, not;
        static readonly Func<T, T, T> or, and, xor;

        public static Func<T, T> Negate { get { return negate; } }

        public static Func<T, T> Not { get { return not; } }

        public static Func<T, T, T> Or { get { return or; } }

        public static Func<T, T, T> And { get { return and; } }

        public static Func<T, T, T> Xor { get { return xor; } }

        static readonly Func<T, T, T> add, subtract, multiply, divide;

        public static Func<T, T, T> Add { get { return add; } }

        public static Func<T, T, T> Subtract { get { return subtract; } }

        public static Func<T, T, T> Multiply { get { return multiply; } }

        public static Func<T, T, T> Divide { get { return divide; } }

        static readonly Func<T, T, bool> equal, notEqual, greaterThan, lessThan, greaterThanOrEqual, lessThanOrEqual;

        public static Func<T, T, bool> Equal { get { return equal; } }

        public static Func<T, T, bool> NotEqual { get { return notEqual; } }

        public static Func<T, T, bool> GreaterThan { get { return greaterThan; } }

        public static Func<T, T, bool> LessThan { get { return lessThan; } }

        public static Func<T, T, bool> GreaterThanOrEqual { get { return greaterThanOrEqual; } }

        public static Func<T, T, bool> LessThanOrEqual { get { return lessThanOrEqual; } }

        static Operator()
        {
            add = ExpressionUtil.CreateExpression<T, T, T>(Expression.Add);
            subtract = ExpressionUtil.CreateExpression<T, T, T>(Expression.Subtract);
            divide = ExpressionUtil.CreateExpression<T, T, T>(Expression.Divide);
            multiply = ExpressionUtil.CreateExpression<T, T, T>(Expression.Multiply);

            greaterThan = ExpressionUtil.CreateExpression<T, T, bool>(Expression.GreaterThan);
            greaterThanOrEqual = ExpressionUtil.CreateExpression<T, T, bool>(Expression.GreaterThanOrEqual);
            lessThan = ExpressionUtil.CreateExpression<T, T, bool>(Expression.LessThan);
            lessThanOrEqual = ExpressionUtil.CreateExpression<T, T, bool>(Expression.LessThanOrEqual);
            equal = ExpressionUtil.CreateExpression<T, T, bool>(Expression.Equal);
            notEqual = ExpressionUtil.CreateExpression<T, T, bool>(Expression.NotEqual);

            negate = ExpressionUtil.CreateExpression<T, T>(Expression.Negate);
            and = ExpressionUtil.CreateExpression<T, T, T>(Expression.And);
            or = ExpressionUtil.CreateExpression<T, T, T>(Expression.Or);
            not = ExpressionUtil.CreateExpression<T, T>(Expression.Not);
            xor = ExpressionUtil.CreateExpression<T, T, T>(Expression.ExclusiveOr);

            Type typeT = typeof(T);

            if (typeT.IsValueType && typeT.IsGenericType && (typeT.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                Type nullType = typeT.GetGenericArguments()[0];
                zero = (T)Activator.CreateInstance(nullType);
                nullOp = (INullOp<T>)Activator.CreateInstance(
                        typeof(StructNullOp<>).MakeGenericType(nullType));
            }
            else
            {
                zero = default(T);

                if (typeT.IsValueType)
                {
                    nullOp = (INullOp<T>)Activator.CreateInstance(
                        typeof(StructNullOp<>).MakeGenericType(typeT));
                }
                else
                {
                    nullOp = (INullOp<T>)Activator.CreateInstance(
                        typeof(ClassNullOp<>).MakeGenericType(typeT));
                }
            }
        }
    }

    public static class Operator<TValue, TResult>
    {
        private static readonly Func<TValue, TResult> convert;

        public static Func<TValue, TResult> Convert { get { return convert; } }

        static Operator()
        {
            convert = ExpressionUtil.CreateExpression<TValue, TResult>(body => Expression.Convert(body, typeof(TResult)));
            add = ExpressionUtil.CreateExpression<TResult, TValue, TResult>(Expression.Add, true);
            subtract = ExpressionUtil.CreateExpression<TResult, TValue, TResult>(Expression.Subtract, true);
            multiply = ExpressionUtil.CreateExpression<TResult, TValue, TResult>(Expression.Multiply, true);
            divide = ExpressionUtil.CreateExpression<TResult, TValue, TResult>(Expression.Divide, true);
        }

        private static readonly Func<TResult, TValue, TResult> add, subtract, multiply, divide;

        public static Func<TResult, TValue, TResult> Add { get { return add; } }

        public static Func<TResult, TValue, TResult> Subtract { get { return subtract; } }

        public static Func<TResult, TValue, TResult> Multiply { get { return multiply; } }

        public static Func<TResult, TValue, TResult> Divide { get { return divide; } }
    }

}
