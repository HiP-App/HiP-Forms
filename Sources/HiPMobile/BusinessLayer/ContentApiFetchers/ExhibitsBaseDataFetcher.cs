﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
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

        public Task<int> FetchNeededDataForExhibits(Dictionary<int, DateTimeOffset> existingExhibitsIdTimestampMapping)
        {
            requiredExhibitImages = new List<int?>();
            newExhibits = new List<ExhibitDto>();
            updatedExhibits = new List<ExhibitDto>();

            foreach (var exhibit in fetchedChangedExhibits)
            {
                if (existingExhibitsIdTimestampMapping.TryGetValue(exhibit.Id, out var dbExhibitDate))
                {
                    if (Math.Abs((exhibit.Timestamp - dbExhibitDate).Seconds) > 1)
                    {
                        updatedExhibits.Add(exhibit);
                        requiredExhibitImages.Add(exhibit.Image);
                    }
                }
                else
                {
                    newExhibits.Add(exhibit);
                    requiredExhibitImages.Add(exhibit.Image);
                }
            }
            return Task.FromResult(requiredExhibitImages.Count + fetchedChangedExhibits.Count);
        }

        public async Task FetchMediaData(CancellationToken token, IProgressListener listener)
        {
            await mediaDataFetcher.FetchMedias(requiredExhibitImages, token, listener);
        }

        public Task<Dictionary<MediaDto, string>> WriteMediaToDiskAsync()
        {
            return mediaDataFetcher.WriteMediaToDiskAsync();
        }

        public void ProcessExhibits(IProgressListener listener, ITransactionDataAccess dataAccess, Dictionary<MediaDto, string> mediaToFilePath)
        {
            var fetchedMedia = mediaDataFetcher.CombineMediasAndFiles(dataAccess, mediaToFilePath);

            ProcessUpdatedExhibits(listener, fetchedMedia, dataAccess);
            ProcessNewExhibits(listener, fetchedMedia, dataAccess);
        }

        private void ProcessUpdatedExhibits(IProgressListener listener, FetchedMediaData fetchedMedia, ITransactionDataAccess dataAccess)
        {
            var exhibits = dataAccess.Exhibits().GetExhibits().ToDictionary(x => x.IdForRestApi);

            foreach (var exhibitDto in updatedExhibits)
            {
                var dbExhibit = exhibits[exhibitDto.Id];

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

        private void ProcessNewExhibits(IProgressListener listener, FetchedMediaData fetchedMedia, ITransactionDataAccess dataAccess)
        {
            foreach (var exhibitDto in newExhibits)
            {
                var dbExhibit = ExhibitConverter.Convert(exhibitDto);

                AddImageToExhibit(dbExhibit, exhibitDto.Image, fetchedMedia);
                dataAccess.Exhibits().AddExhibit(dbExhibit);
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

        public async Task<bool> AnyExhibitChanged(IReadOnlyDataAccess dataAccess)
        {
            ExhibitsDto changedExhibits;

            var dbExhibits = dataAccess.Exhibits().GetExhibits().ToList();
            if (dbExhibits.Any())
            {
                var latestTimestamp = dbExhibits.Max(x => x.Timestamp);
                changedExhibits = await exhibitsApiAccess.GetExhibits(latestTimestamp);
            }
            else
            {
                changedExhibits = await exhibitsApiAccess.GetExhibits();
            }

            fetchedChangedExhibits = changedExhibits.Items;
            return fetchedChangedExhibits.Any();
        }
    }
}