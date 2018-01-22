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
using System.Linq;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses
{
    public class MockMediaApiAccess : IMediasApiAccess
    {
        private readonly IList<MediaDto> dtos = new List<MediaDto>
        {
            new MediaDto
            {
                Description = "Description",
                Id = 0,
                Status = null, // TODO
                Timestamp = DateTimeOffset.Now,
                Title = "Title",
                Type = MediaTypeDto.Audio,
                Used = true
            },
            new MediaDto
            {
                Description = "Description",
                Id = 1,
                Status = null, // TODO
                Timestamp = DateTimeOffset.Now,
                Title = "Title",
                Type = MediaTypeDto.Image,
                Used = true
            }
        };

        public Task<MediasDto> GetMedias() =>
            Task.FromResult(new MediasDto { Items = dtos, Total = dtos.Count });

        public Task<MediasDto> GetMedias(DateTimeOffset timestamp) =>
            Task.FromResult(new MediasDto { Items = new List<MediaDto>(), Total = 0 });

        public Task<MediasDto> GetMedias(IList<int> includeOnly)
        {
            var included = dtos.Where(it => includeOnly.Contains(it.Id)).ToList();
            return Task.FromResult(new MediasDto { Items = included, Total = included.Count });
        }

        public Task<MediasDto> GetMedias(DateTimeOffset timestamp, IList<int> includeOnly) =>
            Task.FromResult(new MediasDto { Items = new List<MediaDto>(), Total = 0 });

        public Task<IList<int>> GetIds() => Task.FromResult((IList<int>) dtos.Select(it => it.Id).ToList());
    }
}