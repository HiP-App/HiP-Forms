using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthApiDto
    {

    public class Error
        {
        [JsonProperty (PropertyName = "error")]
        public string ErrorMessage { get; set; }
        [JsonProperty (PropertyName = "error_description")]
        public string ErrorDescription { get; set; }
        }

    }
