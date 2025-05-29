using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace ColorValley.Models;

/// <summary>
/// This class holds a high score entry.
/// </summary>
public class HighScoreEntry
{
    private static readonly Encoding _utf8Encoding = Encoding.UTF8;

    /// <summary>
    /// The rank or trophy
    /// </summary>
    [JsonIgnore]
    public string RankOrTrophy { get; set; } = string.Empty;

    /// <summary>
    /// The high score value.
    /// </summary>
    [JsonInclude]
    [JsonPropertyName("level")]
    public int Level { get; set; }


    /// <summary>
    /// The high score value.
    /// </summary>
    [JsonInclude]
    [JsonPropertyName("score")]
    public int Score { get; set; }

    /// <summary>
    /// The name of the person that has created the high score.
    /// </summary>
    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The name of the person that has created the high score.
    /// </summary>
    [JsonInclude]
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The hash that was generated out of the data of this entry.
    /// </summary>
    [JsonInclude]
    [JsonPropertyName("hash")]
    public string Hash { get; set; } = string.Empty;

    public string ComputeHash()
    {
        using (HMACSHA256 sha256Hash =  new HMACSHA256(_utf8Encoding.GetBytes(AppSettings.HashSalt)))
        {
            var unixTimeStamp = ((DateTimeOffset)CreatedAt).ToUnixTimeSeconds();

            var hashString = $"{this.Name}_{this.Level}_{this.Score}_{unixTimeStamp}";

            // ComputeHash returns byte array
            byte[] bytes = sha256Hash.ComputeHash(_utf8Encoding.GetBytes(hashString));

            // Convert byte array to a string
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2")); // format as hexadecimal
            }
            return builder.ToString();
        }
    }

}

