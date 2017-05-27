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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
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
        private IList<int?> requiredPageImages;
        private IList<PageDto> pageItems;

        public async Task FetchFullExhibitDataIntoDatabase (int idForRestApi, CancellationToken token, IProgressListener listener)
        {
            this.idForRestApi = idForRestApi;
            requiredPageImages = new List<int?> ();

            double totalSteps = await FetchNeededDataForFullExhibit();

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

        private async Task<int> FetchNeededDataForFullExhibit()
        {
            pageItems = (await pagesApiAccess.GetPages(idForRestApi)).Items;
            foreach (var page in pageItems)
            {
                requiredPageImages.Add (page.Image);
            }
            return requiredPageImages.Count;
        }

        private async Task ProcessPages(CancellationToken token, IProgressListener listener)
        {
            var fetchedMedia = await mediaDataFetcher.FetchMedias(requiredPageImages, token, listener);
            if (token.IsCancellationRequested)
            {
                return;
            }

            foreach (var pageDto in pageItems)
            {
                var dbPage = PageConverter.Convert(pageDto);

                //AddImageToPage(dbPage, pageDto.Image, fetchedMedia); //TODO: Implementation

                listener.ProgressOneStep();
            }
        }
    }
}