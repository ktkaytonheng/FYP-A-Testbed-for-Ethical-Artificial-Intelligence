using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Transform target;
    public Transform targetArea;
    public TurretAgent agent;
    public GameVariables gameVariables;

    [Header("Attributes")]

    public float range = 15f;
    public float fireRate = 100f;
    public float turnSpeed = 20f;
    public float bulletSize = 0.1f;
    public float bulletSpeed = 80f;
    private float fireCountdown = 0f;
    public bool explosionEnabled = true;
    public float explosionKillRadius = 10f;
    public float explosionPushRadius = 20f;
    public float explosionForce = 800f;
    public float targetAreaSpeed = 20f;

    [Header("Unity Setup fields")]

    public bool turretActivated = false;
    public bool manualFiringMode = false;
    public string enemyTag = "Enemy";
    public Transform partToRotate;
    

    public GameObject bulletPrefab;
    public Transform firePoint;
    
    // Start is called before the first frame update
    void Start()
    {
        Transform Simulation = transform.parent;
        agent = transform.GetComponent<TurretAgent>();
        targetArea = transform.Find("TargetingSphere");
        explosionKillRadius = targetArea.GetComponent<SphereCollider>().radius * targetArea.transform.lossyScale.x;
        gameVariables = transform.parent.Find("GameVariables").GetComponent<GameVariables>();
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

        if (nearestEnemy != null && shortestDistance <= range) target = nearestEnemy.transform;
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

        if (gameVariables.trainingMode) {
            if (target != null) targetArea.position = Vector3.MoveTowards(targetArea.position, target.position, targetAreaSpeed);
            if (target != null) agent.targeted = (Vector3.Distance(targetArea.position, target.position) <= 1) ? true : false;
        }

        // Fire
        if (Input.GetKeyDown("space")) {
            Debug.Log("Manual shoot!");
            Shoot();
        }
    }

    // public void MoveTargetingAreaUp() {
    //     targetArea.Translate(0f, 0f, -1 * moveTargetAreaDist);
    // }

    // public void MoveTargetingAreaDown() {
    //     targetArea.Translate(0f, 0f, moveTargetAreaDist);
    // }
    // ------------------------------------------------------------------------------------------------------

    public void Shoot() {
        agent.totalWorthValueShot = 0;
        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation, gameObject.transform);
        bulletObject.transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        if (bullet != null) bullet.Seek(target);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
