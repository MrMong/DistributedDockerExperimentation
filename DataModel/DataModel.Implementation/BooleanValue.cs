using System;
using System.Numerics;

namespace DistributedExperimentation.DataModel.Implementation 
{
    public class BooleanValue : IBooleanValue
    {
        private Boolean value;

        private BooleanValue(Boolean value) {
            this.value = value;
        }

        public static BooleanValue create(Boolean value) {
            return new BooleanValue(value);
        }

        public ValueType getBooleanValue()
        {
            return this.value;
        }

        public string getRawValue()
        {
            return Convert.ToString(this.value).ToLower();
        }

        public string getValueTypeName()
        {
            return "boolean";
        }

        public bool isBoolean()
        {
            return true;
        }

        public bool isCharacterString()
        {
            return false;
        }

        public bool isInteger()
        {
            return false;
        }

        public bool isPrimitive()
        {
            return true;
        }

        public bool isReal()
        {
            return false;
        }
    }
}