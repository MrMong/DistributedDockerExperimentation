using System;
using System.Collections.Generic;

namespace DistributedExperimentation.DataModel
{
    public interface IExperimentSeries
    {
        String getId();
        String getName();
        String getDescription();
        String getExperimentSoftware();
        IList<IExperiment> getExperiments();
    }
}