using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DistributedExperimentation.DataModel.Implementation;

namespace DistributedExperimentation.DataModel.Parsing.Implementation
{
    public class ExperimentSeriesObjectBuilder : ExperimentSeriesBuilder
    {
        private ExperimentSeriesObjectBuilder() {
            this.experiments = new List<IExperiment>();
        }

        public static ExperimentSeriesObjectBuilder create() {
            return new ExperimentSeriesObjectBuilder();
        }

        override
        public object build()
        {
            bool isOk = ((this.id != null) && (this.name != null) &&
                        (this.description != null) && 
                        (this.softwarename != null));
            ExperimentSeries expSeries = null;
            if (isOk) {
                expSeries =  ExperimentSeries.create(this.id, this.name, this.description, 
                                                     this.experiments.Cast<IExperiment>().ToList(),
                                                     this.softwarename);
            } else {
                throw new ArgumentException("Properties 'id', 'name', 'description', " +
                                            "'experiments' and 'softwarename' of " +
                                            "ExperimentSeries must be setted completely.");
            }
            return expSeries;
        }
    }
}