using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour {
    public GameObject checkpointOBJ;

    [Range(0, 11)]
    public int beginAt = 0;

    public static float X;
    
    private List<Checkpoint> checkpoints = new List<Checkpoint>();

    void Awake() {
        foreach(Transform child in checkpointOBJ.transform) checkpoints.Add(child.GetComponent<Checkpoint>());
    }

    void Start() {
        MovePlayer(GetCheckpoint(beginAt));
    }

    [Header("Leave Empty")]
    public Checkpoint current;

    void Update() {
        X = transform.position.x;

        foreach(Checkpoint check in checkpoints) {
            if(X > check.x) current = check;
            else break;
        }
    }

    public void MovePlayer(Vector2 pos) {
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        Camera.main.transform.position = new Vector3(pos.x, pos.y, Camera.main.transform.position.z);
    }

    public Vector2 GetCheckpoint(int i) {
        if(i > checkpoints.Count) return new Vector2(checkpoints[0].x, checkpoints[0].y);
        return new Vector2(checkpoints[i].x, checkpoints[i].y);
    }
}
