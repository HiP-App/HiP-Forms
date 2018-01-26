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
    public class MockRoutesApiAccess : IRoutesApiAccess
    {
        private readonly List<RouteDto> dtos = new List<RouteDto>
        {
            new RouteDto
            {
                Audio = 0,
                Description = "Description",
                Distance = 10,
                Duration = 10,
                Exhibits = new List<int> { 0, 1 },
                Id = 0,
                Image = 1,
                Status = "PUBLISHED",
                Tags = new List<int>(),
                Timestamp = DateTimeOffset.Now,
                Title = "Title"
            }
        };

        public Task<RoutesDto> GetRoutes() =>
            Task.FromResult(new RoutesDto { Items = dtos, Total = dtos.Count });

        public Task<RoutesDto> GetRoutes(DateTimeOffset timestamp) =>
            Task.FromResult(new RoutesDto { Items = new List<RouteDto>(), Total = 0 });

        public Task<RoutesDto> GetRoutes(IList<int> includeOnly)
        {
            var included = dtos.Where(it => includeOnly.Contains(it.Id)).ToList();
            return Task.FromResult(new RoutesDto { Items = included, Total = included.Count });
        }

        public Task<RoutesDto> GetRoutes(DateTimeOffset timestamp, IList<int> includeOnly) =>
            Task.FromResult(new RoutesDto { Items = dtos, Total = dtos.Count });

        public Task<IList<int>> GetIds() => Task.FromResult((IList<int>) dtos.Select(it => it.Id).ToList());
    }
}