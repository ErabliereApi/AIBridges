using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AIBridges.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class AIModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the model, e.g., "GPT-3", "DALL-E"
        /// </summary>
        /// <remarks>
        /// This should be unique for each model.
        /// </remarks>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the model
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Versions supported by the model, e.g., "v1.0, v2.0"
        /// </summary>
        public string Versions { get; set; } = string.Empty;

        /// <summary>
        /// Supported actions for the model, e.g., "text-generation, image-generation"
        /// </summary>
        public string ActionNames { get; set; } = string.Empty;

        /// <summary>
        /// The type is eather api or local
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Local file name or api endpoint
        /// </summary>
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>
        /// API key for authentication, if required
        /// </summary>
        public string Key { get; set; } = string.Empty;
    }
}