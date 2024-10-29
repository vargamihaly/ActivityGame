using System.Text.Json.Serialization;

namespace ActivityGameBackend.Application.Games;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MethodType
{
    Drawing = 0,       // Rajzolás
    Description = 1,   // Körülírás
    Mimic = 2,         // Mutogatás
}
