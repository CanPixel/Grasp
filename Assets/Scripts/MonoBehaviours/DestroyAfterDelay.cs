using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour {
    public float delay = 1;

    public bool start = false;

    void Start() {
        if(start) DestroyOBJ();
    }

    public void DestroyOBJ() {
       // Destroy(gameObject, delay);
    }
}
