using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverbHandler : MonoBehaviour {
    private AudioReverbZone reverbZone;

    private static ReverbHandler self;

    void Awake() {
        self = this;
        reverbZone = GetComponent<AudioReverbZone>();
    }

   public static void SetReverbType(AudioReverbPreset preset) {
        self.reverbZone.reverbPreset = preset;
    }
}
