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
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers
{
    public class ExhibitsBaseDataFetcher : IExhibitsBaseDataFetcher
    {
        private readonly IExhibitsApiAccess exhibitsApiAccess;
        private readonly IPagesApiAccess pagesApiAccess;
        private readonly IMediaDataFetcher mediaDataFetcher;

        [Dependency]
        public ExhibitConverter ExhibitConverter { private get; set; }

        [Dependency]
        public PageConverter PageConverter { private get; set; }

        public ExhibitsBaseDataFetcher(IExhibitsApiAccess exhibitsApiAccess, IPagesApiAccess pagesApiAccess, IMediaDataFetcher mediaDataFetcher)
        {
            this.exhibitsApiAccess = exhibitsApiAccess;
            this.pagesApiAccess = pagesApiAccess;
            this.mediaDataFetcher = mediaDataFetcher;
        }

        private IList<ExhibitDto> fetchedChangedExhibits;
        private IList<int?> requiredExhibitImages;
        private IList<ExhibitDto> newExhibits;
        private IList<ExhibitDto> updatedExhibits;

        public async Task<int> FetchNeededDataForExhibits(Dictionary<int, DateTimeOffset> existingExhibitsIdTimestampMapping)
        {
            requiredExhibitImages = new List<int?>();
            newExhibits = new List<ExhibitDto>();
            updatedExhibits = new List<ExhibitDto>();
            var requiredAppetizerPages = new List<int>();

            foreach (var exhibit in fetchedChangedExhibits)
            {
                DateTimeOffset? dbExhibitData = null;
                if (existingExhibitsIdTimestampMapping.ContainsKey(exhibit.Id))
                {
                    dbExhibitData = existingExhibitsIdTimestampMapping[exhibit.Id];
                }

                if (dbExhibitData.HasValue && Math.Abs((exhibit.Timestamp - dbExhibitData.Value).Seconds) > 1)
                {
                    updatedExhibits.Add(exhibit);
                }
                else if (!dbExhibitData.HasValue)
                {
                    newExhibits.Add(exhibit);
                }

                if (!dbExhibitData.HasValue || Math.Abs((exhibit.Timestamp - dbExhibitData.Value).Seconds) > 1)
                {
                    // Keeping it as safety check don't want appetizer page ids to be treated as normal pages
                    // Currently in the exhibit response body first page number belongs to appetizer page.
                    // Once DataStore remove appetizer page from their side then we can remove this from our side as well.
                    if (exhibit.Pages != null && exhibit.Pages.Any())
                    {
                        requiredAppetizerPages.Add(exhibit.Pages.First());
                    }
                    requiredExhibitImages.Add(exhibit.Image);
                }
            }
            return requiredExhibitImages.Count + fetchedChangedExhibits.Count;
        }

        public async Task FetchMediaData(CancellationToken token, IProgressListener listener)
        {
            await mediaDataFetcher.FetchMedias(requiredExhibitImages, token, listener);
        }

        private FetchedMediaData fetchedMedia;

        public async Task ProcessExhibits(IProgressListener listener)
        {
            fetchedMedia = await mediaDataFetcher.CombineMediasAndFiles();

            ProcessUpdatedExhibits(listener);
            ProcessNewExhibits(listener);

            if (fetchedChangedExhibits.Any())
            {
                var exhibitSet = ExhibitManager.GetExhibitSets().SingleOrDefault();
                exhibitSet.Timestamp = fetchedChangedExhibits.Max(x => x.Timestamp);
            }
        }

        private void ProcessUpdatedExhibits(IProgressListener listener)
        {
            var exhibits = ExhibitManager.GetExhibits().ToList();

            foreach (var exhibitDto in updatedExhibits)
            {
                var dbExhibit = exhibits.First(x => x.IdForRestApi == exhibitDto.Id);

                ExhibitConverter.Convert(exhibitDto, dbExhibit);

                AddImageToExhibit(dbExhibit, exhibitDto.Image, fetchedMedia);
                //TODO: Check if exhibit was already downloaded
                //if(dbExhibit.DetailsDataLoaded)
                //{
                //    IoCManager.Resolve<INewDataCenter>().AddExhibitToBeUpdated(dbExhibit);
                //}

                var removedPages = dbExhibit.Pages.Where(x => !exhibitDto.Pages.Contains(x.IdForRestApi));
                foreach (var page in removedPages)
                {
                    dbExhibit.Pages.Remove(page);
                }

                listener.ProgressOneStep();
            }
        }

        private void ProcessNewExhibits(IProgressListener listener)
        {
            var exhibitSet = ExhibitManager.GetExhibitSets().SingleOrDefault();

            foreach (var exhibitDto in newExhibits)
            {
                var dbExhibit = ExhibitConverter.Convert(exhibitDto);

                AddImageToExhibit(dbExhibit, exhibitDto.Image, fetchedMedia);
                exhibitSet.ActiveSet.Add(dbExhibit);
                listener.ProgressOneStep();
            }
        }
        private void AddImageToExhibit(Exhibit dbExhibit, int? mediaId, FetchedMediaData fetchedMediaData)
        {
            if (mediaId.HasValue)
            {
                var image = fetchedMediaData.Images.SingleOrDefault(x => x.IdForRestApi == mediaId);

                if (image != null)
                {
                    dbExhibit.Image = image;
                }
                else
                {
                    dbExhibit.Image = BackupData.BackupImage;
                }
            }
            else
            {
                dbExhibit.Image = BackupData.BackupImage;
            }
        }

        public async Task<bool> AnyExhibitChanged()
        {
            ExhibitsDto exhibits;
            var exhibitSet = ExhibitManager.GetExhibitSets().SingleOrDefault();
            if (exhibitSet != null)
            {
                try
                {
                    exhibits = await exhibitsApiAccess.GetExhibits(exhibitSet.Timestamp);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    throw;
                }
            }
            else
            {
                exhibits = await exhibitsApiAccess.GetExhibits();

                using (DbManager.StartTransaction())
                {
                    DbManager.CreateBusinessObject<ExhibitSet>();
                }
            }

            fetchedChangedExhibits = exhibits.Items;

            return fetchedChangedExhibits.Any();
        }
    }
}