using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ShortUrl
{
    private static readonly string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private static readonly Random Random = new Random();
    private static readonly Dictionary<string, ShortUrl> UrlDatabase = new Dictionary<string, ShortUrl>();

    public string OriginalUrl { get; }
    public string ShortCode { get; private set; }
    public DateTime CreationDate { get; }
    public List<DateTime> VisitLog { get; } = new List<DateTime>();

    public ShortUrl(string originalUrl, string customAlias = null)
    {
        if (!Uri.IsWellFormedUriString(originalUrl, UriKind.Absolute))
            throw new ArgumentException("Invalid URL format");

        OriginalUrl = originalUrl;
        CreationDate = DateTime.Now;

        if (!string.IsNullOrWhiteSpace(customAlias))
        {
            if (UrlDatabase.ContainsKey(customAlias))
                throw new ArgumentException("Custom alias already exists");
            ShortCode = customAlias;
        }
        else
        {
            ShortCode = GenerateUniqueShortCode();
        }

        UrlDatabase.Add(ShortCode, this);
    }

    private string GenerateUniqueShortCode()
    {
        string code;
        do
        {
            code = GenerateRandomBase62(6); // Default length 6
        } while (UrlDatabase.ContainsKey(code));

        return code;
    }

    private static string GenerateRandomBase62(int length)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            int index = Random.Next(Base62Chars.Length);
            sb.Append(Base62Chars[index]);
        }
        return sb.ToString();
    }

    public static ShortUrl Resolve(string shortCode)
    {
        if (UrlDatabase.TryGetValue(shortCode, out var shortUrl))
        {
            shortUrl.LogVisit();
            return shortUrl;
        }
        return null;
    }

    private void LogVisit()
    {
        VisitLog.Add(DateTime.Now);
    }

    public void PrintStatistics()
    {
        Console.WriteLine($"Short URL: {ShortCode}");
        Console.WriteLine($"Original URL: {OriginalUrl}");
        Console.WriteLine($"Total visits: {VisitLog.Count}");

        var today = DateTime.Today;
        var yesterday = today.AddDays(-1);

        Console.WriteLine($"Today's visits: {VisitLog.Count(d => d.Date == today)}");
        Console.WriteLine($"Yesterday's visits: {VisitLog.Count(d => d.Date == yesterday)}");

        Console.WriteLine("\nVisit timeline (last 10):");
        
    }
}

public class UrlShortenerApp
{
    public void Run()
    {
        Console.WriteLine("URL Shortener Application");       

        while (true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Create short URL");
            Console.WriteLine("2. Visit short URL");
            Console.WriteLine("3. View statistics");
            Console.WriteLine("4. Exit");

            Console.Write("Enter choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CreateShortUrl();
                    break;
                case "2":
                    VisitShortUrl();
                    break;
                case "3":
                    ViewStatistics();
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }

    private void CreateShortUrl()
    {
        try
        {
            Console.Write("Enter original URL: ");
            string url = Console.ReadLine();

            Console.Write("Enter custom alias (optional): ");
            string alias = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(alias))
            {
                var shortUrl = new ShortUrl(url);
                Console.WriteLine($"Created short URL: {shortUrl.ShortCode}");
            }
            else
            {
                var shortUrl = new ShortUrl(url, alias);
                Console.WriteLine($"Created custom short URL: {shortUrl.ShortCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private void VisitShortUrl()
    {
        Console.Write("Enter short code: ");
        string code = Console.ReadLine();

        var shortUrl = ShortUrl.Resolve(code);
        if (shortUrl != null)
        {
            Console.WriteLine($"Redirecting to: {shortUrl.OriginalUrl}");
        }
        else
        {
            Console.WriteLine("Short URL not found");
        }
    }

    private void ViewStatistics()
    {
        Console.Write("Enter short code: ");
        string code = Console.ReadLine();

        if (ShortUrl.Resolve(code) is ShortUrl shortUrl)
        {
            shortUrl.PrintStatistics();
        }
        else
        {
            Console.WriteLine("Short URL not found");
        }
    }
}

namespace URL_Shortener_Simulator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var app = new UrlShortenerApp();
            app.Run();
        }
    }
}
