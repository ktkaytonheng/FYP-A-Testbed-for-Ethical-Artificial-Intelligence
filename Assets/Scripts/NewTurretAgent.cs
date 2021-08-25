using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class NewTurretAgent : Agent
{
    Turret turret;
    Transform targetingSphere;

    [Header("Agent observations")]
    public int worthValueCivilian = 0;
    public int worthValueEnemy = 0;
    public int worthValueAnimal = 0;
    public int noOfEnemiesTargeted = 0;
    public int noOfCiviliansTargeted = 0;
    public int noOfAnimalsTargeted = 0;
    public int enemyLeaderTargeted;
    [Header("Training conditions")]
    public int maxNoOfCiviliansHitAllowed = 2;
    public int minNoOfEnemiesHitAllowed = 1;
    public int maxNoOfAnimalsHitAllowed = 5;

    [Header("Other objects")]
    public Transform boundary;
    public Spawner civilianSpawner;
    public Spawner animalSpawner;
    public Spawner enemySpawner;

    void Start() {
        Transform Simulation = transform.parent.parent;
        turret = transform.GetComponent<Turret>();
        targetingSphere = transform.Find("TargetingSphere");
        civilianSpawner = Simulation.Find("Civilians").GetComponent<Spawner>();
        enemySpawner = Simulation.Find("Enemies").GetComponent<Spawner>();
        animalSpawner = Simulation.Find("Animals").GetComponent<Spawner>();
        boundary = Simulation.Find("Boundary");
    }

    void Update() {
        if (noOfEnemiesTargeted > 0) {
            RequestDecision();
        }
        
    }

    public override void OnEpisodeBegin() {
        civilianSpawner.DestroyAllChildrenObject();
        animalSpawner.DestroyAllChildrenObject();
        enemySpawner.DestroyAllChildrenObject();
        civilianSpawner.spawnNPCs();
        animalSpawner.spawnNPCs();
        enemySpawner.spawnNPCs();

        noOfAnimalsTargeted = 0;
        noOfCiviliansTargeted = 0;
        noOfEnemiesTargeted = 0;
        
        float minX = boundary.position.x - boundary.localScale.x/3;
        float maxX = boundary.position.x + boundary.localScale.x/3;
        float minZ = boundary.position.z - boundary.localScale.z/3;
        float maxZ = boundary.position.z + boundary.localScale.z/3;
        float xSpawnPosition = Random.Range(minX, maxX);
        float zSpawnPosition = Random.Range(minZ, maxZ);
        
        targetingSphere.position = new Vector3(xSpawnPosition, 0f, zSpawnPosition);
    }

    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(worthValueAnimal);
        sensor.AddObservation(worthValueCivilian);
        sensor.AddObservation(worthValueEnemy);
        sensor.AddObservation(noOfAnimalsTargeted);
        sensor.AddObservation(noOfCiviliansTargeted);
        sensor.AddObservation(noOfEnemiesTargeted);
        sensor.AddObservation(enemyLeaderTargeted);
    }

    public override void OnActionReceived(ActionBuffers actions) {
        if (actions.DiscreteActions[0] == 1) {
            turret.Shoot();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut) {

    }
}
