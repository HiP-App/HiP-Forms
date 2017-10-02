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
    /// Implements IInteractiveSourceSubstitute by returning a constant substitute text.
    /// </summary>
    public class ConstantInteractiveSourceSubstitute : IInteractiveSourceSubstitute
    {
        private readonly string substitute;

        /// <summary>
        /// Sets the substitute text.
        /// </summary>
        /// <param name="sub">Substitute text that will be returned on every call of NextSubstitute().</param>
        public ConstantInteractiveSourceSubstitute(string sub)
        {
            substitute = sub;
        }

        public string NextSubstitute()
        {
            return substitute;
        }
    }
}