using UnityEngine;

public class BothHandsRemap2D : MonoBehaviour
{
    [Header("Tracked Sprites")]
    public Transform leftHandSprite;
    public Transform rightHandSprite;

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

    private Vector3 targetLeftPos;
    private Vector3 targetRightPos;

    void Update()
    {
        KinectManager kinectManager = KinectManager.Instance;

        if (kinectManager && kinectManager.IsInitialized())
        {
            long userId = kinectManager.GetPrimaryUserID();

            if (userId != 0)
            {
                // LEFT HAND
                if (kinectManager.IsJointTracked(userId, (int)KinectInterop.JointType.HandLeft))
                {
                    Vector3 rawLeft = kinectManager.GetJointPosition(userId, (int)KinectInterop.JointType.HandLeft);
                    float leftX = Mathf.InverseLerp(kinectXMin, kinectXMax, rawLeft.x);
                    float leftY = Mathf.InverseLerp(kinectYMin, kinectYMax, rawLeft.y);
                    float worldX = Mathf.Lerp(worldXMin, worldXMax, leftX);
                    float worldY = Mathf.Lerp(worldYMin, worldYMax, leftY);
                    targetLeftPos = new Vector3(worldX, worldY, 0f);

                    if (leftHandSprite != null)
                        leftHandSprite.position = Vector3.Lerp(leftHandSprite.position, targetLeftPos, Time.deltaTime * smoothingSpeed);
                }

                // RIGHT HAND
                if (kinectManager.IsJointTracked(userId, (int)KinectInterop.JointType.HandRight))
                {
                    Vector3 rawRight = kinectManager.GetJointPosition(userId, (int)KinectInterop.JointType.HandRight);
                    float rightX = Mathf.InverseLerp(kinectXMin, kinectXMax, rawRight.x);
                    float rightY = Mathf.InverseLerp(kinectYMin, kinectYMax, rawRight.y);
                    float worldX = Mathf.Lerp(worldXMin, worldXMax, rightX);
                    float worldY = Mathf.Lerp(worldYMin, worldYMax, rightY);
                    targetRightPos = new Vector3(worldX, worldY, 0f);

                    if (rightHandSprite != null)
                        rightHandSprite.position = Vector3.Lerp(rightHandSprite.position, targetRightPos, Time.deltaTime * smoothingSpeed);
                }
            }
        }
    }
}
