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

using System.Collections.Generic;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts {
    public interface IPagesApiAccess {

        Task<PagesDto> GetPages();

        Task<PagesDto> GetPages(long timestamp);

        Task<PagesDto> GetPages(IList<int> includeOnly);

        Task<PagesDto> GetPages(long timestamp, IList<int> includeOnly);

        Task<PagesDto> GetPages(int exhibitId);

        Task<PagesDto> GetPages(int exhibitId, long timestamp);

        Task<PagesDto> GetPages(int exhibitId, IList<int> includeOnly);

        Task<PagesDto> GetPages(int exhibitId, long timestamp, IList<int> includeOnly);

    }
}