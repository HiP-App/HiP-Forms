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

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.Common
{
    public interface IDbChangedHandler
    {
        /// <summary>
        /// Adds an observer
        /// </summary>
        /// /// <param name="observer">The observer to be added.</param>
        void AddObserver(IDbChangedObserver observer);

        /// <summary>
        /// Removes an observer
        /// </summary>
        /// /// <param name="observer">The observer to be removed.</param>
        void RemoveObserver(IDbChangedObserver observer);

        /// <summary>
        /// Notifies all observers to handle
        /// </summary>
        void NotifyAll();
    }

    public interface IDbChangedObserver
    {
        /// <summary>
        /// Called when the database changed.
        /// </summary>
        void DbChanged();
    }

    public class DbChangedHandler : IDbChangedHandler
    {
        private readonly List<IDbChangedObserver> observers;

        public DbChangedHandler()
        {
            observers = new List<IDbChangedObserver>();
        }

        public void AddObserver(IDbChangedObserver observer)
        {
            observers.Add(observer);
        }

        public void RemoveObserver(IDbChangedObserver observer)
        {
            observers.Remove(observer);
        }

        public void NotifyAll()
        {
            foreach (var observer in observers)
            {
                observer.DbChanged();
            }
        }
    }
}