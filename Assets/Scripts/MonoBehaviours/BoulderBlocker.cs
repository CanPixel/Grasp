using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderBlocker : MonoBehaviour {
    void OnTriggerEnter(Collider col) {
        if(col.tag == "Boulder") col.GetComponent<Rigidbody>().isKinematic = true;
    }
}
