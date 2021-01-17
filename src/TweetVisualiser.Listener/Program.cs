using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitter.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TweetVisualiser.Listener.Twitter;

namespace TweetVisualiser.Listener
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            await CreateHostBuilder(args).RunConsoleAsync();
        }

        // for this app to work, you need to set the following environment variables:
        // TweetListener_ConsumerKey
        // TweetListener_ConsumerSecret
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables("TweetListener_");
                })
                .ConfigureServices((host, services) =>
                {
                    services.AddTransient<TwitterContext>(ctx =>
                    {
                        var auth = new ApplicationOnlyAuthorizer()
                        {
                            CredentialStore = new InMemoryCredentialStore
                            {
                                ConsumerKey = host.Configuration["ConsumerKey"],
                                ConsumerSecret = host.Configuration["ConsumerSecret"]
                            },
                        };
                        auth.AuthorizeAsync().GetAwaiter().GetResult();
                        return new TwitterContext(auth);
                    });
                    services.AddTransient<ITwitterRulesService, TwitterRulesService>();
                    services.AddTransient<ITwitterStreamingService, TwitterStreamingService>();
                    services.AddHostedService<StreamingService>();
                })
                .UseSerilog();
    }
}
