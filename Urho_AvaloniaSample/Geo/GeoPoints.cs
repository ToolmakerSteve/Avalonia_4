using System;
using System.Collections;
using System.Collections.Generic;
using Global;

namespace Geo
{
    /// <summary>
    /// A compact representation of a set of points with the same IGeoContext.
    /// </summary>
    public class GeoPoints<TGeo, TPoint> : IList<TGeo>
            where TGeo : IGeoPoint, new() where TPoint : IPoint, new()
    {
        #region "-- data --"
        IGeoContext Context;
        public readonly List<TPoint> Values = new List<TPoint>();
        #endregion


        #region "-- new --"
        public GeoPoints(IGeoContext context)
        {
            Context = context;
        }
        #endregion


        #region "-- IEnumerable<TGeo> --"
        public IEnumerator<TGeo> GetEnumerator()
        {
            foreach (var value in Values)
            {
                yield return new TGeo() { IValue = value, Context = Context };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion


        #region "-- IList<TGeo> --"
        public TGeo this[int index]
        {
            get => new TGeo() { IValue = Values[index], Context = Context };
            set => Values[index] = ToOurContext(value);
        }

        public int Count => Values.Count;

        public bool IsReadOnly => false;

        public void Add(TGeo item)
        {
            Values.Add(ToOurContext(item));
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
        public bool Contains(TGeo item)
        {
            TPoint value = ToOurContext(item);
            return Values.Contains(value);
        }

        public void CopyTo(TGeo[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TBD: Should this use "NearlyEquals" value comparison?
        /// A: Yes, if any unit conversion might have occurred.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(TGeo item)
        {
            TPoint value = ToOurContext(item);
            return Values.IndexOf(value);
        }

        public void Insert(int index, TGeo item)
        {
            Values.Insert(index, ToOurContext(item));
        }

        /// <summary>
        /// TBD: Should this use "NearlyEquals" value comparison?
        /// A: UNCLEAR, given that it is a "destructive" operation.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(TGeo item)
        {
            TPoint value = ToOurContext(item);
            return Values.Remove(value);
        }

        public void RemoveAt(int index)
        {
            Values.RemoveAt(index);
        }
        #endregion


        #region "-- public methods --"
        public TPoint ToOurContext(TGeo value)
        {
            TPoint finalValue = (TPoint)value.IValue;
            if (value.Context != Context)
            {   // Convert to our Context.
                throw new NotImplementedException("TPoint.ToOurContext with different Context.");
                //TODO finalValue = OurUnit.FromMeters(value.AsMeters);
            }

            return finalValue;
        }
        #endregion

    }
}
