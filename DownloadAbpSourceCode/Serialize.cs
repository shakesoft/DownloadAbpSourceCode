using Newtonsoft.Json;

namespace DownloadSource
{
    public static class Serialize
    {
        public static string ToJson(this SourceCodeDownloadInputDto self) => JsonConvert.SerializeObject(self);
    }
}