// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers
{
    /// <summary>
    /// Provides access to database methods.
    /// </summary>
    public static class DbManager
    {
        /// <summary>
        /// Provides read-only database access. To modify the database in any way,
        /// use <see cref="BaseTransaction"/> instead.
        /// </summary>
        public static IReadOnlyDataAccess DataAccess { get; } = IoCManager.Resolve<IDataAccess>();

        /// <summary>
        /// Starts a new write transaction.
        /// </summary>
        /// <param name="itemsToTrack">
        ///     Entities that should be added to the change tracker.
        ///     See <see cref="IDataAccess.InTransactionAsync{T}"/> for an example scenario where this is needed.
        ///     Note that <see cref="BackupData.BackupImage"/> and <see cref="BackupData.BackupImageTag"/> are included by default
        ///     so they can be freely assigned to any other entity without being considered "new".
        /// </param>
        /// <param name="func">
        /// The function that should be executed in the scope of the transaction.
        /// </param>
        /// <returns>The instance returned by func.</returns>
        public static Task<T> InTransactionAsync<T>(IEnumerable<object> itemsToTrack, Func<BaseTransaction, Task<T>> func) =>
            IoCManager.Resolve<IDataAccess>().InTransactionAsync(itemsToTrack.Concat(new[] { BackupData.BackupImage, BackupData.BackupImageTag }), func);

        /// <summary>
        /// Starts a new write transaction.
        /// </summary>
        /// <param name="itemToTrack">
        /// Entitiy that should be added to the change tracker.
        /// See <see cref="IDataAccess.InTransactionAsync{T}"/> for an example scenario where this is needed.
        /// Note that <see cref="BackupData.BackupImage"/> and <see cref="BackupData.BackupImageTag"/> are included by default
        /// so they can be freely assigned to any other entity without being considered "new".
        /// </param>
        /// <param name="func">
        /// The function that should be executed in the scope of the transaction.
        /// </param>
        /// <returns>The instance returned by func.</returns>
        public static Task<T> InTransactionAsync<T>(object itemToTrack, Func<BaseTransaction, Task<T>> func) =>
            InTransactionAsync(new[] { itemToTrack }, func);

        /// <summary>
        /// Starts a new write transaction.
        /// </summary>
        /// <param name="func">
        /// The function that should be executed in the scope of the transaction.
        /// </param>
        /// <returns>The transaction object which can perform committing or rolling back.</returns>
        public static Task<T> InTransactionAsync<T>(Func<BaseTransaction, Task<T>> func) =>
            InTransactionAsync(Enumerable.Empty<object>(), func);

        /// <summary>
        /// Starts a new write transaction.
        /// </summary>
        /// <param name="itemsToTrack">
        ///     Entities that should be added to the change tracker.
        ///     See <see cref="IDataAccess.InTransactionAsync{T}"/> for an example scenario where this is needed.
        ///     Note that <see cref="BackupData.BackupImage"/> and <see cref="BackupData.BackupImageTag"/> are included by default
        ///     so they can be freely assigned to any other entity without being considered "new".
        /// </param>
        /// <param name="func">
        /// The function that should be executed in the scope of the transaction.
        /// </param>
        /// <returns>The transaction object which can perform committing or rolling back.</returns>
        public static Task InTransactionAsync(IEnumerable<object> itemsToTrack, Func<BaseTransaction, Task> func) =>
            IoCManager.Resolve<IDataAccess>().InTransactionAsync<object>(itemsToTrack.Concat(new[] { BackupData.BackupImage, BackupData.BackupImageTag }), async transaction =>
            {
                await func.Invoke(transaction);
                return Task.CompletedTask;
            });

        /// <summary>
        /// Starts a new write transaction.
        /// </summary>
        /// <param name="itemToTrack">
        /// Entitiy that should be added to the change tracker.
        /// See <see cref="IDataAccess.InTransactionAsync{T}"/> for an example scenario where this is needed.
        /// Note that <see cref="BackupData.BackupImage"/> and <see cref="BackupData.BackupImageTag"/> are included by default
        /// so they can be freely assigned to any other entity without being considered "new".
        /// </param>
        /// <param name="func">
        /// The function that should be executed in the scope of the transaction.
        /// </param>
        /// <returns>The transaction object which can perform committing or rolling back.</returns>
        public static Task InTransactionAsync(object itemToTrack, Func<BaseTransaction, Task> func) =>
            InTransactionAsync<object>(new[] { itemToTrack }, async transaction =>
            {
                await func.Invoke(transaction);
                return Task.CompletedTask;
            });

        /// <summary>
        /// Starts a new write transaction.
        /// </summary>
        /// <param name="func">
        /// The function that should be executed in the scope of the transaction.
        /// </param>
        /// <returns>The instance returned by func.</returns>
        public static Task InTransactionAsync(Func<BaseTransaction, Task> func) =>
            InTransactionAsync<object>(Enumerable.Empty<object>(), async transaction =>
            {
                await func.Invoke(transaction);
                return Task.CompletedTask;
            });

        /// <summary>
        /// Starts a new write transaction.
        /// </summary>
        /// <param name="itemsToTrack">
        ///     Entities that should be added to the change tracker.
        ///     See <see cref="IDataAccess.InTransactionAsync{T}"/> for an example scenario where this is needed.
        ///     Note that <see cref="BackupData.BackupImage"/> and <see cref="BackupData.BackupImageTag"/> are included by default
        ///     so they can be freely assigned to any other entity without being considered "new".
        /// </param>
        /// <param name="func">
        /// The function that should be executed in the scope of the transaction.
        /// </param>
        /// <returns>The instance returned by func.</returns>
        public static Task<T> InTransactionAsync<T>(IEnumerable<object> itemsToTrack, Func<BaseTransaction, T> func) =>
            InTransactionAsync(itemsToTrack, t => Task.FromResult(func(t)));

        /// <summary>
        /// Starts a new write transaction.
        /// </summary>
        /// <param name="itemToTrack">
        /// Entitiy that should be added to the change tracker.
        /// See <see cref="IDataAccess.InTransactionAsync{T}"/> for an example scenario where this is needed.
        /// Note that <see cref="BackupData.BackupImage"/> and <see cref="BackupData.BackupImageTag"/> are included by default
        /// so they can be freely assigned to any other entity without being considered "new".
        /// </param>
        /// <param name="func">
        /// The function that should be executed in the scope of the transaction.
        /// </param>
        /// <returns>The instance returned by func.</returns>
        public static Task<T> InTransactionAsync<T>(object itemToTrack, Func<BaseTransaction, T> func) =>
            InTransactionAsync(itemToTrack, t => Task.FromResult(func(t)));

        /// <summary>
        /// Starts a new write transaction.
        /// </summary>
        /// <param name="func">
        /// The function that should be executed in the scope of the transaction.
        /// </param>
        /// <returns>The transaction object which can perform committing or rolling back.</returns>
        public static Task<T> InTransactionAsync<T>(Func<BaseTransaction, T> func) =>
            InTransactionAsync(t => Task.FromResult(func(t)));

        /// <summary>
        /// Starts a new write transaction.
        /// </summary>
        /// <param name="itemsToTrack">
        ///     Entities that should be added to the change tracker.
        ///     See <see cref="IDataAccess.InTransactionAsync{T}"/> for an example scenario where this is needed.
        ///     Note that <see cref="BackupData.BackupImage"/> and <see cref="BackupData.BackupImageTag"/> are included by default
        ///     so they can be freely assigned to any other entity without being considered "new".
        /// </param>
        /// <param name="func">
        /// The function that should be executed in the scope of the transaction.
        /// </param>
        /// <returns>The transaction object which can perform committing or rolling back.</returns>
        public static Task InTransactionAsync(IEnumerable<object> itemsToTrack, Action<BaseTransaction> func) =>
            InTransactionAsync(itemsToTrack, t =>
            {
                func(t);
                return Task.CompletedTask;
            });

        /// <summary>
        /// Starts a new write transaction.
        /// </summary>
        /// <param name="itemToTrack">
        /// Entitiy that should be added to the change tracker.
        /// See <see cref="IDataAccess.InTransactionAsync{T}"/> for an example scenario where this is needed.
        /// Note that <see cref="BackupData.BackupImage"/> and <see cref="BackupData.BackupImageTag"/> are included by default
        /// so they can be freely assigned to any other entity without being considered "new".
        /// </param>
        /// <param name="func">
        /// The function that should be executed in the scope of the transaction.
        /// </param>
        /// <returns>The transaction object which can perform committing or rolling back.</returns>
        public static Task InTransactionAsync(object itemToTrack, Action<BaseTransaction> func) =>
            InTransactionAsync(itemToTrack, t =>
            {
                func(t);
                return Task.CompletedTask;
            });

        /// <summary>
        /// Starts a new write transaction.
        /// </summary>
        /// <param name="func">
        /// The function that should be executed in the scope of the transaction.
        /// </param>
        /// <returns>The instance returned by func.</returns>
        public static Task InTransactionAsync(Action<BaseTransaction> func) =>
            InTransactionAsync(t =>
            {
                func(t);
                return Task.CompletedTask;
            });

        /// <summary>
        /// Deletes the whole database and restarts the app
        /// </summary>
        public static void DeleteDatabase()
        {
            IoCManager.Resolve<IDataAccess>().DeleteDatabase();
            IoCManager.Resolve<IAppCloser>().RestartOrClose();
        }
    }
}