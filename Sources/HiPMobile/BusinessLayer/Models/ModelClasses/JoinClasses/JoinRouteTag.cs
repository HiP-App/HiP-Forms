using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.JoinClasses
{
    /// <summary>
    /// Represents the many-to-many relationship between
    /// <see cref="Models.Route"/> and <see cref="RouteTag"/>.
    /// </summary>
    public class JoinRouteTag : IJoinEntity<Route>, IJoinEntity<RouteTag>
    {
        public Route Route { get; set; }
        public string RouteId { get; set; }
        Route IJoinEntity<Route>.Navigation { get => Route; set => Route = value; }

        public RouteTag Tag { get; set; }
        public string TagId { get; set; }
        RouteTag IJoinEntity<RouteTag>.Navigation { get => Tag; set => Tag = value; }
    }
}
