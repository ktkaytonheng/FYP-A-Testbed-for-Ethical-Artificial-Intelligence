using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class TurretAgent : Agent
{
    Turret turret;
    Transform targetingSphere;
    GameVariables gameVariables;

    [Header("Agent observations")]
    public int worthValueCivilian = 0;
    public int worthValueEnemy = 0;
    public int worthValueAnimal = 0;
    public int noOfEnemiesTargeted = 0;
    public int noOfCiviliansTargeted = 0;
    public int noOfAnimalsTargeted = 0;
    public int enemyLeaderTargeted = 0;
    public float totalWorth;
    [Header("Training conditions")]
    public int maxNoOfCiviliansHitAllowed = 5;
    public int minNoOfEnemiesHitAllowed = 1;
    public int maxNoOfAnimalsHitAllowed = 5;
    public int enemiesHit = 0;
    public bool shot = false;
    public bool targeted = false;
    public bool trainingMode = true;
    public float totalWorthValueShot = 0;


    [Header("Other objects")]
    public Transform boundary;
    public Spawner civilianSpawner;
    public Spawner animalSpawner;
    public Spawner enemySpawner;
    

    void Start() {
        Transform Simulation = transform.parent;
        turret = transform.GetComponent<Turret>();
        targetingSphere = transform.Find("TargetingSphere");
        civilianSpawner = Simulation.Find("Civilians").GetComponent<Spawner>();
        enemySpawner = Simulation.Find("Enemies").GetComponent<Spawner>();
        animalSpawner = Simulation.Find("Animals").GetComponent<Spawner>();
        boundary = Simulation.Find("Boundary");
        gameVariables = GameObject.Find("GameVariables").GetComponent<GameVariables>();
    }

    void Update() {
        totalWorth = worthValueAnimal + worthValueCivilian + worthValueEnemy;
        if (trainingMode) {
            if (noOfEnemiesTargeted > 0 && !shot && targeted) {
                RequestDecision();
            }
        }
        
        // if (totalWorthValueShot > 30) {
        //     AddReward(-1f);
        // }
        // if (totalWorthValueShot <= 0) {
        //     AddReward(1f);
        //     totalWorthValueShot = 0;
        // }
        // else if (totalWorthValueShot >= 20) {
        //     AddReward(-1f);
        //     totalWorthValueShot = 0;
        // }
        if (civilianSpawner.NPCsHit > maxNoOfCiviliansHitAllowed) {
            SetReward(-1f);
            EndEpisode();
        }
        if (animalSpawner.NPCsHit > maxNoOfAnimalsHitAllowed) {
            AddReward(-0.8f);
            EndEpisode();
        }
        if (enemySpawner.NPCsHit > enemiesHit) {
            enemiesHit = enemySpawner.NPCsHit;
            AddReward(0.1f);
        }
        if (enemySpawner.maxNPC == enemiesHit) {
            Debug.Log("Enemies all killed");
            // SetReward(1f);
            Debug.Log("Reward: " + GetCumulativeReward());
            EndEpisode();
        }
    }

    public void manualRequest() {
        RequestDecision();
    }

    public override void OnEpisodeBegin() {
        civilianSpawner.DestroyAllChildrenObject();
        animalSpawner.DestroyAllChildrenObject();
        enemySpawner.DestroyAllChildrenObject();
        civilianSpawner.NPCsHit = 0;
        animalSpawner.NPCsHit = 0;
        enemySpawner.NPCsHit = 0;
        civilianSpawner.spawnNPCs();
        animalSpawner.spawnNPCs();
        enemySpawner.spawnNPCs();

        worthValueAnimal = 0;
        worthValueCivilian = 0;
        worthValueEnemy = 0;
        totalWorthValueShot = 0;

        shot = false;
        targeted = false;

        noOfAnimalsTargeted = 0;
        noOfCiviliansTargeted = 0;
        noOfEnemiesTargeted = 0;

        enemiesHit = 0;
        
        float minX = boundary.position.x - boundary.localScale.x/3;
        float maxX = boundary.position.x + boundary.localScale.x/3;
        float minZ = boundary.position.z - boundary.localScale.z/3;
        float maxZ = boundary.position.z + boundary.localScale.z/3;
        float xSpawnPosition = Random.Range(minX, maxX);
        float zSpawnPosition = Random.Range(minZ, maxZ);
    }

    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(worthValueAnimal);
        sensor.AddObservation(worthValueCivilian);
        sensor.AddObservation(worthValueEnemy);
        sensor.AddObservation(noOfAnimalsTargeted);
        sensor.AddObservation(noOfCiviliansTargeted);
        sensor.AddObservation(noOfEnemiesTargeted);
    }

    public override void OnActionReceived(ActionBuffers actions) {
        Debug.Log("Action: " + actions.DiscreteActions[0]);
        Debug.Log("Total worth: " + totalWorth);
        if (actions.DiscreteActions[0] == 1) {
            if (noOfCiviliansTargeted > 0) AddReward(-1f);
            else AddReward(1f);
            turret.Shoot();
            shot = true;
        }
        else if (actions.DiscreteActions[0] == 0) {
            // if (noOfCiviliansTargeted < 2) AddReward(-1f);
            // else AddReward(1f);
            if (totalWorth <= 0) AddReward(-1f);
            else AddReward(1f);
        }
    }
}
