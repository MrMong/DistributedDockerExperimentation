using System;
using System.Collections.Generic;

namespace DistributedExperimentation.DataModel.Implementation 
{
    public class Experiment : IExperiment
    {
        private String id;
        private String name;
        private String description;
        private IParameterList parameters;
        private Experiment(String id, String name, String description, 
                           IParameterList parameters) 
        {
            bool isOK = ((parameters != null) && (isValidId(id)) && 
                        (name != null) && (description != null));
            if (isOK) {
                this.id = id;
                this.name = name;
                this.description = description;
                this.parameters = parameters;
            } else {
                throw new ArgumentException("Arguments 'id', 'name', 'description', " +
                                            " and 'parameters' must be not null.\n" +
                                            "Argument 'id' must have a length between " +
                                            "1 and 60 characters and must consist " +
                                            "the characters 0-9a-zA-Z only.\n" +
                                            "Argument 'parameters' must be a " +
                                            "valid implemented IParameterList.");
            }
        }

        public static Experiment create(String id, String name, String description,
                                        IParameterList parameters)
        {
            return new Experiment(id, name, description, parameters);
        }

        public string getDescription()
        {
            return this.description;
        }

        public string getId()
        {
            return this.id;
        }

        public string getName()
        {
            return this.name;
        }

        public IParameterCollection getParameters()
        {
            return (IParameterCollection)this.parameters;
        }

        private bool isValidId(String id) 
        {
            return ((id != null) && 
                    (id.Length > 0) && (id.Length < 61) &&
                    (System.Text.RegularExpressions.Regex.IsMatch(id, "^([A-Za-z0-9])+$")));
        }
    }
}