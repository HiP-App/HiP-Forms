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

using System.Linq;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers {
    public class BaseDataFetcher {

        private readonly IExhibitsApiAccess exhibitsApiAccess = IoCManager.Resolve<IExhibitsApiAccess> ();

        public async void FetchBaseDataIntoDatabase ()
        {
            //There must be at most one exhibit set in the db at every moment
            var exhibitSet = ExhibitManager.GetExhibitSets ().SingleOrDefault();

            ExhibitsDto exhibits;
            if (exhibitSet != null)
            {
                exhibits = await exhibitsApiAccess.GetExhibits(exhibitSet.Timestamp);
            }
            else
            {
                exhibits = await exhibitsApiAccess.GetExhibits();
            }

            //TODO process further
        }

    }
}