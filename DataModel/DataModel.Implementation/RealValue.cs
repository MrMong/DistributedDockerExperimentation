using System;
using System.Numerics;

namespace DistributedExperimentation.DataModel.Implementation 
{
    public class RealValue : IRealValue
    {
        private ValueType value;

        private RealValue(ValueType value) {
            bool isOK = ((value != null) && 
                        (checkAsReal(value)));
            if (isOK) {
                this.value = value;
            } else {
                throw new ArgumentException("Argument 'value' must be a not null " +
                                            "Real value with a allowed type of\n" +
                                            "Float, Double, Single.");
            }
        }

        public static RealValue create(ValueType value) {
            return new RealValue(value);
        }

        public string getRawValue()
        {
            return Convert.ToString(this.value);
        }

        public ValueType getRealValue()
        {
            return this.value;
        }

        public string getValueTypeName()
        {
            return "real";
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
            return false;
        }

        public bool isPrimitive()
        {
            return true;
        }

        public bool isReal()
        {
            return true;
        }

        private bool checkAsReal(ValueType checkValue) {
            return (checkValue is float 
                    || checkValue is double 
                    || checkValue is Single);
        }
    }
}