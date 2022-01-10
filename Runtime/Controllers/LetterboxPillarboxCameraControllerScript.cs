#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using UnityPatterns.Controllers;

/// <summary>
/// Unity letterbox/pillarbox controllers namespace
/// </summary>
namespace UnityLetterboxPillarbox.Controllers
{
    /// <summary>
    /// A class that describes a letterbox/pillarbox camera controller script
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class LetterboxPillarboxCameraControllerScript : AControllerScript, ILetterboxPillarboxCameraController
    {
        /// <summary>
        /// Force aspect ratio
        /// </summary>
        [SerializeField]
        [Tooltip("Forces camera aspect ratio with the specified values.")]
        private Vector2 forceAspectRatio = new Vector2(21.0f, 9.0f);

        /// <summary>
        /// Letterbox/Pillarbox color
        /// </summary>
        [SerializeField]
        [Tooltip("Letterbox/Pillarbox color")]
        private Color letterboxPillarboxColor = Color.black;

        /// <summary>
        /// Old force aspect ratio
        /// </summary>
        private Vector2 oldForceAspectRatio;

        /// <summary>
        /// Old letterbox/pillarbox color
        /// </summary>
        private Color oldLetterboxPillarboxColor;

        /// <summary>
        /// Old screen size
        /// </summary>
        private Vector2Int oldScreenSize;

        /// <summary>
        /// Is clearing frame buffer
        /// </summary>
        private bool isClearingFrameBuffer;

#if UNITY_EDITOR
        /// <summary>
        /// Is begin frame rendering not registered yet
        /// </summary>
        private bool isBeginFrameRenderingEventNotRegistered = true;
#endif

        /// <summary>
        /// Force aspect ratio
        /// </summary>
        public Vector2 ForceAspectRatio
        {
            get => forceAspectRatio;
            set => forceAspectRatio = value;
        }

        /// <summary>
        /// Letterbox/Pillarbox color
        /// </summary>
        public Color LetterboxPillarboxColor
        {
            get => letterboxPillarboxColor;
            set => letterboxPillarboxColor = value;
        }

        /// <summary>
        /// Camera
        /// </summary>
        public Camera Camera { get; private set; }

        /// <summary>
        /// Updates aspect ratio
        /// </summary>
        private void UpdateAspectRatio()
        {
            if (Camera)
            {
                float screen_spect_ratio = Screen.width / (float)Screen.height;
                float force_aspect_ratio = forceAspectRatio.x / forceAspectRatio.y;
                oldForceAspectRatio = forceAspectRatio;
                oldLetterboxPillarboxColor = letterboxPillarboxColor;
                oldScreenSize = new Vector2Int(Screen.width, Screen.height);
                isClearingFrameBuffer = true;
                if (Mathf.Approximately(screen_spect_ratio, force_aspect_ratio))
                {
                    Camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                }
                else if (screen_spect_ratio > force_aspect_ratio)
                {
                    float normalized_width = force_aspect_ratio / screen_spect_ratio;
                    Camera.rect = new Rect((1.0f - normalized_width) * 0.5f, 0.0f, normalized_width, 1.0f);
                }
                else
                {
                    float normalized_height = screen_spect_ratio / force_aspect_ratio;
                    Camera.rect = new Rect(0.0f, (1.0f - normalized_height) * 0.5f, 1.0f, normalized_height);
                }
            }
        }

        /// <summary>
        /// Gets invoked when frame rendering has begun
        /// </summary>
        /// <param name="scriptableRenderContext">Scriptable render context</param>
        /// <param name="cameras">Cameras</param>
        private void BeginFrameRenderingEvent(ScriptableRenderContext scriptableRenderContext, Camera[] cameras)
        {
#if UNITY_EDITOR
            if (!this)
            {
                RenderPipelineManager.beginFrameRendering -= BeginFrameRenderingEvent;
            }
            else if (isClearingFrameBuffer || !EditorApplication.isPlaying)
#else
            if (isClearingFrameBuffer)
#endif
            {
                isClearingFrameBuffer = false;
                GL.Clear(true, true, letterboxPillarboxColor, 0.0f);
            }
        }

        /// <summary>
        /// Gets invoked when script has been started
        /// </summary>
        protected virtual void Start()
        {
            RenderPipelineManager.beginFrameRendering += BeginFrameRenderingEvent;
            if (TryGetComponent(out Camera camera))
            {
                Camera = camera;
            }
            else
            {
                Debug.LogError("Please attach a camera component to this force camera aspect ratio controller.", this);
            }
            oldForceAspectRatio = forceAspectRatio;
            oldLetterboxPillarboxColor = letterboxPillarboxColor;
            UpdateAspectRatio();
        }

        /// <summary>
        /// Gets invoked when script performs a frame update
        /// </summary>
        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (isBeginFrameRenderingEventNotRegistered)
            {
                RenderPipelineManager.beginFrameRendering += BeginFrameRenderingEvent;
                isBeginFrameRenderingEventNotRegistered = false;
            }
#endif
            if (((oldForceAspectRatio != forceAspectRatio) || (oldScreenSize.x != Screen.width) || (oldScreenSize.y != Screen.height) || (oldLetterboxPillarboxColor != letterboxPillarboxColor)) && Camera)
            {
                UpdateAspectRatio();
            }
        }

        /// <summary>
        /// Gets invoked when script has been destroyed
        /// </summary>
        protected virtual void OnDestroy() => RenderPipelineManager.beginFrameRendering -= BeginFrameRenderingEvent;
    }
}
