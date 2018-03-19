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

using Microsoft.EntityFrameworkCore;
using System;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer
{
    // ReSharper disable once InconsistentNaming
    class EFCoreTransaction : BaseTransaction
    {
        private readonly AppDatabaseContext db;

        public override ITransactionDataAccess DataAccess { get; }

        public DbContextDebugView DebugView => new DbContextDebugView(db);

        public EFCoreTransaction(AppDatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            DataAccess = new EFCoreDataAccess(db);
        }

        public override void Commit()
        {
            ThrowIfDisposed();
            db.SaveChangesAndDetach();
        }

        public override void Rollback()
        {
            ThrowIfDisposed();
        }
    }
}