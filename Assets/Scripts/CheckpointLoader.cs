using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointLoader : MonoBehaviour
{
    public static List<Checkpoint> checkpoints = new List<Checkpoint>();
    public static int checkpoint = 0;
    private static CheckpointLoader self;

    public Checkpoint current;

    [Range(0, 11)]
    public int beginAt;
    
    void Awake()
    {
        if(self != null) {
            Destroy(gameObject);
            return;
        }
        self = this;
        DontDestroyOnLoad(gameObject);
        checkpoint = beginAt;
    }

    public static Checkpoint GetCurrent() {
        return self.current;
    }

    public static void SetCurrent(Checkpoint check) {
        self.current = check;
    }
}
