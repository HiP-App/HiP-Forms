using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthApiAccess.AuthApiDto
    {

    public class Token
        {
        [JsonProperty (PropertyName = "access_token")]
        public string AccessToken { get; set; }
        [JsonProperty (PropertyName = "expires_in")]
        public int Expiry { get; set; }
        [JsonProperty (PropertyName = "token_type")]
        public string TokenType { get; set; }
        }

    }
