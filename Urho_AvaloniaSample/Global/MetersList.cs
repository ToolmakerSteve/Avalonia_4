using System;
using System.Collections;
using System.Collections.Generic;

namespace Global
{
    public class MetersList : IList<Meters>
    {
        #region "-- data --"
        public readonly List<double> Values = new List<double>();
        #endregion


        #region "-- new --"
        public MetersList()
        {
        }
        #endregion


        #region "-- IEnumerable<Length> --"
        public IEnumerator<Meters> GetEnumerator()
        {
            foreach (var value in Values)
            {
                yield return new Meters(value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion


        #region "-- IList<Meters> --"
        public Meters this[int index]
        {
            get => new Meters(Values[index]);
            set => Values[index] = value.Value;
        }

        public int Count => Values.Count;

        public bool IsReadOnly => false;

        public void Add(Meters item)
        {
            Values.Add(item.Value);
        }

        public void Clear()
        {
            Values.Clear();
        }

        /// <summary>
        /// TBD: Should add an alternative using NearlyEquals and a Tolerance.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(Meters item)
        {
            return Values.Contains(item.Value);
        }

        public void CopyTo(Meters[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TBD: Should add an alternative using NearlyEquals and a Tolerance.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(Meters item)
        {
            return Values.IndexOf(item.Value);
        }

        public void Insert(int index, Meters item)
        {
            Values.Insert(index, item.Value);
        }

        /// <summary>
        /// TBD: Should add an alternative using NearlyEquals and a Tolerance.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(Meters item)
        {
            return Values.Remove(item.Value);
        }

        public void RemoveAt(int index)
        {
            Values.RemoveAt(index);
        }
        #endregion


        #region "-- public methods --"
        #endregion
    }
}
