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
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers
{
    public class BaseDataFetcher : IBaseDataFetcher
    {

        private readonly IExhibitsApiAccess exhibitsApiAccess;

        private readonly IPagesApiAccess pagesApiAccess;

        private readonly IMediaDataFetcher mediaDataFetcher;

        [Dependency]
        public ExhibitConverter ExhibitConverter { private get; set; }

        [Dependency]
        public PageConverter PageConverter { private get; set; }

        private IList<ExhibitDto> fetchedChangedExhibits;

        public BaseDataFetcher(IExhibitsApiAccess exhibitsApiAccess, IPagesApiAccess pagesApiAccess, IMediaDataFetcher mediaDataFetcher)
        {
            this.exhibitsApiAccess = exhibitsApiAccess;
            this.pagesApiAccess = pagesApiAccess;
            this.mediaDataFetcher = mediaDataFetcher;
        }

        public async Task<bool> IsDatabaseUpToDate()
        {
            var exhibitSet = ExhibitManager.GetExhibitSets().SingleOrDefault();

            fetchedChangedExhibits = await FetchExhibits(exhibitSet);

            return fetchedChangedExhibits.Any();
        }

        public async Task FetchBaseDataIntoDatabase()
        {
            var exhibitSet = ExhibitManager.GetExhibitSets().SingleOrDefault();

            if (fetchedChangedExhibits == null)
            {
                fetchedChangedExhibits = await FetchExhibits(exhibitSet);
            }

            if (exhibitSet == null)
            {
                using (DbManager.StartTransaction())
                {
                    exhibitSet = DbManager.CreateBusinessObject<ExhibitSet>();
                }
            }

            var newExhibits = new List<ExhibitDto>();
            var updatedExhibits = new Dictionary<ExhibitDto, Exhibit>();
            var requiredAppetizerPages = new List<int>();
            var requiredImages = new List<int>();

            foreach (var exhibit in fetchedChangedExhibits)
            {
                var dbExhibit = exhibitSet.SingleOrDefault(x => x.IdForRestApi == exhibit.Id);

                if (dbExhibit != null && exhibit.Timestamp != dbExhibit.UnixTimestamp)
                {
                    updatedExhibits.Add(exhibit, dbExhibit);
                    if (exhibit.Pages.Any())
                    {
                        requiredAppetizerPages.Add(exhibit.Pages.First());
                    }
                    requiredImages.Add(exhibit.Image);
                }
                else if (dbExhibit == null)
                {
                    newExhibits.Add(exhibit);
                    if (exhibit.Pages.Any())
                    {
                        requiredAppetizerPages.Add(exhibit.Pages.First());
                    }
                    requiredImages.Add(exhibit.Image);
                }
            }

            var appetizerPages = (await pagesApiAccess.GetPages(requiredAppetizerPages)).Items;
            foreach (var page in appetizerPages)
            {
                requiredImages.Add(page.Image);
            }

            using (DbManager.StartTransaction())
            {
                var fetchedMedia = await mediaDataFetcher.FetchMedias(requiredImages);

                ProcessUpdatedExhibits(updatedExhibits, fetchedMedia, appetizerPages);
                ProcessNewExhibits(newExhibits, fetchedMedia, appetizerPages);
            }
        }

        private void ProcessUpdatedExhibits(Dictionary<ExhibitDto, Exhibit> updatedExhibits, FetchedMediaData fetchedMediaData, IList<PageDto> pages)
        {
            foreach (var exhibitPair in updatedExhibits)
            {
                var exhibitDto = exhibitPair.Key;
                var dbExhibit = exhibitPair.Value;

                ExhibitConverter.Convert(exhibitDto, dbExhibit);

                AddImageToExhibit(dbExhibit, exhibitDto.Image, fetchedMediaData);
                AddAppetizerPageToExhibit(pages, exhibitDto, dbExhibit);
                //TODO: If exhibits content was already downloaded 
                //-> Show dialog whether to download new data or do it directly depending on setting
            }
        }

        private void AddAppetizerPageToExhibit(IList<PageDto> pages, ExhibitDto exhibitDto, Exhibit dbExhibit)
        {
            if (exhibitDto.Pages.Any())
            {
                var page = pages.SingleOrDefault(x => x.Id == exhibitDto.Pages.First());

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

        private void ProcessNewExhibits(IEnumerable<ExhibitDto> newExhibits, FetchedMediaData fetchedMediaData, IList<PageDto> pages)
        {
            foreach (var exhibitDto in newExhibits)
            {
                var dbExhibit = ExhibitConverter.Convert(exhibitDto);

                AddImageToExhibit(dbExhibit, exhibitDto.Image, fetchedMediaData);
                AddAppetizerPageToExhibit(pages, exhibitDto, dbExhibit);
            }
        }

        private async Task<IList<ExhibitDto>> FetchExhibits(ExhibitSet exhibitSet)
        {
            ExhibitsDto exhibits;
            if (exhibitSet != null)
            {
                exhibits = await exhibitsApiAccess.GetExhibits(exhibitSet.Timestamp);
            }
            else
            {
                exhibits = await exhibitsApiAccess.GetExhibits();
            }

            return exhibits.Items;
        }

    }
}