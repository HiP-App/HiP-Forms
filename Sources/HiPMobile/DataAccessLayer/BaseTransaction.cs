﻿// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
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

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer
{
    public abstract class BaseTransaction
    {
        /// <summary>
        /// Gets an instance of <see cref="ITransactionDataAccess"/> that tracks changes of queried entities.
        /// </summary>
        public abstract ITransactionDataAccess DataAccess { get; }

        /// <summary>
        /// Commits changes done in the context of this transaction.
        /// </summary>
        public abstract void Commit();

        /// <summary>
        /// Reverts changes done to database objects made in the context of this transaction.
        /// </summary>
        public abstract void Rollback();

    }
}