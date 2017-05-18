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
using Microsoft.Practices.Unity;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers {
    public class ExhibitsBaseDataFetcher : IExhibitsBaseDataFetcher {

        private readonly IExhibitsApiAccess exhibitsApiAccess;
        private readonly IPagesApiAccess pagesApiAccess;
        private readonly IMediaDataFetcher mediaDataFetcher;

        [Dependency]
        public ExhibitConverter ExhibitConverter { private get; set; }

        [Dependency]
        public PageConverter PageConverter { private get; set; }

        public ExhibitsBaseDataFetcher (IExhibitsApiAccess exhibitsApiAccess, IPagesApiAccess pagesApiAccess, IMediaDataFetcher mediaDataFetcher)
        {
            this.exhibitsApiAccess = exhibitsApiAccess;
            this.pagesApiAccess = pagesApiAccess;
            this.mediaDataFetcher = mediaDataFetcher;
        }

        private IList<ExhibitDto> fetchedChangedExhibits;
        private IList<int> requiredExhibitImages;
        private IList<ExhibitDto> newExhibits;
        private IDictionary<ExhibitDto, Exhibit> updatedExhibits;
        private IList<PageDto> appetizerPages;

        public async Task<int> FetchNeededDataForExhibits()
        {
            var exhibitSet = ExhibitManager.GetExhibitSets().SingleOrDefault();
            if (fetchedChangedExhibits == null)
            {
                await AnyExhibitChanged();
            }
            if (exhibitSet == null)
            {
                using (DbManager.StartTransaction())
                {
                    exhibitSet = DbManager.CreateBusinessObject<ExhibitSet>();
                }
            }

            requiredExhibitImages = new List<int>();
            newExhibits = new List<ExhibitDto>();
            updatedExhibits = new Dictionary<ExhibitDto, Exhibit>();
            var requiredAppetizerPages = new List<int>();

            foreach (var exhibit in fetchedChangedExhibits)
            {
                var dbExhibit = exhibitSet.SingleOrDefault(x => x.IdForRestApi == exhibit.Id);

                if (dbExhibit != null && exhibit.Timestamp != dbExhibit.UnixTimestamp)
                {
                    updatedExhibits.Add(exhibit, dbExhibit);
                }
                else if (dbExhibit == null)
                {
                    newExhibits.Add(exhibit);
                }

                if (dbExhibit == null || exhibit.Timestamp != dbExhibit.UnixTimestamp)
                {
                    if (exhibit.Pages.Any())
                    {
                        requiredAppetizerPages.Add(exhibit.Pages.First());
                    }
                    requiredExhibitImages.Add(exhibit.Image);
                }
            }

            appetizerPages = (await pagesApiAccess.GetPages(requiredAppetizerPages)).Items;
            foreach (var page in appetizerPages)
            {
                requiredExhibitImages.Add(page.Image);
            }

            return requiredExhibitImages.Count + fetchedChangedExhibits.Count;
        }

        public async Task ProcessExhibits(CancellationToken token, IProgressListener listener)
        {
            var fetchedMedia = await mediaDataFetcher.FetchMedias(requiredExhibitImages, token, listener);
            if (token.IsCancellationRequested)
            {
                return;
            }
            ProcessUpdatedExhibits(fetchedMedia, listener);
            ProcessNewExhibits(fetchedMedia, listener);
        }

        private void ProcessUpdatedExhibits(FetchedMediaData fetchedMediaData, IProgressListener listener)
        {
            foreach (var exhibitPair in updatedExhibits)
            {
                var exhibitDto = exhibitPair.Key;
                var dbExhibit = exhibitPair.Value;

                ExhibitConverter.Convert(exhibitDto, dbExhibit);

                AddImageToExhibit(dbExhibit, exhibitDto.Image, fetchedMediaData);
                AddAppetizerPageToExhibit(exhibitDto, dbExhibit, fetchedMediaData);
                //TODO: If exhibits content was already downloaded 
                //-> Show dialog whether to download new data or do it directly depending on setting

                listener.ProgressOneStep();
            }
        }

        private void ProcessNewExhibits(FetchedMediaData fetchedMediaData, IProgressListener listener)
        {
            foreach (var exhibitDto in newExhibits)
            {
                var dbExhibit = ExhibitConverter.Convert(exhibitDto);

                AddImageToExhibit(dbExhibit, exhibitDto.Image, fetchedMediaData);
                AddAppetizerPageToExhibit(exhibitDto, dbExhibit, fetchedMediaData);

                listener.ProgressOneStep();
            }
        }

        private void AddAppetizerPageToExhibit(ExhibitDto exhibitDto, Exhibit dbExhibit, FetchedMediaData fetchedMediaData)
        {
            if (exhibitDto.Pages.Any())
            {
                var page = appetizerPages.SingleOrDefault(x => x.Id == exhibitDto.Pages.First());

                if (page != null)
                {
                    if (dbExhibit.Pages.Any())
                    {
                        PageConverter.Convert(page, dbExhibit.Pages.First());
                    }
                    else
                    {
                        dbExhibit.Pages.Add(PageConverter.Convert(page));
                    }

                    var image = fetchedMediaData.Images.SingleOrDefault(x => x.IdForRestApi == page.Image);

                    if (image != null)
                    {
                        dbExhibit.Pages.First().AppetizerPage.Image = image;
                    }
                }
            }
        }

        private void AddImageToExhibit(Exhibit dbExhibit, int mediaId, FetchedMediaData fetchedMediaData)
        {
            var image = fetchedMediaData.Images.SingleOrDefault(x => x.IdForRestApi == mediaId);

            if (image != null)
            {
                dbExhibit.Image = image;
            }
        }

        public async Task<bool> AnyExhibitChanged()
        {
            var exhibitSet = ExhibitManager.GetExhibitSets().SingleOrDefault();
            ExhibitsDto exhibits;
            if (exhibitSet != null)
            {
                exhibits = await exhibitsApiAccess.GetExhibits(exhibitSet.Timestamp);
            }
            else
            {
                exhibits = await exhibitsApiAccess.GetExhibits();
            }
            fetchedChangedExhibits = exhibits.Items;

            return fetchedChangedExhibits.Any();
        }

    }
}