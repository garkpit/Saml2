using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleAspNetCore2ApplicationNETFramework.Data;
using SampleAspNetCore2ApplicationNETFramework.Services;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Metadata;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata.Services;
using Sustainsys.Saml2.WebSso;

namespace SampleAspNetCore2ApplicationNETFramework
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        public IConfiguration Configuration { get; }

        private ILogger Logger;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                //.
                ;

            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Account/Manage");
                    options.Conventions.AuthorizePage("/Account/Logout");
                    //options.Conventions.
                });

            // Register no-op EmailSender used by account confirmation and password reset during development
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
            services.AddSingleton<IEmailSender, EmailSender>();

            services.AddAuthentication()
                //.
                .AddSaml2(options => 
                {
                    options.SPOptions.EntityId = new EntityId("https://localhost:44342/Saml2");     // HERE

                    // base path of the SAML2 endpoints. Default is /Saml2
                    options.SPOptions.ModulePath = "/Saml2";

                    // This could be overridden by  IdentityProvider option  RelayStateUsedAsReturnUrl = true
                    options.SPOptions.ReturnUrl = new Uri("/Jay/Hello", UriKind.Relative);
                    // alternatively ... adding tenant id here?
                    // if I could get my method to work this might be interesting
                    // however, it should be at the IdentityProvider level, not global
                    //options.SPOptions.ReturnUrl = new Uri("/Jay/ByTenant/1f6df580-c7db-4bd8-9cf9-c354874e079c", UriKind.Relative);

                    // LOCAL CERT - Is OPTIONAL
                    // without it the response to /Saml2 here looks pretty thin though
                    // We're supposed to have one, and it can be SELF SIGNED, as it is just used for encryption
                    // Their test cert
                    //options.SPOptions.ServiceCertificates.Add(new X509Certificate2("Sustainsys.Saml2.Tests.pfx"));
                    // My test cert
                    options.SPOptions.ServiceCertificates.Add(new X509Certificate2("App_Data/JaySelfCert1.pfx"));

                    //options.SPOptions.Logger = Logger<>;


                    //options.IdentityProviders.Add(
                    //    new IdentityProvider(
                    //        new EntityId("https://localhost:44300/Metadata"), options.SPOptions)    // StubIdp default
                    //    {
                    //        LoadMetadata = true,
                    //        AllowUnsolicitedAuthnResponse = true,
                    //        //Binding = Saml2BindingType.HttpPost,
                    //        //MetadataLocation = "~/App_Data/IdpMetadata.xml",
                    //    });

                    options.IdentityProviders.Add(
                        new IdentityProvider(
                                new EntityId("https://localhost:44300/1f6df580-c7db-4bd8-9cf9-c354874e079c/Metadata"), // StubIdp my Tenant
                                options.SPOptions) 
                            {
                                LoadMetadata = true,

                                // let's try to get it from the local xml file
                                // MetadataLocation = "~/App_Data/1f6df580-c7db-4bd8-9cf9-c354874e079c.Metadata.xml",
                                // OK, that did work, but it seemed to also check the EntityId URL first
                                // also, standard/docs say it is strongly recommended to not do this
                                // HOWEVER my concern is if e.g. EBU were offline then would RTLicense throw an exception? (to all users???)

                                // IdP_Init == they start the login from their end
                                AllowUnsolicitedAuthnResponse = true,

                                // They can say where they want to land if this is true
                                RelayStateUsedAsReturnUrl = false 
                            });

                    //// WTF .. how do I use this (if at all)?
                    //options.SPOptions.AttributeConsumingServices.Add(new AttributeConsumingService()
                    //{
                    //    Index = 1,
                    //    IsDefault = true,
                    //    IsRequired = false,
                    //    RequestedAttributes = { new RequestedAttribute("", "") },
                    //    ServiceDescriptions = { new LocalizedName("name","language")},
                    //    ServiceNames = { new LocalizedName("", "")}
                    //});

                    //options.SPOptions.AuthenticateRequestSigningBehavior = SigningBehavior.IfIdpWantAuthnRequestsSigned;
                    //options.SPOptions.DiscoveryServiceUrl = etc
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
