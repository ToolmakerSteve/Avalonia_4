using Urho;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Diagnostics;
using System.Collections;
//using System.Numerics;

namespace OU
{
    /// <summary>
    /// Miscellaneous functions.
    /// TODO: Move Math functions to Utils.Math.cs.
    /// TODO: Change function names to PascalCase (capitalize first letter).
    /// </summary>
    public static partial class Utils
    {
        public delegate int CompareFunction(object obj1, object obj2);
        public delegate void ForEachFunction(object Key, object UserData);
        public delegate void DelegateForEach(object obj);
        public delegate void DelegateForEachTyped<T>(T obj);
        public delegate bool DelegateMatch(object Value);
        public delegate bool DelegateMatchTyped<T>(T Value);
        public delegate bool DelegateMatchData(object Value, object UserData);
        public delegate bool DelegateMatchDataTyped<T>(T Value, T UserData);
        public delegate Dist3D TwoDWithZ_P3DDelegate(Dist2D xy, double z);
        public delegate Dist2D ThreeD_P2DDelegate(Dist3D xyz);


        #region --- debug functions ----------------------------------------
        // (aka "NoOp"): Indicates deliberate absence of action.
        public static void DoNothing()
        {
        }

        // Used during testing, to add a line where a breakpoint can be set.
        public static void Test()
        {
        }

        // Test point that we hope to reach - good to get here.
        public static void Good()
        {
        }

        // Test point that represents a possible problem.
        public static void Dubious()
        {
            Dubious("unspecified");
        }

        private readonly static HashSet<string> s_dubiousSeen = new HashSet<string>();

        // Only Write these errors ONCE each. (might flood output).
        // When msg is formatted string (so different for each call), use tag so know is same type of message.
        public static void Dubious(string msg_or_format, params object[] args) // Optional tag As String = "")
        {
            if (!s_dubiousSeen.Contains(msg_or_format))
            {
                s_dubiousSeen.Add(msg_or_format);
                string msg = args.Count() > 0 ? string.Format(msg_or_format, args) : msg_or_format;
                Debug.WriteLine("***** Dubious: " + msg + " *****");
            }
        }

        // Test point that represents a definite problem.
        public static void Trouble(string msg = "")
        {
            Debug.WriteLine("***** Trouble: " + msg + " *****");
        }

        public static void Trouble()
        {
            Trouble("unspecified");
        }

        public static void Trouble(string msg_or_format, params object[] args)
        {
            string msg = args.Count() > 0 ? string.Format(msg_or_format, args) : msg_or_format;
            Trouble(msg);
        }

        // Indicates code needs to be written.
        public static void TODO()
        {
        }

        // Convenience: Set breakpoint here to find places that likely need code work.
        public static void TODO(string msg = "")
        {
            if (string.IsNullOrEmpty(msg))
                msg = "(Unspecified)";
            Dubious("TODO: " + msg);
        }


        public static void Kludge()
        {
            Kludge("unspecified");
        }

        private readonly static HashSet<string> s_kludgeSeen = new HashSet<string>();

        // Only Write these ONCE each. (Not error)
        public static void Kludge(string msg)
        {
            if (!s_kludgeSeen.Contains(msg))
            {
                s_kludgeSeen.Add(msg);
                Debug.WriteLine("+++++ Kludge: " + msg + " +++++");
            }
        }

        /// <summary>
        /// Debug.WriteLine if msg different than most recent msg.
        /// </summary>
        /// <param name="msg"></param>
        static public void DebugWriteLineIfChange(string msg)
        {
            if (!msg.Equals(_prevDebugMsg))
            {
                Debug.WriteLine(msg);
                _prevDebugMsg = msg;
            }
        }
        static private string _prevDebugMsg = "";
        #endregion


        //        public static bool Exists(object ob)
        //        {
        //            return (ob != null);
        //        }

        //        // Useful when have "A?.B", where "B" is Boolean.
        //        // Because "A?" turns that into a "Boolean?".
        //        public static bool ExistsAndTrue(bool? b)
        //        {
        //            return (b.HasValue && b.Value);
        //        }


        //        // useful when "collection" allowed to be nothing; "defaultResponse" returned then.
        //        public static bool ContainsOrDefault<T>(T value, ICollection<T> collection, bool defaultResponse)
        //        {
        //            if (Exists(collection))
        //                return collection.Contains(value);
        //            else
        //                return defaultResponse;
        //        }


        //        // Equivalent to "TypeOf ob Is SomeType", where "SomeType" is name of a type;
        //        // here, we have a type variable rather than a type name.
        //        // At the time this was written, I had not found the built-in function that it now calls.
        //        public static bool IsInstanceOf(object ob, Type ty)
        //        {
        //            return ty.IsInstanceOfType(ob);
        //        }

        //        // Its like "IsInstanceOf", but when you wish to test a type, rather than an object.
        //        // At the time this was written, I had not found the built-in function that it now calls.
        //        // (The meaning of this name, from the point-of-view of the descendent "ty", is also more intuitive, given how I use it.)
        //        public static bool IsSameOrDescendentOf(Type ty, Type goalType)
        //        {
        //            return goalType.IsAssignableFrom(ty);
        //        }

        //        // A random number generator instance.
        //        public static Random Random1 = new Random();

        //        // Return integer in (0..limit-1).
        //        public static int RandomInteger(int limit, Random rand = null)
        //        {
        //            if (rand == null)
        //                rand = Random1;
        //            return (int)(Math.Floor(limit * rand.NextDouble()));
        //        }

        //        // NOTE: Won't quite reach "limit" (though will get very close).
        //        public static double RandomIn(double min, double limit, Random rand = null)
        //        {
        //            if (rand == null)
        //                rand = Random1;
        //            double delta = limit - min;
        //            return (min + delta * rand.NextDouble());
        //        }

        //        // Seconds, to nearest millisecond ("Round 3").
        //        public static float SecondsR3(TimeSpan span)
        //        {
        //            return System.Convert.ToSingle(Round3(span.TotalSeconds));
        //        }

        // Helpers for making GetHashCode result, by combining fields.
        // Generic parameters, to avoid boxing overhead.
        public static int MakeHash<T1, T2>(T1 val1, T2 val2)
        {
            // Combine using primes. Largest prime is < 2^30, so that the added values don't overflow long. First prime from http://planetmath.org/goodhashtableprimes.
            // Second prime chosen so that if val1 and val2 are small and near each other, the two halves will tend to set different bits.
            // Specifically, chose a prime number < 2^28 (268 435 456).
            // finding a prime number, far from a power of 2: http://www.wolframalpha.com/input/?i=NextPrime%5B2%5E27+*+1.414,1%5D
            long accum = System.Convert.ToInt64(GetHashCode_OrZero(val1)) * 805306457 + System.Convert.ToInt64(GetHashCode_OrZero(val2)) * 189783887; // 268435399 '999991 '49157
            int hash1 = accum.GetHashCode();
            return hash1;
        }

        public static int MakeHash<T1, T2, T3>(T1 val1, T2 val2, T3 val3)
        {
            // Combine using primes. Largest prime is < 2^30, so that the added values don't overflow long. Primes from http://planetmath.org/goodhashtableprimes.
            // Second prime chosen so that if val1 and val2 are small and near each other, the two halves will be far apart.
            // Specifically, chose a prime number < 2^16.
            // Third chosen from that primes table, < 2^23
            long accum = System.Convert.ToInt64(GetHashCode_OrZero(val1)) * 805306457 + System.Convert.ToInt64(GetHashCode_OrZero(val2)) * 189783887 + System.Convert.ToInt64(GetHashCode_OrZero(val3)) * 12582917;
            int hash1 = accum.GetHashCode();
            return hash1;
        }

        public static int MakeHash<T1, T2, T3, T4>(T1 val1, T2 val2, T3 val3, T4 val4)
        {
            // Combine using primes. Largest prime is < 2^30, so that the added values don't overflow long. Primes from http://planetmath.org/goodhashtableprimes.
            // Fourth is < 2^20, from wolframalpha.
            long accum = System.Convert.ToInt64(GetHashCode_OrZero(val1)) * 805306457 + System.Convert.ToInt64(GetHashCode_OrZero(val2)) * 189783887 + System.Convert.ToInt64(GetHashCode_OrZero(val3)) * 12582917 + System.Convert.ToInt64(GetHashCode_OrZero(val4)) * 741347; // 1048573 '389
            int hash1 = accum.GetHashCode();
            return hash1;
        }

        //        // If add more parameters, use primes about half as large each time. From that table:  .., 50331653, 25165843, 12582917, 6291469.


        public const int INVALID_SIZE = -1;


        public const int FIRST_INDEX = 0;
        public const int INVALID_INDEX = -1;
        public const int INVALID_POSITIVE_INTEGER = -1;

        // Given start and end indices, return count of elements.
        public static int CountFromStartEnd(int iStart, int iEnd)
        {
            return iEnd - iStart + 1;
        }

        public static int LastIndex<T>(this T[] a)
        {
            return a.Length - 1;
        }
        /// <summary>
        ///     ''' TBD: Why is this necessary? Get compiler complaint if remove, even though Array inherits from ICollection.
        ///     ''' </summary>
        ///     ''' <param name="a"></param>
        ///     ''' <returns></returns>
        public static int LastIndex(this Array a)
        {
            return a.Length - 1;
        }
        public static int LastIndex<T>(this List<T> a)
        {
            return a.Count - 1;
        }
        public static int LastIndex<T>(this IList<T> a)
        {
            return a.Count - 1;
        }
        public static int LastIndex<T>(this ICollection<T> a)
        {
            return a.Count - 1;
        }
        public static int LastIndex(this ICollection a)
        {
            return a.Count - 1;
        }
        public static int LastIndex(this string a)
        {
            return a.Length - 1;
        }

        //        // Returns nothing if index out of range.
        //        public static T ElementSafe<T>(T[] list, int index)
        //        {
        //            return ValidIndex(list, index) ? list[index] : default(T);
        //        }
        //        // Returns nothing if index out of range.
        //        public static T ElementSafe<T>(List<T> list, int index)
        //        {
        //            return ValidIndex<T>(list, index) ? list[index] : default(T);
        //        }

        //        public static bool ValidIndex<T>(T[] list, int index)
        //        {
        //            return ((index >= 0) && (index <= LastIndex(list)));
        //        }
        //        public static bool ValidIndex(Array list, int index)
        //        {
        //            return ((index >= 0) && (index <= LastIndex(list)));
        //        }
        //        public static bool ValidIndex<T>(List<T> list, int index)
        //        {
        //            return ((index >= 0) && (index <= LastIndex(list)));
        //        }
        //        public static bool ValidIndex<T>(IList<T> list, int index)
        //        {
        //            return ((index >= 0) && (index <= LastIndex(list)));
        //        }
        //        public static bool ValidIndex<T>(ICollection<T> list, int index)
        //        {
        //            return ((index >= 0) && (index <= LastIndex(list)));
        //        }
        //        public static bool ValidIndex(ICollection list, int index)
        //        {
        //            return ((index >= 0) && (index <= LastIndex(list)));
        //        }

        //        public static bool HasElements<T>(T[] a)
        //        {
        //            return Exists(a) && (a.Length > 0);
        //        }
        //        // NOTE: Equivalent to "NotEmpty" or "HasContents", which are more meaningful names for a string.
        //        public static bool HasElements(string a)
        //        {
        //            return Exists(a) && (a.Length > 0);
        //        }
        //        // NOTE: Equivalent to "HasElements".
        //        public static bool HasContents(string a)
        //        {
        //            return Exists(a) && (a.Length > 0);
        //        }

        //        public static bool HasElements<T>(ICollection<T> a)
        //        {
        //            return Exists(a) && (a.Count > 0);
        //        }


        //        // At least one non-default element. (Usually used with "T" as a reference type, to test for non-Nothing element.)
        //        public static bool SomeElementExists<T>(T[] a)
        //        {
        //            if (HasElements(a))
        //            {
        //                foreach (T element in a)
        //                {
        //                    if (Exists(element))
        //                        return true;
        //                }
        //            }

        //            return false;
        //        }

        //        // Returns "a(index)" if it exists, otherwise returns Nothing.
        //        public static T TryElement<T>(List<T> a, int index) where T : class
        //        {
        //            if (a == null)
        //                return null;
        //            if (index < 0)
        //                return null;
        //            if (a.Count < index + 1)
        //                return null;

        //            return a[index];
        //        }

        //        public static T TryFirstElement<T>(List<T> a) where T : class
        //        {
        //            return TryElement(a, 0);
        //        }

        //        public static T TryLastElement<T>(List<T> a) where T : class
        //        {
        //            if (a == null || a.Count == 0)
        //                return null;
        //            return TryElement(a, a.Count - 1);
        //        }

        //        // Exception if a.Length = 0.
        //        public static T FirstElement<T>(T[] a)
        //        {
        //            return a[0];
        //        }
        //        // Exception if a.Length = 0.
        //        public static T FirstElement<T>(List<T> a)
        //        {
        //            return a[0];
        //        }
        //        // Exception if a.Length = 0.
        //        public static T FirstElement<T>(IList<T> a)
        //        {
        //            return a[0];
        //        }

        //        // Exception if a.Length = 0. Same as "NearEndElement(a, -1)".
        //        public static T LastElement<T>(T[] a)
        //        {
        //            return a[LastIndex(a)];
        //        }
        //        // Exception if a.Length = 0. Same as "NearEndElement(a, -1)".
        //        public static T LastElement<T>(List<T> a)
        //        {
        //            return a[LastIndex(a)];
        //        }
        //        // Exception if a.Length = 0. Same as "NearEndElement(a, -1)".
        //        public static T LastElement<T>(IList<T> a)
        //        {
        //            return a[LastIndex(a)];
        //        }
        //        // Exception if a.Length = 0.
        //        public static char LastElement(string a)
        //        {
        //            return a[LastIndex(a)];
        //        }

        //        // "index=-1" for last element, "-2" for preceding element, "-3" is before "-2", ..
        //        public static T NearEndElement<T>(T[] a, int index)
        //        {
        //            if (index >= 0)
        //                throw new InvalidProgramException("NearEndElement index must be negative.");
        //            // 
        //            return a[a.Length + index];
        //        }
        //        // "index=-1" for last element, "-2" for preceding element, "-3" is before "-2", ..
        //        public static T NearEndElement<T>(List<T> a, int index)
        //        {
        //            if (index >= 0)
        //                throw new InvalidProgramException("NearEndElement index must be negative.");
        //            // 
        //            return a[a.Count + index];
        //        }
        //        // "index=-1" for last element, "-2" for preceding element, "-3" is before "-2", ..
        //        public static T NearEndElement<T>(IList<T> a, int index)
        //        {
        //            if (index >= 0)
        //                throw new InvalidProgramException("NearEndElement index must be negative.");
        //            // 
        //            return a[a.Count + index];
        //        }

