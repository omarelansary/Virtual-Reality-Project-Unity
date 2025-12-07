using MediaPipe.BlazeFace;
using UnityEngine;

namespace Tutorial_4
{
    public class HeadTracker : MonoBehaviour
    {
        [Tooltip("Index of your webcam.")]
        [SerializeField] private int webcamIndex = 0;
        [Tooltip("Threshold of the face detector")]
        [Range(0f, 1f)] 
        [SerializeField] private float threshold = 0.5f;
        [SerializeField] private ResourceSet resources;
        [Tooltip("Focal length of your webcam in pixels")]
        [SerializeField] private int focalLength = 492;
        [Tooltip("Distance between your eyes in meters.")]
        [SerializeField] private float ipd = 0.064f;

        public Vector3 DetectedFace { get; private set; }

        private FaceDetector _detector;
        private WebCamTexture _webCamTexture;

        private void Start()
        {
            _detector = new FaceDetector(resources);
            
            // Source - https://stackoverflow.com/a
            // Posted by S.Richmond
            // Retrieved 2025-11-19, License - CC BY-SA 3.0

            var devices = WebCamTexture.devices;
            /*
            foreach (var device in devices)
            {
                Debug.Log(device.name);
            }
            */
            if (devices.Length == 0)
            {
                Debug.LogWarning("No webcam found");
                return;
            }
            
            var device = devices[webcamIndex];
            _webCamTexture = new WebCamTexture(device.name);
            _webCamTexture.Play();
        }

        private void OnDestroy()
        {
            _detector?.Dispose();
        }

        private void Update()
        {
            if (_webCamTexture == null)
            {
                return;
            }
            
            _detector.ProcessImage(_webCamTexture, threshold);
            if (_detector.Detections.Length == 0)
            {
                DetectedFace = Vector3.zero;
                return;
            }
            
            SetCameraPosition(_detector.Detections[0]);
        }

        private void SetCameraPosition(Detection face)
        {
            // --- 1. Prepare Variables ---
            float w = _webCamTexture.width;
            float h = _webCamTexture.height;

            // Convert normalized UV coordinates (0 to 1) to Pixel coordinates
            Vector2 leftEyePixel = face.leftEye * new Vector2(w, h);
            Vector2 rightEyePixel = face.rightEye * new Vector2(w, h);

            // --- 2. Calculate Image Parameters (Source: Task 4.2) ---
            
            float s_img = Vector2.Distance(leftEyePixel, rightEyePixel);

            // Optional: Log this value to console
            // Debug.Log($"S_img (Pixels between eyes): {s_img}");

            // Avoid division by zero if detection glitched
            if (s_img < 0.1f) return; 

            Vector2 eyesCenter = (leftEyePixel + rightEyePixel) / 2.0f;
            float u = eyesCenter.x;
            float v = eyesCenter.y;

            float cx = w / 2.0f;
            float cy = h / 2.0f;

            float f = (float)focalLength;

            float s_real = ipd;

            // --- 3. Apply Formulas (Source: Page 3) ---

            // Calculate Z (Depth)
            float z = -(f * s_real) / s_img;

            // Calculate X (Horizontal)
            // Formula: x = ((u - c_x) * z) / f 
            float x = ((u - cx) * z) / f;

            // Calculate Y (Vertical)
            // Formula: y = - ((v - c_y) * z) / f 
            float y = -((v - cy) * z) / f;

            // --- 4. Apply to Camera ---
            
            // Use localPosition to move relative to the parent (screen center)
            transform.localPosition = new Vector3(x, y, z);
        }
    }
}