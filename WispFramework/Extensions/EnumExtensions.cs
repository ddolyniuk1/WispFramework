using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using WispFramework.Attributes;

namespace WispFramework.Extensions
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
            var output = value.ToString();
            var type = value.GetType();
            var fi = type.GetField(value.ToString());
            if (fi.GetCustomAttributes(typeof(ValueAttribute), false) is ValueAttribute[] attrs && attrs.Length > 0)
            {
                output = attrs[0].Value;
            }
            return output;
        }
    }
}
