using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CinematicCam : MonoBehaviour {
    private GameObject currentArea;
    private CameraArea camArea;

    private VerySimpleCameraTracker cm;
    public static bool enemyZoom = false;
    public static Transform zoomTarget;
    public static float zoomOffset;
    public static Transform enemy;

    public static float shakeSpeed = 0;
    public static float shakeAmp = 10;

    private AudioSource grain;

    public PostProcessVolume post;
    private ChromaticAberration chroma;
    private Vignette vignette;

    void Start() {
        grain = Camera.main.GetComponent<AudioSource>();
        cm = GetComponent<VerySimpleCameraTracker>();
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        camArea = null;
        enemyZoom = false;

        post.profile.TryGetSettings(out chroma);
        post.profile.TryGetSettings(out vignette);
    }

    void OnTriggerEnter(Collider col) {
        if(!enemyZoom && col.gameObject.tag == "CamArea" && currentArea != col.gameObject) {
            currentArea = col.gameObject;
            camArea = currentArea.GetComponent<CameraArea>();
        }
    }

    void LateUpdate() {
        if(enemy == null) enemyZoom = false;

        if(camArea != null && !enemyZoom) {
            cm.ChangeCam(camArea.offset, camArea.speed);
            float xShake = Mathf.Cos(Time.time * shakeSpeed) * shakeAmp;
            float yShake = Mathf.Sin(Time.time * shakeSpeed) * shakeAmp;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(camArea.rotation.x, camArea.rotation.y, camArea.rotation.z), Time.deltaTime * camArea.speed);
        }
        if(enemyZoom && camArea != null) {
            float closeness = Mathf.Clamp(Mathf.Abs(1.2f / zoomOffset), 0, 1);
            cm.ChangeCam(new Vector3(camArea.offset.x, camArea.offset.y, zoomOffset), 1.2f);
            transform.LookAt(zoomTarget, transform.up);
            float xShake = Mathf.Cos(Time.time * closeness * 5) / 4;
            float yShake = Mathf.Sin(Time.time * closeness * 5) / 4;
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + xShake, transform.localEulerAngles.z + yShake);
            grain.volume = Mathf.Lerp(grain.volume, closeness, Time.deltaTime * 2);
            chroma.intensity.value = Mathf.Lerp(grain.volume, closeness, Time.deltaTime * 2);
            vignette.intensity.value  = Mathf.Lerp(grain.volume, closeness, Time.deltaTime * 1);
        }
        if(!enemyZoom) vignette.intensity.value = chroma.intensity.value = grain.volume = Mathf.Lerp(grain.volume, 0, Time.deltaTime * 2);
    }
}
