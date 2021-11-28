using System;
using System.Collections;
using System.Collections.Generic;

namespace Global
{
    public static partial class Geo
    {
        /// <summary>
        /// A compact representation of a set of points with the same Geo.IContext.
        /// </summary>
        public class Distance3s : IList<Point3D>
        {
            #region --- data ----------------------------------------
            IContext Context;
            public readonly List<Dist3D> Values = new List<Dist3D>();
            #endregion


            #region --- new ----------------------------------------
            public Distance3s(IContext context)
            {
                Context = context;
            }
            #endregion


            #region --- IEnumerable<Geo.Point3D> ----------------------------------------
            public IEnumerator<Point3D> GetEnumerator()
            {
                foreach (var value in Values)
                {
                    yield return new Point3D(value, Context);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            #endregion


            #region --- IList<Geo.Point3D> ----------------------------------------
            public Point3D this[int index]
            {
                get => new Point3D(Values[index], Context);
                set => Values[index] = ToOurContext(value);
            }

            public int Count => Values.Count;

            public bool IsReadOnly => false;

            public void Add(Point3D item)
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
            public bool Contains(Point3D item)
            {
                Dist3D value = ToOurContext(item);
                return Values.Contains(value);
            }

            public void CopyTo(Point3D[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// TBD: Should this use "NearlyEquals" value comparison?
            /// A: Yes, if any units conversion might have occurred.
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public int IndexOf(Point3D item)
            {
                Dist3D value = ToOurContext(item);
                return Values.IndexOf(value);
            }

            public void Insert(int index, Point3D item)
            {
                Values.Insert(index, ToOurContext(item));
            }

            /// <summary>
            /// TBD: Should this use "NearlyEquals" value comparison?
            /// A: UNCLEAR, given that it is a "destructive" operation.
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Remove(Point3D item)
            {
                Dist3D value = ToOurContext(item);
                return Values.Remove(value);
            }

            public void RemoveAt(int index)
            {
                Values.RemoveAt(index);
            }
            #endregion


            #region --- public methods ----------------------------------------
            public Dist3D ToOurContext(Point3D value)
            {
                Dist3D finalValue = (Dist3D)value.Pt;
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
}
