using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour {
    public float chunkSize = 5;
    public float worldSize = 10;

    public GameObject ChunkPrefab, ChunkWallPrefab;
    
    void Start() {
        GenerateChunks();
    }

    protected void GenerateChunks() {
        SpawnWalledChunk(0);
        int i = 1;
        for(i = 1; i < worldSize; i++) SpawnChunk(i * chunkSize);
        SpawnWalledChunk(i * chunkSize, 180);
    }

    private void SpawnWalledChunk(float x, float angle = 0) {
        GameObject chunk = Instantiate(ChunkWallPrefab);
        chunk.name = ChunkPrefab.name;
        chunk.transform.SetParent(transform);
        chunk.transform.position = new Vector3(x, 0, 0);
        chunk.transform.rotation = Quaternion.Euler(0, angle, 0);
        chunk.GetComponent<Chunk>().floor.GetComponent<Renderer>().material.color = Random.ColorHSV();
        chunk.GetComponent<Chunk>().SetSize(chunkSize);
    }

    private void SpawnChunk(float x) {
        GameObject chunk = Instantiate(ChunkPrefab);
        chunk.name = ChunkPrefab.name;
        chunk.transform.SetParent(transform);
        chunk.transform.position = new Vector3(x, 0, 0);
        chunk.GetComponent<Chunk>().floor.GetComponent<Renderer>().material.color = Random.ColorHSV();
        chunk.GetComponent<Chunk>().SetSize(chunkSize);
    }
}
