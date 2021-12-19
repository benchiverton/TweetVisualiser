using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TweetVisualiser.Listener.Models;
using TweetVisualiser.Listener.Twitter;
using TweetVisualiser.Shared.Contracts;
using TweetVisualiser.Shared.Data;

namespace TweetVisualiser.Listener;

public class StreamingService : BackgroundService
{
    private readonly ILogger<StreamingService> _logger;
    private readonly ITwitterStreamingService _twitterStreamingService;
    private readonly ITwitterRulesService _twitterRulesService;
    private readonly ITweetRepository _tweetRepository;

    public StreamingService(ILogger<StreamingService> logger, ITwitterStreamingService twitterStreamingService, ITwitterRulesService twitterRulesService, ITweetRepository tweetRepository)
    {
        _logger = logger;
        _twitterStreamingService = twitterStreamingService;
        _twitterRulesService = twitterRulesService;
        _tweetRepository = tweetRepository;
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

    private async Task WriteContent(FilteredTweetResponse content)
    {
        foreach (var matchingRule in content.matching_rules)
        {
            var tweet = new Tweet
            {
                Id = content.data.id,
                Tag = matchingRule.tag,
                Content = content.data.text,
                StreamedTimeUtc = DateTime.UtcNow
            };
            await _tweetRepository.PersistTweet(tweet);
        }
    }
}
