// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using JetBrains.Annotations;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.DesignTime.Services
{
    public class DesignModeDataAccess : IDataAccess
    {
        public string DatabasePath => "/path/to/database.db";

        public void CreateDatabase(int version) { }

        public T CreateObject<T>() where T : IIdentifiable, new() => new T();

        public T CreateObject<T>([NotNull]string id, bool updateCurrent = false) where T : IIdentifiable, new() => new T { Id = id };

        public void DeleteDatabase() { }

        public bool DeleteItem<T>(string id) where T : IIdentifiable => false;

        public int GetVersion() => 0;

        public T InTransaction<T>(IEnumerable<object> itemsToTrack, Func<BaseTransaction, T> func) =>
            default(T);

        T IReadOnlyDataAccess.GetItem<T>(string id, params string[] pathsToInclude) => null;

        IReadOnlyList<T> IReadOnlyDataAccess.GetItems<T>(params string[] pathsToInclude) => new List<T>();
    }
}
