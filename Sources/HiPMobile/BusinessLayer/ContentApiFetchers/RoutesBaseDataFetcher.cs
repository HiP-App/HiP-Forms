// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentHandling;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers
{
    public class RoutesBaseDataFetcher : IRoutesBaseDataFetcher
    {

        [Dependency]
        public TagConverter TagConverter { private get; set; }

        [Dependency]
        public RouteConverter RouteConverter { private get; set; }

        private readonly IRoutesApiAccess routesApiAccess;
        private readonly IMediaDataFetcher mediaDataFetcher;
        private readonly ITagsApiAccess tagsApiAccess;

        private IList<int?> requiredRouteImages;
        private IList<TagDto> routeTags;
        private IList<RouteDto> updatedRoutes;
        private IList<RouteDto> newRoutes;
        private IList<RouteDto> fetchedChangedRoutes;

        public RoutesBaseDataFetcher(IMediaDataFetcher mediaDataFetcher, IRoutesApiAccess routesApiAccess, ITagsApiAccess tagsApiAccess)
        {
            this.mediaDataFetcher = mediaDataFetcher;
            this.routesApiAccess = routesApiAccess;
            this.tagsApiAccess = tagsApiAccess;
        }

        public async Task<int> FetchNeededDataForRoutes(Dictionary<int, DateTimeOffset> existingRoutesIdTimestampMapping)
        {
            requiredRouteImages = new List<int?>();
            var requiredRouteTags = new List<int>();
            updatedRoutes = new List<RouteDto>();
            newRoutes = new List<RouteDto>();

            foreach (var routeDto in fetchedChangedRoutes)
            {
                DateTimeOffset? dbRouteData = null;
                if (existingRoutesIdTimestampMapping.ContainsKey(routeDto.Id))
                {
                    dbRouteData = existingRoutesIdTimestampMapping[routeDto.Id];
                }

                if (dbRouteData.HasValue && Math.Abs((routeDto.Timestamp - dbRouteData.Value).Seconds) > 1)
                {
                    updatedRoutes.Add(routeDto);
                    requiredRouteImages.Add(routeDto.Image);
                    if (routeDto.Tags != null)
                    {
                        requiredRouteTags.AddRange(routeDto.Tags);
                    }
                }
                else if (!dbRouteData.HasValue)
                {
                    newRoutes.Add(routeDto);
                    requiredRouteImages.Add(routeDto.Image);
                    if (routeDto.Tags != null)
                    {
                        requiredRouteTags.AddRange(routeDto.Tags);
                    }
                }
            }

            if (requiredRouteTags.Any())
            {
                routeTags = (await tagsApiAccess.GetTags(requiredRouteTags)).Items;
                foreach (var tag in routeTags)
                {
                    requiredRouteImages.Add(tag.Image);
                }
            }

            return requiredRouteImages.Count + fetchedChangedRoutes.Count;
        }

        public async Task FetchMediaData(CancellationToken token, IProgressListener listener)
        {
            await mediaDataFetcher.FetchMedias(requiredRouteImages, token, listener);
        }

        private FetchedMediaData fetchedMedia;
        public void ProcessRoutes(IProgressListener listener)
        {
            fetchedMedia = mediaDataFetcher.CombineMediasAndFiles();

            ProcessUpdatedRoutes(listener);
            ProcessNewRoutes(listener);
        }

        private void ProcessUpdatedRoutes(IProgressListener listener)
        {
            var routes = RouteManager.GetRoutes().ToList();

            foreach (var routeDto in updatedRoutes)
            {
                var dbRoute = routes.First(x => x.IdForRestApi == routeDto.Id);

                RouteConverter.Convert(routeDto, dbRoute);

                AddImageToRoute(dbRoute, routeDto.Image, fetchedMedia);
                AddTagsToRoute(dbRoute, routeDto, fetchedMedia);
                AddExhibitsToRoute(dbRoute, routeDto);

                if (dbRoute.DetailsDataLoaded)
                {
                    IoCManager.Resolve<INewDataCenter>().AddRouteToBeUpdated(dbRoute);
                }

                listener.ProgressOneStep();
            }
        }

        private void ProcessNewRoutes(IProgressListener listener)
        {
            foreach (var routeDto in newRoutes)
            {
                var dbRoute = RouteConverter.Convert(routeDto);

                AddImageToRoute(dbRoute, routeDto.Image, fetchedMedia);
                AddTagsToRoute(dbRoute, routeDto, fetchedMedia);
                AddExhibitsToRoute(dbRoute, routeDto);

                listener.ProgressOneStep();
            }
        }

        private void AddImageToRoute(Route dbRoute, int? mediaId, FetchedMediaData fetchedMediaData)
        {
            if (mediaId.HasValue)
            {
                var image = fetchedMediaData.Images.SingleOrDefault(x => x.IdForRestApi == mediaId);

                if (image != null)
                {
                    dbRoute.Image = image;
                }
            }
            else
            {
                dbRoute.Image = BackupData.BackupImage;
            }
        }

        private void AddTagsToRoute(Route dbRoute, RouteDto routeDto, FetchedMediaData fetchedMediaData)
        {
            if (routeDto.Tags != null && routeDto.Tags.Any())
            {
                foreach (var tagId in routeDto.Tags)
                {
                    var tagDto = routeTags.SingleOrDefault(x => x.Id == tagId);

                    if (tagDto != null)
                    {
                        RouteTag dbTag = dbRoute.RouteTags.SingleOrDefault(x => x.IdForRestApi == tagId);
                        if (dbTag != null)
                        {
                            TagConverter.Convert(tagDto, dbTag);
                        }
                        else
                        {
                            dbTag = TagConverter.Convert(tagDto);
                            dbRoute.RouteTags.Add(dbTag);
                        }

                        if (tagDto.Image.HasValue)
                        {
                            var tagImage = fetchedMediaData.Images.SingleOrDefault(x => x.IdForRestApi == tagDto.Image);
                            if (tagImage != null)
                            {
                                dbTag.Image = tagImage;
                            }
                        }
                        else
                        {
                            dbTag.Image = BackupData.BackupImageTag;
                        }
                    }
                }
            }
        }

        private void AddExhibitsToRoute(Route dbRoute, RouteDto routeDto)
        {
            var exhibits = ExhibitManager.GetExhibits().ToList();

            if (routeDto.Exhibits.Count > 0)
            {
                foreach (var exhibitId in routeDto.Exhibits)
                {
                    var dbExhibit = exhibits.SingleOrDefault (x => x.IdForRestApi == exhibitId);

                    if (dbExhibit != null)
                    {
                        var waypoint = DbManager.CreateBusinessObject<Waypoint> ();
                        waypoint.Exhibit = dbExhibit;
                        waypoint.Location = dbExhibit.Location;

                        dbRoute.Waypoints.Add (waypoint);
                    }
                }

                var removedWaypoints = dbRoute.Waypoints.Where(x => !routeDto.Exhibits.Contains (x.Exhibit.IdForRestApi));
                foreach (var waypoint in removedWaypoints)
                {
                    dbRoute.Waypoints.Remove(waypoint);
                }

                dbRoute.DetailsDataLoaded = false;
            }
            else
            {
                // Hide downloadbutton since there is nothing to download
                dbRoute.DetailsDataLoaded = true;
            }
        }

        public async Task<bool> AnyRouteChanged()
        {
            RoutesDto changedRoutes;

            var dbRoutes = RouteManager.GetRoutes().ToList();
            if (dbRoutes.Any())
            {
                var latestTimestamp = dbRoutes.Max(x => x.Timestamp);
                changedRoutes = await routesApiAccess.GetRoutes(latestTimestamp);
            }
            else
            {
                changedRoutes = await routesApiAccess.GetRoutes();
            }

            fetchedChangedRoutes = changedRoutes.Items;
            return fetchedChangedRoutes.Any();
        }

    }
}