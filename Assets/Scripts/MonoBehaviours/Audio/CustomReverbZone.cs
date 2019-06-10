using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomReverbZone : MonoBehaviour {
    public AudioReverbPreset preset;

    void OnTriggerStay(Collider col) {
        if(col.CompareTag("Player")) ReverbHandler.SetReverbType(preset);
    }

    void OnTriggerExit(Collider col) {
        if(col.CompareTag("Player")) ReverbHandler.SetReverbType(SoundManager.GetAudioReverbPreset());
    }
}
