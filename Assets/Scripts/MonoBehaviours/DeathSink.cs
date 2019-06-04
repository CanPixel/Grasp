using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSink : MonoBehaviour {
    private float sink = 500;
    public FadeOut fade;
    public PlayerController playerController;

    private float outOfLake = 0;
    private bool inLake = false;

    void Update() {
        if(sink < 400) CinematicCam.shakeSpeed = (1 / sink) * 500f;
        if(sink < 150) {
            fade.Fade();
            if(sink < 100) playerController.lockControls = true;
        }

        if(outOfLake > 0) outOfLake += Time.deltaTime;
        if(outOfLake > 1.2f) {
            sink = 500;
            outOfLake = 0;
            playerController.SetJump(true);
        }
    }

    void OnTriggerStay(Collider col) {
        if((col.tag == "Enemy" || col.tag == "Player") && col.GetComponent<Rigidbody>() != null) {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if(col.tag == "Player") playerController.SetJump(false);
            rb.AddForce(sink * Vector3.up);
            outOfLake = 0;
            if(sink > 0) sink -= 1;
        }
    }

    void OnTriggerEnter(Collider col) {
        if(col.tag == "Player") {
            inLake = true;
            if(Random.Range(0, 2) == 0) SoundManager.PlaySoundAt("Splash1", col.transform.position);
            else SoundManager.PlaySoundAt("Splash2", col.transform.position);
        }
    }

    void OnTriggerExit(Collider col) {
        if((col.tag == "Enemy" || col.tag == "Player") && sink < 500) {
            sink = Mathf.Clamp(sink + 20, 0, 500);
            outOfLake = 0.1f;
            inLake = false;
            if(sink > 400) playerController.SetJump(true);
        }
    }
}
