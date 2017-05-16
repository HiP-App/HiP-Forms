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

using System.Text;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses {
    public class FileApiAccess : IFileApiAccess {

        private readonly IContentApiAccess contentApiAccess;

        public FileApiAccess (IContentApiAccess contentApiAccess)
        {
            this.contentApiAccess = contentApiAccess;
        }

        public async Task<FileDto> GetFile (int mediaId)
        {
            string requestPath = $@"/Media/{mediaId}/File";
            string response = await contentApiAccess.GetResponseFromUrl(requestPath);

            return new FileDto
                {
                    Data = Encoding.UTF8.GetBytes (response),
                    MediaId = mediaId
                };
        }

    }
}