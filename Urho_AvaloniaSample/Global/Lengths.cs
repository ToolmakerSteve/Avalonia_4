using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    /// <summary>
    /// A compact representation of a set of values with the same unit.
    /// </summary>
    public class Lengths : IList<Length>
    {
        #region "-- data --"
        public readonly List<double> Values = new List<double>();
        public readonly ELengthUnit Unit;
        /// <summary>
        /// Convenience (performance): based on "Unit".
        /// </summary>
        public readonly LengthUnit OurUnit;
        #endregion


        #region "-- new --"
        public Lengths(ELengthUnit unit)
        {
            Unit = unit;
            OurUnit = LengthUnit.AsLengthUnit(unit);
        }
        #endregion


        #region "-- IEnumerable<Length> --"
        public IEnumerator<Length> GetEnumerator()
        {
            foreach (var value in Values)
            {
                yield return new Length(value, Unit);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion


        #region "-- IList<Length> --"
        public Length this[int index]
        {
            get => new Length(Values[index], Unit);
            set => Values[index] = ToOurUnits(value);
        }

        public int Count => Values.Count;

        public bool IsReadOnly => false;

        public void Add(Length item)
        {
            Values.Add(ToOurUnits(item));
        }

        public void Clear()
        {
            Values.Clear();
        }

        /// <summary>
        /// TBD: Should this use "NearlyEquals" value comparison?
        /// A: Yes, if any unit conversion might have occurred.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(Length item)
        {
            double value = ToOurUnits(item);
            return Values.Contains(value);
        }

        public void CopyTo(Length[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TBD: Should this use "NearlyEquals" value comparison?
        /// A: Yes, if any unit conversion might have occurred.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(Length item)
        {
            double value = ToOurUnits(item);
            return Values.IndexOf(value);
        }

        public void Insert(int index, Length item)
        {
            Values.Insert(index, ToOurUnits(item));
        }

        /// <summary>
        /// TBD: Should this use "NearlyEquals" value comparison?
        /// A: UNCLEAR, given that it is a "destructive" operation.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(Length item)
        {
            double value = ToOurUnits(item);
            return Values.Remove(value);
        }

        public void RemoveAt(int index)
        {
            Values.RemoveAt(index);
        }
        #endregion


        #region "-- public methods --"
        public double ToOurUnits(Length value)
        {
            double finalValue = value.Value;
            if (value.Unit != Unit)
            {   // Convert to our Unit.
                finalValue = OurUnit.FromMeters(value.AsMeters);
            }

            return finalValue;
        }
        #endregion

    }
}
