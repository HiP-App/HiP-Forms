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

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthenticationApiAccess{
    public class Constants {

        public const string UserName = "username";
        public const string Password = "password";
        public const string ConfirmPassword = "confirmpassword";

        public const string DataStoreClientId = "C8BJAtkw3gkkXiPWUrg7v9D3PbZdRCij";
        public const string DataStoreClientSecret = "q5YjSYsBdedu0stlH3ogY_d_IYg9zatMn74WsRvJ2dBcqBasKCbuYE7vbaYYIyFF";
        public const string Audience = "https://hip.cs.upb.de/API";
        public const string DataStoreGrantType = "client_credentials";
        //public const string GrantType = "password";
        //public const string Scope = "openid";

        public static KeyValuePair<string, string> ClientId => new KeyValuePair<string, string> ("client_id", "HiP-CmsAngularApp");
        public static KeyValuePair<string, string> ClientSecret => new KeyValuePair<string, string> ("client_secret", "cms-secret");

        public static KeyValuePair<string, string> GrantType => new KeyValuePair<string, string> ("grant_type", "password");

        public static KeyValuePair<string, string> Scope => new KeyValuePair<string, string> ("scope", "openid");
        }
}