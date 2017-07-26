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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;
using Realms;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers
{
    public class DataToRemoveFetcher : IDataToRemoveFetcher
    {
        private readonly IExhibitsApiAccess exhibitsApiAccess;
        private readonly IRoutesApiAccess routesApiAccess;
        private readonly IPagesApiAccess pagesApiAccess;
        private readonly IMediasApiAccess mediasApiAccess;
        private readonly ITagsApiAccess tagsApiAccess;

        public DataToRemoveFetcher(IExhibitsApiAccess exhibitsApiAccess, IRoutesApiAccess routesApiAccess, IPagesApiAccess pagesApiAccess, IMediasApiAccess mediasApiAccess,
                                    ITagsApiAccess tagsApiAccess)
        {
            this.exhibitsApiAccess = exhibitsApiAccess;
            this.routesApiAccess = routesApiAccess;
            this.pagesApiAccess = pagesApiAccess;
            this.mediasApiAccess = mediasApiAccess;
            this.tagsApiAccess = tagsApiAccess;
        }

        private IList<int> allExhibits;
        private IList<int> allRoutes;
        private IList<int> allPages;
        private IList<int> allMedias;
        private IList<int> allTags;

        public async Task FetchDataToDelete (CancellationToken token)
        {
            allExhibits = await exhibitsApiAccess.GetIds();
            if (token.IsCancellationRequested)
            {
                return;
            }
            allRoutes = await routesApiAccess.GetIds();
            if (token.IsCancellationRequested)
            {
                return;
            }
            allPages = await pagesApiAccess.GetIds();
            if (token.IsCancellationRequested)
            {
                return;
            }
            allMedias = await mediasApiAccess.GetIds();
            if (token.IsCancellationRequested)
            {
            }
            allTags = await tagsApiAccess.GetIds();
        }

        public void CleaupRemovedData()
        {
            //Backup data fake id
            allMedias.Add (-1);

            var routes = RouteManager.GetRoutes().ToList();
            var exhibits = ExhibitManager.GetExhibits().ToList();

            var deletedExhibits = exhibits.Where(x => !allExhibits.Contains(x.IdForRestApi)).ToList();
            var deletedRoutes = routes.Where(x => !allRoutes.Contains(x.IdForRestApi));
            var deletedWaypoints = new List<Waypoint>();
            var deletedTags = new List<RouteTag>();
            var deletedImages = new List<Image>();
            var deletedAudios = new List<Audio>();
            var deletedPages = new List<Page>();

            foreach (var route in routes)
            {
                RemoveWaypoints(route, deletedWaypoints, deletedExhibits);
                RemoveRouteTags(route, deletedTags, deletedImages);
                if (route.Image != null && !allMedias.Contains(route.Image.IdForRestApi))
                {
                    deletedImages.Add(route.Image);
                    route.Image = null;
                }
                if (route.Audio != null && !allMedias.Contains(route.Audio.IdForRestApi))
                {
                    deletedAudios.Add(route.Audio);
                    route.Audio = null;
                }
            }
            foreach (var exhibit in exhibits)
            {
                RemovePages(exhibit, deletedPages, deletedImages, deletedAudios);
                if (exhibit.Image != null && !allMedias.Contains(exhibit.Image.IdForRestApi))
                {
                    deletedImages.Add(exhibit.Image);
                    exhibit.Image = null;
                }
            }

            foreach (var exhibit in deletedExhibits)
            {
                DbManager.DeleteBusinessEntity(exhibit);
            }
            foreach (var route in deletedRoutes)
            {
                DbManager.DeleteBusinessEntity(route);
            }
            foreach (var waypoint in deletedWaypoints)
            {
                DbManager.DeleteBusinessEntity(waypoint);
            }
            foreach (var tag in deletedTags)
            {
                DbManager.DeleteBusinessEntity(tag);
            }
            foreach (var image in deletedImages)
            {
                DbManager.DeleteBusinessEntity(image);
            }
            foreach (var audio in deletedAudios)
            {
                DbManager.DeleteBusinessEntity(audio);
            }
            foreach (var page in deletedPages)
            {
                DbManager.DeleteBusinessEntity(page);
            }
        }

        private void RemoveWaypoints(Route route, List<Waypoint> deletedWaypoints, List<Exhibit> deletedExhibits)
        {
            foreach (var waypoint in route.Waypoints)
            {
                if (deletedExhibits.Contains(waypoint.Exhibit))
                {
                    deletedWaypoints.Add(waypoint);
                }
            }
            foreach (var waypointToRemove in deletedWaypoints)
            {
                route.Waypoints.Remove(waypointToRemove);
            }
        }

        private void RemoveRouteTags(Route route, List<RouteTag> deletedTags, List<Image> deletedImages)
        {
            foreach (var tag in route.RouteTags)
            {
                if (!allTags.Contains(tag.IdForRestApi))
                {
                    deletedTags.Add(tag);
                }
                if (tag.Image != null && !allMedias.Contains(tag.Image.IdForRestApi))
                {
                    deletedImages.Add(tag.Image);
                    tag.Image = null;
                }
            }
            foreach (var tagToRemove in deletedTags)
            {
                route.RouteTags.Remove(tagToRemove);
            }
        }

        private void RemovePages(Exhibit exhibit, List<Page> deletedPages, List<Image> deletedImages, List<Audio> deletedAudios)
        {
            foreach (var page in exhibit.Pages)
            {
                if (!allPages.Contains(page.IdForRestApi))
                {
                    deletedPages.Add(page);
                }
                if (page.Audio != null && !allMedias.Contains(page.Audio.IdForRestApi))
                {
                    deletedAudios.Add(page.Audio);
                    page.Audio = null;
                }
                RemovePagesImages(page, deletedImages);
            }
            foreach (var pageToRemove in deletedPages)
            {
                exhibit.Pages.Remove(pageToRemove);
            }
        }

        private void RemovePagesImages(Page page, List<Image> deletedImages)
        {
            if (page.IsImagePage())
            {
                var imagePage = page.ImagePage;
                if (imagePage.Image != null && !allMedias.Contains(imagePage.Image.IdForRestApi))
                {
                    deletedImages.Add(imagePage.Image);
                    imagePage.Image = null;
                }
            }
            else if (page.IsAppetizerPage())
            {
                var appetizerPage = page.AppetizerPage;
                if (appetizerPage.Image != null && !allMedias.Contains(appetizerPage.Image.IdForRestApi))
                {
                    deletedImages.Add(appetizerPage.Image);
                    appetizerPage.Image = null;
                }
            }
            else if (page.IsTimeSliderPage())
            {
                var timesliderPage = page.TimeSliderPage;
                var imagesToRemove = new List<Image>();
                foreach (var image in timesliderPage.Images)
                {
                    if (allMedias.Contains(image.IdForRestApi))
                    {
                        imagesToRemove.Add(image);
                    }
                }
                foreach (var image in imagesToRemove)
                {
                    timesliderPage.Images.Remove(image);
                }
                deletedImages.AddRange(imagesToRemove);
            }
        }
    }
}