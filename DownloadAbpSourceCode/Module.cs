using System;
using Newtonsoft.Json;

namespace DownloadSource
{
    public class Module
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        [JsonProperty("documentUrl")]
        public Uri DocumentUrl { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("shortDescription")]
        public string ShortDescription { get; set; }

        [JsonProperty("isPro")]
        public bool IsPro { get; set; }

        [JsonProperty("efCoreSupport")]
        public bool EfCoreSupport { get; set; }

        [JsonProperty("mongoDBSupport")]
        public bool MongoDbSupport { get; set; }

        [JsonProperty("angularUi")]
        public bool AngularUi { get; set; }

        [JsonProperty("mvcUi")]
        public bool MvcUi { get; set; }
    }
}