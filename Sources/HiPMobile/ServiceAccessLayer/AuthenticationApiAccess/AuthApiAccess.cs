using System;
using System.Collections.Generic;
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

        public async Task<Token> Login(string username, string password)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                Constants.MobileGrantType,
                Constants.MobileAudience,
                Constants.MobileScope,
                Constants.MobileClientId,
                Constants.MobileClientSecret,
                new KeyValuePair<string, string> ("username", username),
                new KeyValuePair<string, string> ("password", password)
            });

            var result = await clientApiClient.PostRequestFormBased(ServerEndpoints.LoginUrl, content);

            string jsonPayload = await result.Content.ReadAsStringAsync();

            Token token = JsonConvert.DeserializeObject<Token>(jsonPayload);

            if (result.StatusCode == HttpStatusCode.BadRequest || result.StatusCode == HttpStatusCode.NotFound || result.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new InvalidUserNamePassword();
            }

            if (result.StatusCode == HttpStatusCode.GatewayTimeout)
            {
                throw new TimeoutException("Request timed out");
            }

            return token;
        }

        public async Task<bool> Register(string username, string password)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                Constants.Connection,
                Constants.BasicClientId,
                new KeyValuePair<string, string> ("email", username),
                new KeyValuePair<string, string> ("password", password) 
            });

            var result = await clientApiClient.PostRequestFormBased(ServerEndpoints.RegisterUrl, content);

            string jsonPayload = await result.Content.ReadAsStringAsync();

            if (result.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public async Task<string> ForgotPassword(string username)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string> (Constants.UserName, username)
            });
            var result = await clientApiClient.PostRequestFormBased(ServerEndpoints.ForgotPasswordUrl, content);
            return await result.Content.ReadAsStringAsync();
        }

    }
}
