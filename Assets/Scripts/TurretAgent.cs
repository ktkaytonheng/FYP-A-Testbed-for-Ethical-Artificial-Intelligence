using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class TurretAgent : Agent
{
    public Turret turret;
    public Transform walls;
    public Spawner civilianSpawner;
    public float maxCivi = 5;
    public Spawner animalSpawner;
    public float maxAnimal = 8;
    public Spawner enemySpawner;
    public float maxEnemy = 1;
    public Transform boundary;
    public Transform targetingArea;
    public Material winMaterial;
    public Material normalMaterial;
    public Transform target = null;
    public float worthValue;
    public float shootCooldown = 3f;
    public bool enemyTargeted = false;
    public int noOfEnemyTargeted = 0;
    public int noOfCivilianTargeted = 0;
    public float lastDebug;

    void Start() {
        Transform Simulation = transform.parent.parent;
        boundary = Simulation.Find("Boundary");
        civilianSpawner = Simulation.Find("Civilians").GetComponent<Spawner>();
        enemySpawner = Simulation.Find("Enemies").GetComponent<Spawner>();
        animalSpawner = Simulation.Find("Animals").GetComponent<Spawner>();
        turret = Simulation.Find("Turret").GetComponent<Turret>();
        walls = Simulation.Find("Walls");
        targetingArea = turret.transform.Find("TargetingSphere");
        lastDebug = Time.time;
        Debug.Log("Starting " + Simulation.name + "@ " + lastDebug);
    }
    void Update() {
        // if (civilianSpawner.noOfNPC < civilianSpawner.maxNPC * 0.75) {
        //     EndEpisode();
        //     SetReward(-10);
        // }
        // if (enemySpawner.noOfNPC == 0) {
        //     EndEpisode();
        //     SetReward(10);
        // }
        // if (noOfEnemyTargeted > 0) {
        //     // Debug.Log("Targeting");
        //     AddReward(0.01f);
        // }
        // else {
        //     AddReward(-0.02f);
        // }
        // if (Time.time - lastDebug > 2f) {
        //     lastDebug = Time.time;
        //     Debug.Log(GetCumulativeReward());
        // }
        if (noOfEnemyTargeted > 0) {
            SetReward(1f);
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        civilianSpawner.DestroyAllChildrenObject();
        animalSpawner.DestroyAllChildrenObject();
        enemySpawner.DestroyAllChildrenObject();
        civilianSpawner.spawnNPCs();
        animalSpawner.spawnNPCs();
        enemySpawner.spawnNPCs();
        // noOfEnemyTargeted = 0;
        // enemyTargeted = false;
        // targetingArea.GetComponent<MeshRenderer>().material = normalMaterial;

        noOfEnemyTargeted = 0;
        noOfCivilianTargeted = 0;
        
        float minX = boundary.position.x - boundary.localScale.x/3;
        float maxX = boundary.position.x + boundary.localScale.x/3;
        float minZ = boundary.position.z - boundary.localScale.z/3;
        float maxZ = boundary.position.z + boundary.localScale.z/3;
        float xSpawnPosition = Random.Range(minX, maxX);
        float zSpawnPosition = Random.Range(minZ, maxZ);
        
        targetingArea.position = new Vector3(xSpawnPosition, 0f, zSpawnPosition);
    }

    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.localPosition);
        if (target == null) {
            sensor.AddObservation(-1);
            sensor.AddObservation(-1);
            sensor.AddObservation(-1);
        }
        else sensor.AddObservation(target.localPosition);
        // sensor.AddObservation(noOfCivilianTargeted);
        // sensor.AddObservation(noOfEnemyTargeted);

        // foreach (Transform wall in walls) {
        //     sensor.AddObservation(wall.position);
        // }

        // if (civilianSpawner.noOfNPC > 0) {
        //     foreach (Transform civilian in civilianSpawner.transform) {
        //         sensor.AddObservation(civilian.position);
        //     }
        // }

        // Dummy values if npcs<max, add 3 observations because of transform.position
        // for (int i=0; i<maxCivi-civilianSpawner.noOfNPC; i++) {
        //     sensor.AddObservation(-1);
        //     sensor.AddObservation(-1);
        //     sensor.AddObservation(-1);
        // }

        // if (enemySpawner.noOfNPC > 0) {
        //     foreach (Transform enemy in enemySpawner.transform) {
        //         sensor.AddObservation(enemy.position);
        //     }
        // }
        // Dummy values if npcs<max, add 3 observations because of transform.position
        // for (int i=0; i<maxEnemy-enemySpawner.noOfNPC; i++) {
        //     sensor.AddObservation(-1);
        //     sensor.AddObservation(-1);
        //     sensor.AddObservation(-1);
        // }

        // if (animalSpawner.noOfNPC > 0) {
        //     foreach (Transform animal in animalSpawner.transform) {
        //         sensor.AddObservation(animal.position);
        //     }
        // }
        // // Dummy values if npcs<max, add 3 observations because of transform.position
        // for (int i=0; i<maxAnimal-animalSpawner.noOfNPC; i++) {
        //     sensor.AddObservation(-1);
        //     sensor.AddObservation(-1);
        //     sensor.AddObservation(-1);
        // }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        // Debug.Log(moveX + ", " + moveZ);

        float moveSpeed = 10f;
        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = -Input.GetAxisRaw("Horizontal");
        continuousActions[1] = -Input.GetAxisRaw("Vertical");
    }

    
    private void OnTriggerEnter(Collider other) {
        // Debug.Log("Triggered");
        // Debug.Log(other.name);
        if (other.tag == "Wall") {
            // Debug.Log("Collided with wall");
            SetReward(-1f);
            targetingArea.GetComponent<MeshRenderer>().material = normalMaterial;
            EndEpisode();
        }
        if (other.tag == "Enemy") {
            SetReward(1f);
            targetingArea.GetComponent<MeshRenderer>().material = winMaterial;
            EndEpisode();
        }
        if (other.tag == "Civilian") {
            // AddReward(-1f);
            noOfCivilianTargeted += 1;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Enemy") {
            noOfEnemyTargeted = 0;
            if (noOfEnemyTargeted == 0) {
                enemyTargeted = false;
                targetingArea.GetComponent<MeshRenderer>().material = normalMaterial;
            }
            
        }
        if (other.tag == "Civilian") {
            // AddReward(5f);
            noOfCivilianTargeted += -1;
        }
    }

    

}
