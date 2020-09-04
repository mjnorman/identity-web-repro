using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using Serilog;
using identityweb.Helpers;
using System.Net.Http;
using Microsoft.IdentityModel.Logging;

namespace identityweb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                AddKnownNetworks(options);
            });

            services.Configure<MicrosoftIdentityOptions>(options =>
            {
                var webProxy = new WebProxy()
                {
                    Address = new Uri($"http://{VCAPHelper.GetSquidHost()}:{VCAPHelper.GetSquidPort()}"),
                    BypassProxyOnLocal = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(VCAPHelper.GetSquidUsername(), VCAPHelper.GetSquidPassword())
                };
                var httpClientHandler = new HttpClientHandler()
                {
                    Proxy = webProxy,
                    UseDefaultCredentials = true,
                    UseProxy = true
                };

                options.Backchannel = new HttpClient(httpClientHandler, true);

                options.GetClaimsFromUserInfoEndpoint = true;

                options.TokenValidationParameters.NameClaimType = "sub";
                options.Events = new OpenIdConnectEvents()
                {
                    OnTokenValidated = context =>
                    {
                        Log.Debug("OnTokenValidated: {context}", context.ProtocolMessage.RedirectUri);
                        return Task.CompletedTask;
                    },
                    OnRedirectToIdentityProvider = context =>
                    {
                        context.ProtocolMessage.Scope = WebUtility.HtmlEncode(context.ProtocolMessage.Scope);
                        Log.Debug("OnRedirectToIdentityProvider: {context}", context.ProtocolMessage.RedirectUri);
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Log.Debug("OnAuthenticationFailed");
                        return Task.CompletedTask;
                    },
                    OnAuthorizationCodeReceived = context =>
                    {
                        Log.Debug("OnAuthorizationCodeReceived");

                        var code = context.ProtocolMessage.Code;
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        Log.Debug("OnMessageReceived");
                        return Task.CompletedTask;
                    },
                    OnTokenResponseReceived = context =>
                    {
                        Log.Debug("OnTokenResponseReceived");
                        return Task.CompletedTask;
                    },
                    OnUserInformationReceived = context =>
                    {
                        Log.Debug("OnUserInformationReceived");
                        return Task.CompletedTask;
                    },
                    OnRemoteFailure = context =>
                    {
                        Log.Debug("OnRemoteFailure");
                        return Task.CompletedTask;
                    },
                    OnTicketReceived = context =>
                    {
                        Log.Debug("OnTicketReceived");
                        return Task.CompletedTask;
                    }

                };
            });

            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 443;
            });

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"));

            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
           services.AddRazorPages()
                .AddMicrosoftIdentityUI();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseForwardedHeaders();
                IdentityModelEventSource.ShowPII = true;

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseForwardedHeaders();

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        public void AddKnownNetworks(ForwardedHeadersOptions options)
        {

            try
            {
                options.KnownNetworks.Clear();
                options.ForwardLimit = 2;
                foreach (var networkzone in Configuration.GetSection("KnownNetworks").Get<string[]>())
                {
                    var network = networkzone.Split("/");
                    options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse(network[0]), Convert.ToInt32(network[1])));
                    Log.Debug($"Network {networkzone} added to KnownNetworks");
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
