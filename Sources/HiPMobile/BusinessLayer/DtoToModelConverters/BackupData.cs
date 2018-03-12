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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters
{
    public static class BackupData
    {
        private const int BackupImageIdForRestApi = -1;
        private const int BackupImageTagIdForRestApi = -2;
        private static readonly DateTimeOffset BackupTimestamp = DateTimeOffset.MinValue;

        public static async Task Init()
        {
            // We do NOT use DbManager.StartTransaction() here because that would attach BackupImage & BackupImageTag
            // to the transaction and these properties are still null at this point.
            using (var transaction = DbManager.DataAccess.StartTransaction(Enumerable.Empty<object>()))
            {
                var dataAccess = transaction.DataAccess;
                var dataLoader = IoCManager.Resolve<IDataLoader>();
                var fileManager = IoCManager.Resolve<IMediaFileManager>();

                backupImage = dataAccess.GetItems<Image>().SingleOrDefault(x => x.IdForRestApi == BackupImageIdForRestApi);
                if (backupImage == null)
                {
                    backupImageData = dataLoader.LoadByteData("noImage.png");
                    var path = await fileManager.WriteMediaToDiskAsync(backupImageData, BackupImageIdForRestApi, BackupTimestamp);

                    dataAccess.AddItem(backupImage = new Image
                    {
                        Title = "No Image",
                        Description = "Hier fehlt das Bild",
                        IdForRestApi = BackupImageIdForRestApi,
                        DataPath = path
                    });
                }

                backupImageTag = dataAccess.GetItems<Image>().SingleOrDefault(x => x.IdForRestApi == BackupImageTagIdForRestApi);
                if (backupImageTag == null)
                {
                    var backupImageDataTag = dataLoader.LoadByteData("noImageTag.jpg");
                    var path = await fileManager.WriteMediaToDiskAsync(backupImageDataTag, BackupImageTagIdForRestApi, BackupTimestamp);

                    dataAccess.AddItem(backupImageTag = new Image
                    {
                        Title = "No Tag Image",
                        Description = "Hier fehlt das Tag-Bild",
                        IdForRestApi = BackupImageTagIdForRestApi,
                        DataPath = path
                    });
                }

                mockAudioData = dataLoader.LoadByteData("mockaudio.mp3");
            }
        }

        private static byte[] backupImageData;

        public static byte[] BackupImageData => backupImageData ?? throw new NullReferenceException("BackupData.Init() not called yet!");

        private static Image backupImage;

        public static Image BackupImage => backupImage ?? throw new NullReferenceException("BackupData.Init() not called yet!");

        private static Image backupImageTag;

        public static Image BackupImageTag => backupImageTag ?? throw new NullReferenceException("BackupData.Init() not called yet!");

        private static byte[] mockAudioData;

        public static byte[] MockAudioData => mockAudioData ?? throw new NullReferenceException("BackupData.Init() not called yet!");
    }
}