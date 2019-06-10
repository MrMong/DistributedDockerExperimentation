using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DistributedExperimentation.DataModel.Implementation;

namespace DistributedExperimentation.DataModel.Parsing.Implementation
{
    public class ExperimentSeriesObjectParser : IExperimentSeriesParser
    {
        IExperimentSeriesBuilder builder;
        private ExperimentSeriesObjectParser(IExperimentSeriesBuilder builder) {
            bool isOk = (builder != null);
            if (isOk) {
                this.builder = builder;
            } else {
                throw new ArgumentException("Argument 'builder' must be a not null.");
            }
        }

        public static ExperimentSeriesObjectParser create(IExperimentSeriesBuilder builder) {
            return new ExperimentSeriesObjectParser(builder);
        }

        public IExperimentSeriesBuilder getBuilder()
        {
            return this.builder;
        }

        public object parse(object parseObject)
        {
            bool isOk = ((parseObject != null) &&
                        (parseObject is ExperimentSeries));              
            object result = null;
            if (isOk) {
                result = parse((ExperimentSeries)parseObject);
            } else {
                throw new ArgumentException("Argument 'parseObject' must be a " +
                                            "not null ExperimentSeries object.");           
            }
            return result;
        }

        private object parse(ExperimentSeries parseObject) 
        {
            this.builder.setId(parseObject.getId());
            this.builder.setName(parseObject.getName());
            this.builder.setDescription(parseObject.getDescription());
            this.builder.addRange(parseObject.getExperiments());
            this.builder.setSoftwareName(parseObject.getExperimentSoftware());
            object result = this.builder.build();
            this.builder.reset();
            return result;
        }
    }
}