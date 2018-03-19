using System;
using Newtonsoft.Json;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos
{
    public class QuestionDto
    {
        [JsonProperty("status")]
        public string Status { get; private set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; private set; }

        [JsonProperty("text")]
        public string Text { get; private set; }

        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("exhibitId")]
        public int ExhibitId { get; private set; }

        [JsonProperty("options")]
        public string[] Options { get; private set; }

        [JsonProperty("image")]
        public int Image { get; private set; }

    }
}