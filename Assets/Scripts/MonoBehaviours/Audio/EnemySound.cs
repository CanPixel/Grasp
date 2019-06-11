using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySound : MonoBehaviour {
    public AudioSource scream;

    public void Scream() {
        if(scream.isPlaying) return;
        scream.Play();
    }
}
