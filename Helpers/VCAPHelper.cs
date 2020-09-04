using System;
using Serilog;
using System.Text.Json;
using System.Linq;
using System.Reflection;
using identityweb.Models;

namespace identityweb.Helpers
{
    public static class VCAPHelper
    {
        private static readonly string VCAP_SERVICES = "VCAP_SERVICES";
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        
        public static VCAPApplication ParseVCAPApplication(string EnvironmentVariable)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Debug("Executing {Class}.{Method} ", m.ReflectedType.Name, m.Name);
            var env = Environment.GetEnvironmentVariable(EnvironmentVariable);
            VCAPApplication vcap;
            vcap = !String.IsNullOrEmpty(env) ? JsonSerializer.Deserialize<VCAPApplication>(env) : new VCAPApplication();

            return vcap;
        }

        public static VCAPServices ParseVCAPServices()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Debug("Executing {Class}.{Method} ", m.ReflectedType.Name, m.Name );
            var env = Environment.GetEnvironmentVariable(VCAP_SERVICES);
            var services = JsonSerializer.Deserialize<VCAPServices>(env, jsonOptions);
            Log.Debug("ENV variables are {env}", env);
            Log.Debug("Leaving ParseVCAPServices ");
            return services;
        }

        public static RedisService GetRedisService()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Debug("Executing {Class}.{Method} ", m.ReflectedType.Name, m.Name);
            var services = ParseVCAPServices();
            var service = GetNormalizedRedisService(services);
            return service;
        }

        private static RedisService GetNormalizedRedisService(VCAPServices services)
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Debug("Executing {Class}.{Method} ", m.ReflectedType.Name, m.Name);
            RedisService serviceInstance;
            if (services.RedisServices != null)
            {
                serviceInstance = services.RedisServices.First();
            }
            else if (services.RedisServices2 != null)
            {
                serviceInstance = services.RedisServices2.First();
            }
            else
            {
                serviceInstance = services.RedisServices3.First();
            }

            return serviceInstance;
        }

        public static string GetRedisHost()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Debug("Executing {Class}.{Method} ", m.ReflectedType.Name, m.Name);
            var service = GetRedisService();
            var credentials = service.Credentials;
            var host = credentials.Host;
            Log.Debug("REDIS -- Host is {host}, Service is {service}, Credentials are {credentials}", host, service, credentials);

            return host;
        }

        public static string GetRedisInstanceName()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Debug("Executing {Class}.{Method} ", m.ReflectedType.Name, m.Name);
            var service = GetRedisService();
            var instance = service.InstanceName;
            Log.Debug("REDIS -- Instance is {instance}", instance);
            return instance;
        }

        public static string GetRedisPassword()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Debug("Executing {Class}.{Method} ", m.ReflectedType.Name, m.Name);
            var service = GetRedisService();
            var password = service.Credentials.Password;
            Log.Debug("REDIS -- Password is {password}", password);
            return password;
        }

        public static int GetRedisPort()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Debug("Executing {Class}.{Method} ", m.ReflectedType.Name, m.Name);
            var service = GetRedisService();
            var port = service.Credentials.Port;
            Log.Debug("REDIS -- port is {port}", port);
            return port;
        }

        public static SquidProxyService GetSquidService()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Debug("Executing {Class}.{Method} ", m.ReflectedType.Name, m.Name);
            var services = ParseVCAPServices();
            var service = services.SquidProxyServices.FirstOrDefault();
            return service;
        }

        public static string GetSquidHost()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Debug("Executing {Class}.{Method} ", m.ReflectedType.Name, m.Name);
            var service = GetSquidService();
            Log.Debug("SQUID -- Host is {host}", service.Credentials.Host);
            return service.Credentials.Host;
        }

        public static int GetSquidPort()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Debug("Executing {Class}.{Method} ", m.ReflectedType.Name, m.Name);
            var service = GetSquidService();
            Log.Debug("SQUID -- port is {port}", service.Credentials.Port);
            return service.Credentials.Port;
        }

        public static string GetSquidPassword()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Debug("Executing {Class}.{Method} ", m.ReflectedType.Name, m.Name);
            var service = GetSquidService();
            Log.Debug("SQUID -- password is {password}", service.Credentials.Password);
            return service.Credentials.Password;
        }

        public static string GetSquidUri()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Debug("Executing {Class}.{Method} ", m.ReflectedType.Name, m.Name);
            var service = GetSquidService();
            Log.Debug("SQUID -- Uri is {uri}", service.Credentials.Uri);
            return service.Credentials.Uri;
        }

        public static string GetSquidUsername()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Log.Debug("Executing {Class}.{Method} ", m.ReflectedType.Name, m.Name);
            var service = GetSquidService();
            Log.Debug("SQUID -- username is {username}", service.Credentials.Username);
            return service.Credentials.Username;
        }

    }
}