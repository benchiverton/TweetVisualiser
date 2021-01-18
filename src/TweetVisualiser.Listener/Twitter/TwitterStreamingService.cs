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
                       .StartAsync(async streamContent =>
                       {
                           // sometimes we get empty tweets...
                           if (string.IsNullOrEmpty(streamContent.Content))
                           {
                               return;
                           }

                           try
                           {
                               await onTweetReceived(JsonSerializer.Deserialize<FilteredTweetResponse>(streamContent.Content));
                           }
                           catch (Exception ex)
                           {
                               // if we don't handle exceptions receiving the tweet, we quickly hit our rate limit when recovering
                               _logger.LogError(ex, "An exception was thrown when receiving a tweet. It will not be retried.");
                           }
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
