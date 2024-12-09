using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using Game.Data;
using System;

namespace Game.Model
{
    public static class PropertyContextUtils
    {
        public static FieldInfo[] GetContextAttributes(object target)
        {
            return target.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.GetCustomAttribute<ComponentContextAttribute>() != null)
                .ToArray();
        }
        public static RangeAttribute FieldContainsRangeAttribute(FieldInfo field)
        {
            RangeAttribute rangeAttribute = field.GetCustomAttribute<RangeAttribute>();
            return rangeAttribute;
        }

        public static bool IsNumeric(Type type)
        {
            return
            type == typeof(byte) ||
            type == typeof(short) ||
            type == typeof(ushort) ||
            type == typeof(int) ||
            type == typeof(long) ||
            type == typeof(ulong) ||
            type == typeof(float) ||
            type == typeof(double) ||
            type == typeof(decimal);
        }

        public static object ConvertToCorrectType(object inValue, Type fieldType)
        {
            var outValue = Convert.ChangeType(inValue, fieldType);
            return outValue;
        }
    }
}
