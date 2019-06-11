using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FadeOut : MonoBehaviour {
    public RawImage overlay;
    public PlayerController pc;

    private bool fade = false, done = false;

    public bool OnTrigger = true, FadeIn = false;

    public float fadeSpeed = 0.5f;

    public UnityEvent postFade;

    void Start() {
        AudioListener.volume = 1;
        StartFadeIn();
    }

    protected void StartFadeIn() {
        if(FadeIn) {
            overlay.color = new Color(0, 0, 0, 1f);
            fade = true;
        }
    }
    
    void LateUpdate() {
        if(fade) {
            overlay.enabled = true;
            if(FadeIn) {
                if(!done) {
                    if(overlay.color.a > 0.1f) overlay.color = Color.Lerp(overlay.color, new Color(0, 0, 0, 0), Time.deltaTime * fadeSpeed);
                    else done = true;
                } 
            }
            else {
                if(!done) {
                    overlay.color = Color.Lerp(overlay.color, new Color(0, 0, 0, 1f), Time.deltaTime * 1.2f);
                    AudioListener.volume = Mathf.Lerp(AudioListener.volume, 0, Time.deltaTime);
                    if(overlay.color.a > 0.975f) {
                        done = true;
                        reset();
                        postFade.Invoke();
                    }
                }
            }
        }
    }

    public void reset() {
        fade = false;
        done = false;
        StartFadeIn();
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
