using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Options")]

    public float timeBetweenCiviSpawn = 3f;
    public float timeBetweenEnemySpawn = 3f;
    public int noOfCiviSpawns = 1;
    public int noOfEnemySpawns = 1;
    public int maxCivilians = 6;
    public int maxEnemies = 10;
        
    [Header("Prefabs")]
    public GameObject civilianPrefab;
    public GameObject enemyPrefab;


    private float timeOfSpawnCivi;
    private float timeOfSpawnEnemy;
    private Transform boundary;
    private float minX;
    private float maxX;
    private float minZ;
    private float maxZ;
    private int noOfCivilians;
    private int noOfEnemies;
    private Transform Civilians;
    private Transform Enemies;

    void Start() {
        boundary = GameObject.Find("Boundary").transform;
        Civilians = GameObject.Find("Civilians").transform;
        Enemies = GameObject.Find("Enemies").transform;
        minX = boundary.position.x - boundary.localScale.x/2;
        maxX = boundary.position.x + boundary.localScale.x/2;
        minZ = boundary.position.z - boundary.localScale.z/2;
        maxZ = boundary.position.z + boundary.localScale.z/2;
        timeOfSpawnEnemy = Time.time;
        timeOfSpawnCivi = Time.time;
        noOfCivilians = 0;
        noOfEnemies = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (((Time.time-timeOfSpawnCivi) >= timeBetweenCiviSpawn)) {
            // Number to spawn = noOfCiviSpawns, unless spawn numbers go above max
            int spawnNo = (noOfCivilians + noOfCiviSpawns > maxCivilians) ? (maxCivilians - noOfCivilians) : noOfCiviSpawns; 
            for (int i=0; i<spawnNo; i++) {
                float xSpawnPosition = Random.Range(minX, maxX);
                float zSpawnPosition = Random.Range(minZ, maxZ);
                
                Vector3 spawnPosition = new Vector3(xSpawnPosition, 1.23f, zSpawnPosition);
                Quaternion rotation = new Quaternion(0,0,0,0);
                Instantiate(civilianPrefab, spawnPosition, rotation, Civilians);
                IncrementCivilianCount();
            }
            timeOfSpawnCivi = Time.time;
            Debug.Log("Civilians spawned");
            Debug.Log("No of Civilians: " + noOfCivilians);
        }
        if (((Time.time-timeOfSpawnEnemy) >= timeBetweenEnemySpawn)) {
            // Number to spawn = noOfEnemySpawns, unless spawn numbers go above max
            int spawnNo = (noOfEnemies + noOfEnemySpawns > maxEnemies) ? (maxEnemies - noOfEnemies) : noOfEnemySpawns; 
            for (int i=0; i<spawnNo; i++) {
                float xSpawnPosition = Random.Range(minX, maxX);
                float zSpawnPosition = Random.Range(minZ, maxZ);
                
                Vector3 spawnPosition = new Vector3(xSpawnPosition, 1.23f, zSpawnPosition);
                Quaternion rotation = new Quaternion(0,0,0,0);
                Instantiate(enemyPrefab, spawnPosition, rotation, Enemies);
                IncrementEnemyCount();
            }
            timeOfSpawnEnemy = Time.time;
            Debug.Log("Enemies spawned");
            Debug.Log("No of Enemies: " + noOfEnemies);
        }
    }

    public void IncrementCivilianCount() {
        noOfCivilians++;
    }

    public void DecrementCivilianCount() {
        noOfCivilians--;
        Debug.Log("No of Civilians: " + noOfCivilians);
    }
    
    public void IncrementEnemyCount() {
        noOfEnemies++;
    }
    
    public void DecrementEnemyCount() {
        noOfEnemies--;
        Debug.Log("No of Enemies: " + noOfEnemies);
    }
}
