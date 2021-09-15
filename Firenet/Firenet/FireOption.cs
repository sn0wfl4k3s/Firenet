using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Firenet
{
    public class FireOption
    {
        private string _jsonCredentialsPath;
        private Action<string> _warningLogger;
        private ConverterRegistry _converters;

        internal string ProjectId => GetProjectId();
        internal string JsonCredentialsPath => _jsonCredentialsPath;
        internal Action<string> WarningLogger => _warningLogger;
        internal ConverterRegistry Converters => _converters;

        public FireOption()
        {
            _converters = new ConverterRegistry
            {
                new DefaultGuidConverter(),
                new DefaultDatetimeConverter(),
                new DefaultNullableDatetimeConverter(),
            };
        }


        /// <summary>
        /// Add a converter that implement IFirestoreConverter<T> where T is any type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="converter"></param>
        /// <returns></returns>
        public FireOption AddConverter<T>(IFirestoreConverter<T> converter) where T : unmanaged
        {
            _converters.Add(converter);
            return this;
        }

        /// <summary>
        /// Enable the logger warning using a action defined.
        /// </summary>
        /// <param name="logger">action for message warning from firestore.</param>
        /// <returns></returns>
        public FireOption EnableWarningLogger(Action<string> logger)
        {
            _warningLogger = logger;
            return this;
        }

        /// <summary>
        /// Enable the logger warning using the writeline method from System.Diagnostics.Debug namespace.
        /// </summary>
        /// <returns></returns>
        public FireOption EnableWarningLogger()
        {
            _warningLogger = message => System.Diagnostics.Debug.WriteLine(message);
            return this;
        }


        /// <summary>
        /// Set the path of credentials json file.
        /// </summary>
        /// <param name="jsonCredentialsPath"></param>
        /// <returns></returns>
        public FireOption SetJsonCredentialspath(string jsonCredentialsPath)
        {
            if (string.IsNullOrEmpty(jsonCredentialsPath))
                throw new ArgumentNullException(nameof(jsonCredentialsPath));
            if (!File.Exists(jsonCredentialsPath))
                throw new FileNotFoundException("Credentials file not found.");
            _jsonCredentialsPath = jsonCredentialsPath;
            return this;
        }

        /// <summary>
        /// Get the credentials from the environment variable 'GOOGLE_APPLICATION_CREDENTIALS' which should previusly configured.
        /// </summary>
        /// <returns></returns>
        public FireOption UseGoogleEnvironmentVariable()
        {
            _jsonCredentialsPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            if (string.IsNullOrEmpty(_jsonCredentialsPath))
                throw new ArgumentNullException("Has no 'GOOGLE_APPLICATION_CREDENTIALS' environment variable configured.");
            if (!File.Exists(_jsonCredentialsPath))
                throw new FileNotFoundException("Credentials file not found.");
            return this;
        }

        private string GetProjectId ()
            => JsonConvert.DeserializeObject<CredentialFile>(File.ReadAllText(_jsonCredentialsPath)).ProjectId ??
                throw new ArgumentNullException(nameof(ProjectId));
    }
}
