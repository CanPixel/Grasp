using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScale : MonoBehaviour {
    private static List<BeatScale> beaters = new List<BeatScale>();

    private Vector3 originScale;

    [Range(0, 3f)]
    public float sensitivity = 1.2f;

    void Start() {
        originScale = transform.localScale;
        beaters.Add(this);
    }

    void FixedUpdate() {
        transform.localScale = Vector3.Lerp(transform.localScale, originScale, Time.deltaTime * 2);
    }

    private void BeatIndiv() {
        transform.localScale *= sensitivity;
    }

    public static void Beat() {
        foreach(BeatScale beat in beaters) beat.BeatIndiv();
    }
}
