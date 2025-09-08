using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class CheckService : BackgroundService
{
    private readonly CheckConfig checkConfig;

    private readonly EmailService emailService;

    private readonly ILogger<CheckService> logger;


    private readonly IHostApplicationLifetime appLifetime;

    private readonly IHttpClientFactory httpClientFactory;

    public CheckService(IOptions<CheckConfig> checkConfig, IHttpClientFactory  httpclientFactory, EmailService emailService, IHostApplicationLifetime appLifetime, ILogger<CheckService> logger)
    {
        this.checkConfig = checkConfig.Value ?? throw new ArgumentNullException(nameof(checkConfig));
        this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.appLifetime = appLifetime ?? throw new ArgumentNullException(nameof(appLifetime));
        this.httpClientFactory = httpclientFactory ?? throw new ArgumentNullException(nameof(httpclientFactory));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            this.logger.LogInformation("Starting ...");
            var check = await this.CheckWebsite();
            if (check)
            {
                await this.emailService.SendEmailAsync(
                    this.checkConfig.Sender,
                    this.checkConfig.Recipient,
                    "Azimuth is unchanged 🙄",
                    string.Empty,
                    false);
            }
            else
            {
                await this.emailService.SendEmailAsync(
                    this.checkConfig.Sender,
                    this.checkConfig.Recipient,
                    "🎉 Azimuth has changed 🎉",
                    string.Empty,
                    false);
            }

            this.logger.LogInformation("Finished");
        }
        finally
        {
            this.appLifetime.StopApplication();
        }
        
    }

    private async Task<bool> CheckWebsite()
    {
        var client = this.httpClientFactory.CreateClient();
        var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, this.checkConfig.Url), HttpCompletionOption.ResponseHeadersRead);
        return response.IsSuccessStatusCode;
    }


    //private async Task<(bool Status, string? Value, string? Message)> CheckWebsite()
    //{
    //    try
    //    {
    //        using var playwright = await Playwright.CreateAsync();

    //        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    //        {
    //            Headless = true, // Set to false for debugging
    //        });

    //        var page = await browser.NewPageAsync();

    //        await page.GotoAsync(this.checkConfig.Url, new PageGotoOptions
    //        {
    //            WaitUntil = WaitUntilState.NetworkIdle, // Wait for network to be idle
    //            Timeout = this.checkConfig.TimeoutMs
    //        });

    //        var element = await page.QuerySelectorAsync(this.checkConfig.QuerySelector);

    //        if (element != null)
    //        {
    //            var value = await element.InnerTextAsync();
    //            return (true, value, null);
    //        }
    //        else
    //        {
    //            return (false, null, "Element is null");
    //        }
    //    }
    //    catch (TimeoutException ex)
    //    {
    //        this.logger.LogError(ex, "Element with class '{querySelector}' not found within {timeout}s on {url}", this.checkConfig.QuerySelector, this.checkConfig.TimeoutMs / 1000, this.checkConfig.Url);
    //        return (false, null,
    //                   $"Element with class '{this.checkConfig.QuerySelector}' not found within {this.checkConfig.TimeoutMs / 1000}s on {this.checkConfig.Url}");
    //    }
    //    catch (Exception ex)
    //    {
    //        this.logger.LogError(ex, "Exception while looking for querySelector '{querySelector}' not found on {url}", this.checkConfig.QuerySelector, this.checkConfig.Url);
    //        return (false, null,
    //                   $"Exception while looking for querySelector '{this.checkConfig.QuerySelector}' not found on {this.checkConfig.Url}");
    //    }
    //}
}