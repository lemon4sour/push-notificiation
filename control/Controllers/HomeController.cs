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
    public async Task<IActionResult> NotifyTopic([FromBody] NotificationRequest request)
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

    public class NotificationRequest
    {
        public string NotificationHeader { get; set; }
        public string NotificationBody { get; set; }
        public string Topics { get; set; }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
