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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Attributes;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers
{
    public class MediaDataFetcher : IMediaDataFetcher
    {
        private readonly IMediasApiAccess mediasApiAccess;
        private readonly IFileApiAccess fileApiAccess;

        [Dependency]
        public MediaToImageConverter ImageConverter { private get; set; }

        [Dependency]
        public MediaToAudioConverter AudioConverter { private get; set; }

        public MediaDataFetcher(IMediasApiAccess mediasApiAccess, IFileApiAccess fileApiAccess)
        {
            this.mediasApiAccess = mediasApiAccess;
            this.fileApiAccess = fileApiAccess;
        }

        private IList<MediaDto> fetchedMedias;
        private IList<FileDto> fetchedFiles;

        public async Task FetchMedias(IList<int?> mediaIds, CancellationToken token, IProgressListener progressListener)
        {
            var requiredImages = mediaIds.Where(x => x.HasValue).Select(y => y.Value).Distinct().ToList();

            if (requiredImages.Any())
            {
                fetchedMedias = await FetchMediaDtos(requiredImages);
                fetchedFiles = await FetchFileDtos(fetchedMedias, token, progressListener);
            }
        }

        public async Task<FetchedMediaData> CombineMediasAndFiles(ITransactionDataAccess dataAccess)
        {
            var fetchedData = new FetchedMediaData
            {
                Audios = new List<Audio>(),
                Images = new List<Image>()
            };

            if (fetchedMedias == null)
            {
                return fetchedData;
            }

            var fileManager = IoCManager.Resolve<IMediaFileManager>();

            foreach (var mediaDto in fetchedMedias)
            {
                var isAudio =
                    mediaDto.Type == MediaTypeDto.Audio ? true :
                    mediaDto.Type == MediaTypeDto.Image ? false :
                    throw new ArgumentOutOfRangeException("Unsupported media type");

                var dbMedia = isAudio
                    ? AudioConverter.ConvertReplacingExisting(mediaDto, mediaDto.Id.ToString(), dataAccess)
                    : ImageConverter.ConvertReplacingExisting(mediaDto, mediaDto.Id.ToString(), dataAccess) as Media;

                var file = fetchedFiles?.SingleOrDefault(x => x.MediaId == mediaDto.Id);

                dbMedia.DataPath = (file == null)
                    ? fileManager.PathForRestApiId(mediaDto.Id) // file is already downloaded, assign correct path
                    : await fileManager.WriteMediaToDiskAsync(file.Data, mediaDto.Id, dbMedia.Timestamp); // new file was downloaded, store it

                if (isAudio)
                    fetchedData.Audios.Add((Audio)dbMedia);
                else
                    fetchedData.Images.Add((Image)dbMedia);
            }

            return fetchedData;
        }

        private async Task<IList<MediaDto>> FetchMediaDtos(IList<int> requiredImages)
        {
            var medias = await mediasApiAccess.GetMedias(requiredImages);
            return medias.Items;
        }

        private async Task<IList<FileDto>> FetchFileDtos(IList<MediaDto> mediaDtos, CancellationToken token, IProgressListener progressListener)
        {
            var fileManager = IoCManager.Resolve<IMediaFileManager>();
            var files = new List<FileDto>();
            foreach (var mediaDto in mediaDtos)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                var mediaId = mediaDto.Id;
                var mediaTimestamp = mediaDto.Timestamp;
                if (await fileManager.ContainsMedia(mediaId, mediaTimestamp))
                {
                    Debug.WriteLine($"Skipped download of already present file with id {mediaId}");
                    continue;
                }

                FileDto file;
                try
                {
                    file = await fileApiAccess.GetFile(mediaId);
                }
                catch (NotFoundException)
                {
                    file = new FileDto { Data = BackupData.BackupImageData, MediaId = mediaId };
                }
                files.Add(file);
                progressListener.ProgressOneStep();
            }

            return files;
        }
    }
}