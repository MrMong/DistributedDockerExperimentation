using System;
using System.Numerics;

namespace DistributedExperimentation.DataModel.Implementation 
{
    public class IntegerValue : IIntegerValue
    {
        private ValueType value;

        private IntegerValue(ValueType value) {
            bool isOK = ((value != null) && 
                        (checkAsInteger(value)));
            if (isOK) {
                this.value = value;
            } else {
                throw new ArgumentException("Argument 'value' must be a not null " +
                                            "Integer value with a allowed type of\n" +
                                            "SByte, Int16, Int32, Int64, Byte, " +
                                            "UInt16, UInt32, UInt64, BigInteger.");
            }
        }

        public static IntegerValue create(ValueType value) {
            return new IntegerValue(value);
        }

        public ValueType getIntegerValue()
        {
            return this.value;
        }

        public string getRawValue()
        {
            return Convert.ToString(this.value);
        }

        public string getValueTypeName()
        {
            return "integer";
        }

        public bool isBoolean()
        {
            return false;
        }

        public bool isCharacterString()
        {
            return false;
        }

        public bool isInteger()
        {
            return true;
        }

        public bool isPrimitive()
        {
            return true;
        }

        public bool isReal()
        {
            return false;
        }

        private bool checkAsInteger(ValueType checkValue) {
            return (checkValue is SByte || checkValue is Int16
                    || checkValue is Int32 || checkValue is Int64 
                    || checkValue is Byte || checkValue is UInt16  
                    || checkValue is UInt32 || checkValue is UInt64 
                    || checkValue is BigInteger);
        }
    }
}