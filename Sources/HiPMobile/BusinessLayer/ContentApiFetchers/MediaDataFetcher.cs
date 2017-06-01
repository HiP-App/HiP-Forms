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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
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
            fetchedMedias = await FetchMediaDtos(mediaIds);
            fetchedFiles = await FetchFileDtos(mediaIds, token, progressListener);
        }

        public FetchedMediaData CombineMediasAndFiles()
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

            foreach (var media in fetchedMedias)
            {
                switch (media.Type)
                {
                    case MediaTypeDto.Audio:
                        var audio = AudioConverter.Convert(media);
                        var audioFile = fetchedFiles?.SingleOrDefault(x => x.MediaId == media.Id);

                        if (audioFile != null)
                        {
                            audio.Data = audioFile.Data;
                        }
                        fetchedData.Audios.Add(audio);
                        break;
                    case MediaTypeDto.Image:
                        var image = ImageConverter.Convert(media);
                        var imageFile = fetchedFiles?.SingleOrDefault(x => x.MediaId == media.Id);

                        if (imageFile != null)
                        {
                            image.Data = imageFile.Data;
                        }
                        fetchedData.Images.Add(image);
                        break;
                }
            }

            return fetchedData;
        }

        private async Task<IList<MediaDto>> FetchMediaDtos(IList<int?> requiredImages)
        {
            var medias = await mediasApiAccess.GetMedias(requiredImages.Where(x => x.HasValue).Select(y => y.Value).ToList());
            return medias.Items;
        }

        private async Task<IList<FileDto>> FetchFileDtos(IList<int?> requiredImages, CancellationToken token, IProgressListener progressListener)
        {
            var files = new List<FileDto>();
            foreach (int? mediaId in requiredImages)
            {
                if (!mediaId.HasValue)
                {
                    continue;
                }
                if (token.IsCancellationRequested)
                {
                    break;
                }

                FileDto file;
                try
                {
                    file = await fileApiAccess.GetFile(mediaId.Value);
                }
                catch (NotFoundException)
                {
                    file = new FileDto {Data = BackupData.BackupImageData, MediaId = mediaId.Value};
                }
                files.Add(file);
                progressListener.ProgressOneStep();
            }

            return files;
        }
    }
}