using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour {
    public GameObject checkpointOBJ;

    [Range(0, 11)]
    public int beginAt = 0;

    public static float X;
    
    private List<Checkpoint> checkpoints = new List<Checkpoint>();

    private PlayerController controller;
    private FadeOut fadeIn;

    void Awake() {
        fadeIn = Camera.main.GetComponent<FadeOut>();
        controller = GetComponent<PlayerController>();
        foreach(Transform child in checkpointOBJ.transform) checkpoints.Add(child.GetComponent<Checkpoint>());
    }

    void Start() {
        current = GetCheckpoint(beginAt);
        MovePlayer(current);
    }

    [Header("Leave Empty")]
    public Checkpoint current;

    void Update() {
        if(X < transform.position.x) X = transform.position.x;

        foreach(Checkpoint check in checkpoints) {
            if(X > check.x) current = check;
            else break;
        }
    }

    public void MovePlayer(Vector2 pos) {
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        Camera.main.transform.position = new Vector3(pos.x, pos.y, Camera.main.transform.position.z);
    }

    public void MovePlayer(Checkpoint check) {
        MovePlayer(new Vector2(check.x, check.y));
    }

    public Checkpoint GetCheckpoint(int i) {
        if(i > checkpoints.Count) return checkpoints[0];
        return checkpoints[i];
    }

    public void Die() {
        MovePlayer(current);
        controller.lockControls = false;
        fadeIn.reset();
    }
}
