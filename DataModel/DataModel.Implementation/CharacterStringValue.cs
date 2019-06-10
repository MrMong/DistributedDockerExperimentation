using System;
using System.Numerics;

namespace DistributedExperimentation.DataModel.Implementation 
{
    public class CharacterStringValue : ICharacterStringValue
    {
        private String value;

        private CharacterStringValue(String value) {
            bool isOK = (value != null);
            if (isOK) {
                this.value = value;
            } else {
                throw new ArgumentException("Argument 'value' must be\n" +
                                            "a not null String value");
            }
        }

        public static CharacterStringValue create(String value) {
            return new CharacterStringValue(value);
        }

        public string getCharacterStringValue()
        {
            return this.value;
        }

        public string getRawValue()
        {
            return this.value;
        }

        public string getValueTypeName()
        {
            return "characterstring";
        }

        public bool isBoolean()
        {
            return false;
        }

        public bool isCharacterString()
        {
            return true;
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