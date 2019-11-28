using System;
using System.Collections.Generic;
using System.Text;

namespace WispFramework.Attributes
{
    public class ValueAttribute : Attribute
    {
        public string Value { get; set; }

        public ValueAttribute(string value)
        {
            Value = value;
        }
    }
}
