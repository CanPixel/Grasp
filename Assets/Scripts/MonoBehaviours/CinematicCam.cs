using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicCam : MonoBehaviour {
    private GameObject currentArea;
    private CameraArea camArea;

    private VerySimpleCameraTracker cm;
    public static bool enemyZoom = false;
    public static Transform zoomTarget;
    public static float zoomOffset;

    public static float shakeSpeed = 0;
    public static float shakeAmp = 10;

    void Start() {
        cm = GetComponent<VerySimpleCameraTracker>();
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        camArea = null;
        enemyZoom = false;
    }

    void OnTriggerEnter(Collider col) {
        if(!enemyZoom && col.gameObject.tag == "CamArea" && currentArea != col.gameObject) {
            currentArea = col.gameObject;
            camArea = currentArea.GetComponent<CameraArea>();
        }
    }

    void FixedUpdate() {
        if(camArea != null && !enemyZoom) {
            cm.ChangeCam(camArea.offset, camArea.speed);
            float xShake = Mathf.Cos(Time.time * shakeSpeed) * shakeAmp;
            float yShake = Mathf.Sin(Time.time * shakeSpeed) * shakeAmp;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(camArea.rotation.x, camArea.rotation.y, camArea.rotation.z), Time.deltaTime * camArea.speed);
        }
        if(enemyZoom && camArea != null) {
            cm.ChangeCam(new Vector3(camArea.offset.x, camArea.offset.y, zoomOffset), 1.2f);
            transform.LookAt(zoomTarget, transform.up);
           // transform.localRotation = Quaternion.LookRotation(zoomTarget.position);
        }
    }
}
