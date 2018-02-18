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

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    /// <remarks>
    /// For inheritance to be mapped correctly to a relational model, Entity Framework Core requires
    /// a base class (an interface is not sufficient). That's why we need this base type.
    /// </remarks>
    public abstract class AchievementBase : IAchievement
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ThumbnailUrl { get; set; }

        public string NextId { get; set; }

        public bool IsUnlocked { get; set; }

        public int Points { get; set; }
    }
}