using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using Serilog;

namespace WindowsServiceDemo;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            //if (_logger.IsEnabled(LogLevel.Information))
            //{
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await TestAPICall();
            //}
            await Task.Delay(60000, stoppingToken);
        }
    }
    protected async Task TestAPICall()
    {
        // Base URL of the API
        string apiUrl = "https://jsonplaceholder.typicode.com/posts";
        //string logFilePath = "logfile.txt";
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("logfile.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Create an HttpClient instance
        using HttpClient client = new();

        try
        {
            Log.Information("Fetching data from API: {ApiUrl}", apiUrl);
            // Make the GET request
            var posts = await client.GetFromJsonAsync<Post[]>(apiUrl);

            // Display the posts
            if (posts != null)
            {
                foreach (var post in posts)
                {
                    //string logEntry = $"[{DateTime.Now}] Post ID: {post.Id}, Title: {post.Title}\nContent: {post.Body}\n";
                    //Console.WriteLine(logEntry); // Optional: Print to console
                    //await logFile.WriteLineAsync(logEntry);
                    
                    Log.Information("Post ID: {PostId}, Title: {Title}, Content: {Content}",
                        post.Id, post.Title, post.Body);

                    //_logger.LogInformation($"Post ID: {post.Id}, Title: {post.Title}");
                    //_logger.LogInformation($"Content: {post.Body}\n");
                }
            }
        }
        catch (Exception ex)
        {
            //string errorLog = $"[{DateTime.Now}] Error: {ex.Message}";
            //Console.WriteLine(errorLog);

            Log.Error("An error occurred: {ErrorMessage}", ex.Message);

            // Log the error to the file
            //await File.AppendAllTextAsync(logFilePath, errorLog + Environment.NewLine);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
// Define a class to represent the response
public class Post
{
    public int UserId { get; set; }
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}
