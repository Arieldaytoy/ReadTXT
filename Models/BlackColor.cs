using System.Text.Json.Serialization;

namespace ReadTXT;

public class BlackColor
{
    [JsonPropertyName("r")]
    public byte R { get; set; }

    [JsonPropertyName("g")]
    public byte G { get; set; }

    [JsonPropertyName("b")]
    public byte B { get; set; }

    [JsonPropertyName("a")]
    public byte A { get; set; }

    [JsonIgnore]
    public Color Color => Color.FromArgb(A, R, G, B);
}
