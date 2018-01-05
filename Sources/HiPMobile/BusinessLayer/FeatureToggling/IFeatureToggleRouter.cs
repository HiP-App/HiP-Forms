using System;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.FeatureToggling
{
    public interface IFeatureToggleRouter
    {
        IObservable<bool> IsFeatureEnabled(int featureId);
        Task RefreshEnabledFeaturesAsync();
    }
}