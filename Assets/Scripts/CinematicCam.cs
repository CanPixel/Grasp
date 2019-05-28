﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicCam : MonoBehaviour {
    private GameObject currentArea;
    private CameraArea camArea;

    private VerySimpleCameraTracker cm;

    void Awake() {
        cm = GetComponent<VerySimpleCameraTracker>();
    }

    void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "CamArea" && currentArea != col.gameObject) {
            currentArea = col.gameObject;
            camArea = currentArea.GetComponent<CameraArea>();
        }
    }

    void FixedUpdate() {
        if(camArea != null) {
            cm.ChangeCam(camArea.offset, camArea.speed);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(camArea.rotation), Time.deltaTime * camArea.speed);
        }
    }
}