        //        // SIDE-EFFECT: Modifies the last element of "a".
        //        // Exception if a.Length = 0.
        //        public static void SetLastElement<T>(T[] a, T value)
        //        {
        //            a[LastIndex(a)] = value;
        //        }
        //        // SIDE-EFFECT: Modifies the last element of "a".
        //        // Exception if a.Length = 0.
        //        public static void SetLastElement<T>(List<T> a, T value)
        //        {
        //            a[LastIndex(a)] = value;
        //        }
        //        // SIDE-EFFECT: Modifies the last element of "a".
        //        // Exception if a.Length = 0.
        //        public static void SetLastElement<T>(IList<T> a, T value)
        //        {
        //            a[LastIndex(a)] = value;
        //        }

        //        // "n=1" returns LastElement; "n=2" returns element before LastElement, etc.
        //        // Exception if no such element.
        //        public static T LastElementN<T>(IList<T> a, int n)
        //        {
        //            return a[a.Count - n];
        //        }

        //        // Return N elements, from end of a.
        //        public static List<T> CopyLastN<T>(List<T> a, int n)
        //        {
        //            int startIndex = a.Count - n;
        //            return a.GetRange(startIndex, n);
        //        }

        //        // Return elements of a, starting with a(index), through last element of a.
        //        public static List<T> CopyIToEnd<T>(List<T> a, int index)
        //        {
        //            int keepCount = a.Count - index;
        //            return a.GetRange(index, keepCount);
        //        }

        //        // Insert before final "n" elements of list.
        //        // E.g. "n=1" inserts before last element, "n=2" inserts before last 2 elements.
        //        public static void InsertBeforeLastN<T>(this IList<T> a, int n, T value)
        //        {
        //            int insertIndex = a.Count - n;
        //            a.Insert(insertIndex, value);
        //        }

        //        /// <summary>
        //        ///     ''' Remove final "n" elements of list.
        //        ///     ''' </summary>
        //        ///     ''' <typeparam name="T"></typeparam>
        //        ///     ''' <param name="a"></param>
        //        ///     ''' <param name="n"></param>
        //        public static void RemoveLastN<T>(this List<T> a, int n)
        //        {
        //            a.RemoveRange(a.Count - n, n);
        //        }

        //        // Remove element at index "index". Result is one element shorter.
        //        // Similar to List.RemoveAt, but for arrays.
        //        public static void RemoveAt<T>(this T[] a, int index)
        //        {
        //            // Move elements after "index" down 1 position.
        //            Array.Copy(a, index + 1, a, index, Information.UBound(a) - index);
        //            var oldA = a;
        //            a = new T[Information.UBound(a) - 1 + 1];
        //            // Shorten by 1 element.
        //            if (oldA != null)
        //                Array.Copy(oldA, a, Math.Min(Information.UBound(a) - 1 + 1, oldA.Length));
        //        }

        //        public static void DropFirstElement<T>(this T[] a)
        //        {
        //            a.RemoveAt(0);
        //        }

        //        public static void DropLastElement<T>(this T[] a)
        //        {
        //            a.RemoveAt(Information.UBound(a));
        //        }

        //        // SIDE-EFFECT: a is increased in length, with value as new final element.
        //        public static void Append<T>(this T[] a, T value)
        //        {
        //            var oldA = a;
        //            a = new T[Information.UBound(a) + 1 + 1];
        //            // Lengthen by 1 element. SIDE-EFFECT: Increases UBound by 1.
        //            if (oldA != null)
        //                Array.Copy(oldA, a, Math.Min(Information.UBound(a) + 1 + 1, oldA.Length));
        //            // Set value of (new) final element.
        //            a[Information.UBound(a)] = value;
        //        }


        //        public static bool Contains(Array list, object target)
        //        {
        //            return (Array.IndexOf(list, target) >= 0);
        //        }


        //        // For displaying list elements, e.g. "{11, 22, 333}".
        //        // Not a good idea to call this on a very long list!
        //        public static string ElementsToString<T>(IList<T> a)
        //        {
        //            StringBuilder sb = new StringBuilder();
        //            sb.Append("{");

        //            bool isFirst = true;
        //            foreach (T element in a)
        //            {
        //                if (isFirst)
        //                    isFirst = false;
        //                else
        //                    sb.Append(", ");

        //                sb.Append(element.ToString());
        //            }

        //            sb.Append("}");
        //            return sb.ToString();
        //        }


        //        public static bool ElementsEqual<T>(T[] a, T[] b)
        //        {
        //            if (a == null)
        //                return (b == null);

        //            if (a.Length != b.Length)
        //                return false;

        //            for (int i = 0; i <= LastIndex(a); i++)
        //            {
        //                if (!a[i].Equals(b[i]))
        //                    return false;
        //            }

        //            return true;
        //        }

        //        public static bool ElementsEqual<T>(IList<T> a, IList<T> b)
        //        {
        //            if (a == null)
        //                return (b == null);

        //            if (a.Count != b.Count)
        //                return false;

        //            for (int i = 0; i <= LastIndex(a); i++)
        //            {
        //                if (!a[i].Equals(b[i]))
        //                    return false;
        //            }

        //            return true;
        //        }


        //        public static int WrapIt(this int i, int nWrap)
        //        {
        //            if (i < 0)
        //                return (i + nWrap);
        //            if (i >= nWrap)
        //                return (i - nWrap);
        //            return i;
        //        }

        //        public static IEnumerable<T> Backwards<T>(this IList<T> list1)
        //        {
        //            for (int i = LastIndex(list1); i >= FIRST_INDEX; i += -1)
        //                yield return list1[i];
        //        }

        //        /// <summary> "NoDup": Avoids duplicating existing point.
        //        ///     ''' </summary>
        //        ///     ''' <param name="list1"></param>
        //        ///     ''' <param name="previousT"></param>
        //        ///     ''' <param name="tolerance"></param>
        //        ///     ''' <returns></returns>
        //        public static IEnumerable<Distance2D> BackwardsNoDup(this IList<Distance2D> list1, Distance2D? previousT, double tolerance = EpsilonForOne)
        //        {
        //            int iLast = LastIndex(list1);
        //            for (int i = iLast; i >= FIRST_INDEX; i += -1)
        //            {
        //                // Depending on whether appending or prepending, duplication might be at either first or last iterator item.
        //                if (i == FIRST_INDEX || i == iLast)
        //                {
        //                    if (previousT.HasValue && list1[i].NearlyEquals(previousT.Value, tolerance))
        //                        continue;
        //                }

        //                yield return list1[i];
        //            }
        //        }

        //        /// <summary> "NoDup": Avoids duplicating existing point.
        //        ///     ''' </summary>
        //        ///     ''' <param name="list1"></param>
        //        ///     ''' <param name="previousT"></param>
        //        ///     ''' <param name="tolerance"></param>
        //        ///     ''' <returns></returns>
        //        public static IEnumerable<Distance2D> ForwardNoDup(this IList<Distance2D> list1, Distance2D? previousT, double tolerance = EpsilonForOne)
        //        {
        //            int iLast = LastIndex(list1);
        //            for (int i = FIRST_INDEX; i <= iLast; i++)
        //            {
        //                // Depending on whether appending or prepending, duplication might be at either first or last iterator item.
        //                if (i == FIRST_INDEX || i == iLast)
        //                {
        //                    if (previousT.HasValue && list1[i].NearlyEquals(previousT.Value, tolerance))
        //                        continue;
        //                }

        //                yield return list1[i];
        //            }
        //        }


        // Used with NearlyEquals_ScaledTolerance and GreaterThan_ScaledTolerance (with Double parameters).
        public static double ScaledToleranceDouble = Math.Pow(10, -14);

        /// <summary>Using Float Precision, closest value that can be achieved just below 1 (equals: 0.99999994f).</summary>
        public const float MaxFloatBelowOne = 0.9999999F;    // Float can't represent this exactly, so will be slightly different.

        public const float NearOne = MaxFloatBelowOne;
        public const double NearZeroX2 = 2 * NearZero;  // 0.0000024
        /// <summary> For situations (such as matrix compose/decompose) where errors may be significantly larger than last bit.</summary>
        public const double NearZeroX10 = 10 * NearZero;  // 0.0000012

        public const float NearNegativeOne = -NearOne;
        public const float NearNegativeZero = (float)-NearZero;
        public const float NearAboveOne = (float)(1 + NearZero);


        // or could use NearZeroRadians = .001;
        public const float NearZeroRadians = 0.0008457279F;  // Quaternion's W may round to 1 below this.

        public const double EpsilonForOne = NearZeroX10;    // Small relative to 1.0. For double, could use even smaller value.
        public const float EpsilonForOneF = (float)NearZeroX10;   // Small relative to 1.0

        //        // scaled tolerance wouldn't make sense for Integer, so this is an absolute (unscaled) tolerance.
        //        public static bool NearlyEquals_Integer(this int number, int target, int tolerance)
        //        {
        //            return (Math.Abs(number - target) <= tolerance);
        //        }

        //        // A reasonable "toleranceFraction" is CSng(NearZeroX2) or CSng(NearZeroX10). Larger if multiple operations may accumulate error.
        //        public static bool NearlyEquals_ScaledTolerance(this float number, float target, float toleranceFraction)
        //        {
        //            if ((-1 <= target) && (target <= 1))
        //                return number.NearlyEquals(target, toleranceFraction);

        //            float tolerance = Math.Abs(target) * toleranceFraction;
        //            return number.NearlyEquals(target, tolerance);
        //        }
        //        // A reasonable "toleranceFraction" is ScaledToleranceDouble. Larger if multiple operations may accumulate error.
        //        public static bool NearlyEquals_ScaledTolerance(this double number, double target, double toleranceFraction)
        //        {
        //            if ((-1 <= target) && (target <= 1))
        //                return number.NearlyEquals(target, toleranceFraction);

        //            double tolerance = Math.Abs(target) * toleranceFraction;
        //            return number.NearlyEquals(target, tolerance);
        //        }
        //        // A reasonable "toleranceFraction" is ScaledToleranceDouble. Larger if multiple operations may accumulate error.
        //        public static bool NearlyEquals_ScaledTolerance(this Distance2D p1, Distance2D p2, double toleranceFraction)
        //        {
        //            return (p1.X(Distance).NearlyEquals_ScaledTolerance(p2.X(Distance), toleranceFraction) &&
        //                    p1.Y.NearlyEquals(p2.Y, toleranceFraction));
        //        }

        //        // If within tolerance, is considered equal.
        //        // Therefore, number must exceed target by more than tolerance.
        //        public static bool GreaterThan_Tolerance(this double number, double target, double tolerance)
        //        {
        //            return number > (target + tolerance);
        //        }

        //        // A reasonable "toleranceFraction" is ScaledToleranceDouble. Larger if multiple operations may accumulate error.
        //        public static bool GreaterThan_ScaledTolerance(this double number, double target, double toleranceFraction)
        //        {
        //            if ((-1 <= target) && (target <= 1))
        //                return number.GreaterThan_Tolerance(target, toleranceFraction);

        //            double tolerance = Math.Abs(target) * toleranceFraction;
        //            return number.GreaterThan_Tolerance(target, tolerance);
        //        }


        //        // Results of CompareTo
        //        public const int IsBefore = -1;
        //        public const int IsIdentical = 0;
        //        public const int IsAfter = 1;

        //        // Does CompareTo, but considers NearlyEqual values to be Equal.
        //        public static int CompareTo_WithNearlyEquals(this double a, double b)
        //        {
        //            if (a.NearlyEquals(b))
        //                return IsIdentical;

        //            return a.CompareTo(b);
        //        }

        //        // Used to sort pairs of values, e.g. (a1, a2) vs. (b1, b2),
        //        // so that a's are in order. When a's are (nearly) identical,
        //        // also puts b's in order.
        //        // Uses CompareTo_WithNearlyEquals rather than CompareTo,
        //        // so that minor math errors don't prevent values from being considered equal.
        //        public static int CompareTo_WithNearlyEquals(double a1, double a2, double b1, double b2)
        //        {
        //            return a1.NearlyEquals(b1) ? a2.CompareTo_WithNearlyEquals(b2) : a1.CompareTo_WithNearlyEquals(b1);
        //        }

        //        public static void Swap(ref double dblFirst, ref double dblSecond)
        //        {
        //            double tmp = dblFirst;
        //            dblFirst = dblSecond;
        //            dblSecond = tmp;
        //        }

        //        public static void Swap(ref int intFirst, ref int intSecond)
        //        {
        //            int tmp = intFirst;
        //            intFirst = intSecond;
        //            intSecond = tmp;
        //        }

        //        public static void Swap(ref float sngFirst, ref float sngSecond)
        //        {
        //            float tmp = sngFirst;
        //            sngFirst = sngSecond;
        //            sngSecond = tmp;
        //        }

        //        public static void Swap(ref PointF ptfFirst, ref PointF ptfSecond)
        //        {
        //            PointF tmp = ptfFirst;
        //            ptfFirst = ptfSecond;
        //            ptfSecond = tmp;
        //        }

        //        public static void Swap(ref Distance2D ptdFirst, ref Distance2D ptdSecond)
        //        {
        //            Distance2D tmp = ptdFirst;
        //            ptdFirst = ptdSecond;
        //            ptdSecond = tmp;
        //        }

        //        public static void Swap(ref Distance3D ptdFirst, ref Distance3D ptdSecond)
        //        {
        //            Distance3D tmp = ptdFirst;
        //            ptdFirst = ptdSecond;
        //            ptdSecond = tmp;
        //        }

        //        public static void Swap(ref object First, ref object Second)
        //        {
        //            object tmp = First;
        //            First = Second;
        //            Second = tmp;
        //        }

        //        public static void SwapTyped<T>(ref T First, ref T Second)
        //        {
        //            T tmp = First;
        //            First = Second;
        //            Second = tmp;
        //        }


        //        public static void SmallerFirst(ref double First, ref double Second)
        //        {
        //            if (First > Second)
        //                Swap(ref First, ref Second);
        //        }

        //        // Like RangeFirstLast, but allows EITHER a or b to be the smaller number.
        //        // NOTE: Returns a range that enumerates from SMALLER to LARGER.
        //        public static IEnumerable<int> RangeUnordered(int a, int b)
        //        {
        //            if (a <= b)
        //                return RangeFirstLast(a, b);
        //            else
        //                return RangeFirstLast(b, a);
        //        }

        //        // The only form of Range function takes count. This is alternative function that takes last value.
        //        public static IEnumerable<int> RangeFirstLast(int first, int last)
        //        {
        //            int count = last - first + 1;
        //            Debug.Assert(count > 0);
        //            return Enumerable.Range(first, count);
        //        }

        /// <summary> Restrict value to be between specified Min/Max. </summary>
        public static double Clamp(double dblValue, double dblMin, double dblMax)
        {
            if (dblValue < dblMin)
                return dblMin;
            if (dblValue > dblMax)
                return dblMax;
            return dblValue;
        }

        /// <summary> Restrict value to be between specified Min/Max. </summary>
        public static float Clamp(float sngValue, float sngMin, float sngMax)
        {
            if (sngValue < sngMin)
                return sngMin;
            if (sngValue > sngMax)
                return sngMax;
            return sngValue;
        }

        /// <summary> Restrict value to be between specified Min/Max. </summary>
        public static int Clamp(int intValue, int intMin, int intMax)
        {
            if (intValue < intMin)
                return intMin;
            if (intValue > intMax)
                return intMax;
            return intValue;
        }


