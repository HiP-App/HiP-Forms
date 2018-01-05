using System;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.FeatureToggling
{
    public interface IFeatureToggleRouter
    {
        /// <summary>
        /// Keep watching this observable and update the UI according to the
        /// updates to the feature state.
        /// </summary>
        /// <param name="featureId"></param>
        /// <returns></returns>
        IObservable<bool> IsFeatureEnabled(FeatureId featureId);
        
        /// <summary>
        /// Try and fetch an update for the feature toggles from the server.
        /// If the update fails, no change is made. If the update succeeds,
        /// observers are updated with new feature states as needed.
        /// </summary>
        /// <returns></returns>
        Task RefreshEnabledFeaturesAsync();
    }
}