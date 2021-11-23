using System;
using System.Collections;
using System.Collections.Generic;
using Global;

namespace Geo
{
    /// <summary>
    /// A compact representation of a set of points with the same IGeoContext.
    /// </summary>
    public class GeoDistance3s : IList<GeoPoint3D>
    {
        #region --- data ----------------------------------------
        IGeoContext Context;
        public readonly List<Distance3D> Values = new List<Distance3D>();
        #endregion


        #region --- new ----------------------------------------
        public GeoDistance3s(IGeoContext context)
        {
            Context = context;
        }
        #endregion


        #region --- IEnumerable<GeoPoint3D> ----------------------------------------
        public IEnumerator<GeoPoint3D> GetEnumerator()
        {
            foreach (var value in Values)
            {
                yield return new GeoPoint3D(value, Context);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion


        #region --- IList<GeoPoint3D> ----------------------------------------
        public GeoPoint3D this[int index]
        {
            get => new GeoPoint3D(Values[index], Context);
            set => Values[index] = ToOurContext(value);
        }

        public int Count => Values.Count;

        public bool IsReadOnly => false;

        public void Add(GeoPoint3D item)
        {
            Values.Add(ToOurContext(item));
        }

        public void Clear()
        {
            Values.Clear();
        }

        /// <summary>
        /// TBD: Should this use "NearlyEquals" value comparison?
        /// A: Yes, if any units conversion might have occurred.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(GeoPoint3D item)
        {
            Distance3D value = ToOurContext(item);
            return Values.Contains(value);
        }

        public void CopyTo(GeoPoint3D[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TBD: Should this use "NearlyEquals" value comparison?
        /// A: Yes, if any units conversion might have occurred.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(GeoPoint3D item)
        {
            Distance3D value = ToOurContext(item);
            return Values.IndexOf(value);
        }

        public void Insert(int index, GeoPoint3D item)
        {
            Values.Insert(index, ToOurContext(item));
        }

        /// <summary>
        /// TBD: Should this use "NearlyEquals" value comparison?
        /// A: UNCLEAR, given that it is a "destructive" operation.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(GeoPoint3D item)
        {
            Distance3D value = ToOurContext(item);
            return Values.Remove(value);
        }

        public void RemoveAt(int index)
        {
            Values.RemoveAt(index);
        }
        #endregion


        #region --- public methods ----------------------------------------
        public Distance3D ToOurContext(GeoPoint3D value)
        {
            Distance3D finalValue = (Distance3D)value.Pt;
            if (value.Context != Context)
            {   // Convert to our Context.
                throw new NotImplementedException("Distance3D.ToOurContext with different Context.");
                //TODO finalValue = OurUnit.FromMeters(value.AsMeters);
            }

            return finalValue;
        }
        #endregion

    }
}
