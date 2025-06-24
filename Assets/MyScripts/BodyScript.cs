using UnityEngine;

public class BodyScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform playerTransform;         // Reference to XR Origin
    [SerializeField] private Transform headTransform; // Usually the camera
    private Quaternion startRotation;
    public float yOffset;
    void Start()
    {
        transform.rotation = Quaternion.Euler(-40,  playerTransform.rotation.eulerAngles.y, playerTransform.rotation.eulerAngles.z);
        startRotation = Quaternion.Euler(1.719f, -3.369f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y + yOffset, playerTransform.position.z + 0.08f);
        
    }

    public void ResetRotation()
    {
        float currentYAngle = headTransform.rotation.eulerAngles.y;
        float rotationDelta = startRotation.y - currentYAngle;

        // Rotate the rig around the head’s current position
        playerTransform.RotateAround(headTransform.position, Vector3.up, rotationDelta);
    }
}
