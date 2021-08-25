using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Transform target;
    public Transform targetArea;

    [Header("Attributes")]

    public bool manualMove;
    public float range = 15f;
    public float targetAreaSpeed = 1f;
    public float fireRate = 100f;
    public float turnSpeed = 20f;
    public float bulletSize = 0.1f;
    public float bulletSpeed = 80f;
    private float fireCountdown = 0f;
    public int currentTotalWorth;
    public bool explosionEnabled = true;
    public bool explosionPushEnabled = false;
    public float explosionKillRadius;
    public float explosionPushRadius = 20f;
    public float explosionForce = 800f;
    public float moveTargetAreaDist = 0.5f;

    [Header("Unity Setup fields")]

    public bool turretActivated = false;
    public bool manualFiringMode = false;
    public string enemyTag = "Enemy";
    public Transform partToRotate;
    

    public GameObject bulletPrefab;
    public Transform firePoint;
    public bool targetFound = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Transform Simulation = transform.parent;
        targetArea = transform.Find("TargetingSphere");
        explosionKillRadius = targetArea.GetComponent<SphereCollider>().radius * targetArea.transform.lossyScale.x;
    }

    void UpdateTarget() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies) {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance) {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range) {
            target = nearestEnemy.transform;
            // transform.Find("TargetingSphere").GetComponent<TurretAgent>().target = target;
        }
        else target = null;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTarget();
        if (targetArea == null) return;

        Vector3 dir = targetArea.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        // For moving the targeting area
        if (manualMove) {
            if (Input.GetKey("w")) {
                targetArea.Translate(0f, 0f, -1 * moveTargetAreaDist);
            }
            if (Input.GetKey("a")) {
                targetArea.Translate(moveTargetAreaDist, 0f, 0f);
            }
            if (Input.GetKey("s")) {
                targetArea.Translate(0f, 0f, moveTargetAreaDist);
            }
            if (Input.GetKey("d")) {
                targetArea.Translate(-1 * moveTargetAreaDist, 0f, 0f);
            }
        }
        else {
            if (target != null) targetArea.position = Vector3.MoveTowards(targetArea.position, target.position, targetAreaSpeed);
        }

        // Set target Area size
        if (Input.GetKey("e")) {
            targetArea.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            explosionKillRadius = targetArea.GetComponent<SphereCollider>().radius * targetArea.transform.lossyScale.x;
        }
        if (Input.GetKey("q")) {
            targetArea.localScale += new Vector3(-0.1f, -0.1f, -0.1f);
            explosionKillRadius = targetArea.GetComponent<SphereCollider>().radius * targetArea.transform.lossyScale.x;
        }

        // Fire
        if (Input.GetKeyDown("space")) {
            Debug.Log("Manual shoot!");
            Shoot();
        }

        // Old targeting system, ignore ----------------------------------------------------------------
        // if (turretActivated) { 
        //     Vector3 dir = targetArea.position - transform.position;
        //     Quaternion lookRotation = Quaternion.LookRotation(dir);
        //     Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        //     partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        //     if (!manualFiringMode) {
        //         if (fireCountdown <= 0f) {
        //             Shoot();
        //             float actualFireRate = (fireRate >= 100) ? fireRate : 100; 
        //             fireCountdown = 100f / fireRate;
        //         }
        //         fireCountdown -= Time.deltaTime;
        //     }
        //     else {
        //         if (Input.GetKeyDown("space")) {
        //             Debug.Log("Manual shoot!");
        //             Shoot();
        //         }
        //     }
        // }
        // ------------------------------------------------------------------------------------------------
    }

    // Functions to move the targeting area, for the turret agent to call --------------------------------
    // Currently useless as agent changed to not move the targeting area, removed if not changed
    public void MoveTargetingAreaLeft() {
        targetArea.Translate(moveTargetAreaDist, 0f, 0f);
    }

    public void MoveTargetingAreaRight() {
        targetArea.Translate(-1 * moveTargetAreaDist, 0f, 0f);
    }

    public void MoveTargetingAreaUp() {
        targetArea.Translate(0f, 0f, -1 * moveTargetAreaDist);
    }

    public void MoveTargetingAreaDown() {
        targetArea.Translate(0f, 0f, moveTargetAreaDist);
    }
    // ------------------------------------------------------------------------------------------------------

    public void Shoot() {
        // GameObject targetPosition = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // targetPosition.transform.position = targetArea.position;
        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation, gameObject.transform);
        bulletObject.transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        if (bullet != null) bullet.Seek(targetArea);
    }

    // void OnDrawGizmosSelected() {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, range);
    // }
}
