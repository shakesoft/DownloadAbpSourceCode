#region Header

// + ++- --+ + -+ + +-+-+- ++++
// 
// Radio Cabs of Wollongong Co-Operative Society Ltd
// 
// Author: Chuyang Hu
// 
// File Name: SourceCodeDownloadInputDto.cs
// 
// Modification Time: 2:17
// Modification Date: 28/12/2020
// 
// Create Time: 2:17
// Create Date: 28/12/2020
// 
// --+ -+ --- + +-+-+- -+-+

#endregion

using Newtonsoft.Json;

namespace DownloadSource
{
    public class SourceCodeDownloadInputDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("templateSource")]
        public object TemplateSource { get; set; }

        [JsonProperty("includePreReleases")]
        public bool IncludePreReleases { get; set; }

        public static SourceCodeDownloadInputDto FromJson(string json) => JsonConvert.DeserializeObject<SourceCodeDownloadInputDto>(json);
    }
}