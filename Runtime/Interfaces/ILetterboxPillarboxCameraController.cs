using UnityEngine;
using UnityPatterns;

/// <summary>
/// Unity letterbox/pillarbox namespace
/// </summary>
namespace UnityLetterboxPillarbox
{
    /// <summary>
    /// An interface that represents a letterbox/pillarbox camera controller
    /// </summary>
    public interface ILetterboxPillarboxCameraController : IController
    {
        /// <summary>
        /// Force aspect ratio
        /// </summary>
        Vector2 ForceAspectRatio { get; set; }

        /// <summary>
        /// Letterbox/Pillarbox color
        /// </summary>
        Color LetterboxPillarboxColor { get; set; }

        /// <summary>
        /// Camera
        /// </summary>
        Camera Camera { get; }
    }
}
