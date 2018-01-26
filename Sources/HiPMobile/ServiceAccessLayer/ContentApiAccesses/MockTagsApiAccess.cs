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
using OsmSharp.Tags;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses
{
    public class MockTagsApiAccess : ITagsApiAccess
    {
        private readonly IList<TagDto> dtos = new List<TagDto>
        {
            new TagDto
            {
                Description = "Description",
                Id = 0,
                Image = 1,
                Status = "PUBLISHED",
                Timestamp = DateTimeOffset.Now,
                Title = "Title",
                Used = true
            },
            new TagDto
            {
                Description = "Description B",
                Id = 1,
                Image = 1,
                Status = "PUBLISHED",
                Timestamp = DateTimeOffset.Now,
                Title = "Title B",
                Used = true
            }
        };

        public Task<TagsDto> GetTags() =>
            Task.FromResult(new TagsDto { Items = dtos, Total = dtos.Count });

        public Task<TagsDto> GetTags(DateTimeOffset timestamp) =>
            Task.FromResult(new TagsDto { Items = new List<TagDto>(), Total = 0 });

        public Task<TagsDto> GetTags(IList<int> includeOnly)
        {
            var included = dtos.Where(it => includeOnly.Contains(it.Id)).ToList();
            return Task.FromResult(new TagsDto { Items = included, Total = included.Count });
        }

        public Task<TagsDto> GetTags(DateTimeOffset timestamp, IList<int> includeOnly) =>
            Task.FromResult(new TagsDto { Items = new List<TagDto>(), Total = 0 });

        public Task<IList<int>> GetIds() =>
            Task.FromResult((IList<int>) dtos.Select(it => it.Id).ToList());
    }
}