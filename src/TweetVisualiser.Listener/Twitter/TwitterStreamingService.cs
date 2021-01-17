using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.Extensions.Logging;
using TweetVisualiser.Listener.Models;

namespace TweetVisualiser.Listener.Twitter
{
    public interface ITwitterStreamingService
    {
        Task StartStreaming(Func<FilteredTweetResponse, Task> onTweetReceived);
        Task StopStreaming();
    }

    public class TwitterStreamingService : ITwitterStreamingService
    {
        private readonly ILogger<TwitterStreamingService> _logger;
        private readonly TwitterContext _twitterContext;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private bool _streaming = false;

        public TwitterStreamingService(ILogger<TwitterStreamingService> logger, TwitterContext twitterContext)
        {
            _logger = logger;
            _twitterContext = twitterContext;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartStreaming(Func<FilteredTweetResponse, Task> onTweetReceived)
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting streaming...");
                    _streaming = true;
                    await _twitterContext.Streaming.WithCancellation(_cancellationTokenSource.Token)
                       .Where(s => s.Type == StreamingType.Filter)
                       .StartAsync(streamContent =>
                       {
                           if (!string.IsNullOrEmpty(streamContent.Content))
                           {
                               return onTweetReceived(JsonSerializer.Deserialize<FilteredTweetResponse>(streamContent.Content));
                           }
                           return Task.CompletedTask;
                       });
                }
                catch (TaskCanceledException _)
                {
                    // streaming has stopped as cancellation has been requested
                    _streaming = false;
                    _logger.LogInformation("Streaming has stopped.");
                }
                catch (Exception ex)
                {
                    _streaming = false;
                    _logger.LogError(ex, "Streaming has stopped because of an exception. Recovering...");
                }
            }
        }

        public async Task StopStreaming()
        {
            _logger.LogInformation("Stopping streaming...");
            _cancellationTokenSource.Cancel();
            while (_streaming)
            {
                await Task.Delay(25);
            }
        }
    }
}
