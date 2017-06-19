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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
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

        private IList<int?> requiredMedias;
        private IList<PageDto> pageItems;

        public async Task FetchFullExhibitDataIntoDatabase (string exhibitId, int idForRestApi, CancellationToken token, IProgressListener listener)
        {
            requiredMedias = new List<int?>();

            double totalSteps = await FetchNeededMediaForFullExhibit(idForRestApi);

            if (token.IsCancellationRequested)
            {
                return;
            }

            listener.SetMaxProgress(totalSteps);

            using (var transaction = DbManager.StartTransaction())
            {
                await ProcessPages(exhibitId, token, listener);
                if (token.IsCancellationRequested)
                {
                    transaction.Rollback();
                }
            }
            IoCManager.Resolve<IDbChangedHandler>().NotifyAll();
        }

        private async Task<int> FetchNeededMediaForFullExhibit(int idForRestApi)
        {
            pageItems = (await pagesApiAccess.GetPages(idForRestApi)).Items;

            // Since AppetizerPages have been loaded before, do not consider them anymore
            List<PageDto> appetizerPagesToRemove = new List<PageDto> ();
            foreach (var page in pageItems)
            {
                if (page.Type != PageTypeDto.AppetizerPage)
                {
                    if (page.Image.HasValue)
                    {
                        requiredMedias.Add(page.Image);
                    }
                    if (page.Type == PageTypeDto.SliderPage)
                    {
                        if (page.Images.Count > 0)
                        {
                            foreach (var image in page.Images)
                            {
                                if (image.Image.HasValue)
                                {
                                    requiredMedias.Add(image.Image);
                                }
                            }
                        }
                    }
                    if (page.Audio.HasValue)
                    {
                        requiredMedias.Add(page.Audio);
                    }
                }
                else
                {
                    appetizerPagesToRemove.Add(page);
                }
            }

            foreach (var appPage in appetizerPagesToRemove)
            {
                pageItems.Remove(appPage);
            }
            
            return requiredMedias.Count;
        }

        private async Task ProcessPages(string exhibitId, CancellationToken token, IProgressListener listener)
        {
            await FetchMediaData(token, listener);
            var fetchedMedia = mediaDataFetcher.CombineMediasAndFiles();

            if (token.IsCancellationRequested)
            {
                return;
            }

            Exhibit exhibit = ExhibitManager.GetExhibit(exhibitId);

            foreach (var pageDto in pageItems)
            {
                var dbPage = PageConverter.Convert(pageDto);

                AddContentToPage(dbPage, pageDto, fetchedMedia, listener);
                // Add Page with content to the exhibit
                exhibit.Pages.Add(dbPage);
            }

            exhibit.DetailsDataLoaded = true;
        }

        private async Task FetchMediaData(CancellationToken token, IProgressListener listener)
        {
            await mediaDataFetcher.FetchMedias(requiredMedias, token, listener);
        }

        private void AddContentToPage(Page dbPage, PageDto content, FetchedMediaData fetchedMedia, IProgressListener listener)
        {
            if (content != null && dbPage != null)
            {
                PageConverter.Convert (content, dbPage);

                switch (dbPage.PageType)
                {
                    case PageType.AppetizerPage:
                        // Should not be reached
                        break;
                    case PageType.ImagePage:
                        var image = fetchedMedia.Images.SingleOrDefault(x => x.IdForRestApi == content.Image);

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
                        var fetchedImages = fetchedMedia.Images;

                        if (fetchedImages.Count > 0)
                        {
                            foreach (var fImg in fetchedImages)
                            {
                                foreach (var cImg in content.Images)
                                {
                                    if (fImg.IdForRestApi == cImg.Image)
                                    {
                                        dbPage.TimeSliderPage.Images.Add(fImg);
                                    }
                                }
                            }
                        }
                        else
                        {
                            dbPage.TimeSliderPage.Images.Add(BackupData.BackupImage);
                        }
                        break;
                }

                var audio = fetchedMedia.Audios.SingleOrDefault(x => x.IdForRestApi == content.Audio);

                if (audio != null)
                {
                    dbPage.Audio = audio;
                    listener.ProgressOneStep();
                }
            }
        }
    }
}