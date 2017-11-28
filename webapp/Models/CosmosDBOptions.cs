using System;
using Microsoft.Extensions.Configuration;

namespace cosmoschat.Models
{
    internal class CosmosDBOptions
    {
        public CosmosDBOptions(IConfigurationSection configurationSection)
        {
            if (configurationSection == null)
            {
                throw new ArgumentNullException("configurationSection", "CosmosDB section missing in appsettings.json");
            }

            string configurationEndpoint = configurationSection["Endpoint"];
            string configurationKey = configurationSection["Key"];

            if (string.IsNullOrEmpty(configurationEndpoint))
            {
                throw new ArgumentNullException("configurationEndpoint", "CosmosDB configuration in appsettings.json missing a valid Uri in Endpoint attribute.");
            }

            if (string.IsNullOrEmpty(configurationKey))
            {
                throw new ArgumentNullException("configurationKey", "CosmosDB configuration in appsettings.json missing Key attribute.");
            }

            if (Uri.TryCreate(configurationEndpoint, UriKind.Absolute, out Uri _endpoint)){
                Endpoint = _endpoint;
            }

            if (Endpoint == null)
            {
                throw new ArgumentNullException("configurationEndpoint", "CosmosDB configuration in appsettings.json missing a valid Uri in Endpoint attribute.");
            }

            Key = configurationKey;
        }

        /// <summary>
        /// Cosmos DB account endpoint
        /// </summary>
        public Uri Endpoint { get; private set; }

        /// <summary>
        /// Cosmos DB account key
        /// </summary>
        public string Key { get; private set; }
    }
}
