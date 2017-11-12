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
using System.Linq;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters
{
    public static class BackupData
    {

        private const int BackupImageIdForRestApi = -1;
        private const int BackupImageTagIdForRestApi = -2;
        
        public static async Task Init()
        {
            var dataAccess = IoCManager.Resolve<IDataAccess>();
            var dataLoader = IoCManager.Resolve<IDataLoader>();
            var fileManager = IoCManager.Resolve<IMediaFileManager>();
            
            backupImage = dataAccess.GetItems<Image>().SingleOrDefault(x => x.IdForRestApi == BackupImageIdForRestApi);
            if (backupImage == null)
            {
                backupImage = DbManager.CreateBusinessObject<Image>();

                backupImage.Title = "No Image";
                backupImage.Description = "Hier fehlt das Bild";
                backupImage.IdForRestApi = BackupImageIdForRestApi;
                
                backupImageData = dataLoader.LoadByteData("noImage.png");
                var (md5, path) = await fileManager.WriteMediaToDiskAsync(backupImageData);
                backupImage.DataMd5 = md5;
                backupImage.DataPath = path;
            }
            
            backupImageTag = dataAccess.GetItems<Image>().SingleOrDefault(x => x.IdForRestApi == BackupImageTagIdForRestApi);
            if (backupImageTag == null)
            {
                backupImageTag = DbManager.CreateBusinessObject<Image>();

                backupImageTag.Title = "No Tag Image";
                backupImageTag.Description = "Hier fehlt das Tag-Bild";
                backupImageTag.IdForRestApi = BackupImageTagIdForRestApi;
                
                var backupImageDataTag = dataLoader.LoadByteData("noImageTag.jpg");
                var (md5, path) = await fileManager.WriteMediaToDiskAsync(backupImageDataTag);
                backupImageTag.DataMd5 = md5;
                backupImageTag.DataPath = path;
            }
        }
        
        private static byte[] backupImageData;

        public static byte[] BackupImageData => backupImageData ?? throw new NullReferenceException("BackupData.Init() not called yet!");

        private static Image backupImage;

        public static Image BackupImage => backupImage?? throw new NullReferenceException("BackupData.Init() not called yet!");

        private static Image backupImageTag;

        public static Image BackupImageTag => backupImageTag ?? throw new NullReferenceException("BackupData.Init() not called yet!");
    }
}