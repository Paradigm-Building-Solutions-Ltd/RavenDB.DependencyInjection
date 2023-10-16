using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Raven.DependencyInjection
{
    /// <summary>
    /// The configurations for <see cref="RavenOptions"/>.
    /// </summary>
    public class RavenOptionsSetup : IConfigureOptions<RavenOptions>, IPostConfigureOptions<RavenOptions>
    {
        private readonly IConfiguration _configuration;
        private RavenOptions? _options;

        /// <summary>
        /// The constructor for <see cref="RavenOptionsSetup"/>.
        /// </summary>
        /// <param name="configuration"></param>
        public RavenOptionsSetup(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// The default configuration if needed.
        /// </summary>
        /// <param name="options"></param>
        public void Configure(RavenOptions options)
        {
            if (options.Settings == null)
            {
                var settings = new RavenSettings();
                _configuration.Bind(options.SectionName, settings);

                options.Settings = settings;
            }

            if (options.GetConfiguration == null)
            {
                options.GetConfiguration = _configuration;
            }
        }

        /// <summary>
        /// Post configuration for <see cref="RavenOptions"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        public void PostConfigure(string name, RavenOptions options)
        {
            _options = options;

            if (options.Certificate == null)
            {
                options.Certificate = GetCertificateFromFileSystem();
                _options.Certificate = options.Certificate;
            }

            if (options.GetDocumentStore == null)
            {
                options.GetDocumentStore = GetDocumentStore;
            }
        }

        private IDocumentStore GetDocumentStore(Action<IDocumentStore>? configureDbStore)
        {
            if (string.IsNullOrEmpty(_options?.Settings?.DatabaseName))
            {
                throw new InvalidOperationException("You haven't configured a DatabaseName. Ensure your appsettings.json contains a RavenSettings section.");
            }
            if (_options.Settings.Urls == null || _options.Settings.Urls.Length == 0)
            {
                throw new InvalidOperationException("You haven't configured your Raven database URLs. Ensure your appsettings.json contains a RavenSettings section.");
            }

            var documentStore = new DocumentStore
            {
                Urls = _options.Settings.Urls,
                Database = _options.Settings.DatabaseName
            };

            if (_options.Certificate != null)
            {
                documentStore.Certificate = _options.Certificate;
            }

            configureDbStore?.Invoke(documentStore);

            documentStore.Initialize();

            return documentStore;
        }

        private X509Certificate2? GetCertificateFromFileSystem()
        {
            var certFilePath = _options?.Settings?.CertFilePath;

            if (!string.IsNullOrEmpty(certFilePath))
            {
                if (!File.Exists(certFilePath))
                {
                    throw new InvalidOperationException($"The Raven certificate file, {certFilePath} is missing.");
                }

                return new X509Certificate2(certFilePath, _options?.Settings?.CertPassword);
            }

            return null;
        }
    }
}
