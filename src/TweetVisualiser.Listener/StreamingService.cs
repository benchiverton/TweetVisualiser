using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TweetVisualiser.Listener.Models;
using TweetVisualiser.Listener.Twitter;

namespace TweetVisualiser.Listener
{
    public class StreamingService : BackgroundService
    {
        private readonly ILogger<StreamingService> _logger;
        private readonly ITwitterStreamingService _twitterStreamingService;
        private readonly ITwitterRulesService _twitterRulesService;

        public StreamingService(ILogger<StreamingService> logger, ITwitterStreamingService twitterStreamingService, ITwitterRulesService twitterRulesService)
        {
            _logger = logger;
            _twitterStreamingService = twitterStreamingService;
            _twitterRulesService = twitterRulesService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _twitterRulesService.AddRules(stoppingToken);
            await _twitterStreamingService.StartStreaming(WriteContent);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await _twitterStreamingService.StopStreaming();
            await _twitterRulesService.DeleteRules(stoppingToken);
        }

        private Task WriteContent(FilteredTweetResponse content)
        {
            _logger.LogInformation($"Tag: {string.Join(',', content.matching_rules.Select(r => r.tag))}, Text: {content.data.text}");
            return Task.CompletedTask;
        }
    }
}
