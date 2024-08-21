using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using control.Models;
using control.Services;
using FirebaseAdmin.Messaging;

namespace control.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly FirebaseService _firebaseService;

    public HomeController(ILogger<HomeController> logger, FirebaseService firebaseService)
    {
        _logger = logger;
        _firebaseService = firebaseService;
    }

    async public Task<IActionResult> Index()

    {
        Dictionary<string, FirebaseToken> tokens = await _firebaseService.GetTokensAsync();
        _firebaseService.SubscribeTopics(tokens);
        return View(tokens);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> NotifyTopic([FromBody] TopicNotificationRequest request)
    {
        var topicList = request.Topics.Split(',');

        foreach (var topic in topicList)
        {
            var trimmedTopic = topic.Trim();

            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = request.NotificationHeader,
                    Body = request.NotificationBody,
                },
                Topic = trimmedTopic,
            };

            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine($"Successfully sent message to topic '{trimmedTopic}': {response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to topic '{trimmedTopic}': {ex.Message}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        return Ok(new { message = "Notifications sent successfully!" });
    }

    public class TopicNotificationRequest
    {
        public string NotificationHeader { get; set; }
        public string NotificationBody { get; set; }
        public string Topics { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> NotifyTokens([FromBody] NotificationRequest request)
    {
        var tokens = request.Tokens;

        if (tokens == null || tokens.Count == 0)
        {
            return BadRequest(new { message = "No tokens provided." });
        }

        var message = new MulticastMessage()
        {
            Notification = new Notification
            {
                Title = request.NotificationHeader,
                Body = request.NotificationBody,
            },
            Tokens = tokens,
        };

        try
        {
            var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);

            if (response.FailureCount > 0)
            {
                return Ok(new { message = $"Successfully sent to {response.SuccessCount} tokens, but failed to send to {response.FailureCount} tokens." });
            }

            return Ok(new { message = "Notifications sent successfully!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    public class NotificationRequest
    {
        public string NotificationHeader { get; set; }
        public string NotificationBody { get; set; }
        public List<string> Tokens { get; set; }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
