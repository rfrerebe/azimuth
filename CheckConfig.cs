public class CheckConfig
{

    public const string Key = nameof(CheckConfig);

    public required string Sender { get; set; }

    public required string Recipient { get; set; }

    public required string Url { get; set; }

    public required int TimeoutMs { get; set; } = 30000;

    //public required string QuerySelector { get; set; } = "div.bye-azimuth";

    //public required string ExpectedText { get; set; } = "est fermé pour l'été";
}