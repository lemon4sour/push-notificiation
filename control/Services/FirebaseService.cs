using control.Models;
using Firebase.Database;
using Firebase.Database.Query;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
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

  public async Task<Dictionary<string, FirebaseToken>> GetTokensAsync()
  {
    var response = await _firebaseClient
      .Child("tokens")
      .OnceAsync<FirebaseToken>();

    Dictionary<string, FirebaseToken> tokens = response.ToDictionary(
      item => item.Key,
      item => item.Object);

    return tokens ?? [];
  }

  public async Task SubscribeTopics(Dictionary<string, FirebaseToken> tokens)
  {
    foreach (KeyValuePair<string, FirebaseToken> kvp in tokens) {
      foreach (var topic in kvp.Value.topics) {
        var response = await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync([kvp.Key], topic);
        if (response.FailureCount != 0)
          Console.WriteLine(response.Errors[0].Reason);
        else {
          Console.WriteLine($"Successfully subscibed {kvp.Key} to {topic}");
        }
      }
    }
  }
}