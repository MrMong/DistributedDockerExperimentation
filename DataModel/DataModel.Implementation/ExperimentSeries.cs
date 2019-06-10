using System;
using System.Collections.Generic;

namespace DistributedExperimentation.DataModel.Implementation 
{
    public class ExperimentSeries : IExperimentSeries
    {
        String id;
        String name;
        String description;
        String softwarename;
        private List<IExperiment> experiments;

        private ExperimentSeries(String id, String name, String description, 
                                 List<IExperiment> experiments, String softwarename) 
        {
            bool isOK = ((experiments != null) && (experiments.Count > 0) &&
                        (isValidId(id)) && (name != null) && (description != null) &&
                        (softwarename != null));
            if (isOK) {
                this.id = id;
                this.name = name;
                this.description = description;
                this.experiments = experiments;
                this.softwarename = softwarename;
            } else {
                throw new ArgumentException("Arguments 'id', 'name', 'description', " +
                                            "'experiments' and 'softwarename' must be not null.\n" +
                                            "Argument 'id' must have a length between " +
                                            "1 and 60 characters and must consist " +
                                            "the characters 0-9a-zA-Z only.\n" +                                            
                                            "Argument 'experiments' must be a valid non-empty " + 
                                            "list of IExperiments.");
            }
        }

        public static ExperimentSeries create(String id, String name, String description, 
                                              List<IExperiment> experiments, String softwarename)
        {
            return new ExperimentSeries(id, name, description, experiments, softwarename);
        }

        public string getDescription()
        {
            return this.description;
        }

        public IList<IExperiment> getExperiments()
        {
            return this.experiments;
        }

        public string getExperimentSoftware()
        {
            return this.softwarename;
        }

        public string getId()
        {
            return this.id;
        }

        public string getName()
        {
            return this.name;
        }

        private bool isValidId(String id) 
        {
            return ((id != null) && 
                    (id.Length > 0) && (id.Length < 61) &&
                    (System.Text.RegularExpressions.Regex.IsMatch(id, "^([A-Za-z0-9])+$")));
        }        
    }
}