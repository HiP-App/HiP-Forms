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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers.Threading
{
    public class REAsyncLock
    {
        private AsyncLocal<SemaphoreSlim> currentSemaphore =
            new AsyncLocal<SemaphoreSlim>() { Value = new SemaphoreSlim(1) };

        public async  Task<T> DoWithLock<T>(Func<Task<T>> body)
        {
            SemaphoreSlim currentSem = currentSemaphore.Value;
            await currentSem.WaitAsync();
            var nextSem = new SemaphoreSlim(1);
            currentSemaphore.Value = nextSem;
            try
            {
                return await body();
            }
            finally
            {
                Debug.Assert(nextSem == currentSemaphore.Value);
                await nextSem.WaitAsync();
                currentSemaphore.Value = currentSem;
                currentSem.Release();
            }
        }
    }
}