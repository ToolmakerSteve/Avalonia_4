using System;
using System.Collections;
using System.Collections.Generic;
using Global;

namespace Geo
{
    /// <summary>
    /// A compact representation of a set of points with the same IGeoContext.
    /// </summary>
    public class GeoPoint2Ds<TGeo, TPoint> : IList<GeoPoint2D> where TGeo : IGeoPoint where TPoint : IPoint
    {
        #region "-- data --"
        IGeoContext Context;
        public readonly List<Point2D> Values = new List<Point2D>();
        #endregion


        #region "-- new --"
        public GeoPoint2Ds(IGeoContext context)
        {
            Context = context;
        }
        #endregion


        #region "-- IEnumerable<GeoPoint2D> --"
        public IEnumerator<GeoPoint2D> GetEnumerator()
        {
            foreach (var value in Values)
            {
                yield return new GeoPoint2D(value, Context);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion


        #region "-- IList<GeoPoint2D> --"
        public GeoPoint2D this[int index]
        {
            get => new GeoPoint2D(Values[index], Context);
            set => Values[index] = ToOurContext(value);
        }

        public int Count => Values.Count;

        public bool IsReadOnly => false;

        public void Add(GeoPoint2D item)
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
        public bool Contains(GeoPoint2D item)
        {
            Point2D value = ToOurContext(item);
            return Values.Contains(value);
        }

        public void CopyTo(GeoPoint2D[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TBD: Should this use "NearlyEquals" value comparison?
        /// A: Yes, if any unit conversion might have occurred.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(GeoPoint2D item)
        {
            Point2D value = ToOurContext(item);
            return Values.IndexOf(value);
        }

        public void Insert(int index, GeoPoint2D item)
        {
            Values.Insert(index, ToOurContext(item));
        }

        /// <summary>
        /// TBD: Should this use "NearlyEquals" value comparison?
        /// A: UNCLEAR, given that it is a "destructive" operation.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(GeoPoint2D item)
        {
            Point2D value = ToOurContext(item);
            return Values.Remove(value);
        }

        public void RemoveAt(int index)
        {
            Values.RemoveAt(index);
        }
        #endregion


        #region "-- public methods --"
        public Point2D ToOurContext(GeoPoint2D value)
        {
            Point2D finalValue = value.Pt;
            if (value.Context != Context)
            {   // Convert to our Context.
                throw new NotImplementedException("Point2D.ToOurContext with different Context.");
                //TODO finalValue = OurUnit.FromMeters(value.AsMeters);
            }

            return finalValue;
        }
        #endregion

    }
}
