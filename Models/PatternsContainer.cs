using System.Text.Json.Serialization;

namespace ReadTXT;

public class PatternsContainer
{
    [JsonPropertyName("patterns")]
    public required PatternItem[] Patterns { get; set; }
}
