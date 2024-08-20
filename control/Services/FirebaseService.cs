using control.Models;
using Firebase.Database;
using Firebase.Database.Query;
using FirebaseAdmin;
namespace control.Services;

public class FirebaseService
{

  private readonly FirebaseApp _firebaseApp;

  private readonly FirebaseClient _firebaseClient;
  public FirebaseService(FirebaseApp firebaseApp, string databaseUrl)
  {
    _firebaseApp = firebaseApp;
    _firebaseClient = new FirebaseClient(databaseUrl);
  }

  public async Task<List<string>> GetTokensAsync()
  {
    var response = await _firebaseClient
      .Child("tokens")
      .OnceAsync<FirebaseToken>();

    Dictionary<string, FirebaseToken> tokens = response.ToDictionary(
      item => item.Key,
      item => item.Object);

    SubscribeTopics(tokens);
    return tokens != null ? new List<string>(tokens.Keys) : new List<string>();
  }

  async private void SubscribeTopics(Dictionary<string, FirebaseToken> tokens)
  {
    foreach (KeyValuePair<string, FirebaseToken> pair in tokens)
    {
      string tokenKey = pair.Key;
      string userAgent = pair.Value.userAgent;

      string browserTopic;
      string systemTopic;
      string hardwareTopic;

      if (userAgent.Contains("Firefox", StringComparison.OrdinalIgnoreCase))
        browserTopic = "firefox";
      else if (userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase))
        browserTopic = "chrome";
      else if (userAgent.Contains("Opera", StringComparison.OrdinalIgnoreCase))
        browserTopic = "opera";
      else if (userAgent.Contains("Safari", StringComparison.OrdinalIgnoreCase))
        browserTopic = "opera";
      else
        browserTopic = "other_browser";

      if (userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase))
        hardwareTopic = "mobile";
      else if (userAgent.Contains("Windows NT", StringComparison.OrdinalIgnoreCase) ||
              userAgent.Contains("Macintosh", StringComparison.OrdinalIgnoreCase))
        hardwareTopic = "desktop";
      else
        hardwareTopic = "other_device";

      if (userAgent.Contains("Windows NT", StringComparison.OrdinalIgnoreCase))
        systemTopic = "windows";
      else if (userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase) ||
              userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase))
        systemTopic = "android";
      else if (userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase) ||
              userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
        systemTopic = "ios";
      else if (userAgent.Contains("Macintosh", StringComparison.OrdinalIgnoreCase) ||
              userAgent.Contains("Mac OS", StringComparison.OrdinalIgnoreCase))
        systemTopic = "mac";
      else if (userAgent.Contains("Linux", StringComparison.OrdinalIgnoreCase))
        systemTopic = "linux";
      else
        systemTopic = "other_os";

      var topics = new List<string> { browserTopic, hardwareTopic, systemTopic };
      await _firebaseClient
        .Child("tokens")
        .Child(tokenKey)
        .PatchAsync(new Dictionary<string, object> { { "topics", topics } });

    }
  }
}