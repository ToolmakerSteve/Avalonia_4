using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    public partial struct Distance
    {
        /// <summary>
        /// 1:1 with "DistanceUnit.MeterMults".
        /// "Default" is a special case. Used for test cases where data is to use the current default units,
        /// without any conversion. Most code doesn't need to be written to handle it.
        /// OR handle by duplicating the UnitDesc for the default?
        /// </summary>
        public partial class UnitsType : IComparable, IComparable<UnitsType>
        {
            static UnitsType()
            {
                _InitializeDefaults();
            }

            public readonly int TypeIndex;
            public readonly string Name;
            public readonly string Abbreviation;
            public readonly double MetersPerUnit;
            public readonly double UnitsPerMeter;

            public override string ToString() { return "UnitsType<" + Name + ">"; }

            static public UnitsType __RegisterType(string typeName, string typeAbbrev, double metersPerUnit)
            {
                UnitsType unitsType;
                int index;

                lock (s_AllNameLookup)
                {
                    if (!s_AllNameLookup.TryGetValue(typeName, out unitsType))
                    {  // Not found - so create -- this is EXPECTED
                        if (__InstanceCount >= __MAX_NAMES)
                        {   // Too many Instances!  Log Error!
                            throw new OverflowException("Exceeded Maximum number of UnitsTypes: " + __MAX_NAMES);
                        }
                        index = __InstanceCount;
                        __InstanceCount++;
                    }
                    else
                    {   // already exists -- so we're going to overwrite it
                        index = unitsType.TypeIndex;
                    }
                }
                return __RegisterType(__InstanceCount, typeName, typeAbbrev, metersPerUnit);
            }

            static public UnitsType __RegisterType(int index, string typeName, string typeAbbrev, double metersPerUnit)
            {
                if (s_NumInstancesConstructed != 0)
                    throw new InvalidOperationException("Distance.UnitType.__RegisterType() - called AFTER Distances have been constructed: " + s_NumInstancesConstructed);

                UnitsType unitsType = new UnitsType(index, typeName, typeAbbrev, metersPerUnit);

                __AllInstances[index] = unitsType;
                s_AllNameLookup[typeName] = unitsType;

                return unitsType;
            }
            private UnitsType(int index, string typeName, string typeAbbrev, double metersPerUnit)
            {
                TypeIndex = index;
                Name = typeName;
                Abbreviation = typeAbbrev;
                MetersPerUnit = metersPerUnit;
                UnitsPerMeter = 1.0 / metersPerUnit;
            }

            static public UnitsType __GetByName(string typeName)
            {
                UnitsType unitsType;
                if (!s_AllNameLookup.TryGetValue(typeName, out unitsType))
                {
                    // NOTE/TODO: Log Error!
                    unitsType = UnitsType._DEFAULT;
                }
                return unitsType;
            }

            public const int __MAX_NAMES = 255;
            static public int __InstanceCount { get; private set; }

            static private Dictionary<string, UnitsType> s_AllNameLookup = new Dictionary<string, UnitsType>(__MAX_NAMES);
            static private UnitsType[] __AllInstances = new UnitsType[__MAX_NAMES];

            static public UnitsType __GetByIndex(int typeIndex)
            {
                return __AllInstances[typeIndex];
            }

            static public IEnumerable<UnitsType> __GetAllInstances()
            {
                for (int i = 0; i < __InstanceCount; i++)
                    yield return __AllInstances[i];
            }

            public override int GetHashCode()
            {
                return TypeIndex;
            }
            public override bool Equals(object obj)
            {
                if (obj == null || !(obj is UnitsType))
                {
                    return false;
                }
                UnitsType other = (UnitsType)obj;
                return TypeIndex == other.TypeIndex;
            }

            int IComparable<UnitsType>.CompareTo(UnitsType other)
            {
                return TypeIndex.CompareTo(other.TypeIndex);
            }

            int IComparable.CompareTo(object obj)
            {
                if (obj is UnitsType)
                {
                    UnitsType other = (UnitsType)obj;
                    return TypeIndex.CompareTo(other.TypeIndex);
                }
                throw new ArgumentException("EUnits.CompareTo() with wrong object type.");
            }

            static public bool operator ==(UnitsType left, UnitsType right)
            {
                return left.TypeIndex == right.TypeIndex;
            }
            static public bool operator !=(UnitsType left, UnitsType right)
            {
                return left.TypeIndex != right.TypeIndex;
            }

        }

    }
}
