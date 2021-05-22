using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Options")]
    public string NPCType;
    public float timeBetweenNPCSpawn = 3f;
    public int noOfNPCSpawns = 1;
    public int maxNPC = 8;
        
    [Header("Prefabs")]
    public GameObject NPCPrefab;


    private float timeOfSpawnNPC;
    private Transform boundary;
    private float minX;
    private float maxX;
    private float minZ;
    private float maxZ;
    private int noOfNPC;
    private Transform NPCs;

    void Start() {
        boundary = GameObject.Find("Boundary").transform;
        NPCType = gameObject.name;
        NPCs = gameObject.transform;
        minX = boundary.position.x - boundary.localScale.x/2;
        maxX = boundary.position.x + boundary.localScale.x/2;
        minZ = boundary.position.z - boundary.localScale.z/2;
        maxZ = boundary.position.z + boundary.localScale.z/2;
        timeOfSpawnNPC = Time.time;
        noOfNPC = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (((Time.time-timeOfSpawnNPC) >= timeBetweenNPCSpawn)) {
            // Number to spawn = noOfCiviSpawns, unless spawn numbers go above max
            int spawnNo = (noOfNPC + noOfNPCSpawns > maxNPC) ? (maxNPC - noOfNPC) : noOfNPCSpawns; 
            for (int i=0; i<spawnNo; i++) {
                float xSpawnPosition = Random.Range(minX, maxX);
                float zSpawnPosition = Random.Range(minZ, maxZ);
                
                Vector3 spawnPosition = new Vector3(xSpawnPosition, 1.23f, zSpawnPosition);
                Quaternion rotation = new Quaternion(0,0,0,0);
                Instantiate(NPCPrefab, spawnPosition, rotation, NPCs);
                IncrementNPCCount();
            }
            timeOfSpawnNPC = Time.time;
            Debug.Log(gameObject.name + " spawned");
            Debug.LogFormat("No of {0}: {1}", gameObject.name, noOfNPC);
        }
    }

    public void IncrementNPCCount() {
        noOfNPC++;
    }

    public void DecrementNPCCount() {
        noOfNPC--;
        Debug.LogFormat("No of {0}: {1}", gameObject.name, noOfNPC);
    }
}
