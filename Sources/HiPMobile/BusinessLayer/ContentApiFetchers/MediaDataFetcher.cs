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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

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

        public async Task<FetchedMediaData> CombineMediasAndFiles()
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

            foreach (var media in fetchedMedias)
            {
                switch (media.Type)
                {
                    case MediaTypeDto.Audio:
                        var audio = AudioConverter.Convert(media);
                        var audioFile = fetchedFiles?.SingleOrDefault(x => x.MediaId == media.Id);

                        if (audioFile != null)
                        {
                            var path = await fileManager.WriteMediaToDiskAsync(audioFile.Data, audio.IdForRestApi, audio.Timestamp);
                            audio.DataPath = path;
                        }
                        else /* Download was skipped because file is already downloaded */
                        {
                            audio.DataPath = fileManager.PathForRestApiId(audio.IdForRestApi);
                        }
                        fetchedData.Audios.Add(audio);
                        break;
                    case MediaTypeDto.Image:
                        var image = ImageConverter.Convert(media);
                        var imageFile = fetchedFiles?.SingleOrDefault(x => x.MediaId == media.Id);

                        if (imageFile != null)
                        {
                            var path = await fileManager.WriteMediaToDiskAsync(imageFile.Data, image.IdForRestApi, image.Timestamp);
                            image.DataPath = path;
                        }
                        else /* Download was skipped because file is already downloaded */
                        {
                            image.DataPath = fileManager.PathForRestApiId(image.IdForRestApi);
                        }
                        fetchedData.Images.Add(image);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Unsupported MediaTypeDto!");
                }
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