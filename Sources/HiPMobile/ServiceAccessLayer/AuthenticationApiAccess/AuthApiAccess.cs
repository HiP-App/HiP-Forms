using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthApiDto;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.Exceptions;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthenticationApiAccess
{
    public class AuthApiAccess : IAuthApiAccess
    {
        private readonly IContentApiClient clientApiClient;

        public AuthApiAccess(IContentApiClient clientApiClient)
        {
            this.clientApiClient = clientApiClient;
        }

        public async Task<Token> Login(string email, string password)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                Constants.MobileGrantType,
                Constants.MobileAudience,
                Constants.MobileScope,
                Constants.MobileClientId,
                Constants.MobileClientSecret,
                //new KeyValuePair<string, string>("email", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("username", email)  // server is taking variable username as email.

            });

            var result = await clientApiClient.PostRequestFormBased(ServerEndpoints.LoginUrl, content, prependBasePath: false);

            string jsonPayload = await result.Content.ReadAsStringAsync();

            Token token = JsonConvert.DeserializeObject<Token>(jsonPayload);

            if (result.StatusCode == HttpStatusCode.BadRequest || result.StatusCode == HttpStatusCode.NotFound || result.StatusCode == HttpStatusCode.Forbidden)
            {
                //throw new InvalidUserNamePassword();
                throw new InvalidEmailPassword();
            }

            if (result.StatusCode == HttpStatusCode.GatewayTimeout)
            {
                throw new TimeoutException("Request timed out");
            }

            return token;
        }

        public async Task<bool> Register(string username, string password, string firstName, string lastName, string email)
        {
            var content = "{" +
                          "\"email\": \"" + email + "\", " +
                          "\"usernname\": \"" + username + "\", " +
                          "\"password\": \"" + password + "\"," +
                          "\"firstName\": \"" + firstName + "\"," +
                          "\"lastName\": \"" + lastName +  "\"" +
                          "}";

            var result = await clientApiClient.PostRequestBody(ServerEndpoints.RegisterUrl, content, false);
          

            if (result.StatusCode == HttpStatusCode.Created)
            {
                return true;
            }
            else
            {
                var resultContent = await result.Content.ReadAsStringAsync();
                Debug.WriteLine(resultContent);
            }

            return false;
        }

        public async Task<bool> ForgotPassword(string email)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                Constants.BasicClientId,
                Constants.Connection,
                new KeyValuePair<string, string>("email", email),
            });

            var result = await clientApiClient.PostRequestFormBased(ServerEndpoints.ForgotPasswordUrl, content, prependBasePath: false);

            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}