        //        public enum ClampStatus
        //        {
        //            InRange,
        //            Low,
        //            High
        //        }

        //        // inRange = True if "value" is within range, False if it had to be clamped.
        //        public static double ClampReport(double value, double minValue, double maxValue, out ClampStatus eClampStatus)
        //        {
        //            if (value < minValue)
        //            {
        //                eClampStatus = ClampStatus.Low;
        //                return minValue;
        //            }
        //            else if (value > maxValue)
        //            {
        //                eClampStatus = ClampStatus.High;
        //                return maxValue;
        //            }
        //            else
        //            {
        //                eClampStatus = ClampStatus.InRange;
        //                return value;
        //            }
        //        }

        //        public static bool ValueWasInRange(ClampStatus eClampStatus)
        //        {
        //            return (eClampStatus == ClampStatus.InRange);
        //        }

        //        // Restrict value to not be less than minValue.
        //        public static double ClampMin(double value, double minValue)
        //        {
        //            if (value < minValue)
        //                return minValue;
        //            return value;
        //        }
        //        public static float ClampMin(float value, float minValue)
        //        {
        //            if (value < minValue)
        //                return minValue;
        //            return value;
        //        }
        //        public static int ClampMin(int value, int minValue)
        //        {
        //            if (value < minValue)
        //                return minValue;
        //            return value;
        //        }


        //        // Restrict value to not be greater than maxValue.
        //        public static double ClampMax(double value, double maxValue)
        //        {
        //            if (value > maxValue)
        //                return maxValue;
        //            return value;
        //        }

        //        // Restrict value to not be greater than maxValue.
        //        public static float ClampMax(float value, float maxValue)
        //        {
        //            if (value > maxValue)
        //                return maxValue;
        //            return value;
        //        }

        //        // Restrict value to not be greater than maxValue.
        //        public static int ClampMax(int value, int maxValue)
        //        {
        //            if (value > maxValue)
        //                return maxValue;
        //            return value;
        //        }

        //        // True if "a < b < c" aka "(a < b) And (b < c)".
        //        public static bool Less3(double a, double b, double c)
        //        {
        //            return (a < b) && (b < c);
        //        }

        //        // True if "a <= b <= c" aka "(a <= b) And (b <= c)".
        //        public static bool Leq3(double a, double b, double c)
        //        {
        //            return (a <= b) && (b <= c);
        //        }

        //        // True if "a <= b <= c" aka "(a <= b) And (b <= c)".
        //        // "b" allowed to be "tol" outside of range (a, c).
        //        // TBD: Should we use a "scaledTolerance", multiplied by "c-a"?
        //        public static bool Leq3_WithTolerance(double a, double b, double c, double tol)
        //        {
        //            return (a - tol <= b) && (b <= c + tol);
        //        }

        //        // True if value is between a and b, either of which may be the larger value.
        //        // Not allowed to be exactly a or b; uses "<".
        //        public static bool BetweenExclusive(double value, double a, double b)
        //        {
        //            return ((a < value) && (value < b)) || ((b < value) && (value < a));
        //        }

        //        // True if value is between a and b, either of which may be the larger value.
        //        // "tol": Must be INSIDE a and b, by at least "tol".
        //        public static bool BetweenExclusive_WithTolerance(double value, double a, double b, double tol)
        //        {
        //            if (a <= b)
        //                return (a + tol < value) && (value < b - tol);
        //            else
        //                return (b + tol < value) && (value < a - tol);
        //        }

        //        // True if value is between a and b, either of which may be the larger value.
        //        // "tol": Allowed to be slightly OUTSIDE of a and b, up to "tol".
        //        public static bool BetweenInclusive_WithTolerance(double value, double a, double b, double tol)
        //        {
        //            if (a <= b)
        //                return (a - tol < value) && (value < b + tol);
        //            else
        //                return (b - tol < value) && (value < a + tol);
        //        }

        //        // True if value is between a and b, either of which may be the larger value.
        //        // Allowed to be exactly a or b; uses "<=".
        //        public static bool BetweenInclusive(double value, double a, double b)
        //        {
        //            return ((a <= value) && (value <= b)) || ((b <= value) && (value <= a));
        //        }

        //        /// <summary> True if value is between a and b, either of which may be the larger value. </summary>
        //        public static bool Between(float value, float a, float b)
        //        {
        //            return ((a <= value) && (value <= b)) || ((b <= value) && (value <= a));
        //        }

        //        /// <summary> True if value is between a and b, either of which may be the larger value. </summary>
        //        public static bool Between(int value, int a, int b)
        //        {
        //            return ((a <= value) && (value <= b)) || ((b <= value) && (value <= a));
        //        }

        //        public static double PercentToFraction(double percent)
        //        {
        //            return percent / 100.0;
        //        }

        //        public static SizeF MultiplyByScalar(SizeF sz, float mult)
        //        {
        //            return new SizeF(mult * sz.Width, mult * sz.Height);
        //        }
        //        public static SizeF MultiplyByScalar(float mult, SizeF sz)
        //        {
        //            return new SizeF(mult * sz.Width, mult * sz.Height);
        //        }


        //        // PERFORMANCE: Quicker than Distance, because does not need SQRT.
        //        public static double DistanceSquared3D(double x1, double y1, double z1, double x2, double y2, double z2)
        //        {
        //            var dx = x2 - x1;
        //            var dy = y2 - y1;
        //            var dz = z2 - z1;
        //            return (dx * dx) + (dy * dy) + (dz * dz);
        //        }

