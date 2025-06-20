using UnityEngine;

public class RoomLimits : MonoBehaviour
{
    public float yLimitTop;
    public float yLimitBottom;

    private GameObject meshVolumeObj;
    // Start is called once before the first eyecution of Update after the MonoBehaviour is created
    void Start()
    {
        yLimitTop = 4.7f;
        yLimitBottom = -1.3f;
        meshVolumeObj = FindFirstObjectByType<RoomMeshAnchor>().gameObject;
        meshVolumeObj.transform.position = new Vector3(0, 0, 0);
        meshVolumeObj.transform.Rotate(0,0,0);
        
    }

    // Update is called once per frame
    void Update()
    {
       
        if (transform.position.y < yLimitBottom)
        {
            transform.position = new Vector3(transform.position.x, yLimitBottom, transform.position.z);
        }
        
        if (transform.position.y > yLimitTop)
        {
            transform.position = new Vector3(transform.position.x, yLimitTop, transform.position.z);
        }
        
    }
}
