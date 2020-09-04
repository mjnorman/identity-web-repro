using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace identityweb.Models
{
    public class VCAPApplication
    {
        [JsonPropertyName("application_name")]
        public string ApplicationName { get; set; }
        
        [JsonPropertyName("cf_api")]
        public string CfAPI { get; set; }
    }

    public class VCAPServiceCredentials
    {
        public string Host { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
    }

    public class RedisService
    {
        [JsonPropertyName("credentials")]
        public VCAPServiceCredentials Credentials { get; set; }

        [JsonPropertyName("instance_name")]
        public string InstanceName { get; set; }

    }


    public class SquidProxyService
    {
        [JsonPropertyName("credentials")]
        public SquidProxyCredentials Credentials { get; set; }

        [JsonPropertyName("instance_name")]
        public string InstanceName { get; set; }

    }

    public class SquidProxyCredentials : VCAPServiceCredentials
    {
        public string Uri { get; set; }
        public string Username { get; set; }

    }

    public class VCAPServices
    {
        [JsonPropertyName("c-proxy")]
        public SquidProxyService[] SquidProxyServices { get; set; }

        [JsonPropertyName("redis")]
        public RedisService[] RedisServices { get; set; }

        [JsonPropertyName("p.redis")]
        public RedisService[] RedisServices2 { get; set; }

        [JsonPropertyName("p-redis")]
        public RedisService[] RedisServices3 { get; set; }
    }
}