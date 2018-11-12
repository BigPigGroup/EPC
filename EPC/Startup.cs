using System;
using System.Timers;
using EPC.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EPC {

	public class Startup {
		private static Timer aTimer;

		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			services.Configure<CookiePolicyOptions>(options => {
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			} else {
				app.UseExceptionHandler("/Home/Error");
				//app.UseHsts();
			}

			//app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseMvc();

			SetTimer();
		}

		private static void SetTimer() {
			// Create a timer with a two second interval.
			aTimer = new Timer(600_000);
			// Hook up the Elapsed event for the timer. 
			aTimer.Elapsed += OnTimedEvent;
			aTimer.AutoReset = true;
			aTimer.Enabled = true;
		}

		private static readonly HomeController Controller = new HomeController();

		private static void OnTimedEvent(Object source, ElapsedEventArgs e) {
			Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}", e.SignalTime);
			Controller.PostTemperaturesAsync();
		}
	}

}