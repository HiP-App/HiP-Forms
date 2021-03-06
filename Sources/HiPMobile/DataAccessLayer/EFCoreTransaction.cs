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
using Microsoft.EntityFrameworkCore.Storage;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer
{
    // ReSharper disable once InconsistentNaming
    internal class EFCoreTransaction : BaseTransaction
    {
        private readonly AppDatabaseContext db;

        private bool rolledBack = false;
        private bool isComplete = false;
        private readonly IDbContextTransaction transaction;
        private readonly ITransactionDataAccess dataAccess;

        public override ITransactionDataAccess DataAccess
        {
            get
            {
                if (isComplete) throw new InvalidOperationException("This transaction has already been completed!");
                return dataAccess;
            }
        }

        public DbContextDebugView DebugView => new DbContextDebugView(db);

        public EFCoreTransaction(AppDatabaseContext db, bool useNativeTransaction)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            dataAccess = new EFCoreDataAccess(db);
            if (useNativeTransaction)
            {
                transaction = db.Database.BeginTransaction();
            }
        }

        public override void Commit()
        {
            if (rolledBack) return;

            db.SaveChangesAndDetach();
            transaction?.Commit();
            db.Dispose();
            isComplete = true;
        }

        public override void Rollback()
        {
            rolledBack = true;
            transaction?.Rollback();
            db.Dispose();
            isComplete = true;
        }
    }
}