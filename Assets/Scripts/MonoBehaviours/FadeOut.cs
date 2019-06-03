using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour {
    public RawImage overlay;
    public PlayerController pc;

    private bool fade = false;
    
    void FixedUpdate() {
        if(fade) {
            overlay.enabled = true;
            overlay.color = Color.Lerp(overlay.color, new Color(0, 0, 0, 1f), Time.deltaTime * 1.2f);
            AudioListener.volume = Mathf.Lerp(AudioListener.volume, 0, Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider col) {
        if(col.tag == "Player") {
            fade = true;
            pc.lockControls = true;
        }
    }
}
