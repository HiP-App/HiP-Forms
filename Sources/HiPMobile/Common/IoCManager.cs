// /*
//  * Copyright (C) 2016 History in Paderborn App - Universität Paderborn
//  *
//  * Licensed under the Apache License, Version 2.0 (the "License");
//  * you may not use this file except in compliance with the License.
//  * You may obtain a copy of the License at
//  *
//  *      http://www.apache.org/licenses/LICENSE-2.0
//  *
//  * Unless required by applicable law or agreed to in writing, software
//  * distributed under the License is distributed on an "AS IS" BASIS,
//  * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  * See the License for the specific language governing permissions and
//  * limitations under the License.
//  */


using Microsoft.Practices.Unity;

namespace de.upb.hip.mobile.pcl.Common {
    /// <summary>
    /// This class manages the access to the Inversion of Control container for the application.
    /// </summary>
    public static class IoCManager {

        // Singleton Pattern
        private static UnityContainer container;
        public static UnityContainer UnityContainer {
            get {
                if (container == null)
                {
                    UnityContainer = new UnityContainer ();
                }
                return container;
            }
            private set { container = value; }
        }
    }
}