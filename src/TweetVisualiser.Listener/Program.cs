using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitter.OAuth;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using TweetVisualiser.Listener.Twitter;
using TweetVisualiser.Shared.Data;

namespace TweetVisualiser.Listener;

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
                config.AddUserSecrets<Program>();
            })
            .ConfigureServices((host, services) =>
            {
                services.AddTransient<CosmosClient>(ctx => new CosmosClient(host.Configuration["CosmosDB:Uri"], host.Configuration["CosmosDB:PrimaryKey"]));
                services.AddTransient<ITweetRepository>(ctx =>
                {
                    var tweetRepository = new TweetRepository(ctx.GetService<ILogger<TweetRepository>>(), ctx.GetService<CosmosClient>());
                    tweetRepository.Setup().GetAwaiter().GetResult();
                    return tweetRepository;
                });
                services.AddTransient<TwitterContext>(ctx =>
                {
                    var auth = new ApplicationOnlyAuthorizer()
                    {
                        CredentialStore = new InMemoryCredentialStore
                        {
                            ConsumerKey = host.Configuration["TweetListener:ConsumerKey"],
                            ConsumerSecret = host.Configuration["TweetListener:ConsumerSecret"]
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
