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

using System.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters {
    public static class BackupData {

        private static byte[] backupImageData;
        public static byte[] BackupImageData {
            get {
                if (backupImageData == null)
                {
                    var dataLoader = IoCManager.Resolve<IDataLoader>();
                    backupImageData = dataLoader.LoadByteData("noImage.jpg");
                }
                return backupImageData;
            }
        }

        private static Image backupImage;
        public static Image BackupImage {
            get
            {
                if (backupImage == null)
                {
                    var dataAccess = IoCManager.Resolve<IDataAccess>();
                    backupImage = dataAccess.GetItems<Image> ().SingleOrDefault(x => x.IdForRestApi == -1);
                    if (backupImage == null)
                    {
                        backupImage = DbManager.CreateBusinessObject<Image>();

                        backupImage.Title = "No Image";
                        backupImage.Description = "Hier fehlt das Bild";
                        backupImage.Data = BackupImageData;
                        backupImage.IdForRestApi = -1;
                    }
                }

                return backupImage;
            }
        }

    }
}