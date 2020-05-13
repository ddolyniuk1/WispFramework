using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WispFramework.Attributes;

namespace WispFramework.Extensions.Enums
{
    public static class EnumExtensions
    {
        /// <summary>
        /// If the enum field has a ValueAttribute, retrieve that string value, otherwise return the
        /// enum field to string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetValue(this Enum value)
        {
            var attr = value.GetEnumAttribute<ValueAttribute>().FirstOrDefault();
            if (attr != null)
            {
                return attr.Value;
            }
            return value.ToString();
        }

        /// <summary>
        /// If the enum field has a DescriptionAttribute, retrieve that string value, otherwise return the
        /// enum field to string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            var attr = value.GetEnumAttribute<DescriptionAttribute>().FirstOrDefault();
            if (attr != null)
            {
                return attr.Description;
            }
            return value.ToString();
        }

        /// <summary>
        /// If the enum field has a ValueAttribute, retrieve that string value, otherwise return the
        /// enum field to string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DisplayNameDescription(this Enum value)
        {
            var attr = value.GetEnumAttribute<DisplayNameAttribute>().FirstOrDefault();
            if (attr != null)
            {
                return attr.DisplayName;
            }
            return value.ToString();
        }

        /// <summary>
        /// Get the attributes T for the enum value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetEnumAttribute<T>(this Enum value)
        { 
            var type = value.GetType();
            var fi = type.GetField(value.ToString());
            if (fi.GetCustomAttributes(typeof(T), false) is T[] attrs && attrs.Length > 0)
            {
                return attrs;
            }

            return default;
        }
    }
}
