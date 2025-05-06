using UnityEngine;

public class RightHandRemap2D : MonoBehaviour
{
    [Header("Tracked Sprite")]
    public Transform handSprite;

    [Header("Kinect Input Range")]
    public float kinectXMin = -0.5f;
    public float kinectXMax = 0.5f;
    public float kinectYMin = 0.5f;
    public float kinectYMax = 2.0f;

    [Header("Game World Output Range")]
    public float worldXMin = -5f;
    public float worldXMax = 5f;
    public float worldYMin = -3f;
    public float worldYMax = 3f;

    [Header("Smoothing")]
    public float smoothingSpeed = 10f;

    private Vector3 targetPosition;

    void Update()
    {
        KinectManager kinectManager = KinectManager.Instance;

        if (kinectManager && kinectManager.IsInitialized())
        {
            long userId = kinectManager.GetPrimaryUserID();

            if (userId != 0 &&
                kinectManager.IsJointTracked(userId, (int)KinectInterop.JointType.HandRight))
            {
                Vector3 rawPos = kinectManager.GetJointPosition(userId, (int)KinectInterop.JointType.HandRight);

                // Normalize Kinect position to 0-1
                float normX = Mathf.InverseLerp(kinectXMin, kinectXMax, rawPos.x);
                float normY = Mathf.InverseLerp(kinectYMin, kinectYMax, rawPos.y);

                // Remap to world coordinates
                float worldX = Mathf.Lerp(worldXMin, worldXMax, normX);
                float worldY = Mathf.Lerp(worldYMin, worldYMax, normY);

                targetPosition = new Vector3(worldX, worldY, 0f);

                // Smooth follow
                if (handSprite != null)
                {
                    handSprite.position = Vector3.Lerp(handSprite.position, targetPosition, Time.deltaTime * smoothingSpeed);
                }
            }
        }
    }
}
