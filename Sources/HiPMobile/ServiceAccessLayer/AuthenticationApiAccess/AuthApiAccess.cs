using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net.Http;
using Newtonsoft.Json;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthApiDto;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.Exceptions;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthenticationApiAccess {
    public class AuthApiAccess: IAuthApiAccess {

        private readonly IContentApiClient clientApiClient;

        public AuthApiAccess (IContentApiClient clientApiClient)
        {
        this.clientApiClient = new ContentApiClient ();
        }
        public async Task<Token> GetToken (string userName, string password)
            {
            FormUrlEncodedContent content = new FormUrlEncodedContent (new[]
            {
                Constants.ClientId, Constants.ClientSecret, Constants.GrantType, Constants.Scope,
                new KeyValuePair<string, string> (Constants.UserName, userName),
                new KeyValuePair<string, string> (Constants.Password, password)
            });

            var result = await clientApiClient.PostRequestFormBased (ServerEndpoints.TokenUrl, content);
            string jsonPayload = await result.Content.ReadAsStringAsync ();
            Token token = JsonConvert.DeserializeObject<Token> (jsonPayload);

            if (result.StatusCode == HttpStatusCode.BadRequest || result.StatusCode == HttpStatusCode.NotFound)
                {
                throw  new InvalidUserNamePassword();
                }
            
            if (result.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                throw new TimeoutException ("Request timed out");
                }

            return token;
            }
        public async Task<bool> Register (string userName, string password, string confirmPassword)
            {
            FormUrlEncodedContent content = new FormUrlEncodedContent (new[]
            {
            new KeyValuePair<string, string> (Constants.UserName, userName),
            new KeyValuePair<string, string> (Constants.Password, password),
            new KeyValuePair<string, string> (Constants.ConfirmPassword, confirmPassword)
            });
            IContentApiClient client = new ContentApiClient ();
            var result = await client.PostRequestFormBased (ServerEndpoints.TokenUrl, content);

            if (result.StatusCode == HttpStatusCode.OK)
                {
                return true;
                }
            return false;
            }

        public async Task<string> ForgotPassword (string userName)
            {
            FormUrlEncodedContent content = new FormUrlEncodedContent (new[]
            {
            new KeyValuePair<string, string> (Constants.UserName, userName)
            });
            var result = await clientApiClient.PostRequestFormBased (ServerEndpoints.ForgotPasswordUrl, content);
            return await result.Content.ReadAsStringAsync ();
            }

        }
    }
