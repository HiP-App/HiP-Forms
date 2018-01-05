using System;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.FeatureToggling
{
    public interface IFeatureToggleRouter
    {
        IObservable<bool> IsFeatureEnabled(FeatureId featureId);
        Task RefreshEnabledFeaturesAsync();
    }
}