        // PERFORMANCE: Quicker than Distance, because does not need SQRT.
        public static double DistanceSquared2D(double x1, double y1, double x2, double y2)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            return (dx * dx) + (dy * dy);
        }
        // PERFORMANCE: Quicker than Distance, because does not need SQRT.
        public static DistD.Squared DistanceSquared2D(DistD x1, DistD y1, DistD x2, DistD y2)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            return (dx * dx) + (dy * dy);
        }
        public static double CalcDistance2D(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(DistanceSquared2D(x1, y1, x2, y2));
        }
        public static DistD CalcDistance2D(DistD x1, DistD y1, DistD x2, DistD y2)
        {
            return DistanceSquared2D(x1, y1, x2, y2).Sqrt();
        }

        public static DistD CalcDistance2D(Dist2D p1, Dist2D p2)
        {
            return DistanceSquared2D(p1.X, p1.Y, p2.X, p2.Y).Sqrt();
        }

        public static DistD CalcDistance2D(Dist3D p1, Dist3D p2)
        {
            return CalcDistance2D(p1.X, p1.Y, p2.X, p2.Y);
        }


        /// <summary> Weighted Average of two numbers.
        ///     ''' When wgtB==0, returns a, when wgtB==1, returns b.
        ///     ''' Implicitly, wgtA = 1 - wgtB. That is, the weights are normalized.
        ///     ''' </summary>
        public static double Lerp(double a, double b, double wgtB)
        {
            return a + (wgtB * (b - a));
        }
        public static DistD Lerp(DistD a, DistD b, double wgtB)
        {
            return a + (wgtB * (b - a));
        }
        /// <summary> Weighted Average of two numbers.
        ///     ''' When wgt==0, returns a, when wgtB==1, returns b.
        ///     ''' Implicitly, wgtA = 1 - wgtB. That is, the weights are normalized.
        ///     ''' NOTE: Identical to XNA MathHelper.Lerp.
        ///     ''' </summary>
        public static float Lerp(float a, float b, float wgtB)
        {
            return a + (wgtB * (b - a));
        }
        /// <summary> Weighted Average of two numbers.
        ///     ''' When wgt==0, returns a, when wgtB==1, returns b.
        ///     ''' Implicitly, wgtA = 1 - wgtB. That is, the weights are normalized.
        ///     ''' </summary>
        public static byte Lerp(byte a, byte b, float wgtB)
        {
            // TODO: Math.Truncate - is it needed?
            return System.Convert.ToByte(Math.Truncate(a + (wgtB * (b - a))));
        }
        public static PointF Lerp(PointF a, PointF b, float wgtB)
        {
            return new PointF(Lerp(a.X, b.X, wgtB), Lerp(a.Y, b.Y, wgtB));
        }
        public static Dist2D Lerp(Dist2D a, Dist2D b, double wgtB)
        {
            return new Dist2D(Lerp(a.X, b.X, wgtB), Lerp(a.Y, b.Y, wgtB));
        }
        public static Dist3D Lerp(Dist3D a, Dist3D b, double wgtB)
        {
            return new Dist3D(Lerp(a.X, b.X, wgtB), Lerp(a.Y, b.Y, wgtB), Lerp(a.Z, b.Z, wgtB));
        }
        public static Vector3 Lerp(Vector3 a, Vector3 b, float wgtB)
        {
            return new Vector3(Lerp(a.X, b.X, wgtB), Lerp(a.Y, b.Y, wgtB), Lerp(a.Z, b.Z, wgtB));
        }
        public static Vector2 Lerp(Vector2 a, Vector2 b, float wgtB)
        {
            return new Vector2(Lerp(a.X, b.X, wgtB), Lerp(a.Y, b.Y, wgtB));
        }

        public static float Average(float a, float b)
        {
            // Equivalent.
            return (a + b) / 0.5f;
            //return Lerp(a, b, 0.5f);
        }
        public static double Average(double a, double b)
        {
            // Equivalent.
            return (a + b) / 0.5;
            //return Lerp(a, b, 0.5);
        }
        public static DistD Average(DistD a, DistD b)
        {
            // Equivalent.
            return (DistD)((a + b) / 0.5);
            //return Lerp(a, b, 0.5);
        }
        public static Vector3 Average(Vector3 a, Vector3 b)
        {
            return Vector3.Lerp(a, b, 0.5F);
        }
        public static PointF Average(PointF a, PointF b)
        {
            return Lerp(a, b, 0.5F);
        }
        public static Dist2D Average(Dist2D a, Dist2D b)
        {
            return Lerp(a, b, 0.5);
        }
        public static Dist3D Average(Dist3D a, Dist3D b)
        {
            return Lerp(a, b, 0.5);
        }

        public static Dist2D Average(IList<Dist2D> points)
        {
            var count = 0;
            DistD sumX = DistD.Zero;
            DistD sumY = DistD.Zero;

            foreach (Dist2D point in points)
            {
                count += 1;
                sumX += point.X;
                sumY += point.Y;
            }

            if (count == 0)
                return new Dist2D(DistD.Zero, DistD.Zero);
            else
                return new Dist2D(sumX / count, sumY / count);
        }

        // Geometric Average.
        public static double GeometricAverage(double a, double b)
        {
            return Math.Sqrt(a * b);
        }
        // Similar to Lerp, but weighting is geometric.
        // E.g. wgtB=0.5 => GeometricAverage(a, b).  See Test_GeometricLerp for more examples.
        // REQUIRE (a, b) are greater than zero.  REQUIRE wgtB in [0..1].
        public static double GeometricLerp(double a, double b, double wgtB)
        {
            if (wgtB <= 0)
                return a;
            if (wgtB >= 1)
                return b;
            double ratio = b / a;
            double logRatio = Math.Log(ratio);
            // "0" = Math.Log(1); e.g. the Log for ratio "1", which represents "a".
            double weightedLogRatio = Lerp(0, logRatio, wgtB);
            double unloggedWeightedRatio = Math.Exp(weightedLogRatio);
            return a * unloggedWeightedRatio;
        }

        private static void Test_GeometricLerp()
        {
            const double a = 10;
            const double b = 100;
            double geoAvg50 = GeometricAverage(a, b);
            double geoAvg25 = GeometricAverage(a, geoAvg50);
            double geoAvg75 = GeometricAverage(geoAvg50, b);
            double test25 = GeometricLerp(a, b, 0.25);
            double test50 = GeometricLerp(a, b, 0.5);
            double test75 = GeometricLerp(a, b, 0.75);
        }

        /// <summary> Calculate wgtB that would return result, if did Lerp(a, b, wgtB).
        ///     ''' That is, where result is, w.r.t. a and b.
        ///     ''' &lt; 0 is before a, &gt; 1 is after b.
        ///     ''' Parameters could be named "zeroValue, oneValue, value":
        ///     '''   When value = zeroValue, return 0;
        ///     '''   when value = oneValue, return 1.
        ///     ''' "wgt" could be named "frac", short for "fraction", as it is a value in range (0, 1).
        ///     '''   CAUTION: "result" is LAST parameter, not FIRST parameter - to be most similar to LERP parameters.
        ///     '''      wgt = WgtFromResult(a, b, result)  =>  Lerp(a, b, wgt)  =>  result
        ///     ''' </summary>
        public static double WgtFromResult(double a, double b, double result)
        {
            double denominator = b - a;

            if (Math.Abs(denominator) < 0.00000001)
            {
                // Avoid divide-by-zero (a & b are nearly equal).
                if (Math.Abs(result - a) < 0.00000001)
                    // Result is close to a (but also to b): Give simplest answer: average them.
                    return 0.5;
                // Cannot compute.
                return double.NaN;
            }

            // result = a + (wgt * (b - a))   =>
            // wgt * (b - a) = (result - a)   =>
            double wgt = (result - a) / denominator;

            // Dim verify As Double = Lerp(a, b, wgt)
            // If Not NearlyEqual(result, verify) Then
            // Test()    ' test
            // End If

            return wgt;
        }

        // Only meaningful if points are (nearly) colinear, and resultPt and pB are both in same direction from pA.
        public static double WgtFromResult(Dist2D pA, Dist2D pB, Dist2D resultPt)
        {
            Dist2D dB = pB - pA;
            Dist2D dResult = resultPt - pA;
            DistD lengthB = dB.Length;
            DistD lengthResult = dResult.Length;
            return lengthResult / lengthB;
        }

        // Find analogous interpolated value.
        // inNear:inGoal:inFar corresponds to outNear:outGoal:outFar.
        // CAUTION: "inGoal" - maybe it should have been FIRST parameter, but it isn't.
        public static double MatchingLerp(double inNear, double inFar, double inGoal, double outNear, double outFar)
        {
            double wgt = WgtFromResult(inNear, inFar, inGoal);
            // outGoal
            return Lerp(outNear, outFar, wgt);
        }

        // Find analogous interpolated value.
        // inNear:inGoal:inFar corresponds to outNear:outGoal:outFar.
        // CAUTION: "inGoal" - maybe it should have been FIRST parameter, but it isn't.
        public static Dist2D MatchingLerp(double inNear, double inFar, double inGoal, Dist2D outNear, Dist2D outFar)
        {
            double wgt = WgtFromResult(inNear, inFar, inGoal);
            // outGoal
            return Lerp(outNear, outFar, wgt);
        }

        // <summary> Convert a value from one range to another.
        // Effectively, stretch input range to cover output range.
        // E.g. input value mid-way between input min & max ends up mid-way between output min & max.
        // If value lt minInput, return minOutput.
        // If value gt maxInput, return maxOutput.
        // (Like MatchingLerp, but weight is clamped.)
        // </summary>
        public static double MapInputToOutput_Ranges(double dblValue, double dblMinInput, double dblMaxInput, double dblMinOutput, double dblMaxOutput)
        {
            // Input range: minInput..maxInput
            double rawWeight = WgtFromResult(dblMinInput, dblMaxInput, dblValue);
            // Restrict to specified range
            double safeWeight = Clamp(rawWeight, 0.0, 1.0);
            // Output range: minOutput..maxOutput.  Input range gets mapped on to output range.
            double output = Lerp(dblMinOutput, dblMaxOutput, safeWeight);
            return output;
        }


        // Interpolate, with separate weights in X and Y.
        public static Dist2D LerpXY(Dist2D a, Dist2D b, Vec2D wgtB)
        {
            return new Dist2D(Lerp(a.X, b.X, wgtB.X), Lerp(a.Y, b.Y, wgtB.Y));
        }
        public static SizeF LerpXY(SizeF a, SizeF b, SizeF wgtB)
        {
            return new SizeF(Lerp(a.Width, b.Width, wgtB.Width), Lerp(a.Height, b.Height, wgtB.Height));
        }


        // xyWgt ranges (0..1) in x and y. (0,0) will return X0Y0, (0,1) will return X0Y1, etc.
        // For example, if xyWgt is relative location within an image,
        // and the XnYn values are GPS coords at the 4 corners of the image,
        // The result is GPS coord corresponding to xyWgt.
        // E.g. given (0.5, 0.5), the result will be the GPS coord at center of image.
        // NOTE: corners are ZIGZAG (not clockwise).
        public static Dist2D Lerp2D(Vec2D xyWgt, Dist2D X0Y0, Dist2D X1Y0, Dist2D X0Y1, Dist2D X1Y1)
        {
            Dist2D xY0 = Lerp(X0Y0, X1Y0, xyWgt.X);
            Dist2D xY1 = Lerp(X0Y1, X1Y1, xyWgt.X);

            Dist2D xy = Lerp(xY0, xY1, xyWgt.Y);
            return xy;
        }

        public static Vector3 Lerp2D(Vector2 xyWgt, Vector3 X0Y0, Vector3 X1Y0, Vector3 X0Y1, Vector3 X1Y1)
        {
            Vector3 xY0 = Lerp(X0Y0, X1Y0, xyWgt.X);
            Vector3 xY1 = Lerp(X0Y1, X1Y1, xyWgt.X);

            Vector3 xy = Lerp(xY0, xY1, xyWgt.Y);
            return xy;
        }

        public static Vector2 Lerp2D(Vector2 xyWgt, Vector2 X0Y0, Vector2 X1Y0, Vector2 X0Y1, Vector2 X1Y1)
        {
            Vector2 xY0 = Lerp(X0Y0, X1Y0, xyWgt.X);
            Vector2 xY1 = Lerp(X0Y1, X1Y1, xyWgt.X);

            Vector2 xy = Lerp(xY0, xY1, xyWgt.Y);
            return xy;
        }

        // xyWgt ranges (0..1) in x and y. (0,0) will return ZAtX0Y0, (0,1) will return ZAtX0Y1, etc.
        // For example, if xyWgt is relative location within an image,
        // and the ZAtXnYn values are altitudes at the 4 corners of the image,
        // The result is altitude corresponding to xyWgt.
        // E.g. given (0.5, 0.5), the result will be the altitude at center of image.
        public static double Lerp2D(Vec2D xyWgt, double ZAtX0Y0, double ZAtX1Y0, double ZAtX0Y1, double ZAtX1Y1)
        {
            double zAt_xY0 = Lerp(ZAtX0Y0, ZAtX1Y0, xyWgt.X);
            double zAt_xY1 = Lerp(ZAtX0Y1, ZAtX1Y1, xyWgt.X);

            double zAt_xy = Lerp(zAt_xY0, zAt_xY1, xyWgt.Y);
            return zAt_xy;
        }

        public static float Lerp2D(Vector2 xyWgt, float ZAtX0Y0, float ZAtX1Y0, float ZAtX0Y1, float ZAtX1Y1)
        {
            float zAt_xY0 = Lerp(ZAtX0Y0, ZAtX1Y0, xyWgt.X);
            float zAt_xY1 = Lerp(ZAtX0Y1, ZAtX1Y1, xyWgt.X);

            float zAt_xy = Lerp(zAt_xY0, zAt_xY1, xyWgt.Y);
            return zAt_xy;
        }


        //// Returns xyWgt, which if given to Lerp2D, would return this "xy".
        //// So if xy = X0Y0, will return (0, 0). if xy = X1Y0, will return (1, 0), etc.
        //// For example, if 4 corners are GPS coordinates in corners of an image, and pass in a GPS coordinate,
        //// return tells relative location within the image.
        //// InverseLerp2D is essentially a 2-D version of WgtFromResult. (The implementation is different, because in 2-D the situation is non-linear.)
        //public static Distance2D InverseLerp2D(Distance2D xy, Distance2D X0Y0, Distance2D X1Y0, Distance2D X0Y1, Distance2D X1Y1)
        //{
        //    // ----> Main Work, Pass 1 <----
        //    Distance2D xyWgt = _InverseLerp2D_Pass1(xy, X0Y0, X1Y0, X0Y1, X1Y1);

        //    // Verify
        //    Distance2D xy_verify = Lerp2D(xyWgt, X0Y0, X1Y0, X0Y1, X1Y1);

        //    double newError = CalcDistance2D(xy, xy_verify);
        //    // Made it more accurate, for the case where it is oscillating between an x-error and a y-error.
        //    // This forces more passes. TODO: Find a way to converge more quickly.
        //    double tolerance = CalcDistance2D(X0Y0, X1Y1) * 0.00005; // 0.0001 ' 0.0002
        //                                                         // ' COMMENTED OUT: Always do second pass, for best accuracy.
        //                                                         // If error1 <= tolerance Then
        //                                                         // ' Good
        //                                                         // Return xyWgt1
        //                                                         // End If

        //    int nMorePasses = 12; // 8
        //    do
        //    {
        //        Distance2D oldXyWgt = xyWgt;
        //        double oldError = newError;
        //        // --- Another pass. Project to lines near the point (according to prior pass). ---
        //        // ----> Main Work, Pass N <----
        //        xyWgt = _InverseLerp2D_PassN(xyWgt, xy, X0Y0, X1Y0, X0Y1, X1Y1);
        //        xy_verify = Lerp2D(xyWgt, X0Y0, X1Y0, X0Y1, X1Y1);
        //        newError = CalcDistance2D(xy, xy_verify);
        //        if (newError <= tolerance)
        //            // Good
        //            return xyWgt;
        //        else if (newError > oldError)
        //        {
        //            // Bad, but getting worse, so don't use the new result.
        //            Dubious("InverseLerp2D - error increasing {0} => {1}", oldError, newError);
        //            return oldXyWgt;
        //        }

        //        nMorePasses -= 1;
        //    }
        //    while ((nMorePasses > 0));

        //    // NOTE: TourGolf Base Data Library.mMain.Test_InverseLerp2D never reached here.
        //    // tmstest Dubious("InverseLerp2D - error {0} not within tolerance, after all passes", newError)
        //    return xyWgt;
        //}

        //// A pass of InverseLerp2D algorithm.
        //// Each pass improves upon the previous estimate (guess) "xyWgt1".
        //private static Distance2D _InverseLerp2D_PassN(Distance2D xyWgt1, Distance2D xy, Distance2D X0Y0, Distance2D X1Y0, Distance2D X0Y1, Distance2D X1Y1)
        //{

        //    // NOTE: the "x-guess" line yields an improved approximation to yWgt (yWgt2);
        //    // the "y-guess" line yields improved xWgt.
        //    double xWgt = xyWgt1.X;
        //    Distance2D XguessY0 = Lerp(X0Y0, X1Y0, xWgt);
        //    Distance2D XguessY1 = Lerp(X0Y1, X1Y1, xWgt);
        //    double yWgt2;
        //    // "t" along the X-guess line is adjusted yWgt.
        //    // EXPLAIN: each "constant xWgt" line is drawn from yWgt=0 at one end, to yWgt=1 at other end.
        //    // Projecting to closest point on such a line, yields a "t" that likewise goes from 0 to 1.
        //    // That "t" is an estimate of yWgt.
        //    // From the first pass, we have a line that is near to xy,
        //    // so the projection is more accurate than our first pass, which was projecting
        //    // to the sides of the quadrilateral.
        //    // If the incoming xyWgt1 were exactly correct,
        //    // The line (XguessY0, XguessY1) and the line (X0Yguess, X1Yguess) would both pass THRU "xy".
        //    // By definition of Lerp2D, the output "xyWgt2" would yield the same weights.
        //    // (xy would be a point on each line; its "weight" along each line would be the x-wgt and y-wgt.)
        //    // To understand this, first study "WgtFromResult", and understand how that works in 1 dimension.
        //    // InverseLerp2D is essentially a 2-D version of WgtFromResult. (The implementation is different, because in 2-D the situation is non-linear.)
        //    // 
        //    // x params => yWgt2 ("y" is not a typo).
        //    Distance2D Xguess_closest = ClosestPointOnLine2D_AndT(xy, XguessY0, XguessY1, yWgt2);

        //    double yWgt = xyWgt1.Y;
        //    Distance2D X0Yguess = Lerp(X0Y0, X0Y1, yWgt);
        //    Distance2D X1Yguess = Lerp(X1Y0, X1Y1, yWgt);
        //    double xWgt2;
        //    // y params => xWgt2 ("x" is not a typo).
        //    Distance2D Yguess_closest = ClosestPointOnLine2D_AndT(xy, X0Yguess, X1Yguess, xWgt2);

        //    // Dim Xguess_error As Double = CalcDistance2D(xy, Xguess_closest)
        //    // Dim Yguess_error As Double = CalcDistance2D(xy, Yguess_closest)

        //    // Improved guess.
        //    return new Distance2D(xWgt2, yWgt2);
        //}

        //// Approximate algorithm. 
        //private static Distance2D _InverseLerp2D_Pass1(Distance2D xy, Distance2D X0Y0, Distance2D X1Y0, Distance2D X0Y1, Distance2D X1Y1)
        //{
        //    double wx0;
        //    // Project on to 4 edges, to find "t" along each edge.
        //    // Interpolate the results between each pair of near-parallel edges.

        //    // "w" is weight. 0=>x0, 1=>x1.
        //    double distx0 = PointDistanceToLine2D_AndT(xy, X0Y0, X1Y0, wx0);
        //    double wx1;
        //    double distx1 = PointDistanceToLine2D_AndT(xy, X0Y1, X1Y1, wx1);
        //    double wy0;
        //    double disty0 = PointDistanceToLine2D_AndT(xy, X0Y0, X0Y1, wy0);
        //    double wy1;
        //    double disty1 = PointDistanceToLine2D_AndT(xy, X1Y0, X1Y1, wy1);

        //    // Interpolate between the answers at x0 and x1:
        //    // As distx0 approaches 0, the answer approaches tx0;
        //    // As distx1 approaches 0, the answer approaches tx1.
        //    double wwx = distx0 / (distx0 + distx1);
        //    double wwy = disty0 / (disty0 + disty1);
        //    // "w" is weight. 0=>x0, 1=>x1.
        //    double wx = Lerp(wx0, wx1, wwx);
        //    double wy = Lerp(wy0, wy1, wwy);

        //    if ((wx < 0.0) || (wx > 1.0))
        //    {
        //        if ((wx < -0.1) || (wx > 1.1))
        //            Dubious();
        //        wx = Clamp(wx, 0.0, 1.0);
        //    }
        //    if ((wy < 0) || (wy > 1.0))
        //    {
        //        if ((wy < -0.1) || (wy > 1.1))
        //            Dubious();
        //        wy = Clamp(wy, 0.0, 1.0);
        //    }

        //    Distance2D xyWgt = new Distance2D(wx, wy);
        //    return xyWgt;
        //}

        //// Returns xyWgt, which if given to Lerp2D, would return this "xy".
        //// So if xy = X0Y0, will return (0, 0). if xy = X1Y0, will return (1, 0), etc.
        //// For example, if 4 corners are GPS coordinates in corners of an image, and pass in a GPS coordinate,
        //// return tells relative location within the image.
        //// TODO: This results in divide-by-zero, when sides are parallel.
        //public static Distance2D InverseLerp2D_A(Distance2D xy, Distance2D X0Y0, Distance2D X1Y0, Distance2D X0Y1, Distance2D X1Y1)
        //{
        //    double x = xy.X;
        //    // Formula derived in Mathematica, by solving for (x, y) => (xWgt, yWgt), given the Lerp2D formula above.
        //    // TODO: Mathematica showed TWO solutions; how know which one comes out in positive range? Or just take abs?

        //    double y = xy.Y;
        //    double x00 = X0Y0.X;
        //    double y00 = X0Y0.Y;
        //    double x10 = X1Y0.X;
        //    double y10 = X1Y0.Y;
        //    double x01 = X0Y1.X;
        //    double y01 = X0Y1.Y;
        //    double x11 = X1Y1.X;
        //    double y11 = X1Y1.Y;

        //    double a = (x10 * (-y + y00) + x00 * (y - y10) + x * (-y00 + y10));
        //    double b = (x11 * (y00 - y01) + x10 * (-y00 + y01) + (x00 - x01) * (y10 - y11));
        //    double c = (x10 * y - x11 * y + x * y00 - 2 * x10 * y00 + x11 * y00 - x * y01 + x10 * y01 + x01 * (y - y10) - x * y10 + x * y11 - x00 * (y - 2 * y10 + y11));
        //    double inner = a * b + c;
        //    double num2 = Math.Abs(2 * inner);   // Math.Sqrt(4 * innerx1 ^ 2)

        //    double detx = 2 * (x01 * (y00 - y10) + x11 * (-y00 + y10) - (x00 - x10) * (y01 - y11));
        //    double numx1 = (x00 - x01 - x10 + x11) * y - (x - 2 * x01 + x11) * y00 + (x - 2 * x00 + x10) * y01 + (x - x01) * y10 + (-x + x00) * y11;

        //    double dety = 2 * (x10 * (y00 - y01) + x11 * (-y00 + y01) - (x00 - x01) * (y10 - y11));
        //    double numy1 = (x00 - x01 - x10 + x11) * y + 2 * x10 * y00 - x11 * y00 - x10 * y01 - 2 * x00 * y10 + x01 * y10 + x * (-y00 + y01 + y10 - y11) + x00 * y11;

        //    double wx = (numx1 + num2) / detx;
        //    double wy = (numy1 - num2) / dety;
        //    if ((wx < 0))
        //        wx = Math.Abs(wx);
        //    if ((wy < 0))
        //        wy = Math.Abs(wy);
        //    if ((wx > 1) || (wy > 1))
        //        Dubious();
        //    Distance2D xyWgt = new Distance2D(wx, wy);

        //    // Verify
        //    Distance2D xy_verify = Lerp2D(xyWgt, X0Y0, X1Y0, X0Y1, X1Y1);
        //    if (!xy_verify.NearlyEquals(xy))
        //        Trouble();

        //    return xyWgt;
        //}

        // Since we have "Single" variant, also have "Double" variant, so don't accidentally coerce double to single, in non-strict projects.



        public static double Floor(double value)
        {
            return Math.Floor(value);
        }

        public static float Floor(float value)
        {
            return System.Convert.ToSingle(Math.Floor(value));
        }

        // Floor always returns an Integer (or potentially a Long).
        // Often, we simply want it as Integer.
        public static int FloorInt(double value)
        {
            return (int)(Math.Floor(value));
        }


        // Since we have "Single" variant, also have "Double" variant, so don't accidentally coerce double to single, in non-strict projects.
        public static double Ceiling(double value)
        {
            return Math.Ceiling(value);
        }

        public static float Ceiling(float value)
        {
            return System.Convert.ToSingle(Math.Ceiling(value));
        }

        // Floor always returns an Integer (or potentially a Long).
        // Often, we simply want it as Integer.
        public static int CeilingInt(double value)
        {
            return (int)(Math.Ceiling(value));
        }

        // True mathematical definition of modulus: even if dividend is negative,
        // result is positive.
        public static double Modulus(double dividend, double divisor)
        {
            double result = dividend % divisor;
            return result < 0 ? result + divisor : result;
        }

        // True mathematical definition of modulus: even if dividend is negative,
        // result is positive.
        public static int Modulus(int dividend, int divisor)
        {
            int result = dividend % divisor;
            return result < 0 ? result + divisor : result;
        }

        // Smallest power of 2 that is greater than or equal to "value".
        public static int EnclosingPowerOf2(int value)
        {
            return _NearPowerOf2(value, 1);
        }

        // Closest power of 2, where closest is by comparing logarithms.
        // Ex: between 512 and 1024, exp(9.5, 2) is 724.07;
        // 724 and less => 512, 725 and above => 1024.
        public static int NearestPowerOf2(int value)
        {
            return _NearPowerOf2(value, 0);
        }

        // Largest power of 2 that is less than or equal to "value".
        public static int ContainedPowerOf2(int value)
        {
            return _NearPowerOf2(value, -1);
        }

        // If value <= maxNearest, then do "Nearest", else do "Contained".
        // That is, use nearest for small values, but don't make large values even larger.
        // "maxResult" is the largest value that will ever be returned.
        public static int NearestOrContainedPowerOf2(int value, int maxNearest, int maxResult)
        {
            int result;
            if (value <= maxNearest)
                result = NearestPowerOf2(value);
            else
                result = ContainedPowerOf2(value);

            // Don't be larger than maxResult.
            // But return a power of 2, even if maxResult is not a power of 2!
            while (result > maxResult)
                result = result >> 1;

            return result;
        }

        // "comparison" = "-1" for contained, "0" for neareast, "1" for enclosing.
        private static int _NearPowerOf2(int value, int comparison)
        {
            double logValue = Math.Log(value, 2);

            if (comparison < 0)
                logValue = Floor(logValue);
            else if (comparison == 0)
                logValue = Math.Round(logValue);
            else
                logValue = Ceiling(logValue);

            // TODO: Is this ever off-by-1?  I think doing Math.Round makes sure it isn't...
            int result = (int)(Math.Round(Math.Pow(2, logValue)));
            return result;
        }

        public delegate double UnaryDeleg(double value);

        // Round, and convert to integer.
        public static int RoundInt(this double value)
        {
            return (int)Math.Round(value);
        }

        // Round to one decimal place (except near zero).
        public static double Round1(double value, bool accurateNearZero = true)
        {
            return RoundN(value, 1, accurateNearZero);
        }
        // Round to two decimal places (except near zero).
        public static double Round2(double value)
        {
            return RoundN(value, 2);
        }
        // Round to three decimal places (except near zero).
        public static double Round3(double value)
        {
            return RoundN(value, 3);
        }
        // Round to four decimal places (except near zero).
        public static double Round4(double value)
        {
            return RoundN(value, 4);
        }
        // Round to five decimal places (except near zero).
        public static double Round5(double value)
        {
            return Math.Round(value, 5);
        }
        // Round to N decimal places (except near zero).
        public static double RoundN(double value, int nFracDigits, bool accurateNearZero = true)
        {
            double round = Math.Round(value, nFracDigits);
            if (round == 0 && accurateNearZero)
                // Show some significant data.
                return System.Convert.ToDouble(SmallValueString(value, nFracDigits));
            return round;
        }

        // WGS-84 are +-180, with a meter distance represented by change in 5th digit.
        // Show numbers in that range to 6 digits (~0.1m, depending on latitude).
        // Larger values don't need to show so many digits after the decimal.
        public static double Round4or6(double value)
        {
            if (Math.Abs(value) <= 180)
                return RoundN(value, 6);
            return RoundN(value, 4);
        }

        // Like Round, but acts to nearest 1/parts. Compare to FloorFraction.
        public static double RoundFraction(double value, int parts)
        {
            if (parts == 1)
                return Math.Round(value);

            return Math.Round(value * parts) / parts;
        }


        static public Vector3 Round3(Vector3 vec)
        {
            return new Vector3((float)Round3(vec.X), (float)Round3(vec.Y), (float)Round3(vec.Z));
        }


        //        // Floor, and Zero the low two digits; e.g. "6543.21" => "6500".
        //        public static double FloorHundred(double value)
        //        {
        //            return Math.Floor(value / 100) * 100;
        //        }
        //        // Floor, and Zero the low three digits; e.g. "6543.21" => "6000".
        //        public static double FloorThousand(double value)
        //        {
        //            return Math.Floor(value / 1000) * 1000;
        //        }

        //        // Like Floor, but acts on 5th decimal place; e.g. "88.123456" => "88.12345" 
        //        public static double Floor5(double value)
        //        {
        //            return Math.Floor(value * 100000) / 100000;
        //        }

        //        // Like Floor, but acts to nearest 1/parts. Compare to Floor5.
        //        public static double FloorFraction(double value, int parts)
        //        {
        //            if (parts == 1)
        //                return Math.Floor(value);

        //            return Math.Floor(value * parts) / parts;
        //        }

        //        // Like Ceiling, but acts on 5th decimal place; e.g. "88.123451" => "88.12346" 
        //        public static double Ceiling5(double value)
        //        {
        //            return Math.Ceiling(value * 100000) / 100000;
        //        }

        //        // Like Ceiling, but acts to nearest 1/parts. Compare to Floor5. 
        //        public static double CeilingFraction(double value, int parts)
        //        {
        //            if (parts == 1)
        //                return Math.Ceiling(value);

        //            return Math.Ceiling(value * parts) / parts;
        //        }


        //        // Divide by 1000, Round, append " K". E.g. "311555" => "312 K".
        //        public static string InThousandsK(double value)
        //        {
        //            return Math.Round(value / 1000).ToString() + " K";
        //        }

        // Used to show "varying" part of large positions within a Golf site.
        // "nToLeft": Don't show more than this digits to left of decimal. (Omit high digits)
        public static string AbbrevString(double value, int nToLeft = 4)
        {
            if (double.IsNaN(value))
                return "NaN";
            if (double.IsInfinity(value))
                return "Infinit";
            if (value == double.MaxValue)
                return "MaxVal";

            double approxV = value;

            // Find out where the significant data is.
            double absV = Math.Abs(value);
            if ((absV >= 1000))
            {
                // Integer portion has at least 4 digits.

                // Mod BEFORE Round, otherwise round-off errors during Mod result in long strings of 999..
                if (nToLeft == 4)
                    approxV = value % 10000;
                else
                    approxV = value % (Math.Pow(10, nToLeft));

                // Keep one decimal digit, to show is not exactly an integer.
                // (However if that digit rounds to zero, will appear to be an integer.)
                approxV = Math.Round(approxV, 1);
            }
            else if ((absV < NearZero))
                // Value is essentially zero.
                approxV = 0;
            else
                return SmallValueString(value, 2);

            return approxV.ToString();
        }

        // A compact approximation, for easy debugging.
        public static string ShortString(double value)
        {
            if (double.IsNaN(value))
                return "NaN";
            if (double.IsInfinity(value))
                return "Infinit";
            if (value == double.MaxValue)
                return "MaxVal";

            double approxV = value;

            // Find out where the significant data is.
            double absV = Math.Abs(value);
            if ((absV >= 1000) || (absV < NearZero))
                // Integer portion has at least 4 digits, or the value is essentially zero.
                // Keep one decimal digit, to show is not exactly an integer.
                // (However if that digit rounds to zero, will appear to be an integer.)
                approxV = Math.Round(value, 1);
            else
                return SmallValueString(value, 2);

            return approxV.ToString();
        }

        public static string AsNDigits(Vector3 vec, int nDigits)
        {
            return string.Format("({0}, {1}, {2})", RoundN(vec.X, nDigits), RoundN(vec.Y, nDigits), RoundN(vec.Z, nDigits));
        }

        public static string ShortString(Vector3 vec)
        {
            return string.Format("({0}, {1}, {2})", ShortString(vec.X), ShortString(vec.Y), ShortString(vec.Z));
        }

        public static string SmallValueString(double value, int nSignificantDigits)
        {
            double absV = Math.Abs(value);

            const int maxShifts = 6;
            int nShifts = 0;
            // For small values, "absV < 100" caused this to keep "N" digits AFTER the number has been increased to > 100,
            // for a total of "N+3" digits (including digits both left and right of decimal point).
            // Changed it to "< 1", for total of "N+1" digits.
            // E.g. it will now round to values like "1.23", "0.123" and "0.0123".
            // Larger values always have 2 significant digits to right, e.g. "123.45".
            while ((nShifts < maxShifts) && (absV < 1))   // (absV < 100)
            {
                absV *= 10;
                nShifts += 1;
            }

            if (nShifts >= maxShifts)
                return string.Format("{0:e" + nSignificantDigits.ToString() + "}", value);
            else if (nShifts > 0)
                // Keep n significant digits.
                return (Math.Sign(value) * Math.Round(absV, nSignificantDigits) / (Math.Pow(10, nShifts))).ToString();
            else
                // No shifts.
                return Math.Round(value, nSignificantDigits).ToString();
        }

        // "value" expected to be in range (-1.000 to 1.000).
        // Decided to always show nSignificantDigits, so debug output "lines up" better.
        // E.g. "1.000" rather than "1", "1.230" rather than "1.23".
        public static string FractionString(double value, int nSignificantDigits = 3)
        {
            return value.ToString("F" + nSignificantDigits.ToString());
        }

        public static string AsShortString(Vector3 v)
        {
            return "{" + ShortString(v.X) + ", " + ShortString(v.Y) + ", " + ShortString(v.Z) + "}";
        }


        public static double AbsDiff(double a, double b)
        {
            return Math.Abs(b - a);
        }

        public static bool IsOdd(int i)
        {
            return ((i & 1) != 0);
        }

        public static double Max3(double a, double b, double c)
        {
            return Math.Max(a, Math.Max(b, c));
        }

        public static float Max3(float a, float b, float c)
        {
            return Math.Max(a, Math.Max(b, c));
        }

        public static int Max3(int a, int b, int c)
        {
            return Math.Max(a, Math.Max(b, c));
        }


        public static double Min3(double a, double b, double c)
        {
            return Math.Min(a, Math.Min(b, c));
        }

        public static float Min3(float a, float b, float c)
        {
            return Math.Min(a, Math.Min(b, c));
        }

        public static int Min3(int a, int b, int c)
        {
            return Math.Min(a, Math.Min(b, c));
        }


        public static double Average3(double a, double b, double c)
        {
            return ((a + b + c) / 3);
        }

        public static DistD Average3(DistD a, DistD b, DistD c)
        {
            return new DistD((Average3(a.Value, b.Value, c.Value)));
        }


        public const double Pi = 3.1415926535897931;  // NOTE: Double precision rounds final "2" to "1"
        public const double PiOver2 = Pi / 2;
        public const double PiOver4 = Pi / 4;

        //        // Polar Coordinates
        //        public struct PolarPoint
        //        {
        //            public static void Test()
        //            {
        //                var t1 = new PolarPoint(0, 0);
        //                var t2 = new PolarPoint(1, 0);
        //                var t3 = new PolarPoint(10, 0);

        //                var t4 = new PolarPoint(10, 1);
        //                var t5 = new PolarPoint(10, 10);
        //                var t6 = new PolarPoint(1, 10);
        //                var t7 = new PolarPoint(0, 10);
        //                var t8 = new PolarPoint(-1, 10);
        //                var t9 = new PolarPoint(-10, 1);
        //                var t10 = new PolarPoint(-10, 0);
        //                var t11 = new PolarPoint(-10, -1);
        //                var t12 = new PolarPoint(-1, -10);
        //                var t13 = new PolarPoint(0, -10);
        //                var t14 = new PolarPoint(1, -10);
        //                var t15 = new PolarPoint(10, -10);
        //                var t16 = new PolarPoint(10, -1);
        //            }

        //            public double Angle;  // In Radians
        //            public double Length;

        //            public static PolarPoint Create(double angle, double length)
        //            {
        //                PolarPoint result = new PolarPoint();
        //                result.Angle = angle;
        //                result.Length = length;
        //                return result;
        //            }

        //            public PolarPoint(Distance2D pt) : this(pt.X(Distance), pt.Y(Distance))
        //            {
        //            }

        //            public PolarPoint(double x, double y)
        //            {
        //                Length = Math.Sqrt(x * x + y * y);

        //                // From http://en.wikipedia.org/wiki/Polar_coordinate_system
        //                // Angle in range (-Pi, Pi].
        //                if (x == 0)
        //                {
        //                    if (y > 0)
        //                        Angle = PiOver2;
        //                    else if (y < 0)
        //                        Angle = -PiOver2;
        //                    else
        //                        Angle = 0;// Arbitrary.
        //                }
        //                else
        //                {
        //                    Angle = Math.Atan(y / x);
        //                    if (x < 0)
        //                    {
        //                        if (y >= 0)
        //                            Angle += Pi;
        //                        else
        //                            Angle -= Pi;
        //                    }
        //                }
        //            }

        //            public static PolarPoint operator +(PolarPoint p1, Distance2D p2)
        //            {
        //                Distance2D sum = p1.AsPoint2D() + p2;
        //                return new PolarPoint(sum);
        //            }

        //            public Distance2D AsPoint2D()
        //            {
        //                // TODO: How will GeoContext deal with units?
        //                Distance2D pt = new Distance2D(Length * Math.Cos(Angle), Length * Math.Sin(Angle), null);
        //                return pt;
        //            }
        //        }

        // Handles "Nothing" properly
        public static int GetHashCode_OrZero(object ob)
        {
            if (ob == null)
                return 0;

            return ob.GetHashCode();
        }

        public struct Pair<T> : IEquatable<Pair<T>>
        {
            public readonly T First;
            public readonly T Second;

            public Pair(T first, T second)
            {
                this.First = first;
                this.Second = second;
            }

            public new bool Equals(Pair<T> other)
            {
                return Object.Equals(this.First, other.First) && Object.Equals(this.Second, other.Second);
            }
            public override bool Equals(object obj)
            {
                return obj is Pair<T> && Equals((Pair<T>)obj);
            }
            public override int GetHashCode()
            {
                return MakeHash(this.First, this.Second);
            }

            public override string ToString()
            {
                return "Pair: " + Convert.ToString(this.First) + ", " + Convert.ToString(this.Second);
            }
        }


        //        public class CPair<T> : IEquatable<CPair<T>>
        //        {
        //            public readonly T First;
        //            public readonly T Second;

        //            public CPair(T first, T second)
        //            {
        //                this.First = first;
        //                this.Second = second;
        //            }

        //            public new bool Equals(CPair<T> other)
        //            {
        //                return Object.Equals(this.First, other.First) && Object.Equals(this.Second, other.Second);
        //            }
        //            public override bool Equals(object obj)
        //            {
        //                return obj is Pair<T> && Equals((Pair<T>)obj);
        //            }
        //            public override int GetHashCode()
        //            {
        //                return MakeHash(this.First, this.Second);
        //            }

        //            public override string ToString()
        //            {
        //                return "Pair: " + Convert.ToString(this.First) + ", " + Convert.ToString(this.Second);
        //            }
        //        }

        //        public static double ToDouble(string str)
        //        {
        //            return StrVal(str);
        //        }

        //        // String to Double, Culture Independent
        //        public static double StrVal(string str)
        //        {
        //            if (string.IsNullOrEmpty(str))
        //                return 0;

        //            if (str.Contains(","))
        //                str = str.Replace(",", ".");
        //            if (str.Contains("%"))
        //                str = str.Replace("%", "");

        //            return Conversion.Val(str);
        //        }

        //        public static string AmericanDecimalSeparator(string str)
        //        {
        //            if (str.Contains(","))
        //                str = str.Replace(",", ".");

        //            return str;
        //        }


        //        public static int Str2Int(string str)
        //        {
        //            if (int.TryParse(str, out var result))
        //                return result;
        //            else
        //                return 0;
        //        }

        //        // SQL Uses a 0/1 value to represent boolean.
        //        public static bool Str0Or1ToBoolean(string str)
        //        {
        //            return (Str2Int(str) != 0);
        //        }

        //        // SQL Uses a 0/1 value to represent boolean.
        //        public static string BooleanAs0Or1Str(bool value)
        //        {
        //            // Could do "Convert.ToInt32(forMobile).ToString", but decided was more obvious to do as an If.
        //            return value ? "1" : "0";
        //        }

        //        // NOTE: Forces string to include a fractional part; e.g. "12" => "12.0"
        //        // Converts to American expectation of "." as "Decimal separator".
        //        public static string ValStr(float sngValue)
        //        {
        //            string strRet = System.Convert.ToString(sngValue).Replace(",", ".");

        //            if (!strRet.Contains("."))
        //                strRet += ".0";

        //            return strRet;
        //        }

        //        public static string ValStr(int value)
        //        {
        //            return System.Convert.ToString(value);
        //        }

        //        public static string ValStr0(int value)
        //        {
        //            return System.Convert.ToString(value);
        //        }

        //        // public static string DoubleAsString(double n)
        //        // {
        //        // string s = string.Format("{0:F20}", n);
        //        // int i = s.LastIndexOfAny("123456789.".ToCharArray());
        //        // if (i > 0 && i < s.Length - 1)
        //        // s = s.Remove(i + 1);
        //        // return s;
        //        // }


        //        // Integer; No fractional part.  (So, no "Decimal separator" character.)
        //        public static string ValStrInt(int value)
        //        {
        //            return System.Convert.ToString(value);
        //        }


        //        // NOTE: Forces string to include a fractional part; e.g. "12" => "12.0"
        //        // Converts to American expectation of "." as "Decimal separator".
        //        public static string ValStr(double dblValue)
        //        {
        //            string strRet = System.Convert.ToString(dblValue).Replace(",", ".");

        //            if (!strRet.Contains("."))
        //                strRet += ".0";

        //            return strRet;
        //        }


        //        // Round to N decimal digits.
        //        // Unlike ValStr for Single/Double, don't force fractional part, unless "forceDecimal=True".
        //        // Alternatively, see ValStrNFixed.
        //        // Converts to American expectation of "." as "Decimal separator".
        //        public static string ValStrN(double value, int nDigits, bool forceDecimal = false)
        //        {
        //            value = Math.Round(value, nDigits);

        //            // Converts to American expectation of "." as "Decimal separator".
        //            string ret = System.Convert.ToString(value).Replace(",", ".");

        //            // optionally force fractional part
        //            if (forceDecimal && !ret.Contains("."))
        //                ret += ".0";

        //            return ret;
        //        }

        //        // Round to 0 decimal digits. Unlike ValStr for Single/Double, don't force fractional part.
        //        // Converts to American expectation of "." as "Decimal separator".
        //        public static string ValStr0(double value)
        //        {
        //            return ValStrN(value, 0);
        //        }

        //        // Round to 1 decimal digits. Unlike ValStr for Single/Double, don't force fractional part.
        //        // Converts to American expectation of "." as "Decimal separator".
        //        public static string ValStr1(double value)
        //        {
        //            return ValStrN(value, 1);
        //        }

        //        // Round to 2 decimal digits. Unlike ValStr for Single/Double, don't force fractional part.
        //        // Converts to American expectation of "." as "Decimal separator".
        //        public static string ValStr2(double value)
        //        {
        //            return ValStrN(value, 2);
        //        }

        //        // Round to 3 decimal digits. Unlike ValStr for Single/Double, don't force fractional part.
        //        // Converts to American expectation of "." as "Decimal separator".
        //        public static string ValStr3(double value)
        //        {
        //            return ValStrN(value, 3);
        //        }

        //        // Round to 5 decimal digits. Unlike ValStr for Single/Double, don't force fractional part.
        //        // Converts to American expectation of "." as "Decimal separator".
        //        public static string ValStr5(double value)
        //        {
        //            return ValStrN(value, 5);
        //        }


        //        // "nDigits=3" is like ":3f", but always has American decimal separator.
        //        // Always has "nDigits" digits to right of decimal point.
        //        public static string ValStrNFixed(double value, int nDigits)
        //        {
        //            string format = "{0:f" + nDigits.ToString() + "}";
        //            string ret = AmericanDecimalSeparator(string.Format(format, value));
        //            return ret;
        //        }

        //        public static string ValStr6Fixed(double value)
        //        {
        //            return ValStrNFixed(value, 6);
        //        }

        //        public static string ValStr7Fixed(double value)
        //        {
        //            return ValStrNFixed(value, 7);
        //        }



        //        // "nDigits=7" is like ":e7", but always has American decimal separator.
        //        // Always has "nDigits" digits to right of decimal point.
        //        public static string ValStrNExp(double value, int nDigits)
        //        {
        //            string format = "{0:e" + nDigits.ToString() + "}";
        //            string ret = AmericanDecimalSeparator(string.Format(format, value));
        //            return ret;
        //        }

        //        public static string ValStr7Exp(double value)
        //        {
        //            return ValStrNExp(value, 7);
        //        }

        //        // color7 is "#RRGGBB".
        //        public static Color GetColor_FromCssColor7(string color7)
        //        {
        //            int red = ExtractRedInt(color7);
        //            int green = ExtractGreenInt(color7);
        //            int blue = ExtractBlueInt(color7);

        //            return Color.FromArgb(red, green, blue);
        //        }

        //        // color7 is "#RRGGBB".
        //        public static string GetCssColor7_FromColor(Color color)
        //        {
        //            string rr = Int_To_HexColor2(color.R);
        //            string gg = Int_To_HexColor2(color.G);
        //            string bb = Int_To_HexColor2(color.B);

        //            return "#" + rr + gg + bb;
        //        }

        //        // => (0..255)
        //        public static int ExtractRedInt(string color7)
        //        {
        //            return HexColor2_To_Int(ExtractRR(color7, 0));
        //        }
        //        // => (0..255)
        //        public static int ExtractGreenInt(string color7)
        //        {
        //            return HexColor2_To_Int(ExtractRR(color7, 1));
        //        }
        //        // => (0..255)
        //        public static int ExtractBlueInt(string color7)
        //        {
        //            return HexColor2_To_Int(ExtractRR(color7, 2));
        //        }

        //        // color is "#RRGGBB". compIndex: "0" => "RR", "1" => "GG", "2" => "BB".
        //        public static string ExtractRR(string color, int compIndex)
        //        {
        //            int index = (2 * compIndex) + 1;
        //            return color.Substring(index, 2);
        //        }


        //        // ("00".."FF") => (0..255)
        //        public static int HexColor2_To_Int(string rr)
        //        {
        //            return Convert.ToInt32(rr, 16);
        //        }

        //        // (0..255) => ("00".."FF")
        //        public static string Int_To_HexColor2(int red255)
        //        {
        //            return string.Format("{0:X2}", red255);
        //        }


        //        public static string FormatTimeSpanAsWithText(TimeSpan tsSpan)
        //        {
        //            string strRet = string.Empty;

        //            if (tsSpan.TotalDays >= 1)
        //            {
        //                strRet += tsSpan.TotalDays + " days ";
        //                strRet += tsSpan.TotalHours + " hours";
        //            }
        //            else
        //            {
        //                if (tsSpan.Hours >= 1)
        //                    strRet += tsSpan.Hours + " hours ";

        //                if (tsSpan.Minutes >= 1)
        //                    strRet += tsSpan.Minutes + " minutes ";

        //                strRet += tsSpan.Seconds + " seconds";
        //            }

        //            return strRet;
        //        }

        //        public static string FormatTimeSpanAsDaysWithTime(TimeSpan tsSpan)
        //        {
        //            string strRet = string.Empty;

        //            if (tsSpan.Days > 0)
        //                strRet += tsSpan.Days + " days ";

        //            strRet += FormatTimeField(tsSpan.Hours) + ":" + FormatTimeField(tsSpan.Minutes) + ":" + FormatTimeField(tsSpan.Seconds);

        //            return strRet;
        //        }

        //        public static string FormatTimeField(int intTime)
        //        {
        //            string strRet = string.Empty;

        //            if (intTime < 10)
        //                strRet = "0" + intTime;
        //            else
        //                strRet = System.Convert.ToString(intTime);

        //            return strRet;
        //        }

        //        public static int SizeOf(Type tpeType)
        //        {
        //            return System.Runtime.InteropServices.Marshal.SizeOf(tpeType);
        //        }





        //        public static string IntegerAsString2(int value)
        //        {
        //            return value.ToString("D2");
        //        }

        //        public static string IntegerAsHexString2(int value)
        //        {
        //            return value.ToString("X2");
        //        }


        //        public static string ZeroPadLeft(string strNumber, int nChars)
        //        {
        //            return strNumber.PadLeft(nChars, '0');
        //        }

        //        public static string ZeroPadLeft(int number, int nChars)
        //        {
        //            return System.Convert.ToString(number).PadLeft(nChars, '0');
        //        }

        //        public static string ZeroPadLeft(double number, int nChars)
        //        {
        //            return System.Convert.ToString(number).PadLeft(nChars, '0');
        //        }

        //        // This is equivalent to what is traditionally referred to as "left-pad" or "left-padding",
        //        // but "intMaxPrefixZero" is ONE SMALLER than the total number of characters desired.
        //        // E.g. "Left-pad N to 3 characters, using zeroes" => Format_Number_Preceding_Zeros(N, 2).
        //        public static string Format_Number_Preceding_Zeros(int intNumber, int intMaxPrefixZero)
        //        {
        //            return ZeroPadLeft(intNumber, intMaxPrefixZero + 1);
        //        }

        //        public static string Format_Number_Preceding_Zeros(double dblNumber, int intMaxPrefixZero)
        //        {
        //            return ZeroPadLeft(dblNumber, intMaxPrefixZero + 1);
        //        }


        //        public static string Format_XMLAttribute_For_Write(string str)
        //        {
        //            if (str == string.Empty)
        //                return string.Empty;

        //            Debug.WriteLine(str);
        //            Debug.WriteLine(Strings.Chr(34));
        //            Debug.WriteLine(System.Convert.ToString(Strings.Chr(34)));

        //            if (str.Contains(System.Convert.ToString(Strings.Chr(34))))
        //                str = str.Replace(System.Convert.ToString(Strings.Chr(34)), "&#34;");

        //            str = @"\\.\DISPLAY3";

        //            return str;
        //        }

        //        public static string Format_XMLAttribute_From_Read(string str)
        //        {
        //            if (str == string.Empty)
        //                return string.Empty;

        //            if (str.Contains(System.Convert.ToString(Strings.Chr(34))))
        //                str = str.Replace("&#34;", System.Convert.ToString(Strings.Chr(34)));

        //            return str;
        //        }

        //        public static string Format_ScreenDeviceName(string str)
        //        {
        //            if (!string.IsNullOrEmpty(str))
        //            {
        //                int intI = str.IndexOf(ControlChars.NullChar);

        //                if (intI > 0)
        //                    str = str.Substring(0, intI);
        //            }

        //            return str;
        //        }

        //        public static string Parse_Response_KeyValue(string strResponse, string strKey)
        //        {
        //            if (strResponse == string.Empty || !strResponse.Contains(strKey))
        //                return string.Empty;

        //            string strParse = strResponse.Substring(strResponse.IndexOf(strKey) + strKey.Length + 1);

        //            if (strParse.Contains(Constants.vbCrLf))
        //                strParse = strParse.Substring(0, strParse.IndexOf(Constants.vbCrLf));

        //            return strParse;
        //        }

        //        public static string Get_Relative_Path(string strParent, string strChild)
        //        {
        //            if (!strChild.Contains(strParent))
        //                // Throw New Exception("Invalid parameters")
        //                return string.Empty;

        //            if (!strParent.EndsWith(@"\"))
        //                strParent += @"\";

        //            string strSub = strChild.Remove(0, strParent.Length);

        //            if (!strSub.EndsWith(@"\") && strSub.Contains("."))
        //                strSub = strSub.Substring(0, strSub.LastIndexOf(@"\") + 1);

        //            if (strSub == @"\")
        //                return string.Empty;

        //            return strSub;
        //        }

        //        public static string Add_Relative_Path(string strParent, string strRelative)
        //        {
        //            if (strParent == string.Empty)
        //            {
        //                throw new Exception("Invalid parameters");
        //                return string.Empty;
        //            }

        //            string strRet = string.Empty;
        //            bool bolParentSuffixed = strParent.EndsWith(@"\");
        //            bool bolRelativePrefixed = strRelative.StartsWith(@"\");

        //            if (strRelative == string.Empty && !bolParentSuffixed)
        //                return strParent + @"\";

        //            if (!bolParentSuffixed && !bolRelativePrefixed)
        //                strRet = strParent + @"\" + strRelative;
        //            else if (bolParentSuffixed && bolRelativePrefixed)
        //                strRet = strParent + strRelative.Substring(1);
        //            else
        //                strRet = strParent + strRelative;

        //            return strRet;
        //        }

        //        public static string Get_Container_Folder(string strFileSearchpath)
        //        {
        //            if (strFileSearchpath == string.Empty)
        //            {
        //                throw new Exception("Invalid parameter");
        //                return string.Empty;
        //            }

        //            System.IO.FileInfo fleFile = new System.IO.FileInfo(strFileSearchpath);

        //            if (fleFile.Exists)
        //                return fleFile.DirectoryName;

        //            return string.Empty;
        //        }



        //        public static string Get_SupportedImages_FileDialog_Filter(bool bolVectorImages, bool bolRasterImages)
        //        {
        //            string result = "";

        //            if (bolVectorImages)
        //            {
        //                if (!string.IsNullOrEmpty(result))
        //                    result += "|";

        //                result += "AutoCAD DXF files|*.dxf";
        //            }

        //            if (bolRasterImages)
        //            {
        //                if (!string.IsNullOrEmpty(result))
        //                    result += "|";

        //                result += "BMP files|*.bmp";
        //                result += "|DirectX DDS files|*.dds";
        //                result += "|CompuServe GIF files|*.gif";
        //                result += "|JPEG files|*.jpg;*.jpeg";
        //                result += "|PNG files|*.png";
        //                result += "|TIFF files|*.tif;*.tiff";
        //            }

        //            if (bolVectorImages && bolRasterImages)
        //            {
        //                if (!string.IsNullOrEmpty(result))
        //                    result += "|";

        //                result += "All image files|*.dxf;*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.tif;*.tiff;";
        //            }
        //            else if (bolRasterImages)
        //            {
        //                if (!string.IsNullOrEmpty(result))
        //                    result += "|";

        //                result += "All image files|*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.tif;*.tiff;";
        //            }

        //            if (!string.IsNullOrEmpty(result))
        //                result += "|";

        //            result += "All files|*.*";

        //            return result;
        //        }

        //        public static bool Is_ImageType_Supported(string strSearchpath)
        //        {
        //            string strExtension = strSearchpath.Substring(strSearchpath.LastIndexOf("."), strSearchpath.Length - strSearchpath.LastIndexOf(".")).ToLower();

        //            if (strExtension.Equals(".dxf") || strExtension.Equals(".bmp") || strExtension.Equals(".gif") || strExtension.Equals(".jpg") || strExtension.Equals(".jpeg") || strExtension.Equals(".png") || strExtension.Equals(".tif") || strExtension.Equals(".tiff"))
        //                return true;

        //            return false;
        //        }

        //        public static bool Is_ImageType_PNG(string strSearchpath)
        //        {
        //            string strExtension = strSearchpath.Substring(strSearchpath.LastIndexOf("."), strSearchpath.Length - strSearchpath.LastIndexOf(".")).ToLower();

        //            if (strExtension.Equals(".png"))
        //                return true;

        //            return false;
        //        }

        //        public static bool Is_ImageType_TIFF(string strSearchpath)
        //        {
        //            string strExtension = strSearchpath.Substring(strSearchpath.LastIndexOf("."), strSearchpath.Length - strSearchpath.LastIndexOf(".")).ToLower();

        //            if (strExtension.Equals(".tif") || strExtension.Equals(".tiff"))
        //                return true;

        //            return false;
        //        }


        //        // "True" when we are sure new approach works.  "False" is the old logic (that does not avoid lock).
        //        const bool AVOID_BITMAP_LOCK = true;



        //        public static bool ContainsIgnoreCase(this string s, string chars)
        //        {
        //            return s.IndexOf(chars, 0, StringComparison.OrdinalIgnoreCase) >= 0;
        //        }

        //        // Exception if a.Length = 0.
        //        public static char LastCharacter(this string a)
        //        {
        //            return a[LastIndex(a)];
        //        }

        //        // If s.Length <= n, returns s (for null, returns empty string).
        //        public static string FirstNCharacters(this string s, int n)
        //        {
        //            if (string.IsNullOrEmpty(s))
        //                return "";
        //            else if (s.Length <= n)
        //                return s;
        //            else
        //                return s.Substring(0, n);
        //        }

        //        // Adjust so always "n" characters. Either adding blanks at right, or truncating.
        //        // HACK "limitCapitals": Option when variable-width characters; if more than this upper-case letters,
        //        // subtract 1 from n to limit display size.
        //        public static string PadRightOrTruncate(string s, int n, int limitCapitals = 0)
        //        {
        //            if (s == null)
        //                s = "";

        //            string result;
        //            // ">= n" rather than ">": When many capitals, will shorten to "n-1".
        //            if (limitCapitals > 0 && s.Length >= n)
        //            {
        //                result = s.Substring(0, n);
        //                // HACK: If "too many" capitals, reduce length by 1 to keep variable-width string from taking "too much" room.
        //                if (CountUppercase(result) > limitCapitals)
        //                    result = s.Substring(0, n - 1);
        //            }
        //            else
        //                // NOTE: Safe to call Substring, because after PadRight, we have at least n characters.
        //                result = s.PadRight(n).Substring(0, n);

        //            return result;
        //        }

        //        public static int CountUppercase(string s)
        //        {
        //            int count = 0;

        //            foreach (char c in s)
        //            {
        //                if (char.IsUpper(c))
        //                    count += 1;
        //            }

        //            return count;
        //        }

        //        // If s.Length <= n, returns s (for null, returns empty string).
        //        public static string LastNCharacters(this string s, int n)
        //        {
        //            if (string.IsNullOrEmpty(s))
        //                return "";
        //            else if (s.Length <= n)
        //                return s;
        //            else
        //                return s.Substring(s.Length - n);
        //        }

        //        public static string MaybeRemoveEnding(string str, string ending)
        //        {
        //            if (str.EndsWith(ending))
        //                str = str.Remove(str.Length - ending.Length);

        //            return str;
        //        }

        //        public static string MaybeRemoveEnding_IgnoreCase(string str, string ending)
        //        {
        //            if (str.EndsWith(ending, StringComparison.OrdinalIgnoreCase))
        //                str = str.Remove(str.Length - ending.Length);

        //            return str;
        //        }

        //        // E.g., removeCount = 1 returns the string with the last character removed.
        //        public static string RemoveLastN(this string str, int removeCount)
        //        {
        //            return str.Remove(str.Length - removeCount);
        //        }

        //        // For easier string comparison/sorting, if argument is Nothing, converts it to an empty string.
        //        public static string NullToEmpty(string str)
        //        {
        //            if (Exists(str))
        //                return str;

        //            return "";
        //        }


        //        // Equivalent to HasContents.
        //        public static bool NotEmpty(string str)
        //        {
        //            return !string.IsNullOrEmpty(str);
        //        }


        //        public static string Indent(int indentCount, string indentStr, string msg)
        //        {
        //            StringBuilder sb = new StringBuilder(indentCount * indentStr.Length + msg.Length);

        //            for (int i = 1; i <= indentCount; i++)
        //                sb.Append(indentStr);

        //            sb.Append(msg);

        //            return sb.ToString();
        //        }

        //        // E.g. array containing (11, 22, 33) turn into string "11, 22, 33".
        //        public static string ElementsToString(IList lis, string separator)
        //        {
        //            StringBuilder sb = new StringBuilder();

        //            bool first = true;
        //            foreach (object ob in lis)
        //            {
        //                if (first)
        //                    first = false;
        //                else
        //                    sb.Append(separator);

        //                sb.Append(ob.ToString());
        //            }

        //            return sb.ToString();
        //        }


        //        // Call this if we have a path that should exist, but don't know/care/specify whether it should be a file or directory.
        //        public static bool FileOrDirectoryExists(string path)
        //        {
        //            return System.IO.File.Exists(path) || System.IO.Directory.Exists(path);
        //        }

        //        // strDirectory optionally includes final "\" or "/".
        //        public static void EnsureDirectoryExists(string strDirectory)
        //        {
        //            strDirectory = MaybeRemoveFinalSlash(strDirectory);
        //            if (!System.IO.Directory.Exists(strDirectory))
        //                System.IO.Directory.CreateDirectory(strDirectory);
        //        }

        //        public static void EnsureParentDirectoryExists(string strPath)
        //        {
        //            string strDirectory = System.IO.Path.GetDirectoryName(strPath);
        //            EnsureDirectoryExists(strDirectory);
        //        }

        //        public static bool IsAbsolutePath(string strPath)
        //        {
        //            // "\\" for UNC path; ":" for drive-letter path.
        //            return strPath.StartsWith(@"\\") || strPath.Contains(':');
        //        }

        //        // Return True if file existed.
        //        // Unlike IO.File.Delete, can handle missing or invalid name.
        //        // Retries several times, in case there is a transient lock on file (e.g. Dropbox).
        //        // "suppressExceptions": We don't care if anything is wrong; just silently do nothing.
        //        public static bool DeleteFileIfExists(string filePath, bool suppressExceptions = false)
        //        {
        //            // NOTE: Normally it is not necessary to check Exists before calling Delete;
        //            // the advantage of this approach is it avoids Exception if filePath is invalid.
        //            // E.g. filename empty or characters not permitted in a path.
        //            // More importantly, if a folder on the path does not exist or is inaccessible (permissions), this won't exception.
        //            // Can still get exception if file exists, but is locked or protected against change.
        //            if (System.IO.File.Exists(filePath))
        //            {
        //                int nMore = 3;
        //                while (nMore > 0)
        //                {
        //                    // SIDE-EFFECT: decrements nMore.
        //                    if (Try1DeleteFile(filePath, suppressExceptions, ref nMore))
        //                        return true;
        //                    //TODO Application.DoEvents();
        //                    System.Threading.Thread.Sleep(50);
        //                }
        //            }

        //            return false;
        //        }

        //        // Suppress exceptions, except on last attempt.
        //        // SIDE-EFFECT: decrements nMore.
        //        private static bool Try1DeleteFile(string filePath, bool suppressExceptions, ref int nMore)
        //        {
        //            nMore -= 1;
        //            try
        //            {
        //                System.IO.File.Delete(filePath);
        //                return true;
        //            }
        //            catch (Exception ex)
        //            {
        //                if (suppressExceptions || nMore > 0)
        //                    return false;
        //                else
        //                    throw;
        //            }
        //        }

        //        // When "force", deletes any destination file.
        //        // Retries several times, in case there is a transient lock on file (e.g. Dropbox).
        //        // Suppresses exceptions; returns false if fails.
        //        public static bool MoveFile(string srcFile, string dstFile, bool force = false)
        //        {
        //            if (System.IO.File.Exists(srcFile))
        //            {
        //                if (System.IO.File.Exists(dstFile))
        //                {
        //                    if (force)
        //                    {
        //                        if (!DeleteFileIfExists(dstFile, true))
        //                            // Unable to delete dstFile.
        //                            return false;
        //                    }
        //                    else
        //                        // dstFile exists; Caller did not give permission to delete.
        //                        return false;
        //                }

        //                // Could be a parameter.
        //                bool suppressExceptions = true;
        //                int nMore = 3;
        //                while (nMore > 0)
        //                {
        //                    // SIDE-EFFECT: decrements nMore.
        //                    if (Try1MoveFile(srcFile, dstFile, suppressExceptions, ref nMore))
        //                        return true;
        //                    System.Threading.Thread.Sleep(50);
        //                }
        //            }

        //            return false;
        //        }

        //        // Suppress exceptions, except on last attempt.
        //        // SIDE-EFFECT: decrements nMore.
        //        private static bool Try1MoveFile(string srcFile, string dstFile, bool suppressExceptions, ref int nMore)
        //        {
        //            nMore -= 1;
        //            try
        //            {
        //                System.IO.File.Move(srcFile, dstFile);
        //                return true;
        //            }
        //            catch (Exception ex)
        //            {
        //                if (suppressExceptions || nMore > 0)
        //                    return false;
        //                else
        //                    throw;
        //            }
        //        }


        //        // NOTE: Always returns in lower-case.
        //        // Includes initial ".", unless "withPeriod=False"
        //        // Is equivalent to System.IO.Path.GetExtension.
        //        public static string GetFileExtension(string filePath, bool withPeriod = true)
        //        {
        //            if (string.IsNullOrEmpty(filePath))
        //                return "";

        //            int lastPeriodIndex = filePath.LastIndexOf(".");
        //            if (lastPeriodIndex < 0)
        //                return "";

        //            // When "withPeriod=False", move 1 forward in string, to skip the initial period.
        //            int maybeOne = withPeriod ? 0 : 1;
        //            string extension = filePath.Substring(lastPeriodIndex + maybeOne).ToLower();
        //            return extension;
        //        }

        //        // Removes final "." and characters after it.
        //        public static string StripFileExtension(string filePath)
        //        {
        //            int lastPeriodIndex;
        //            return StripFileExtension(filePath, out lastPeriodIndex);
        //        }

        //        // Removes final "." and characters after it.
        //        public static string StripFileExtension(string filePath, out int lastPeriodIndex)
        //        {
        //            if (string.IsNullOrEmpty(filePath))
        //            {
        //                // "-1": No period.
        //                lastPeriodIndex = -1;
        //                return "";
        //            }

        //            lastPeriodIndex = filePath.LastIndexOf(".");
        //            // If "-1": No period.
        //            if (lastPeriodIndex < 0)
        //                return filePath;

        //            filePath = filePath.Remove(lastPeriodIndex);
        //            return filePath;
        //        }

        //        /// <summary>
        //        ///     ''' Similar to stripping extension, but starts with FIRST dot.
        //        ///     ''' (Extension starts with LAST dot.)
        //        ///     ''' </summary>
        //        ///     ''' <param name="filePath"></param>
        //        ///     ''' <returns></returns>
        //        public static string StripFirstDotAndFollowing(string filePath, out int firstPeriodIndex)
        //        {
        //            firstPeriodIndex = -1;

        //            if (string.IsNullOrEmpty(filePath))
        //                return "";

        //            firstPeriodIndex = filePath.IndexOf(".");
        //            if (firstPeriodIndex < 0)
        //                return filePath;

        //            filePath = filePath.Remove(firstPeriodIndex);
        //            return filePath;
        //        }

        //        // Return fileName; Out extension.
        //        public static string SplitFileNameAndExtension(string filePath, out string extension, bool withPeriod = true)
        //        {
        //            extension = GetFileExtension(filePath, withPeriod);

        //            string fileName = StripFileExtension(filePath);
        //            return fileName;
        //        }

        //        // E.g. ("dir/name.ext", "more") => "dir/namemore.ext"
        //        public static string InsertBeforeFileExtension(string filePath, string suffix)
        //        {
        //            string extension = null;
        //            string fileName = SplitFileNameAndExtension(filePath, out extension);

        //            return fileName + suffix + extension;
        //        }

        //        // E.g. newExtension=".png" to make a ".png" file name from a different graphics format name.
        //        // TBD: Should this ADD the "." if it is missing from newExtension?
        //        public static string ReplaceFileExtension(string filePath, string newExtension)
        //        {
        //            filePath = filePath.Remove(filePath.LastIndexOf("."));
        //            filePath = string.Concat(filePath, newExtension);
        //            return filePath;
        //        }

        //        // aka StripDirectories - in case programmer searches for this name.
        //        // Is equivalent to System.IO.Path.GetFileName.
        //        public static string GetFileName(string filePath)
        //        {
        //            return StripDirectories(filePath);
        //        }

        //        // Is equivalent to System.IO.Path.GetFileName.
        //        public static string StripDirectories(string filePath)
        //        {
        //            if (string.IsNullOrEmpty(filePath))
        //                return filePath;

        //            // Path could have a mix of both types of slashes, so attempt to remove both.
        //            var slashIndex = filePath.LastIndexOf(@"\");
        //            if (slashIndex >= 0)
        //                filePath = filePath.Remove(0, slashIndex + 1);

        //            slashIndex = filePath.LastIndexOf("/");
        //            if (slashIndex >= 0)
        //                filePath = filePath.Remove(0, slashIndex + 1);

        //            return filePath;
        //        }

        //        // NOTE: Return value ends with "\" or "/", unless there is no "\" in the filePath,
        //        // in which case it returns empty string.
        //        // If "removeFinalSlash = True", then return value does NOT have that final "\" or "/".
        //        // When "removeFinalSlash", is equivalent to System.IO.Path.GetDirectoryName.
        //        public static string GetDirectoriesPath(string filePath, bool removeFinalSlash = false)
        //        {
        //            string directoriesPath = null;
        //            string filename = null;
        //            SplitDirectoriesFromFileName(filePath, out directoriesPath, out filename, removeFinalSlash);

        //            return directoriesPath;
        //        }

        //        // NOTE: directoriesPath ends with "\" or "/", unless there is no "\" in the filePath,
        //        // in which case directoriesPath becomes empty string.
        //        // If "removeFinalSlash = True", then directoriesPath does NOT have that final "\" or "/".
        //        public static void SplitDirectoriesFromFileName(string filePath, out string directoriesPath, out string filename, bool removeFinalSlash = false)
        //        {
        //            // Find last (back)slash character.
        //            int backslashIndex = filePath.LastIndexOf(@"\");
        //            int slashIndex = filePath.LastIndexOf("/");
        //            if (backslashIndex > slashIndex)
        //                slashIndex = backslashIndex;

        //            // OPTIONAL: Fix bad path that has doubled "/" or "\" after final directory.
        //            if (((slashIndex > 0) && (filePath[slashIndex - 1] == filePath[slashIndex])))
        //                // Remove the extra "/" or "\"
        //                slashIndex -= 1;

        //            if (slashIndex >= 0)
        //            {
        //                if (removeFinalSlash)
        //                    // Everything before the last (back)slash.
        //                    directoriesPath = filePath.Remove(slashIndex);
        //                else
        //                    // Include the last (back)slash.
        //                    directoriesPath = filePath.Remove(slashIndex + 1);
        //                // Everything after the last (back)slash.
        //                filename = filePath.Substring(slashIndex + 1);
        //            }
        //            else
        //            {
        //                directoriesPath = "";
        //                filename = filePath;
        //            }
        //        }

        //        // For prepending to filename, if doesn't have final (back)slash, add it.
        //        // Check for both kinds of slash at path end.
        //        // If neither, check what kind of slash (if any) used in path,
        //        // add same kind of slash at end.
        //        public static string MaybeAddFinalSlash(string dirPath)
        //        {
        //            if ((!dirPath.EndsWith(@"\")) && (!dirPath.EndsWith("/")))
        //            {
        //                if (dirPath.Contains(@"\"))
        //                    dirPath = dirPath + @"\";
        //                else
        //                    // Defaults to cross-platform forward slash.
        //                    dirPath = dirPath + "/";
        //            }

        //            return dirPath;
        //        }

        //        // If path ends in "\" or "/", remove that.
        //        public static string MaybeRemoveFinalSlash(string dirPath)
        //        {
        //            if (dirPath.EndsWith(@"\") || dirPath.EndsWith("/"))
        //                dirPath = dirPath.Remove(dirPath.Length - 1);

        //            return dirPath;
        //        }

        //        // For use on OS (or script) that expects forward slash as directory separator character.
        //        public static string ConvertBackslashToSlash(string filePath)
        //        {
        //            return filePath.Replace(@"\", "/");
        //        }

        //        // Strip directories and extension.
        //        // Is equivalent to System.IO.Path.GetFileNameWithoutExtension.
        //        public static string GetSimpleFileName(string filePath)
        //        {
        //            return StripFileExtension(StripDirectories(filePath));
        //        }

        //        public static string MakePath(string dirPath, string fileName)
        //        {
        //            return MaybeAddFinalSlash(dirPath) + fileName;
        //        }


        //        // Given a value, return the key(s) which map to that value.
        //        public static IList<TKey> KeysFromValue<TKey, TValue>(Dictionary<TKey, TValue> dict, TValue val)
        //        {
        //            if (dict == null)
        //                throw new ArgumentNullException("dict");

        //            return dict.Keys.Where(key => dict[key].Equals(val)).ToList();
        //        }

        //        /// <summary>
        //        ///     ''' Used with dictionary that keeps a list of elements for each key.
        //        ///     ''' </summary>
        //        ///     ''' <typeparam name="TKey"></typeparam>
        //        ///     ''' <typeparam name="TValueElement"></typeparam>
        //        ///     ''' <param name="keyToValueList"></param>
        //        ///     ''' <param name="key"></param>
        //        ///     ''' <param name="valueElement"></param>
        //        ///     ''' <returns>True if started new list (first appearance of 'key')</returns>
        //        public static bool AddTo<TKey, TValueElement>(Dictionary<TKey, List<TValueElement>> keyToValueList, TKey key, TValueElement valueElement)
        //        {
        //            bool isNewList = false;

        //            List<TValueElement> valueElements;
        //            if (!keyToValueList.TryGetValue(key, out valueElements))
        //            {
        //                valueElements = new List<TValueElement>();
        //                keyToValueList[key] = valueElements;
        //                isNewList = true;
        //            }

        //            valueElements.Add(valueElement);
        //            return isNewList;
        //        }

        //        public delegate bool NearlyEqualsT<T>(T a, T b);

        //        private static double t_MaxNElements = 0;

        //        // True if did add.
        //        public static bool MaybeAddTo<TKey, TValueElement>(Dictionary<TKey, List<TValueElement>> keyToValueList, TKey key, TValueElement newValueElement, NearlyEqualsT<TValueElement> nearly)
        //        {
        //            List<TValueElement> valueElements;
        //            if (keyToValueList.TryGetValue(key, out valueElements))
        //            {
        //                //AccumMax(valueElements.Count, t_MaxNElements);
        //                // Is a NearlyEquals element already there?
        //                // TBD: Slow if many elements for a key.
        //                foreach (TValueElement oldValueElement in valueElements)
        //                {
        //                    if (nearly(oldValueElement, newValueElement))
        //                        return false;
        //                    else
        //                        // Do add below.
        //                        Good();
        //                }
        //            }
        //            else
        //            {
        //                valueElements = new List<TValueElement>();
        //                keyToValueList[key] = valueElements;
        //            }

        //            valueElements.Add(newValueElement);
        //            return true;
        //        }



        //        public static void MergeArray<T>(ref T[] a, T[] b)
        //        {
        //            if (b == null || b.Length < 1)
        //                return;

        //            if (a != null)
        //            {
        //                int len = a.Length;
        //                var oldA = a;
        //                a = new T[a.Length + b.Length - 1 + 1];
        //                if (oldA != null)
        //                    Array.Copy(oldA, a, Math.Min(a.Length + b.Length - 1 + 1, oldA.Length));
        //                Array.Copy(b, 0, a, len, b.Length);
        //            }
        //            else
        //            {
        //                a = new T[b.Length - 1 + 1];
        //                Array.Copy(b, 0, a, 0, b.Length);
        //            }
        //        }

        //        // If first element of b is same as last element of a, then omit it.
        //        // (Avoids duplicate value, in this special case.)
        //        public static void MergeArray_OmitCommonEnd<T>(ref T[] a, T[] b)
        //        {
        //            if (b == null || b.Length < 1)
        //                return;

        //            if (a != null)
        //            {
        //                int len = a.Length;
        //                int bStartIndex = 0;
        //                if ((a.Length > 0) && (LastElement(a).Equals(b[0])))
        //                    bStartIndex = 1;
        //                int nAdded = b.Length - bStartIndex;
        //                var oldA = a;
        //                a = new T[len + nAdded - 1 + 1];
        //                if (oldA != null)
        //                    Array.Copy(oldA, a, Math.Min(len + nAdded - 1 + 1, oldA.Length));
        //                Array.Copy(b, bStartIndex, a, len, nAdded);
        //            }
        //            else
        //            {
        //                a = new T[b.Length - 1 + 1];
        //                Array.Copy(b, 0, a, 0, b.Length);
        //            }
        //        }

        //        public static void ArrayPrependItem<T>(ref T[] a, T value)
        //        {
        //            ArrayIncrementLength(ref a);
        //            // Move all elements to one higher index. (Last first).
        //            for (int i = LastIndex(a); i >= 1; i += -1)
        //                a[i] = a[i - 1];

        //            a[0] = value;
        //        }

        //        // "a" allowed to be Nothing; in that case will become a one-element array.
        //        public static void ArrayAppendItem<T>(ref T[] a, T value)
        //        {
        //            ArrayIncrementLength(ref a);
        //            a[LastIndex(a)] = value;
        //        }

        //        // "a" allowed to be Nothing; in that case will become a one-element array.
        //        public static void ArrayAppendItems<T>(ref T[] a, T[] values)
        //        {
        //            foreach (T value in values)
        //                ArrayAppendItem<T>(ref a, value);
        //        }


        //        public static void ArrayRemoveIndex<T>(ref T[] a, int i)
        //        {
        //            if (i > -1)
        //            {
        //                for (int j = i; j <= a.Length - 2; j++)
        //                    a[j] = a[j + 1];

        //                ArrayDecrementLength(ref a);
        //            }
        //        }


        //        public static void ArrayRemoveItem<T>(ref T[] a, T value)
        //        {
        //            int i = Array.IndexOf(a, value);

        //            ArrayRemoveIndex(ref a, i);
        //        }

        //        // Preserves existing contents of "a".
        //        // "a" allowed to be Nothing; in that case will become a one-element array.
        //        // Otherwise, will gain one element at end (preserves existing contents).
        //        public static void ArrayIncrementLength<T>(ref T[] a)
        //        {
        //            if (a == null)
        //                // New last index is zero, therefore contains one element.
        //                a = new T[1];
        //            else
        //            {
        //                var oldA = a;
        //                a = new T[a.Length + 1];
        //                // Old length becomes new last index, therefore one longer.
        //                if (oldA != null)
        //                    Array.Copy(oldA, a, Math.Min(a.Length + 1, oldA.Length));
        //            }
        //        }

        //        public static void ArrayDecrementLength<T>(ref T[] a)
        //        {
        //            if (a == null || a.Length <= 1)
        //            {
        //            }
        //            else
        //            {
        //                var oldA = a;
        //                a = new T[a.Length - 2 + 1];
        //                // Old length becomes new last index, therefore one longer.
        //                if (oldA != null)
        //                    Array.Copy(oldA, a, Math.Min(a.Length - 2 + 1, oldA.Length));
        //            }
        //        }

        //        public static void ArrayInsertItem<T>(ref T[] a, T value, int index)
        //        {
        //            ArrayIncrementLength(ref a);

        //            for (int i = a.Length - 2; i >= index; i += -1)
        //                a[i + 1] = a[i];

        //            a[index] = value;
        //        }

        //        public static void ListRemoveRange<t>(ref List<t> list, ref List<t> toRemove)
        //        {
        //            foreach (t item in toRemove)
        //                list.Remove(item);
        //        }



        //        // "ctr" set to 0, if don't need to append a number (filepath does not yet exist).
        //        // Caller might use ctr to decide to start overwriting existing files.
        //        public static string GetUniqueFileNameIfExists(string filepath, out int ctr)
        //        {
        //            ctr = 0;

        //            if (System.IO.File.Exists(filepath))
        //                filepath = GetUniqueFileName(filepath, out ctr);

        //            return filepath;
        //        }

        //        // SEE ALTERNATIVE: cProject.AutoBackups_RenameOriginal.
        //        public static string GetUniqueFileName(string filepath, out int ctr)
        //        {
        //            string ext = "";
        //            string dir = Utils.GetDirectoriesPath(filepath);
        //            string basename = Utils.SplitFileNameAndExtension(filepath, out ext);

        //            // "0": Incremented before use, so first appended value is "1".
        //            ctr = 0;
        //            do
        //            {
        //                ctr += 1;
        //                filepath = dir + basename + ctr + ext;
        //            }
        //            while (System.IO.File.Exists(filepath));

        //            return filepath;
        //        }



        //        public static T GetTarget<T>(WeakReference<T> weakRef) where T : class
        //        {
        //            if (weakRef == null)
        //                return null;

        //            T target = null;
        //            weakRef.TryGetTarget(out target);


        //            // ' OPTIONAL: Clear a no longer valid weakRef.
        //            // ' REQUIRES: "ByRef" weakRef.
        //            // If target Is Nothing Then
        //            // weakRef = Nothing
        //            // End If

        //            return target;
        //        }

        //        // Re-use an existing WeakReference.
        //        // TBD: If target is Nothing, should this set weakRef to Nothing?
        //        public static void SetTarget<T>(ref WeakReference<T> weakRef, T target) where T : class
        //        {
        //            if (weakRef == null)
        //                weakRef = AsWeakReference(target);
        //            else
        //                weakRef.SetTarget(target);
        //        }

        //        // Return a new WeakReference. See "SetTarget" for an alternative.
        //        // "target" can be Nothing. If so, the result is Nothing.
        //        public static WeakReference<T> AsWeakReference<T>(T target) where T : class
        //        {
        //            if (target == null)
        //                return null;

        //            return new WeakReference<T>(target);
        //        }


        //        // NOTE: If caller can guarantee "value" not already in weakHash, then caller can simply do "weakHash.Add(AsWeakReference(value))".
        //        public static void Remember<T>(T value, HashSet<WeakReference<T>> weakHash) where T : class
        //        {
        //            WeakReference<T> found = Find(value, weakHash);
        //            if (found == null)
        //                // Add value to weakHash.
        //                weakHash.Add(AsWeakReference(value));
        //        }

        //        public static void Forget<T>(T value, HashSet<WeakReference<T>> weakHash) where T : class
        //        {
        //            WeakReference<T> found = Find(value, weakHash);
        //            if (Exists(found))
        //                weakHash.Remove(found);
        //        }

        //        public static WeakReference<T> Find<T>(T value, HashSet<WeakReference<T>> weakHash) where T : class
        //        {
        //            WeakReference<T> found = null;

        //            // OPTIONAL: Prune one obsolete entry each call.
        //            WeakReference<T> toRemove = null;

        //            // Given a collection of weak references, linear search [O(N)] to see whether any targets are "target".
        //            foreach (WeakReference<T> weakRef in weakHash)
        //            {
        //                T existingTarget;
        //                if (weakRef.TryGetTarget(out existingTarget))
        //                {
        //                    // weakRef is still alive.
        //                    if (existingTarget.Equals(value))
        //                    {
        //                        // "value" is already held.
        //                        found = weakRef;
        //                        break;
        //                    }
        //                }
        //                else
        //                    // weakRef is obsolete.
        //                    toRemove = weakRef;
        //            }

        //            // OPTIONAL: Prune (up to) one obsolete entry each call.
        //            if (Exists(toRemove))
        //                weakHash.Remove(toRemove);

        //            return found;
        //        }
    }

}
