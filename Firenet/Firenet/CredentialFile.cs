using Newtonsoft.Json;

namespace Firenet
{
    class CredentialFile
    {
        [JsonProperty("project_id")]
        public string ProjectId { get; set; }
    }
}
