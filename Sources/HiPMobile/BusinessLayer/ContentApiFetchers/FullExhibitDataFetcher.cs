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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using Unity.Attributes;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers
{
    public class FullExhibitDataFetcher : IFullExhibitDataFetcher
    {
        private readonly IPagesApiAccess pagesApiAccess;
        private readonly IMediaDataFetcher mediaDataFetcher;

        [Dependency]
        public PageConverter PageConverter { private get; set; }

        private QuizConverter QuizConverter { get; } = IoCManager.Resolve<QuizConverter>();

        public FullExhibitDataFetcher(IPagesApiAccess pagesApiAccess, IMediaDataFetcher mediaDataFetcher)
        {
            this.pagesApiAccess = pagesApiAccess;
            this.mediaDataFetcher = mediaDataFetcher;
        }

        private IList<int?> requiredMedia;
        private IList<PageDto> pageItems;

        public async Task FetchFullDownloadableDataIntoDatabase(
            string exhibitId, int idForRestApi, CancellationToken token, IProgressListener listener)
        {
            double totalSteps = await FetchNeededMediaForFullExhibit(idForRestApi);

            if (token.IsCancellationRequested)
            {
                return;
            }

            listener.SetMaxProgress(totalSteps);

            // We need to attach all images since the quiz download may download existing images
            var itemsToTrack = DbManager
                               .DataAccess
                               .GetItems<Image>()
                               .Where(image => image.Id != BackupData.BackupImage.Id && image.Id != BackupData.BackupImageTag.Id);
            await mediaDataFetcher.FetchMedias(requiredMedia, token, listener);
            var quizzes = await DownloadQuizes(idForRestApi, token);
            var mediaToFilePath = await mediaDataFetcher.WriteMediaToDiskAsync();
            DbManager.InTransaction(itemsToTrack, transaction =>
            {
                ProcessPages(exhibitId, token, transaction.DataAccess, mediaToFilePath);
                transaction.DataAccess.Quizzes().Add(quizzes);

                if (token.IsCancellationRequested)
                {
                    transaction.Rollback();
                }
            });
        }

        private async Task<IEnumerable<Quiz>> DownloadQuizes(int exhibitId, CancellationToken token)
        {
            var quizApiAccess = IoCManager.Resolve<IQuizApiAccess>();
            try
            {
                var quizDtos = await quizApiAccess.GetQuestionsForExhibitAsync(exhibitId);
                var quizzes = quizDtos.Select(QuizConverter.Convert).ToList();
                foreach (var quiz in quizzes)
                {
                    var quizImage = quiz.Image?.IdForRestApi;
                    await mediaDataFetcher.FetchMedias(new[] { quizImage }.WhereNotNull().ToList(), token, null);
                }

                return quizzes;
            }
            catch (NotFoundException)
            {
                // Don't crash if there are no questions
                Debug.WriteLine($"No quiz found for exhibit {exhibitId}");
                return Enumerable.Empty<Quiz>();
            }
        }

        public async Task FetchFullExhibitDataIntoDatabaseWithFetchedPagesAndMedia(string exhibitId, ExhibitPagesAndMediaContainer exhibitPagesAndMediaContainer,
                                                                                   CancellationToken token, IProgressListener listener, int dbExhibitIdForRestApi)
        {
            requiredMedia = exhibitPagesAndMediaContainer.RequiredMedia;
            pageItems = exhibitPagesAndMediaContainer.PageDtos;

            var mediaToFilePath = await mediaDataFetcher.WriteMediaToDiskAsync();
            await mediaDataFetcher.FetchMedias(requiredMedia, token, listener);
            var quizzes = await DownloadQuizes(dbExhibitIdForRestApi, token);
            DbManager.InTransaction(transaction =>
            {
                ProcessPages(exhibitId, token, transaction.DataAccess, mediaToFilePath);
                transaction.DataAccess.Quizzes().Add(quizzes);

                if (token.IsCancellationRequested)
                {
                    transaction.Rollback();
                }
            });
        }

        public async Task<ExhibitPagesAndMediaContainer> FetchPagesAndMediaForExhibitFromRouteFetcher(int idForRestApi)
        {
            await FetchNeededMediaForFullExhibit(idForRestApi);
            return new ExhibitPagesAndMediaContainer(idForRestApi, requiredMedia, pageItems);
        }

        public async Task<int> FetchNeededMediaForFullExhibit(int idForRestApi)
        {
            requiredMedia = new List<int?>();
            pageItems = (await pagesApiAccess.GetPages(idForRestApi)).Items;

            foreach (var page in pageItems)
            {
                AddMediaId(page.Image);
                if (page.Type == PageTypeDto.SliderPage)
                {
                    foreach (var image in page.Images)
                    {
                        AddMediaId(image.Image);
                    }
                }

                AddMediaId(page.Audio);
            }

            return requiredMedia.Count;
        }

        private void AddMediaId(int? mediaId)
        {
            if (mediaId.HasValue)
            {
                // Only add media IDs if not already in list
                if (!requiredMedia.Contains(mediaId))
                {
                    requiredMedia.Add(mediaId);
                }
            }
        }

        private void ProcessPages(string exhibitId, CancellationToken token, ITransactionDataAccess dataAccess,
                                  Dictionary<MediaDto, string> mediaToFilePath)
        {
            var fetchedMedia = mediaDataFetcher.CombineMediasAndFiles(dataAccess, mediaToFilePath);

            if (token.IsCancellationRequested)
            {
                return;
            }

            var exhibit = dataAccess.Exhibits().GetExhibit(exhibitId);

            foreach (var pageDto in pageItems)
            {
                var dbPage = PageConverter.Convert(pageDto);

                AddContentToPage(dbPage, pageDto, fetchedMedia);
                // Add Page with content to the exhibit
                exhibit.Pages.Add(dbPage);
            }

            // Rearrange additional information pages
            var pagesToBeRemoved = new List<Page>();
            foreach (var pageDto in pageItems)
            {
                if (pageDto.AdditionalInformationPages.Count > 0)
                {
                    foreach (var existingPageWithInfo in exhibit.Pages)
                    {
                        if (pageDto.Id == existingPageWithInfo.IdForRestApi)
                        {
                            foreach (var pageId in pageDto.AdditionalInformationPages)
                            {
                                foreach (var pageToBeAdded in exhibit.Pages)
                                {
                                    if (pageToBeAdded.IdForRestApi == pageId)
                                    {
                                        existingPageWithInfo.AdditionalInformationPages.Add(pageToBeAdded);
                                        pagesToBeRemoved.Add(pageToBeAdded);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (var pageToBeRemoved in pagesToBeRemoved)
            {
                exhibit.Pages.Remove(pageToBeRemoved);
            }

            exhibit.DetailsDataLoaded = true;
        }

        private void AddContentToPage(Page dbPage, PageDto pageDto, FetchedMediaData fetchedMedia)
        {
            if (pageDto != null && dbPage != null)
            {
                PageConverter.Convert(pageDto, dbPage);

                switch (dbPage)
                {
                    case ImagePage imagePage:
                        var image = fetchedMedia.Images.SingleOrDefault(x => x.IdForRestApi == pageDto.Image);
                        imagePage.Image = image ?? BackupData.BackupImage;
                        break;

                    case TextPage textPage:
                        break;

                    case TimeSliderPage timeSliderPage:
                        var fetchedImages = fetchedMedia.Images;

                        if (fetchedImages.Count > 0)
                        {
                            Debug2.Assert(timeSliderPage.SliderImages.Count == pageDto.Images.Count);
                            for (var i = 0; i < timeSliderPage.SliderImages.Count; i++)
                            {
                                var imageId = pageDto.Images[i].Image;
                                var dbImage = fetchedImages.FirstOrDefault(img => img.IdForRestApi == imageId);
                                timeSliderPage.SliderImages[i].Image = dbImage;
                            }
                        }
                        else
                        {
                            timeSliderPage.SliderImages.Add(new TimeSliderPageImage
                            {
                                Page = timeSliderPage,
                                Image = BackupData.BackupImage
                            });
                        }

                        break;
                }

                var audio = fetchedMedia.Audios.SingleOrDefault(x => x.IdForRestApi == pageDto.Audio);

                if (audio != null)
                {
                    dbPage.Audio = audio;
                }
            }
        }
    }
}