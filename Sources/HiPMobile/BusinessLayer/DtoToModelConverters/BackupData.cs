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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters
{
    public static class BackupData
    {
        private const int BackupImageIdForRestApi = -1;
        private const int BackupImageTagIdForRestApi = -2;
        private static readonly DateTimeOffset BackupTimestamp = DateTimeOffset.MinValue;

        private static readonly SemaphoreSlim IsInitializedSema = new SemaphoreSlim(0);

        public static async Task WaitForInitAsync()
        {
            await IsInitializedSema.WaitAsync();
            IsInitializedSema.Release();
        }

        public static async Task Init()
        {
            // We do NOT use DbManager.InTransaction() here because that would attach BackupImage & BackupImageTag
            // to the transaction and these properties are still null at this point.
            var dataAccess = IoCManager.Resolve<IDataAccess>();
            var fileManager = IoCManager.Resolve<IMediaFileManager>();
            var dataLoader = IoCManager.Resolve<IDataLoader>();

            backupImage = dataAccess.GetItems<Image>().SingleOrDefault(x => x.IdForRestApi == BackupImageIdForRestApi);
            backupImageTag = dataAccess.GetItems<Image>().SingleOrDefault(x => x.IdForRestApi == BackupImageTagIdForRestApi);
            string backupImagePath;
            if (backupImage == null)
            {
                backupImageData = dataLoader.LoadByteData("noImage.png");
                backupImagePath = await fileManager.WriteMediaToDiskAsync(backupImageData, BackupImageIdForRestApi, BackupTimestamp);
            }
            else
            {
                backupImagePath = null;
            }

            string backupImageTagPath;
            if (backupImageTag == null)
            {
                var backupImageDataTag = dataLoader.LoadByteData("noImageTag.jpg");
                backupImageTagPath = await fileManager.WriteMediaToDiskAsync(backupImageDataTag, BackupImageTagIdForRestApi, BackupTimestamp);
            }
            else
            {
                backupImageTagPath = null;
            }

            dataAccess.InTransaction(Enumerable.Empty<object>(), transaction =>
            {
                var transactionDataAccess = transaction.DataAccess;

                if (backupImage == null)
                {
                    backupImageData = dataLoader.LoadByteData("noImage.png");

                    Debug2.Assert(backupImagePath != null);
                    transactionDataAccess.AddItem(backupImage = new Image
                    {
                        Title = "No Image",
                        Description = "Hier fehlt das Bild",
                        IdForRestApi = BackupImageIdForRestApi,
                        DataPath = backupImagePath
                    });
                }

                if (backupImageTag == null)
                {
                    Debug2.Assert(backupImageTagPath != null);
                    transactionDataAccess.AddItem(backupImageTag = new Image
                    {
                        Title = "No Tag Image",
                        Description = "Hier fehlt das Tag-Bild",
                        IdForRestApi = BackupImageTagIdForRestApi,
                        DataPath = backupImageTagPath
                    });
                }

                mockAudioData = dataLoader.LoadByteData("mockaudio.mp3");
                return 0;
            });
            IsInitializedSema.Release();
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