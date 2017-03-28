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


namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.InteractiveSources {
    /// <summary>
    /// Implements IInteractiveSourceSubstitute by returning a  constant text followed by a consecutive number combined enclosed in brackets.
    /// </summary>
    public class ConsecutiveNumberAndConstantInteractiveSourceSubstitute : IInteractiveSourceSubstitute {

        private int number;

        private readonly string constantText;

        /// <summary>
        /// Sets the starting number and the constant text.
        /// </summary>
        /// <param name="start">Starting number.</param>
        /// <param name="constantText">Constant text at the beginning of the substitute</param>
        public ConsecutiveNumberAndConstantInteractiveSourceSubstitute(int start, string constantText)
        {
            number = start;
            this.constantText = constantText;
        }

        public string NextSubstitute ()
        {
            return $"[{constantText} {number++}]";
        }

    }
}