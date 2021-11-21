//using System;
//using System.Collections;
//using System.Collections.Generic;

//namespace Global
//{
//    /// <summary>
//    /// A compact representation of a set of values with the same units.
//    /// </summary>
//    public class Distances : IList<Distance>
//    {
//        #region "-- data --"
//        public readonly List<double> Values = new List<double>();
//        public readonly Distance.EUnits Units;
//        /// <summary>
//        /// Convenience (performance): based on "Units".
//        /// </summary>
//        public readonly Distance.UnitDesc OurUnit;
//        #endregion


//        #region "-- new --"
//        public Distances(Distance.EUnits units)
//        {
//            Units = units;
//            OurUnit = Distance.UnitDesc.AsDistanceUnit(units);
//        }
//        #endregion


//        #region "-- IEnumerable<Length> --"
//        public IEnumerator<Distance> GetEnumerator()
//        {
//            foreach (var value in Values)
//            {
//                yield return new Distance(value, Units);
//            }
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return GetEnumerator();
//        }
//        #endregion


//        #region "-- IList<Length> --"
//        public Distance this[int index]
//        {
//            get => new Distance(Values[index], Units);
//            set => Values[index] = ToOurUnits(value);
//        }

//        public int Count => Values.Count;

//        public bool IsReadOnly => false;

//        public void Add(Distance item)
//        {
//            Values.Add(ToOurUnits(item));
//        }

//        public void Clear()
//        {
//            Values.Clear();
//        }

//        /// <summary>
//        /// TBD: Should this use "NearlyEquals" value comparison?
//        /// A: Yes, if any units conversion might have occurred.
//        /// </summary>
//        /// <param name="item"></param>
//        /// <returns></returns>
//        public bool Contains(Distance item)
//        {
//            double value = ToOurUnits(item);
//            return Values.Contains(value);
//        }

//        public void CopyTo(Distance[] array, int arrayIndex)
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// TBD: Should this use "NearlyEquals" value comparison?
//        /// A: Yes, if any units conversion might have occurred.
//        /// </summary>
//        /// <param name="item"></param>
//        /// <returns></returns>
//        public int IndexOf(Distance item)
//        {
//            double value = ToOurUnits(item);
//            return Values.IndexOf(value);
//        }

//        public void Insert(int index, Distance item)
//        {
//            Values.Insert(index, ToOurUnits(item));
//        }

//        /// <summary>
//        /// TBD: Should this use "NearlyEquals" value comparison?
//        /// A: UNCLEAR, given that it is a "destructive" operation.
//        /// </summary>
//        /// <param name="item"></param>
//        /// <returns></returns>
//        public bool Remove(Distance item)
//        {
//            double value = ToOurUnits(item);
//            return Values.Remove(value);
//        }

//        public void RemoveAt(int index)
//        {
//            Values.RemoveAt(index);
//        }
//        #endregion


//        #region "-- public methods --"
//        public double ToOurUnits(Distance value)
//        {
//            double finalValue = value.Value;
//            if (value.Units != Units)
//            {   // Convert to our Units.
//                finalValue = OurUnit.FromMeters(value.Meters);
//            }

//            return finalValue;
//        }
//        #endregion
//    }
//}
