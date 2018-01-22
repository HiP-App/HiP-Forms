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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses
{
    public class MockExhibitsApiAccess : IExhibitsApiAccess
    {
        private readonly IList<ExhibitDto> dtos = new List<ExhibitDto>
        {
            new ExhibitDto
            {
                Description = "Description",
                Id = 0,
                Image = null, // TODO
                Latitude = 0.0, // TODO,
                Longitude = 0.0, // TODO
                Name = "Exhibit A",
                Pages = null, // TODO
                Status = null, // TODO
                Tags = new List<int>
                {
                    0
                },
                Timestamp = DateTimeOffset.Now,
                Used = true
            }
        };

        public Task<ExhibitsDto> GetExhibits() =>
            Task.FromResult(new ExhibitsDto { Items = dtos, Total = dtos.Count });

        public Task<ExhibitsDto> GetExhibits(DateTimeOffset timestamp) =>
            Task.FromResult(new ExhibitsDto { Items = new List<ExhibitDto>(), Total = 0 });

        public Task<ExhibitsDto> GetExhibits(IList<int> includeOnly)
        {
            var included = dtos.Where(it => includeOnly.Contains(it.Id)).ToList();
            return Task.FromResult(new ExhibitsDto { Items = included, Total = included.Count });
        }

        public Task<ExhibitsDto> GetExhibits(DateTimeOffset timestamp, IList<int> includeOnly) =>
            Task.FromResult(new ExhibitsDto { Items = new List<ExhibitDto>(), Total = 0 });

        public Task<IList<int>> GetIds() => Task.FromResult((IList<int>) new List<int> { 0 });
    }
}