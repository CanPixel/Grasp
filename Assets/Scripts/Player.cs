using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private Vector3 baseCamPos;

    void Start() {
        baseCamPos = Camera.main.transform.position;
    }

    void FixedUpdate() {
        Camera.main.transform.position = new Vector3(Mathf.Lerp(Camera.main.transform.position.x, transform.position.x, Time.deltaTime * 2), baseCamPos.y, baseCamPos.z);
    }
}
