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

using System;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses
{
    public class MockFileApiAccess : IFileApiAccess
    {
        public Task<FileDto> GetFile(int mediaId)
        {
            byte[] data;
            switch (mediaId)
            {
                case 0:
                    data = BackupData.MockAudioData;
                    break;
                case 1:
                    data = BackupData.BackupImageData;
                    break;
                default:
                    throw new ArgumentException("Unknown mediaId");
            }

            return Task.FromResult(new FileDto
            {
                Data = data,
                MediaId = mediaId
            });
        }
    }
}