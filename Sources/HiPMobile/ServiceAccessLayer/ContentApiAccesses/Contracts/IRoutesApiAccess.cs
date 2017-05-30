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
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts {
    /// <summary>
    /// Providing access to the routes rest api
    /// </summary>
    public interface IRoutesApiAccess
    {
        /// <summary>
        /// Get all routes
        /// </summary>
        /// <returns></returns>
        Task<RoutesDto> GetRoutes ();

        /// <summary>
        /// Get all routes after the specified timestamp
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        Task<RoutesDto> GetRoutes(DateTimeOffset timestamp);

        /// <summary>
        /// Get all routes for the given ids
        /// </summary>
        /// <param name="includeOnly"></param>
        /// <returns></returns>
        Task<RoutesDto> GetRoutes(IList<int> includeOnly);

        /// <summary>
        /// Get all routes for the given ids after the specified timestamp
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="includeOnly"></param>
        /// <returns></returns>
        Task<RoutesDto> GetRoutes(DateTimeOffset timestamp, IList<int> includeOnly);

        /// <summary>
        /// Get the ids of all existing routes
        /// </summary>
        /// <returns></returns>
        Task<IList<int>> GetIds();
    }
}