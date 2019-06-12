using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTree : MonoBehaviour {
    private float hitDelay = 0;
    private int hit = 0;

    public Transform mesh;
    private bool fall = false;
    
    private Rigidbody rb;

    void Awake() {
        rb = mesh.GetComponent<Rigidbody>();
    }

    void Update() {
        if(!fall) {
            Quaternion rot;
            if(hitDelay > 0) { 
                hitDelay -= Time.deltaTime;
                float rot1 = Mathf.Sin(hitDelay * 8) * 10;
                float rot2 = Mathf.Cos(hitDelay * 8) * 10;
                rot = Quaternion.Euler(rot1, rot2, rot1);
            } else rot = Quaternion.Euler(Mathf.Sin(Time.time / 1.2f) * 3, 0, 0);
            transform.rotation = Quaternion.Lerp(mesh.rotation, rot, Time.deltaTime * 3);
        }
    }

    void OnTriggerStay(Collider col) {
        if(col.gameObject.tag == "Light" && hitDelay <= 0 && Flashlight.isBeamLight()) Hit();
    }

    protected void Hit() {
        SoundManager.PlaySound("Splash1");    
        if(fall) return;
        hitDelay = 0.8f;
        hit++;
        if(hit >= 2) Fall();
    }

    protected void Fall() {
        SoundManager.PlaySound("Splash1");
        SoundManager.PlaySound("CintiqDoor");
        Destroy(GetComponent<BoxCollider>());
        fall = true;
        rb.isKinematic = false;
        mesh.localRotation = Quaternion.Euler(0, 0, -6);
    }
}
