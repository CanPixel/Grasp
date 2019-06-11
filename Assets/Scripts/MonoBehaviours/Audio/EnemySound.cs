using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySound : MonoBehaviour {
    public AudioSource scream;
    public AudioSource hit;

    public AudioClip[] idle;

    private float delay = 0;

    private AudioSource last;

    private float randDelay;

    public void Scream() {
        if(scream.isPlaying) return;
        scream.Play();
    }

    public void Attack() {
        hit.Play();
    }

    void Update() {
        delay += Time.deltaTime;
        if(delay > 2 + randDelay) {
            if(last != null && last.isPlaying) return;
            try {
                AudioClip  rand = idle[Random.Range(0, idle.Length)];
                last = SoundManager.PlaySoundAt(rand, transform.position, 1.2f, Random.Range(0.95f, 1.15f));
                delay = 0;
                randDelay = Random.Range(0.1f, 1.5f);
            } catch(System.Exception) {}
        }
    }
}
