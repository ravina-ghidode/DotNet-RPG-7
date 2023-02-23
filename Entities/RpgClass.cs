using System.Text.Json.Serialization;

namespace DotNetRPG.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RpgClass
    {
        Knight = 1,
        Mage = 2,
        CLeric = 3,
    }
}
