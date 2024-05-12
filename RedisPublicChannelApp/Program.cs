using Microsoft.AspNetCore.SignalR;
using RedisPublicChannelApp.Hubs;
using RedisPublicChannelApp.Services;
using StackExchange.Redis;

namespace RedisPublicChannelApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("redis-13715.c282.east-us-mz.azure.redns.redis-cloud.com:13715,password=vLdemVDtIuO72UqGs9eY6mP0wrJBG59Y"));
            builder.Services.AddScoped<IMessageListener,RedisMessageListener>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapHub<ChatHub>("/chathub");

            app.Run();
        }
    }
}