using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.JoinClasses
{
    public class JoinRouteTag : IJoinEntity<Route>, IJoinEntity<RouteTag>
    {
        public Route Route { get; set; }

        public RouteTag Tag { get; set; }

        Route IJoinEntity<Route>.Navigation { get => Route; set => Route = value; }

        RouteTag IJoinEntity<RouteTag>.Navigation { get => Tag; set => Tag = value; }
    }
}
