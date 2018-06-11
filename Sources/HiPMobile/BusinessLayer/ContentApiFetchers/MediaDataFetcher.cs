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
using JetBrains.Annotations;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers
{
    public class MediaDataFetcher : IMediaDataFetcher
    {
        private readonly IMediasApiAccess mediasApiAccess;
        private readonly IFileApiAccess fileApiAccess;
        private readonly MediaToImageConverter imageConverter;
        private readonly MediaToAudioConverter audioConverter;

        public MediaDataFetcher(IMediasApiAccess mediasApiAccess, IFileApiAccess fileApiAccess, MediaToImageConverter imageConverter, MediaToAudioConverter audioConverter)
        {
            this.mediasApiAccess = mediasApiAccess;
            this.fileApiAccess = fileApiAccess;
            this.imageConverter = imageConverter;
            this.audioConverter = audioConverter;
        }

        private IList<MediaDto> fetchedMedias;
        private IList<FileDto> fetchedFiles;

        public async Task FetchMedias(IList<int?> mediaIds, CancellationToken token, [CanBeNull] IProgressListener progressListener)
        {
            var requiredImages = mediaIds.Where(x => x.HasValue).Select(y => y.Value).Distinct().ToList();

            if (requiredImages.Any())
            {
                fetchedMedias = await FetchMediaDtos(requiredImages);
                fetchedFiles = await FetchFileDtos(fetchedMedias, token, progressListener);
            }
        }

        /// <summary>
        /// Returns a map of Media objects to their file path to be set as the DataPath.
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<MediaDto, string>> WriteMediaToDiskAsync()
        {
            var fileManager = IoCManager.Resolve<IMediaFileManager>();
            var mediaToFilePath = new Dictionary<MediaDto, string>();
            foreach (var mediaDto in fetchedMedias)
            {
                var file = fetchedFiles?.SingleOrDefault(x => x.MediaId == mediaDto.Id);

                mediaToFilePath[mediaDto] = file == null
                    ? fileManager.PathForRestApiId(mediaDto.Id) // file is already downloaded, assign correct path
                    : await fileManager.WriteMediaToDiskAsync(file.Data, mediaDto.Id, mediaDto.Timestamp); // new file was downloaded, store it
            }

            return mediaToFilePath;
        }

        public FetchedMediaData CombineMediasAndFiles(ITransactionDataAccess dataAccess, Dictionary<MediaDto, string> mediaToFilePath)
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

            foreach (var mediaDto in fetchedMedias)
            {
                var isAudio =
                    mediaDto.Type == MediaTypeDto.Audio ? true :
                    mediaDto.Type == MediaTypeDto.Image ? false :
                    throw new ArgumentOutOfRangeException("Unsupported media type");

                var dbMedia = isAudio
                    ? audioConverter.ConvertReplacingExisting(mediaDto, mediaDto.Id.ToString(), dataAccess)
                    : imageConverter.ConvertReplacingExisting(mediaDto, mediaDto.Id.ToString(), dataAccess) as Media;

                dbMedia.DataPath = mediaToFilePath[mediaDto] ?? throw new NullReferenceException($"No file path for image {mediaDto.Id}");

                Debug2.Assert(dbMedia.DataPath != null);

                if (isAudio)
                    fetchedData.Audios.Add((Audio) dbMedia);
                else
                    fetchedData.Images.Add((Image) dbMedia);
            }

            return fetchedData;
        }

        private async Task<IList<MediaDto>> FetchMediaDtos(IList<int> requiredImages)
        {
            var medias = await mediasApiAccess.GetMedias(requiredImages);
            return medias.Items;
        }

        private async Task<IList<FileDto>> FetchFileDtos(IList<MediaDto> mediaDtos, CancellationToken token, [CanBeNull] IProgressListener progressListener)
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
                progressListener?.ProgressOneStep();
            }

            return files;
        }
    }
}