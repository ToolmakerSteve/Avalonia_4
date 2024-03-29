﻿// Based on https://referencesource.microsoft.com/#System.Numerics/System/Numerics/Vector2_Intrinsics.cs
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Runtime.CompilerServices;

namespace OU
{
    // This file contains the definitions for all of the JIT intrinsic methods and properties that are recognized by the current x64 JIT compiler.
    // The implementation defined here is used in any circumstance where the JIT fails to recognize these members as intrinsic.
    // The JIT recognizes these methods and properties by name and signature: if either is changed, the JIT will no longer recognize the member.
    // Some methods declared here are not strictly intrinsic, but delegate to an intrinsic method. For example, only one overload of CopyTo() 

    public partial struct Vec2
    {
        #region --- data ----------------------------------------
        /// <summary>
        /// The X component of the vector.
        /// </summary>
        public float X;
        /// <summary>
        /// The Y component of the vector.
        /// </summary>
		public float Y;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a vector whose elements are all the single specified value.
        /// </summary>
        /// <param name="value">The element to fill the vector with.</param>
        ///[JitIntrinsic]
        public Vec2(Single value) : this(value, value) { }

        /// <summary>
        /// Constructs a vector with the given individual elements.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        ///[JitIntrinsic]
        public Vec2(Single x, Single y)
        {
            X = x;
            Y = y;
        }
        #endregion Constructors

        #region Public Instance Methods
        /// <summary>
        /// Copies the contents of the vector into the given array.
        /// </summary>
        /// <param name="array">The destination array.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(Single[] array)
        {
            CopyTo(array, 0);
        }

        /// <summary>
        /// Copies the contents of the vector into the given array, starting from the given index.
        /// </summary>
        /// <exception cref="ArgumentNullException">If array is null.</exception>
        /// <exception cref="RankException">If array is multidimensional.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If index is greater than end of the array or index is less than zero.</exception>
        /// <exception cref="ArgumentException">If number of elements in source vector is greater than those available in destination array
        /// or if there are not enough elements to copy.</exception>
        public void CopyTo(Single[] array, int index)
        {
            if (array == null)
            {
                // Match the JIT's exception type here. For perf, a NullReference is thrown instead of an ArgumentNull.
                throw new NullReferenceException(/*SR.GetString*/("Arg_NullArgumentNullRef"));
            }
            if (index < 0 || index >= array.Length)
            {
                throw new ArgumentOutOfRangeException(/*SR.GetString*/($"Arg_ArgumentOutOfRangeException, {index}"));
            }
            if ((array.Length - index) < 2)
            {
                throw new ArgumentException(/*SR.GetString*/($"Arg_ElementsInSourceIsGreaterThanDestination, {index}"));
            }
            array[index] = X;
            array[index + 1] = Y;
        }

        /// <summary>
        /// Returns a boolean indicating whether the given Vec2 is equal to this Vec2 instance.
        /// </summary>
        /// <param name="other">The Vec2 to compare this instance to.</param>
        /// <returns>True if the other Vec2 is equal to this instance; False otherwise.</returns>
        ///[JitIntrinsic]
        public bool Equals(Vec2 other)
        {
            return this.X == other.X && this.Y == other.Y;
        }
        #endregion Public Instance Methods

        #region Public Static Methods
        /// <summary>
        /// Returns the dot product of two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The dot product.</returns>
        ///[JitIntrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Vec2 value1, Vec2 value2)
        {
            return value1.X * value2.X +
                   value1.Y * value2.Y;
        }

        /// <summary>
        /// Returns a vector whose elements are the minimum of each of the pairs of elements in the two source vectors.
        /// </summary>
        /// <param name="value1">The first source vector.</param>
        /// <param name="value2">The second source vector.</param>
        /// <returns>The minimized vector.</returns>
        ///[JitIntrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 Min(Vec2 value1, Vec2 value2)
        {
            return new Vec2(
                (value1.X < value2.X) ? value1.X : value2.X,
                (value1.Y < value2.Y) ? value1.Y : value2.Y);
        }

