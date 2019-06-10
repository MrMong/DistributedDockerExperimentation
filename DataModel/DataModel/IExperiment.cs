using System;
using System.Collections.Generic;

namespace DistributedExperimentation.DataModel
{
    public interface IExperiment
    {
        String getId();
        String getName();
        String getDescription();
        IParameterCollection getParameters();
    }
}