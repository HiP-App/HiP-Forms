/*
 * Copyright (C) 2016 History in Paderborn App - Universität Paderborn
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using Itinero;
using Itinero.LocalGeo;
using Itinero.Osm.Vehicles;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using Route = PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.Route;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Routing
{
    /// <summary>
    /// Class for routing operations
    /// 
    /// Implements Singleton pattern
    /// </summary>
    public sealed class RouteCalculator
    {

        private static Router routeRouter;

        private static RouteCalculator instance;
        private static readonly object Padlock = new object();

        /// <summary>
        ///     Initializes the database for the routing from a serialited pbf file
        /// </summary>
        private RouteCalculator()
        {
            RouterDb routingDb;

            var assembly = typeof(RouteCalculator).GetTypeInfo().Assembly;
            using (var stream = assembly.GetManifestResourceStream("PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Routing.osmfile.routerdb"))
            {
                routingDb = RouterDb.Deserialize(stream);
            }

            //Initialize Router after loading Deserializing is important, otherwise Profiles are not loaded properly
            routeRouter = new Router(routingDb);
        }


        /// <summary>
        /// Returns the Singleton
        /// </summary>
        public static RouteCalculator Instance
        {
            get
            {
                lock (Padlock)
                {
                    if (instance == null)
                        instance = new RouteCalculator();
                    return instance;
                }
            }
        }

        /// <summary>
        ///     Simple route from start to endpoint
        /// </summary>
        /// <param name="start">start position</param>
        /// <param name="end">end position</param>
        /// <returns>IList GeoLocation</returns>
        public IList<GeoLocation> CreateSimpleRoute(GeoLocation start, GeoLocation end)
        {
            IList<GeoLocation> result = new List<GeoLocation>();
            // calculate a route.
            var r = routeRouter.Calculate(Vehicle.Pedestrian.Fastest(),
                                           (float)start.Latitude, (float)start.Longitude, (float)end.Latitude, (float)end.Longitude);

            foreach (var c in r.Shape)
                result.Add(new GeoLocation(c.Latitude, c.Longitude));

            return result;
        }

        /// <summary>
        /// Calculates an ordered route. This means the set of geolocations which need to be visited ina specific order.
        /// </summary>
        /// <param name="routeId">The id of the route that shoudl be calculated.</param>
        /// <param name="userLocation">The users geolocation.</param>
        /// <returns></returns>
        public OrderedRoute CreateOrderedRoute(string routeId, GeoLocation userLocation = default(GeoLocation))
        {
            OrderedRoute resultRoute = new OrderedRoute();
            Route route = RouteManager.GetRoute(routeId);
            if (route != null && route.ActiveSet.Count > 0)
            {
                // include the user location if possible
                if (userLocation != default(GeoLocation))
                {
                    resultRoute.AddSection(CreateSimpleRoute(userLocation, route.ActiveSet.First().Location).ToList());
                }

                // calculate the route for exhibits
                for (int i = 0; i < route.ActiveSet.Count - 1; i++)
                {
                    resultRoute.AddSection((CreateSimpleRoute(route.ActiveSet[i].Location, route.ActiveSet[i + 1].Location).ToList()));
                }
            }
            return resultRoute;
        }

        /// <summary>
        ///     This method calculates one route from several waypoints
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userPosition">position of user</param>
        /// <returns>IList GeoLocation</returns>
        public IList<GeoLocation> CreateRouteWithSeveralWaypoints(string id, GeoLocation userPosition = default(GeoLocation))
        {
            //List of Geolocations for path
            IList<GeoLocation> result = new List<GeoLocation>();
            IList<Coordinate> locations = new List<Coordinate>();

            if (userPosition != default(GeoLocation))
            {
                locations.Add(new Coordinate((float)userPosition.Latitude, (float)userPosition.Longitude));
            }

            foreach (var v in RouteManager.GetRoute(id).ActiveSet)
                locations.Add(new Coordinate((float)v.Location.Latitude, (float)v.Location.Longitude));

            var route = routeRouter.TryCalculate(Vehicle.Pedestrian.Fastest(), locations.ToArray());

            foreach (var c in route.Value.Shape)
                result.Add(new GeoLocation(c.Latitude, c.Longitude));


            return result;
        }

    }
}