using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour {
    public GameObject checkpointOBJ;

    public static float X;
    
    private PlayerController controller;
    private FadeOut fadeIn;

    public static CheckpointManager self;

    void Awake() {
        self = this;
        foreach(Transform child in checkpointOBJ.transform) CheckpointLoader.checkpoints.Add(child.GetComponent<Checkpoint>());
        fadeIn = Camera.main.GetComponent<FadeOut>();
        controller = GetComponent<PlayerController>();
        CheckpointLoader.SetCurrent(GetCheckpoint(CheckpointLoader.checkpoint));
        MovePlayer(GetCheckpoint(CheckpointLoader.checkpoint));
    }

    void Update() {
        if(X < transform.position.x) X = transform.position.x;

        foreach(Checkpoint check in CheckpointLoader.checkpoints) {
            if(X > check.x) CheckpointLoader.checkpoint = check.id;
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
        if(i > CheckpointLoader.checkpoints.Count) return CheckpointLoader.checkpoints[0];
        return CheckpointLoader.checkpoints[i];
    }

    public void Die() {
        StartCoroutine(IDie());
    }

    public IEnumerator IDie() {
        yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
