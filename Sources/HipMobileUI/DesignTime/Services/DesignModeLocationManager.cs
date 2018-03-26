using PaderbornUniversity.SILab.Hip.Mobile.UI.Location;
using Plugin.Geolocator.Abstractions;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.DesignTime.Services
{
    class DesignModeLocationManager : ILocationManager
    {
        public Position LastKnownLocation => new Position();

        public bool ListeningInBackground => false;

        public void AddLocationListener(ILocationListener listener) { }

        public bool IsLocationAvailable() => false;

        public void RemoveLocationListener(ILocationListener listener) { }
    }
}
