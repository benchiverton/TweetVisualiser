using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace TweetVisualiser.Shared.Data
{
    public interface ITweetRepository
    {

    }

    public class TweetRepository : ITweetRepository
    {
        private const string DatabaseName = "Tweets";

        private readonly ILogger<TweetRepository> _logger;
        private readonly CosmosClient _cosmosClient;

        public TweetRepository(ILogger<TweetRepository> logger, CosmosClient cosmosClient)
        {
            _logger = logger;
            _cosmosClient = cosmosClient;
        }
    }
}
