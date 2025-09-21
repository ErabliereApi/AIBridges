namespace AIBridges.Models;

public class PatchAIModel
{
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the model, e.g., "GPT-3", "DALL-E"
    /// </summary>
    /// <remarks>
    /// This should be unique for each model.
    /// </remarks>
    public string? Name { get; set; }

    /// <summary>
    /// Description of the model
    public string? Description { get; set; }

    /// <summary>
    /// Versions supported by the model, e.g., "v1.0, v2.0"
    /// </summary>
    public string? Versions { get; set; }

    /// <summary>
    /// Supported actions for the model, e.g., "text-generation, image-generation"
    /// </summary>
    public string? ActionNames { get; set; }

    /// <summary>
    /// The type is either api or local
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Local file name or api endpoint
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// API key for authentication, if required
    /// </summary>
    public string? Key { get; set; }
}