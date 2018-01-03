using System;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.FeatureToggling
{
    public interface IFeatureToggleRouter
    {
        IObservable<bool> IsFeatureEnabled(int featureId);
    }
}