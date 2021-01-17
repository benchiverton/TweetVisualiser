using System.Collections.Generic;

namespace TweetVisualiser.Listener.Models
{
    public class FilteredTweetResponse
    {
        public TweetResponse data { get; set; }
        public List<MatchingRuleResponse> matching_rules { get; set; }
    }
}
