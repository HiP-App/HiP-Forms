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

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.InteractiveSources
{
    /// <summary>
    /// Implements IInteractiveSourceSubstitute by returning a consecutive number enclosed in brackets.
    /// </summary>
    public class ConsecutiveNumberInteractiveSourceSubstitute : IInteractiveSourceSubstitute
    {
        private int number;

        /// <summary>
        /// Sets the starting number.
        /// </summary>
        /// <param name="start">Starting number.</param>
        public ConsecutiveNumberInteractiveSourceSubstitute(int start)
        {
            number = start;
        }

        public string NextSubstitute()
        {
            return $"[{number++}]";
        }
    }
}