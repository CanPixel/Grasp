using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {
    [HideInInspector]
    public float x, y;

    public int id;

    void Awake() {
        x = transform.position.x;
        y = transform.position.y;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - Vector3.up * 20, transform.position + Vector3.up * 40);
    }
}
