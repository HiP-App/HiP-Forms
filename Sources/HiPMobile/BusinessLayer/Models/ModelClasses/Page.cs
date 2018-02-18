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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.JoinClasses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using System;
using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    public abstract partial class Page : IIdentifiable
    {
        public string Id { get; set; }

        public PageType PageType { get; set; }

        public Audio Audio { get; set; }

        public int IdForRestApi { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public ICollection<Page> AdditionalInformationPages => new JoinCollectionFacade<Page, JoinPagePage>(this, AdditionalInformationPagesRefs, JoinSide.B);

        public IList<JoinPagePage> AdditionalInformationPagesRefs { get; } = new List<JoinPagePage>();

        public Page()
        {
        }
    }
}

