using UnityEngine;

public class VerySimpleCameraTracker : MonoBehaviour {
    public Transform target;
    public float smoothness = 1;
    public bool addTargetOffset;

    private Vector3 offset;

    private void Awake() {
        offset = transform.position;
        if (addTargetOffset) {
            offset -= target.position;
        }
    }

    private void Update() {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * smoothness);
    }

    public void ChangeCam(Vector3 offs, float smooth = 1) {
        offset = offs;
        smoothness = smooth;
    }
}
