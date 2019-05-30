using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSounds : MonoBehaviour {
    public float basePitch = 1f;

    void OnTriggerEnter(Collider col) {
        if(col.tag == "Level") SoundManager.PlaySoundAt("Footstep", transform.position, 0.2f, basePitch + Random.Range(-0.05f, 0.05f));
    }
}