        /// <summary>
        /// Returns a vector whose elements are the maximum of each of the pairs of elements in the two source vectors
        /// </summary>
        /// <param name="value1">The first source vector</param>
        /// <param name="value2">The second source vector</param>
        /// <returns>The maximized vector</returns>
        ///[JitIntrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 Max(Vec2 value1, Vec2 value2)
        {
            return new Vec2(
                (value1.X > value2.X) ? value1.X : value2.X,
                (value1.Y > value2.Y) ? value1.Y : value2.Y);
        }

        /// <summary>
        /// Returns a vector whose elements are the absolute values of each of the source vector's elements.
        /// </summary>
        /// <param name="value">The source vector.</param>
        /// <returns>The absolute value vector.</returns>        
        ///[JitIntrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 Abs(Vec2 value)
        {
            return new Vec2(Math.Abs(value.X), Math.Abs(value.Y));
        }

        /// <summary>
        /// Returns a vector whose elements are the square root of each of the source vector's elements.
        /// </summary>
        /// <param name="value">The source vector.</param>
        /// <returns>The square root vector.</returns>
        ///[JitIntrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 SquareRoot(Vec2 value)
        {
            return new Vec2((Single)Math.Sqrt(value.X), (Single)Math.Sqrt(value.Y));
        }
        #endregion Public Static Methods

        #region Public Static Operators
        /// <summary>
        /// Adds two vectors together.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The summed vector.</returns>
        ///[JitIntrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator +(Vec2 left, Vec2 right)
        {
            return new Vec2(left.X + right.X, left.Y + right.Y);
        }

        /// <summary>
        /// Subtracts the second vector from the first.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The difference vector.</returns>
        ///[JitIntrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator -(Vec2 left, Vec2 right)
        {
            return new Vec2(left.X - right.X, left.Y - right.Y);
        }

        /// <summary>
        /// Multiplies two vectors together.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The product vector.</returns>
        ///[JitIntrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator *(Vec2 left, Vec2 right)
        {
            return new Vec2(left.X * right.X, left.Y * right.Y);
        }

        /// <summary>
        /// Multiplies a vector by the given scalar.
        /// </summary>
        /// <param name="left">The scalar value.</param>
        /// <param name="right">The source vector.</param>
        /// <returns>The scaled vector.</returns>
        ///[JitIntrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator *(Single left, Vec2 right)
        {
            return new Vec2(left, left) * right;
        }

        /// <summary>
        /// Multiplies a vector by the given scalar.
        /// </summary>
        /// <param name="left">The source vector.</param>
        /// <param name="right">The scalar value.</param>
        /// <returns>The scaled vector.</returns>
        ///[JitIntrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator *(Vec2 left, Single right)
        {
            return left * new Vec2(right, right);
        }

        /// <summary>
        /// Divides the first vector by the second.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The vector resulting from the division.</returns>
        ///[JitIntrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator /(Vec2 left, Vec2 right)
        {
            return new Vec2(left.X / right.X, left.Y / right.Y);
        }

        /// <summary>
        /// Divides the vector by the given scalar.
        /// </summary>
        /// <param name="value1">The source vector.</param>
        /// <param name="value2">The scalar value.</param>
        /// <returns>The result of the division.</returns>
        ///[JitIntrinsic]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator /(Vec2 value1, float value2)
        {
            float invDiv = 1.0f / value2;
            return new Vec2(
                value1.X * invDiv,
                value1.Y * invDiv);
        }

        /// <summary>
        /// Negates a given vector.
        /// </summary>
        /// <param name="value">The source vector.</param>
        /// <returns>The negated vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator -(Vec2 value)
        {
            return Zero - value;
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given vectors are equal.
        /// </summary>
        /// <param name="left">The first vector to compare.</param>
        /// <param name="right">The second vector to compare.</param>
        /// <returns>True if the vectors are equal; False otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vec2 left, Vec2 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given vectors are not equal.
        /// </summary>
        /// <param name="left">The first vector to compare.</param>
        /// <param name="right">The second vector to compare.</param>
        /// <returns>True if the vectors are not equal; False if they are equal.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vec2 left, Vec2 right)
        {
            return !(left == right);
        }
        #endregion Public Static Operators
    }
}
