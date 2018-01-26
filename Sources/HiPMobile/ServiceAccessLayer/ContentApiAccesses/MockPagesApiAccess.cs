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
    public class MockPagesApiAccess : IPagesApiAccess
    {
        private readonly List<PageDto> dtos = new List<PageDto>
        {
            new PageDto
            {
                AdditionalInformationPages = new List<int> { 1 }, // TODO Create that page,
                Audio = null, // TODO
                Description = "Description",
                ExhibitId = 0,
                FontFamily = "DEFAULT",
                HideYearNumbers = false,
                Id = 0,
                Image = 1,
                Images = null, // TODO
                Status = "PUBLISHED",
                Text = "Pagetext",
                Timestamp = DateTimeOffset.Now,
                Title = "Title",
                Type = PageTypeDto.TextPage // TODO Other types
            }
        };

        public Task<PagesDto> GetPages() =>
            Task.FromResult(new PagesDto { Items = dtos, Total = dtos.Count });

        public Task<PagesDto> GetPages(DateTimeOffset timestamp)
            => Task.FromResult(new PagesDto { Items = new List<PageDto>(), Total = 0 });

        public Task<PagesDto> GetPages(IList<int> includeOnly)
        {
            var included = dtos.Where(it => includeOnly.Contains(it.Id)).ToList();
            return Task.FromResult(new PagesDto { Items = included, Total = included.Count });
        }

        public Task<PagesDto> GetPages(DateTimeOffset timestamp, IList<int> includeOnly)
            => Task.FromResult(new PagesDto { Items = new List<PageDto>(), Total = 0 });

        public Task<PagesDto> GetPages(int exhibitId)
        {
            var included = dtos.Where(it => it.ExhibitId == exhibitId).ToList();
            return Task.FromResult(new PagesDto { Items = included, Total = included.Count });
        }

        public Task<PagesDto> GetPages(int exhibitId, DateTimeOffset timestamp)
            => Task.FromResult(new PagesDto { Items = new List<PageDto>(), Total = 0 });

        public Task<PagesDto> GetPages(int exhibitId, IList<int> includeOnly)
        {
            var included = dtos.Where(it => it.ExhibitId == exhibitId && includeOnly.Contains(it.Id)).ToList();
            return Task.FromResult(new PagesDto { Items = included, Total = included.Count });
        }

        public Task<PagesDto> GetPages(int exhibitId, DateTimeOffset timestamp, IList<int> includeOnly)
            => Task.FromResult(new PagesDto { Items = new List<PageDto>(), Total = 0 });

        public Task<IList<int>> GetIds() => Task.FromResult((IList<int>) dtos.Select(it => it.Id).ToList());
    }
}