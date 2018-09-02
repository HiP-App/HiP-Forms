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

using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthApiDto;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthenticationApiAccess
{
    public class MockAuthApiAccess: IAuthApiAccess
    {
        public Task<Token> Login(string email, string password) => Task.FromResult(new Token
        {
            AccessToken = "",
            IdToken = "",
            TokenType = ""
        });

        public Task<bool> Register(string username, string password, string firstName, string lastName, string email) => Task.FromResult(true);

        public Task<bool> ForgotPassword(string email) => Task.FromResult(true);

        public Task<CurrentUser> GetCurrentUser(string accessToken)
        {
            return Task.FromResult(new CurrentUser("id", "id@email.com", "username"));
        }
    }
}