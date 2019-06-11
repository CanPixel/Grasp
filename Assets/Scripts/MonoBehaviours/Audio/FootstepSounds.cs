using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSounds : MonoBehaviour {
    public float basePitch = 1f;

    public Rigidbody rb;

    public bool spacialized = false;

    void OnTriggerEnter(Collider col) {
        if(col.tag == "Level") {
            float pitch = basePitch;
            float impact = Mathf.Clamp(Mathf.Abs(rb.velocity.y) / 5, 0.1f, 0.5f);
            if(impact <= 0.1f) pitch -= 0.5f;
            if(!spacialized) SoundManager.PlaySound("Footstep", impact, pitch + Random.Range(-0.05f, 0.05f));
            else  SoundManager.PlaySoundAt("Footstep", transform.position, impact, pitch + Random.Range(-0.05f, 0.05f));
        }
    }
}
