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
using System.Linq;
using System.Text;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer
{
    class EFCoreTransaction : BaseTransaction
    {
        private readonly AppDatabaseContext _db;

        public override ITransactionDataAccess DataAccess { get; }

        public object DebugView => new DbContextDebugView { Db = _db };

        public EFCoreTransaction(AppDatabaseContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            DataAccess = new EFCoreDataAccess(db);
        }

        public override void Commit()
        {
            ThrowIfDisposed();
            DumpTrackedEntries();
            _db.SaveChangesAndDetach();
        }

        public override void Rollback()
        {
            ThrowIfDisposed();
        }

        private void DumpTrackedEntries()
        {
            var sb = new StringBuilder($"TRACKED ENTITIES:\r\n");
            void Print(string s) => sb.AppendLine("    " + s);

            var entries = _db.ChangeTracker.Entries().OrderBy(e => e.Metadata.Name).ToList();

            if (entries.Count > 0)
            {
                foreach (var e in entries)
                    Print($"{e.Metadata.Name} \"{e.Entity}\" ({e.State})");
            }
            else
            {
                Print("None");
            }

            Console.WriteLine(sb);
        }
    }
}