using Meta.XR.MRUtilityKit;
using UnityEngine;

public class RoomLimits : MonoBehaviour
{   
    private GameObject meshVolumeObj;
    private Vector3 startPosition;
    private ControllerWaves scriptWaves;
    // Start is called once before the first eyecution of Update after the MonoBehaviour is created
    void Start()
    {
        /*
        meshVolumeObj = FindFirstObjectByType<MRUKRoom>().gameObject;
        meshVolumeObj.transform.position = new Vector3(0, 0, 0);
        meshVolumeObj.transform.Rotate(0,0,0);
        */
        scriptWaves = GameObject.Find("WavesManipulator").GetComponent<ControllerWaves>();
        startPosition = transform.position;
    }

    
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, startPosition.y + (scriptWaves.WaterDepth / 4), transform.position.z);
    }
}
