using System;

namespace DistributedExperimentation.DataModel.Parsing
{
    public interface IExperimentSeriesParser
    {
        IExperimentSeriesBuilder getBuilder();
        object parse(object parseObject);
    }
}