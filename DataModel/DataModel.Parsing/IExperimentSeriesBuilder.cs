using System;
using System.Collections.Generic;

namespace DistributedExperimentation.DataModel.Parsing
{
    public interface IExperimentSeriesBuilder
    {
        void setId(String id);
        void setName(String name);
        void setDescription(String description);
        void setSoftwareName(String softwarename);
        void add(IExperiment experiment);
        void addRange(IList<IExperiment> experiments);
        void reset();
        object build();
    }
}