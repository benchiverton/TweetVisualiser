using System;
using Newtonsoft.Json;

namespace TweetVisualiser.Shared.Contracts;

public class Tweet
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    public string Tag { get; set; }
    public string Content { get; set; }
    public DateTime StreamedTimeUtc { get; set; }
}
