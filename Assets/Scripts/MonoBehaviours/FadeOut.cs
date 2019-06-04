using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour {
    public RawImage overlay;
    public PlayerController pc;

    private bool fade = false;

    public bool OnTrigger = true, FadeIn = false;

    void Start() {
        if(FadeIn) {
            overlay.color = new Color(0, 0, 0, 1f);
            fade = true;
        }
    }
    
    void FixedUpdate() {
        if(fade) {
            overlay.enabled = true;
            if(FadeIn) overlay.color = Color.Lerp(overlay.color, new Color(0, 0, 0, 0), Time.deltaTime * 0.5f);
            else {
                overlay.color = Color.Lerp(overlay.color, new Color(0, 0, 0, 1f), Time.deltaTime * 1.2f);
                AudioListener.volume = Mathf.Lerp(AudioListener.volume, 0, Time.deltaTime);
            }
        }
    }

    void OnTriggerEnter(Collider col) {
        if(!OnTrigger) return;
        if(col.tag == "Player") {
            fade = true;
            pc.lockControls = true;
        }
    }

    public void Fade() {
        fade = true;
    }
}
