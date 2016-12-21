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

namespace de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources {
    /// <summary>
    /// Interface for actions that are triggered when an InteractiveSource 
    /// is triggered (clicked).
    /// </summary>
    public interface IInteractiveSourceAction {

        /// <summary>
        /// Displays the provided source.
        /// </summary>
        /// <param name="src">Source to display.</param>
        void Display (Source src);

    }
}