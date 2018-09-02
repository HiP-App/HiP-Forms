using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User
{
    public class CurrentUser
    {
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("email")]
        public string EMail { get; private set; }

        [JsonProperty("displayName")]
        public string Username { get; private set; }

        public CurrentUser([CanBeNull] string id, [CanBeNull] string email, [CanBeNull] string username)
        {
            Id = id;
            EMail = email;
            Username = username;
        }

    }

}
