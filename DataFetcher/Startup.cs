﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataFetcher.BL;
using DataFetcher.Provider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataFetcher
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
            services.AddCors(options =>
            {

                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                    });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            ILogProvider logProvider = new LogProvider();
            services.Add(new ServiceDescriptor(typeof(ILogProvider), logProvider));

            IFetchProvider fetchProvider = new FetchProvider(logProvider);
            services.Add(new ServiceDescriptor(typeof(IFetchProvider), fetchProvider));

            IDataParsing dataParsing = new DataParsing(logProvider);
            services.Add(new ServiceDescriptor(typeof(IDataParsing), dataParsing));

            services.Add(new ServiceDescriptor(typeof(IProductProvider), new ProductProvider(logProvider, fetchProvider, dataParsing)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
