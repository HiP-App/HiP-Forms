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
    public class FullExhibitDataFetcher : IFullExhibitDataFetcher {

        private readonly IPagesApiAccess pagesApiAccess;
        private readonly IMediaDataFetcher mediaDataFetcher;

        [Dependency]
        public PageConverter PageConverter { private get; set; }

        public FullExhibitDataFetcher(IPagesApiAccess pagesApiAccess, IMediaDataFetcher mediaDataFetcher)
        {
            this.pagesApiAccess = pagesApiAccess;
            this.mediaDataFetcher = mediaDataFetcher;
        }

        private int idForRestApi;
        private IList<int?> requiredPages;
        private IList<int?> requiredImages;
        private IList<int?> requiredAudios;
        private IList<PageDto> pageItems;

        public async Task FetchFullExhibitDataIntoDatabase (int idForRestApi, CancellationToken token, IProgressListener listener)
        {
            this.idForRestApi = idForRestApi;
            requiredPages = new List<int?> ();
            requiredImages = new List<int?>();
            requiredAudios = new List<int?>();

            double totalSteps = await FetchNeededPagesForFullExhibit();

            if (token.IsCancellationRequested)
            {
                return;
            }

            listener.SetMaxProgress(totalSteps);

            using (var transaction = DbManager.StartTransaction())
            {
                await ProcessPages(token, listener);
                if (token.IsCancellationRequested)
                {
                    transaction.Rollback();
                }
            }
        }

        private async Task<int> FetchNeededPagesForFullExhibit()
        {
            pageItems = (await pagesApiAccess.GetPages(idForRestApi)).Items;
            foreach (var page in pageItems)
            {
                requiredPages.Add (page.Id);
                if (page.Image.HasValue)
                {
                    requiredImages.Add (page.Image);
                }
                if (page.Audio.HasValue)
                {
                    requiredAudios.Add (page.Audio);
                }
            }
            return requiredPages.Count + requiredImages.Count + requiredAudios.Count;
        }

        private async Task ProcessPages(CancellationToken token, IProgressListener listener)
        {
            var fetchedMediaImages = await mediaDataFetcher.FetchMedias(requiredImages, token, listener);
            var fetchedMediaAudios = await mediaDataFetcher.FetchMedias(requiredAudios, token, listener);

            if (token.IsCancellationRequested)
            {
                return;
            }

            foreach (var pageDto in pageItems)
            {
                var dbPage = PageConverter.Convert(pageDto);

                AddContentToPage(dbPage, pageDto, fetchedMediaImages, fetchedMediaAudios);
                //AddPageToExhibit(pageDto, dbPage, fetchedMediaImages, fetchedMediaAudios); TODO: implementation

                listener.ProgressOneStep();
            }
        }

        private void AddContentToPage(Page dbPage, PageDto content, FetchedMediaData fetchedMediaImages, FetchedMediaData fetchedMediaAudios)
        {
            switch (dbPage.PageType)
            {
                case PageType.AppetizerPage:
                    break;
                case PageType.ImagePage:
                    var image = fetchedMediaImages.Images.SingleOrDefault(x => x.IdForRestApi == content.Image);
                
                    if (image != null)
                    {
                        dbPage.ImagePage.Image = image;
                    }
                    else
                    {
                        dbPage.ImagePage.Image = BackupData.BackupImage;
                    }
                    break;
                case PageType.TextPage:
                    break;
                case PageType.TimeSliderPage:
                    var images = fetchedMediaImages.Images;

                    if (images.Count > 0)
                    {
                        foreach (var img in images)
                        {
                            dbPage.TimeSliderPage.Images.Add (img);
                        }
                    }
                    else
                    {
                        dbPage.TimeSliderPage.Images.Add(BackupData.BackupImage);
                    }
                    break;
            }

            var audio = fetchedMediaAudios.Audios.SingleOrDefault(x => x.IdForRestApi == content.Audio);

            if (audio != null)
            {
                dbPage.Audio = audio;
            }
        }
    }
}