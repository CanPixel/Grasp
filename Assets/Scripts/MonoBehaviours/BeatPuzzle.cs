using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatPuzzle : MonoBehaviour {
    void Start() {
        
    }

    void Update() {
        
    }

    void OnTriggerEnter(Collider col) {
        if(col.tag == "Player") {
            var controller = col.GetComponent<PlayerController>();
            controller.lockControls = true;
        }
    }
}
