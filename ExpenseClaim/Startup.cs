using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ExpenseClaim.Entities;
using ExpenseClaim.Services;
using ExpenseClaim.Services.Contract;
using Microsoft.EntityFrameworkCore;
using ExpenseClaim.Modules;
using Serilog;
using Serilog.Events;

namespace ExpenseClaim
{
    public class Startup
    {
        private IConfiguration _configuration;
        private IGST _gst;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
           
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ClaimContext>(options =>options.UseSqlServer(_configuration.GetConnectionString("ExpenseClaim")));

            //Inject services
            services.AddTransient<IClaimRepository, ClaimRepository>();
            services.AddSingleton<IGST, GST>();
            services.AddTransient<IRemoveInvalidCharFromXML, RemoveInvalidCharFromXML>();

            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
                              IHostingEnvironment env,
                              IConfiguration configuration, 
                              ILogger<Startup> logger,
                              ClaimContext claimcontext, 
                              IGST gST,
                              ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel
                .Information()
                .WriteTo.RollingFile("Log - {Date}.txt", LogEventLevel.Information)
                .CreateLogger();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //Global error handling for Exception
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                   {
                       context.Response.StatusCode = 500;
                       await context.Response.WriteAsync("An Unexpected fault happened.  Try again later." +
                                                         "If issue persists, contac Administrator");
                   });
                });
            }

            //Set appbuilder to serve request incase for static file
            app.UseFileServer();

            //AutoMapper
            AutoMapper.Mapper.Initialize(cfg =>
            {
            cfg.CreateMap<Expenses, ClaimsDto>()
            .ForMember(dest => dest.costCenter, opt => opt.MapFrom(src => src.CostCenterId))
            .ForMember(dest => dest.date, opt => opt.MapFrom(src => src.TransactionDate))
            .ForMember(dest => dest.total, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.gstCalculated, opt => opt.MapFrom(src => gST.CalculateGST(src.TotalAmount)));

                cfg.CreateMap<ClaimsDto, Expenses>()
            .ForMember(dest => dest.CostCenterId, opt => opt.MapFrom(src => src.costCenter))
            .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.date))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.total))
            .ForMember(dest => dest.CostCenter, opt => opt.Ignore()) ;


            });

            //Set middleware of MVC
            //app.UseMvcWithDefaultRoute();
            if(!claimcontext.Expenses.Any())
                claimcontext.EnsureSeedDataForClaim();

            //Convention Base Route
            app.UseMvc();

            app.Run(async (context) =>
            {
                var greeting = configuration["Greeting"];
                await context.Response.WriteAsync(greeting);
            });

        }

        //Used for convention base routing
        private void configureRoute(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Default",
                "{controller=Home}/{action=Index}/{id?}");
        }
    }
}
