using System;
using System.Collections.Generic;

namespace DistributedExperimentation.DataModel.Parsing.Implementation
{
    public abstract class ExperimentSeriesBuilder : IExperimentSeriesBuilder
    {
        protected String id;
        protected String name;
        protected String description;
        protected String softwarename;
        protected List<IExperiment> experiments;

        public void add(IExperiment experiment) {
            if (experiment != null) {
                this.experiments.Add((IExperiment)experiment);
            } else {
                throw new ArgumentException("Argument 'experiments' " +
                                            "must be not null.");
            }
        }

        public void addRange(IList<IExperiment> experiments)
        {
            if (experiments != null) {
                IList<IExperiment> tempList = new List<IExperiment>();
                foreach(IExperiment experiment in experiments) {
                    if (experiment != null) {
                        tempList.Add((IExperiment)experiment);
                    } else {
                        throw new ArgumentException("Argument 'experiment' " +
                                                    "must be not null.");
                    }
                }
                this.experiments.AddRange(tempList);
            } else {
                throw new ArgumentException("Argument 'experiments' " +
                                            "must be not null.");
            }
        }

        public abstract object build();

        public void setDescription(string description)
        {
            if (description != null) {
                this.description = description;
            } else {
                throw new ArgumentException("Argument 'description' " +
                                            "must be not null.");                
            }
        }

        public void setId(string id)
        {
            if (id != null) {
                this.id = id;
            } else {
                throw new ArgumentException("Argument 'id' " +
                                            "must be not null.");                
            }
        }

        public void setName(string name)
        {
            if (name != null) {
                this.name = name;
            } else {
                throw new ArgumentException("Argument 'name' " +
                                            "must be not null.");                
            }
        }

        public void setSoftwareName(string softwarename)
        {
            if (softwarename != null) {
                this.softwarename = softwarename;
            } else {
                throw new ArgumentException("Argument 'softwarename' " +
                                            "must be not null.");                
            }
        }

        public virtual void reset() {
            this.id = null;
            this.name = null;
            this.description = null;
            this.softwarename = null;
            experiments.Clear();
        }
    }
}