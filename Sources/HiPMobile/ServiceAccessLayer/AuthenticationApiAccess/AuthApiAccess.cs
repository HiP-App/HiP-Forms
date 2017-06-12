﻿using System;
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

            var result = await clientApiClient.PostRequestFormBased (Constants.TokenUrl, content);
            
            if (result.StatusCode == HttpStatusCode.BadRequest)
                {
                string payload = await result.Content.ReadAsStringAsync ();
                Error error = JsonConvert.DeserializeObject<Error> (payload);
                if (error.ErrorDescription == "invalid_username_or_password")
                    throw  new InvalidUserNamePassword();
                return null;
                }
            string jsonPayload = await result.Content.ReadAsStringAsync ();
            Token token = JsonConvert.DeserializeObject<Token> (jsonPayload);
            return token;
            }
        public async Task<string> Register (string userName, string password, string confirmPassword)
            {

            FormUrlEncodedContent content = new FormUrlEncodedContent (new[]
            {
            new KeyValuePair<string, string> (Constants.UserName, userName),
            new KeyValuePair<string, string> (Constants.Password, password),
            new KeyValuePair<string, string> (Constants.ConfirmPassword, confirmPassword)
            });
            IContentApiClient client = new ContentApiClient ();
            var result = await client.PostRequestFormBased (Constants.TokenUrl, content);
            return await result.Content.ReadAsStringAsync ();
            }

        public async Task<string> ForgotPassword (string userName)
            {
            FormUrlEncodedContent content = new FormUrlEncodedContent (new[]
            {
            new KeyValuePair<string, string> (Constants.UserName, userName),
            });
            var result = await clientApiClient.PostRequestFormBased (Constants.TokenUrl, content);
            return await result.Content.ReadAsStringAsync ();
            }

        }
    }
