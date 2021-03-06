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
using System.Linq;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentHandling
{
    public class NewDataCenter : INewDataCenter
    {
        private readonly List<Exhibit> exhibitsToBeUpdated = new List<Exhibit>();
        private readonly List<Route> routesToBeUpdated = new List<Route>();

        public void AddRouteToBeUpdated(Route route)
        {
            routesToBeUpdated.Add(route);
        }

        public void AddExhibitToBeUpdated(Exhibit exhibit)
        {
            exhibitsToBeUpdated.Add(exhibit);
        }

        public bool IsNewDataAvailabe()
        {
            return exhibitsToBeUpdated.Any() || routesToBeUpdated.Any();
        }

        public Task UpdateData()
        {
            //TODO Use fetcher for downloading routes and exhibits details data
            return Task.CompletedTask;
        }
    }
}