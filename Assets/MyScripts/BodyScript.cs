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
        transform.rotation = Quaternion.Euler(-40,  headTransform.rotation.eulerAngles.y, headTransform.rotation.eulerAngles.z);
        startRotation = Quaternion.Euler(1.719f, -3.369f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(headTransform.position.x, headTransform.position.y + yOffset, headTransform.position.z + 0.08f);
        transform.rotation = Quaternion.Euler(-40, headTransform.rotation.y, 0);
    }

    public void ResetRotation()
    {
        float currentYAngle = headTransform.rotation.eulerAngles.y;
        float rotationDelta = startRotation.y - currentYAngle;

        // Rotate the rig around the head’s current position
        playerTransform.RotateAround(headTransform.position, Vector3.up, rotationDelta);
    }
}
