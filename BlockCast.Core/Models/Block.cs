namespace BlockCast.Core.Models;

public record Block(string Name, Dictionary<string, string>? Properties = null);