using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSounds : MonoBehaviour {
    public float basePitch = 1f;

    public PlayerController pC;

    void OnTriggerEnter(Collider col) {
        if(col.tag == "Level") {
            float pitch = basePitch;
            float impact = Mathf.Clamp(Mathf.Abs(pC.rigidbody.velocity.y) / 5, 0.1f, 0.5f);
            if(impact <= 0.1f) pitch -= 0.5f;
            SoundManager.PlaySoundAt("Footstep", transform.position, impact, pitch + Random.Range(-0.05f, 0.05f));
        }
    }
}
