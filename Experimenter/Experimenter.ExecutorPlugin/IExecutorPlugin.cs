using DistributedExperimentation.DataModel;

namespace DistributedExperimentation.Experimenter.ExecutorPlugin
{
    // this interface must be implemented from an plugin
    public interface IExecutorPlugin
    {
        void execute(IExperimentSeries experimentSeries);
    }
}