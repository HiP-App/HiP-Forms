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

using NUnit.Framework;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers
{
    public static class Debug2
    {
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
        /// <summary>
        /// Unlike Debug.Assert(...), this method actually throws an Exception if
        /// the condition is not met.
        /// </summary>
        public static void Assert(bool condition, string message = "Assertion failed!")
        {
            if (!condition)
            {
                throw new AssertionException(message);
            }
        }
    }
}