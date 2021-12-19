using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using TweetVisualiser.Shared.Contracts;
using static Microsoft.Azure.Cosmos.Container;

namespace TweetVisualiser.Shared.Data;

public interface ITweetRepository
{
    Task PersistTweet(Tweet tweet);
    Task SubscribeToChanges(string instanceName, ChangesHandler<Tweet> changesHandler);
    Task UnsubscribeFromChanges();
}

public class TweetRepository : ITweetRepository
{
    private const string DatabaseName = "TweetVisualiser";
    private const string ContainerName = "Tweets";
    private const string LeaseContainerName = "Lease";

    private readonly ILogger<TweetRepository> _logger;
    private readonly CosmosClient _cosmosClient;

    private Database _cosmosDb;
    private Container _cosmosContainer;
    private Container _cosmosLeaseContainer;
    private ChangeFeedProcessor _changeFeedProcessor;

    public TweetRepository(ILogger<TweetRepository> logger, CosmosClient cosmosClient)
    {
        _logger = logger;
        _cosmosClient = cosmosClient;
    }

    public async Task Setup()
    {
        _cosmosDb = (await _cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseName)).Database;
        _cosmosContainer = await _cosmosDb.CreateContainerIfNotExistsAsync(ContainerName, "/Tag");
        _cosmosLeaseContainer = await _cosmosDb.CreateContainerIfNotExistsAsync(LeaseContainerName, "/id");
    }

    // for some reason, CreateDatabaseIfNotExistsAsync does not work in Blazor...
    public void Initialise()
    {
        _cosmosDb = _cosmosClient.GetDatabase(DatabaseName);
        _cosmosContainer = _cosmosDb.GetContainer(ContainerName);
        _cosmosLeaseContainer = _cosmosDb.GetContainer(LeaseContainerName);
    }

    public async Task PersistTweet(Tweet tweet)
    {
        var response = await _cosmosContainer.CreateItemAsync(tweet, new PartitionKey(tweet.Tag));
        _logger.LogInformation($"Persisted tweet with tag: {response.Resource.Tag}");
    }

    public async Task SubscribeToChanges(string instanceName, ChangesHandler<Tweet> changesHandler)
    {
        if (_changeFeedProcessor != null)
        {
            _logger.LogWarning($"Only one change feed processor is supported per instance of {nameof(TweetRepository)}.");
            return;
        }
        _changeFeedProcessor = _cosmosContainer.GetChangeFeedProcessorBuilder<Tweet>(processorName: "changeFeedSample", changesHandler)
                .WithInstanceName(instanceName)
                .WithLeaseContainer(_cosmosLeaseContainer)
                .Build();
        await _changeFeedProcessor.StartAsync();
    }

    public async Task UnsubscribeFromChanges()
    {
        await _changeFeedProcessor?.StopAsync();
        _changeFeedProcessor = null;
    }
}
