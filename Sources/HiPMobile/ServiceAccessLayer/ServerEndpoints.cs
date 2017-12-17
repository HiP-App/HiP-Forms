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

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer
{
    public static class ServerEndpoints
    {
        private const string BaseUrl = "https://hip.eu.auth0.com/";

        public const string LoginUrl = BaseUrl + "oauth/token";

        public const string RegisterUrl = BaseUrl + "dbconnections/signup";
        public const string ForgotPasswordUrl = BaseUrl + "dbconnections/change_password";

        /// <summary>
        /// Urlpath for the docker container running the HiP-DataStore instance
        /// </summary>
        public const string DatastoreApiPath = "https://docker-hip.cs.uni-paderborn.de/develop/datastore/api";
        
        public const string AchievementsApiPath = "https://docker-hip.cs.uni-paderborn.de/public/achievements/api";
        
        // TODO !!! DO NOT MERGE with develop API !!!
        public const string FeatureTogglesApiPath = "https://docker-hip.cs.uni-paderborn.de/develop/feature-toggle/api";

        public const string ThumbnailApiPath = "https://docker-hip.cs.upb.de/public/thumbnailservice/api/Thumbnails";
        
        public const string DataStoreTokenUrl = "https://hip.eu.auth0.com/oauth/token";
    }
}