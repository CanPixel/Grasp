using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static bool MUTE = false;

    public AudioReverbPreset baseReverbPreset;

    [Header("Volume Levels")]
    [Range(0f, 1f)]
    public float ambienceVolume = 0.6f;
    public float playerVolume = 0.8f;

    public static float PLAYER_VOLUME;

    [Header("Sound Collection")]
    public AudioClip[] ambientSounds;
    [Space(5)]
    public AudioClip[] sounds;
    private string[] names;

    private static SoundManager instance;

    private float ambientTimer = 0;
    private bool ambientPlaying = false;
    private AudioSource lastAmbient;

    void Awake() {
        PLAYER_VOLUME = playerVolume;
        instance = this;
        names = new string[sounds.Length];
		for(int i = 0; i < sounds.Length; i++) names[i] = sounds[i].name;
    }

    void LateUpdate() {
        ambientTimer += Time.deltaTime;

        if(ambientTimer > 3 && Random.Range(0, 10) < 5 && !ambientPlaying) PlayAmbient();
        if(ambientPlaying && lastAmbient == null) ResetAmbience();
    }

    private void ResetAmbience() {
        ambientPlaying = false;
        ambientTimer = 0;
    }

    private void PlayAmbient() {
        ambientPlaying = true;
        lastAmbient = PlaySound(ambientSounds[Random.Range(0, ambientSounds.Length)], ambienceVolume, Random.Range(0.95f, 1.05f));
    }

    public static AudioSource PlaySound(AudioClip clip, float volume = 1, float pitch = 1) {
         if(MUTE || instance == null) return null;
		var temp = new GameObject("2D TempAudio");
		temp.transform.position = Camera.main.transform.position;
		var source = temp.AddComponent<AudioSource>();
        source.clip = clip;
        source.pitch = pitch;
        source.volume = volume;
        source.spatialBlend = 0;
        source.Play();
        Destroy(temp, source.clip.length);
        return source;
    }

    public static AudioSource PlaySound(string name, float volume = 1, float pitch = 1) {
        return PlaySoundAt(name, Vector3.zero, volume, pitch, true);
	}

    public static AudioSource PlaySoundAt(string name, Vector3 pos, float volume = 1, float pitch = 1, bool twoDimensional = false) {
        if(MUTE || instance == null) return null;
		var temp = new GameObject(name);
		temp.transform.position = pos;
		var source = temp.AddComponent<AudioSource>();
        for(int i = 0; i < instance.sounds.Length; i++) {
			if(name == instance.names[i]) {
                source.clip = instance.sounds[i];
                source.pitch = pitch;
                source.volume = volume;
                source.spatialBlend = (twoDimensional)? 0 : 1;
                source.Play();
                Destroy(temp, source.clip.length);
				return source;
			}
		}
		Debug.LogError("Could not find '" + name + "' sound file!");
        return null;
	}

    public static AudioReverbPreset GetAudioReverbPreset() {
        return instance.baseReverbPreset;
    }
}
