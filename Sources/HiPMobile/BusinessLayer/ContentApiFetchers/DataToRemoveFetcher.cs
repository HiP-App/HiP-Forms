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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.JoinClasses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers
{
    public class DataToRemoveFetcher : IDataToRemoveFetcher
    {
        private readonly IExhibitsApiAccess exhibitsApiAccess;
        private readonly IRoutesApiAccess routesApiAccess;
        private readonly IPagesApiAccess pagesApiAccess;
        private readonly IMediasApiAccess mediasApiAccess;
        private readonly ITagsApiAccess tagsApiAccess;
        private const int DefaultImageId = -2;

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

        public async Task FetchDataToDelete(CancellationToken token)
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
                return;
            }
            allTags = await tagsApiAccess.GetIds();
        }

        public void CleanupRemovedData(ITransactionDataAccess dataAccess)
        {
            //Backup data fake id
            allMedias.Add(-1);

            var routes = dataAccess.Routes().GetRoutes().ToList();
            var exhibits = dataAccess.Exhibits().GetExhibits().ToList();

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
                dataAccess.DeleteItem(exhibit);
            }
            foreach (var route in deletedRoutes)
            {
                dataAccess.DeleteItem(route);
            }
            foreach (var waypoint in deletedWaypoints)
            {
                dataAccess.DeleteItem(waypoint);
            }
            foreach (var tag in deletedTags)
            {
                dataAccess.DeleteItem(tag);
            }
            var fileManager = IoCManager.Resolve<IMediaFileManager>();
            foreach (var image in deletedImages)
            {
                dataAccess.DeleteItem(image);
                fileManager.DeleteFile(image.DataPath);
            }
            foreach (var audio in deletedAudios)
            {
                dataAccess.DeleteItem(audio);
                fileManager.DeleteFile(audio.DataPath);
            }
            foreach (var page in deletedPages)
            {
                dataAccess.DeleteItem(page);
            }

        }

        public async Task PruneMediaFilesAsync(IReadOnlyDataAccess dataAccess)
        {
            var fileManager = IoCManager.Resolve<IMediaFileManager>();

            var restApiIdsToKeep = dataAccess.GetItems<Audio>()
                .Select(it => it.IdForRestApi)
                .Union(dataAccess.GetItems<Image>().Select(it => it.IdForRestApi));

            await fileManager.PruneAsync(restApiIdsToKeep.ToList());
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
            foreach (var tag in route.Tags)
            {
                if (!allTags.Contains(tag.IdForRestApi))
                {
                    deletedTags.Add(tag);
                }
                if (tag.Image != null && !allMedias.Contains(tag.Image.IdForRestApi) && tag.Image.IdForRestApi != DefaultImageId)
                {
                    deletedImages.Add(tag.Image);
                    tag.Image = null;
                }
            }
            foreach (var tagToRemove in deletedTags)
            {
                route.Tags.Remove(tagToRemove);
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
            switch (page)
            {
                case ImagePage imagePage:
                    if (imagePage.Image != null && !allMedias.Contains(imagePage.Image.IdForRestApi))
                    {
                        deletedImages.Add(imagePage.Image);
                        imagePage.Image = null;
                    }
                    break;
                    
                case TimeSliderPage timeSliderPage:
                    foreach (var entry in timeSliderPage.SliderImages.ToList())
                    {
                        if (allMedias.Contains(entry.Image.IdForRestApi))
                        {
                            deletedImages.Add(entry.Image);
                            timeSliderPage.SliderImages.Remove(entry);
                        }
                    }
                    break;
            }
        }
    }
}