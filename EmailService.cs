using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EmailService
{
    private readonly GraphServiceClient graphServiceClient;

    private readonly ILogger<EmailService> logger;

    // Method 1: Using Client Secret Credential
    public EmailService(IOptions<GraphConfig> options, ILogger<EmailService> logger)
    {
        var config = options.Value ?? throw new ArgumentNullException(nameof(options));
        var credential = new ClientSecretCredential(config.Tenant, config.ClientId, config.ClientSecret);

        this.graphServiceClient = new GraphServiceClient(credential, new[] {
            "https://graph.microsoft.com/.default"
        });
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    public async Task SendEmailAsync(string senderEmail, string recipientEmail, string subject, string bodyContent, bool isHtml = true)
    {
        var message = new Message
        {
            Subject = subject,
            Body = new ItemBody
            {
                ContentType = isHtml ? BodyType.Html : BodyType.Text,
                Content = bodyContent
            },
            ToRecipients = new List<Recipient>
            {
                new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = recipientEmail,
                        Name = recipientEmail // Optional: you can provide a display name
                    }
                }
            }
        };

        var requestBody = new SendMailPostRequestBody
        {
            Message = message,
            SaveToSentItems = true
        };

        try
        {
            await graphServiceClient.Users[senderEmail].SendMail.PostAsync(requestBody);
            this.logger.LogInformation("Email sent successfully!");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error sending email: {message}", ex.Message);
            throw;
        }
    }
}
