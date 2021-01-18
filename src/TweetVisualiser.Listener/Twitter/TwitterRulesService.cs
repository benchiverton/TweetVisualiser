using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.Extensions.Logging;

namespace TweetVisualiser.Listener.Twitter
{
    public interface ITwitterRulesService
    {
        Task AddRules(CancellationToken cancellationToken);
        Task DeleteRules(CancellationToken cancellationToken);
    }

    public class TwitterRulesService : ITwitterRulesService
    {
        private readonly ILogger<TwitterRulesService> _logger;
        private readonly TwitterContext _twitterContext;

        private readonly List<StreamingRule> _rules = new List<StreamingRule>
        {
            new StreamingRule
            {
                Tag = "related to Trump",
                Value = "trump"
            },
            new StreamingRule
            {
                Tag = "related to Biden",
                Value = "biden"
            },
            new StreamingRule
            {
                Tag = "related to Boris Johnson",
                Value = "boris johnson"
            },
            new StreamingRule
            {
                Tag = "related to Cats",
                Value = "cat"
            },
        };

        public TwitterRulesService(ILogger<TwitterRulesService> logger, TwitterContext twitterContext)
        {
            _logger = logger;
            _twitterContext = twitterContext;
        }

        public async Task AddRules(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating rules...");
            var result = await _twitterContext.AddStreamingFilterRulesAsync(_rules.Select(r => new StreamingAddRule
            {
                Tag = r.Tag,
                Value = r.Value
            }).ToList(), false, cancellationToken);

            if (!result.HasErrors)
            {
                _logger.LogInformation("Rules successfully created.");
            }
            else
            {
                _logger.LogError("Rules were not successfully created.");
                foreach (var error in result.Errors)
                {
                    _logger.LogError(
                            $"\nTitle: {error.Title}" +
                            $"\nValue: {error.Value}" +
                            $"\nID:    {error.ID}" +
                            $"\nType:  {error.Type}");
                    if (error.Title == "DuplicateRule")
                    {
                        UpdateRuleWithId(error.Value, error.ID);
                    }
                }
            }

            if (result.Rules != null)
            {
                foreach(var rule in result.Rules)
                {
                    UpdateRuleWithId(rule.Value, rule.ID);
                }
            }
        }

        public async Task DeleteRules(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting rules...");
            var result = await _twitterContext.DeleteStreamingFilterRulesAsync(_rules.Select(r => r.ID).ToList(), false, cancellationToken);

            if (!result.HasErrors)
            {
                _logger.LogInformation("Rules successfully deleted.");
            }
            else
            {
                _logger.LogError("Rules were not successfully deleted.");
                foreach (var error in result.Errors)
                {
                    _logger.LogError(
                            $"\nTitle: {error.Title}" +
                            $"\nValue: {error.Value}" +
                            $"\nID:    {error.ID}" +
                            $"\nType:  {error.Type}");
                }
            }
        }

        private void UpdateRuleWithId(string value, string id)
        {
            var duplicateRule = _rules.First(r => r.Value == value);
            _rules.Remove(duplicateRule);
            _rules.Add(new StreamingRule
            {
                ID = id,
                Tag = duplicateRule.Tag,
                Value = duplicateRule.Value
            });
        }
    }
}
