using Newtonsoft.Json;
using System;
using System.IO;

namespace Firenet
{
    public class FireOption
    {
        private string _projectId;
        private string _jsonCredentialsPath;
        private bool _isFromEnviromentVariable;

        public string ProjectId => _projectId;
        public string JsonCredentialsPath => _jsonCredentialsPath;
        public bool IsFromEnviromentVariable => _isFromEnviromentVariable;

        /// <summary>
        /// Set the <paramref name="jsonCredentialsPath"/>.
        /// </summary>
        /// <param name="jsonCredentialsPath"></param>
        /// <returns></returns>
        public FireOption SetJsonCredentialsPath (string jsonCredentialsPath)
        {
            if (string.IsNullOrEmpty(jsonCredentialsPath))
                throw new ArgumentNullException(nameof(JsonCredentialsPath));
            if (!File.Exists(jsonCredentialsPath))
                throw new FileNotFoundException(jsonCredentialsPath);
            _jsonCredentialsPath = jsonCredentialsPath;
            _projectId = JsonConvert.DeserializeObject<CredentialFile>(File.ReadAllText(_jsonCredentialsPath)).ProjectId;
            if (string.IsNullOrEmpty(_projectId))
                throw new ArgumentNullException(nameof(ProjectId));
            _isFromEnviromentVariable = false;
            return this;
        }

        /// <summary>
        /// Get from the environment variable 'GOOGLE_APPLICATION_CREDENTIALS' which should previusly configured.
        /// </summary>
        /// <returns></returns>
        public FireOption GetFromGoogleEnvironmentVariable()
        {
            _jsonCredentialsPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            if (string.IsNullOrEmpty(_jsonCredentialsPath))
                throw new ArgumentNullException(nameof(JsonCredentialsPath));
            if (!File.Exists(_jsonCredentialsPath))
                throw new FileNotFoundException(JsonCredentialsPath);
            _projectId = JsonConvert.DeserializeObject<CredentialFile>(File.ReadAllText(_jsonCredentialsPath)).ProjectId;
            if (string.IsNullOrEmpty(_projectId))
                throw new ArgumentNullException(nameof(ProjectId));
            _isFromEnviromentVariable = true;
            return this;
        }
    }
}
