using System;
using System.Collections.Generic;
using DistributedExperimentation.DataModel;

namespace DistributedExperimentation.Experimenter.ExecutorPlugin
{
    public interface IExecutorPlugin
    {
        void execute(IExperimentSeries experimentSeries);
    }
}