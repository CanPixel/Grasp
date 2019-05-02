using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatUI : MonoBehaviour {
    private Vector3 basePos;
    public Vector3 direction;

    public float speed;

    void Start() {
        basePos = transform.localPosition;
    }

    void FixedUpdate() {
        transform.localPosition = new Vector3(basePos.x + Mathf.Sin(Time.time * speed) * direction.x, basePos.y + Mathf.Sin(Time.time * speed) * direction.y, basePos.z + Mathf.Sin(Time.time * speed) * direction.z);    
    }
}
