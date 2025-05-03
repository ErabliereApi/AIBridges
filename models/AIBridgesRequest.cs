namespace AIBridges.Models
{
    public class AIBridgeRequest
    {
        public string? Model { get; set; }
        public string? Task { get; set; }
        public string? Image { get; set; }
        public string? Key { get; set; }
        public string? Endpoint { get; set; }
        public string? Text { get; set; }
        public string? OutputFormat { get; set; }
        public string? OutputType { get; set; }
        public string? OutputPath { get; set; }
    }
}