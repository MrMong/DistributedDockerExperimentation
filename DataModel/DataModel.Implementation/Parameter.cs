using System;
using System.Collections.Generic;

namespace DistributedExperimentation.DataModel.Implementation 
{
    public class Parameter : IParameter
    {
        private String id;
        private String name;
        private String description;
        private IParameterValue value;
        private Parameter(String id, String name, String description, 
                          IParameterValue value) 
        {
            bool isOK = ((name != null) && (value != null) &&
                        (isValidId(id)) && (description != null));
            if (isOK) {
                this.id = id;
                this.name = name;
                this.description = description;
                this.value = value;
            } else {
                throw new ArgumentException("Arguments 'id', 'name', 'description' and " + 
                                            "'value' must be not null.\n" +
                                            "Argument 'id' must have a length between " +
                                            "1 and 60 characters and must consist " +
                                            "the characters 0-9a-zA-Z only.\n" +
                                            "Argument 'value' must be a valid IParameterValue.");
            }
        }

        private Parameter(String id, String description, IParameterValue value) 
        {
            bool isOK = ((value != null) && (isValidId(id)) &&
                        (description != null));
            if (isOK) {
                this.id = id;
                this.name = "";
                this.description = description;
                this.value = value;
            } else {
                throw new ArgumentException("Arguments 'id', 'description' and " + 
                                            "'value' must be not null.\n" +
                                            "Argument 'id' must have a length between " +
                                            "1 and 60 characters and must consist " +
                                            "the characters 0-9a-zA-Z only.\n" +                                            
                                            "Argument 'value' must be a valid IParameterValue.");
            }
        }

        public static Parameter create(String id, String name, String description, 
                                       IParameterValue value) 
        {
            return new Parameter(id, name, description, value);
        }

        public static Parameter create(String id, String description, IParameterValue value) 
        {
            return new Parameter(id, description, value);
        }

        public string getName()
        {
            return this.name;
        }

        public IParameterValue getValue()
        {
            return this.value;
        }

        public string getId()
        {
            return this.id;
        }

        public string getDescription()
        {
            return this.description;
        }

        public bool isPrimitive()
        {
            return this.value.isPrimitive();
        }

        private bool isValidId(String id) 
        {
            return ((id != null) && 
                    (id.Length > 0) && (id.Length < 61) &&
                    (System.Text.RegularExpressions.Regex.IsMatch(id, "^([A-Za-z0-9])+$")));
        }
    }
}