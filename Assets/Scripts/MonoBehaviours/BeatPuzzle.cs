using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeatPuzzle : MonoBehaviour {
    public GameObject door, button;

    private bool puzzle = false, finished = false;

    public static int bpm;
	private double nextTime;
	private double sampleRate = 0;
	private bool ticked = false;

	private double currentDSP;
    private double lastPulse, lastClick;

    public float sensitivity = 0.3f;

    private int hits = 0, lastHit = 0;

    private PlayerController pc;

    public int HitsUntilWin = 3;

    private float pushDelay = 0;

    void Start() {
        double startTime = currentDSP = AudioSettings.dspTime;
		sampleRate = AudioSettings.outputSampleRate;
		nextTime = startTime + (60.0 / SoundManager.GetBPM());
    }
    
    void LateUpdate() {
        if(!ticked && nextTime >= AudioSettings.dspTime && puzzle) {
			ticked = true;
			BroadcastMessage("OnPulse");
		}
    }

    void Update() {
        if(pushDelay > 0) pushDelay += Time.deltaTime;
        button.transform.localPosition = new Vector3(-0.5f + Mathf.Sin(pushDelay * 2) / 10, button.transform.localPosition.y, button.transform.localPosition.z);
        if(pushDelay > 1.5f) pushDelay = 0;

        if(finished) {
            Vector3 tar = door.transform.localPosition;
            tar.y = -1;
            door.transform.localPosition = Vector3.Lerp(door.transform.localPosition, tar, Time.deltaTime / 5);
        } else {
            if(Input.GetMouseButtonDown(1)) lastClick = AudioSettings.dspTime;
        }
    }

    void OnPulse() {
        lastPulse = AudioSettings.dspTime;
        
        double accuracy = Mathf.Abs((float)(lastPulse - lastClick));
        if(accuracy < sensitivity) {
            hits++;
            pushDelay = 0.1f;
            SoundManager.PlaySoundAt("Clap", transform.position);
        }
        else {
            hits = 0;
            if(lastHit != 0 && hits == 0) SoundManager.PlaySoundAt("Clap", transform.position, 1f, 0.5f);
        }
        lastClick = 1;
        lastHit = hits;

        if(hits >= HitsUntilWin) Finish();
    }

    protected void Finish() {
        SoundManager.PlaySound("CintiqDoor", 1f);
        hits = 0;
        finished = true;
        pc.lockControls = false;
        SoundManager.PlaySound("Scrape", 1f);
    }

    void FixedUpdate() {
        if(finished) return;
		double timePerTick = 60.0 / SoundManager.GetBPM();
		double dspTime = AudioSettings.dspTime;

		while(dspTime >= nextTime) {
			ticked = false;
			nextTime += timePerTick;
			currentDSP = AudioSettings.dspTime;
		}
    }

    void OnTriggerEnter(Collider col) {
        if(col.tag == "Player") {
            pc = col.GetComponent<PlayerController>();
            pc.lockControls = true;
            puzzle = true;
            SoundManager.PlaySoundAt("High", transform.position, 2f);
        }
    }

    public void LoadMenu() {
        SceneManager.LoadScene(0);
    }
}
