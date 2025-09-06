public class GraphConfig
{

    public const string Key = nameof(GraphConfig);

    public required string ClientId { get; set; }

    public required string ClientSecret { get; set; }
    
    public required string Tenant { get; set; }
}