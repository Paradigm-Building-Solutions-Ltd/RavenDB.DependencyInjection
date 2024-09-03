namespace Raven.DependencyInjection
{
    /// <summary>
    /// Contains settings for RavenDB, such as the URL to the database.
    /// </summary>
    public class RavenSettings
    {
        /// <summary>
        /// The URLs where the database resides.
        /// </summary>
        public string[] Urls { get; set; } = new string[] { };

        /// <summary>
        /// The name of the database.
        /// </summary>
        public string DatabaseName { get; set; } = string.Empty;

        /// <summary>
        /// The file path to the PFX certificate to use to connect to Raven. This should be an absolute path. If null or empty, no certificate will be used.
        /// </summary>
        public string? CertFilePath { get; set; }

        /// <summary>
        /// The password to use for the certificate.
        /// </summary>
        public string? CertPassword { get; set; }
    }
}