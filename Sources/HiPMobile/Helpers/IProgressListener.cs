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
namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers {

    public delegate void UpdateDelegate (double newProgress, double maxProgress);

    /// <summary>
    /// Progress listener interface.
    /// </summary>
    public interface IProgressListener {

        /// <summary>
        /// Called when the progress changed.
        /// </summary>
        /// <param name="newProgress">The updated, current progress.</param>
        /// <param name="maxProgress">The maximum progress.</param>
        void UpdateProgress (double newProgress, double maxProgress);

    }
}