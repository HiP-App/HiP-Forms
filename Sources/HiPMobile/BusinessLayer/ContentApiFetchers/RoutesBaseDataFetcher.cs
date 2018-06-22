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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentHandling;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.JoinClasses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Attributes;

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
        private FetchedMediaData fetchedMedia;

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
                if (existingRoutesIdTimestampMapping.TryGetValue(routeDto.Id, out var dbRouteDate))
                {
                    if (Math.Abs((routeDto.Timestamp - dbRouteDate).Seconds) > 1)
                    {
                        updatedRoutes.Add(routeDto);
                        requiredRouteImages.Add(routeDto.Image);
                        if (routeDto.Tags != null)
                        {
                            requiredRouteTags.AddRange(routeDto.Tags);
                        }
                    }
                }
                else
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

        public Task<Dictionary<MediaDto, string>> WriteMediaToDiskAsync()
        {
            return mediaDataFetcher.WriteMediaToDiskAsync();
        }

        public void ProcessRoutes(IProgressListener listener, ITransactionDataAccess dataAccess, Dictionary<MediaDto, string> mediaToFilePath)
        {
            fetchedMedia = mediaDataFetcher.CombineMediasAndFiles(dataAccess, mediaToFilePath);

            ProcessUpdatedRoutes(listener, dataAccess);
            ProcessNewRoutes(listener, dataAccess);
        }

        private void ProcessUpdatedRoutes(IProgressListener listener, ITransactionDataAccess dataAccess)
        {
            var routes = dataAccess.Routes().GetRoutes().ToList();

            foreach (var routeDto in updatedRoutes)
            {
                var dbRoute = routes.First(x => x.IdForRestApi == routeDto.Id);

                RouteConverter.Convert(routeDto, dbRoute);

                AddImageToRoute(dbRoute, routeDto.Image, fetchedMedia);
                AddTagsToRoute(dbRoute, routeDto, fetchedMedia);
                AddExhibitsToRoute(dbRoute, routeDto, dataAccess);

                if (dbRoute.DetailsDataLoaded)
                {
                    IoCManager.Resolve<INewDataCenter>().AddRouteToBeUpdated(dbRoute);
                }

                listener.ProgressOneStep();
            }
        }

        private void ProcessNewRoutes(IProgressListener listener, ITransactionDataAccess dataAccess)
        {
            foreach (var routeDto in newRoutes)
            {
                var dbRoute = RouteConverter.Convert(routeDto);

                AddImageToRoute(dbRoute, routeDto.Image, fetchedMedia);
                AddTagsToRoute(dbRoute, routeDto, fetchedMedia);
                AddExhibitsToRoute(dbRoute, routeDto, dataAccess);

                dataAccess.Routes().AddRoute(dbRoute);
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
                        RouteTag dbTag = dbRoute.Tags.SingleOrDefault(x => x.IdForRestApi == tagId);

                        if (dbTag != null)
                        {
                            TagConverter.Convert(tagDto, dbTag);
                        }
                        else
                        {
                            dbTag = TagConverter.Convert(tagDto);
                            dbRoute.Tags.Add(dbTag);
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

        private void AddExhibitsToRoute(Route dbRoute, RouteDto routeDto, ITransactionDataAccess dataAccess)
        {
            var exhibits = dataAccess.Exhibits().GetExhibits().ToList();

            if (routeDto.Exhibits.Count > 0)
            {
                foreach (var exhibitId in routeDto.Exhibits)
                {
                    var dbExhibit = exhibits.SingleOrDefault(x => x.IdForRestApi == exhibitId);

                    if (dbExhibit != null)
                    {
                        var waypoint = new Waypoint
                        {
                            Exhibit = dbExhibit,
                            Location = dbExhibit.Location
                        };

                        dbRoute.Waypoints.Add(waypoint);
                    }
                }

                var removedWaypoints = dbRoute.Waypoints.Where(x => !routeDto.Exhibits.Contains(x.Exhibit.IdForRestApi));
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

        public async Task<bool> AnyRouteChanged(IReadOnlyDataAccess dataAccess)
        {
            RoutesDto changedRoutes;

            var dbRoutes = dataAccess.Routes().GetRoutes().ToList();
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