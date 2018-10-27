﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace imServer {
	public class Startup {
		public Startup(IHostingEnvironment env) {
			var builder = new ConfigurationBuilder()
				 .SetBasePath(env.ContentRootPath)
				 .AddJsonFile("appsettings.json", true, true);

			this.Configuration = builder.AddEnvironmentVariables().Build();
			this.env = env;

			//单redis节点模式，如需开启集群负载，请将注释去掉并做相应配置
			RedisHelper.Initialization(new CSRedis.CSRedisClient(Configuration["ConnectionStrings:redis1"]));
		}

		public IConfiguration Configuration { get; }
		public IHostingEnvironment env { get; set; }
		
		public void ConfigureServices(IServiceCollection services) {
			services.AddSingleton<IConfiguration>(Configuration);
			services.AddSingleton<IHostingEnvironment>(env);
		}
		
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Console.OutputEncoding = Encoding.GetEncoding("GB2312");
			Console.InputEncoding = Encoding.GetEncoding("GB2312");

			loggerFactory.AddConsole(Configuration.GetSection("Logging"));

			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();

			WebChatHelper.Configuration = Configuration;

			app.UseDefaultFiles();
			app.UseStaticFiles();

			app.Map("/ws", WebSocketHandler.Map);
		}
	}
}
