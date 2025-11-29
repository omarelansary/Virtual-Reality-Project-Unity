using UnityEngine;
using Apt.Unity.Projection;

public class StereoCameraController : MonoBehaviour
{
    [Header("Stereo Settings")]
    public float IPD = 0.064f;                 // Inter-pupillary distance in meters
    public Camera leftEyeCamera;
    public Camera rightEyeCamera;

    [Header("Projection Settings")]
    public ProjectionPlane projectionPlane;
    public bool useOffAxis = true;   // toggle on/off

    //[Header("Toe-in Settings")]
    //public bool useToeIn = false;             // Toggle toe-in on/off
    //public Transform convergencePoint;        // Empty GameObject in front of the camera

    //void Update()
    //{
    //    if (leftEyeCamera == null || rightEyeCamera == null)
    //        return;

    //    // Direction to left/right in world space
    //    Vector3 rightDir = transform.right;
    //    float halfIPD = IPD * 0.5f;

    //    // Position eyes relative to Main Camera
    //    leftEyeCamera.transform.position = transform.position - rightDir * halfIPD;
    //    rightEyeCamera.transform.position = transform.position + rightDir * halfIPD;

    //    if (useToeIn && convergencePoint != null)
    //    {
    //        // TOE-IN: each eye rotates to look at the convergence point
    //        leftEyeCamera.transform.LookAt(convergencePoint.position);
    //        rightEyeCamera.transform.LookAt(convergencePoint.position);
    //    }
    //    else
    //    {
    //        // STANDARD PARALLEL VIEW: same rotation as Main Camera
    //        leftEyeCamera.transform.rotation = transform.rotation;
    //        rightEyeCamera.transform.rotation = transform.rotation;
    //    }
    //}


    void Update()
    {
        if (leftEyeCamera == null || rightEyeCamera == null)
            return;

        // --- IPD positioning (what you already had) ---
        Vector3 rightDir = transform.right;
        float halfIPD = IPD * 0.5f;

        leftEyeCamera.transform.position = transform.position - rightDir * halfIPD;
        rightEyeCamera.transform.position = transform.position + rightDir * halfIPD;

        leftEyeCamera.transform.rotation = transform.rotation;
        rightEyeCamera.transform.rotation = transform.rotation;

        // --- Off-axis part ---
        if (useOffAxis && projectionPlane != null)
        {
            leftEyeCamera.projectionMatrix = GetProjectionMatrix(leftEyeCamera);
            rightEyeCamera.projectionMatrix = GetProjectionMatrix(rightEyeCamera);
        }
        else
        {
            // fall back to normal symmetric perspective
            leftEyeCamera.ResetProjectionMatrix();
            rightEyeCamera.ResetProjectionMatrix();
        }
    }

    private Matrix4x4 GetProjectionMatrix(Camera cam)
    {
        float n = cam.nearClipPlane;
        float f = cam.farClipPlane;

        // eye position
        Vector3 pEye = cam.transform.position;

        // plane corners
        Vector3 pA = projectionPlane.BottomLeft;
        Vector3 pB = projectionPlane.BottomRight;
        Vector3 pC = projectionPlane.TopLeft;
        Vector3 pD = projectionPlane.TopRight;   // not actually needed, but nice to have

        // vectors from eye to each relevant corner
        Vector3 vA = pA - pEye;
        Vector3 vB = pB - pEye;
        Vector3 vC = pC - pEye;
        //Vector3 vD = pD - pEye;

        // plane basis vectors (already normalized in ProjectionPlane)
        Vector3 vR = projectionPlane.DirRight;   // right
        Vector3 vU = projectionPlane.DirUp;      // up
        Vector3 vN = projectionPlane.DirNormal;  // plane normal pointing towards eye

        // distance from eye to plane along normal
        float d = -Vector3.Dot(vA, vN);

        // frustum extents on the near plane
        float l = Vector3.Dot(vR, vA) * n / d;
        float r = Vector3.Dot(vR, vB) * n / d;
        float b = Vector3.Dot(vU, vA) * n / d;
        float t = Vector3.Dot(vU, vC) * n / d;

        // build off-axis projection matrix
        return Matrix4x4.Frustum(l, r, b, t, n, f);
    }
}
