using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Transform target;

    [Header("Attributes")]

    public float range = 15f;
    public float fireRate = 1f;
    public float bulletSize = 0.10f;
    private float fireCountdown = 0f;
    public float explosionKillRadius = 10f;
    public float explosionPushRadius = 20f;
    public float explosionForce = 800f;

    [Header("Unity Setup fields")]

    public bool turretActivated = false;
    public bool manualFiringMode = false;
    public string enemyTag = "Enemy";
    public Transform partToRotate;
    public float turnSpeed = 10f;

    public GameObject bulletPrefab;
    public Transform firePoint;
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
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
        if (target == null) return;

        if (turretActivated) { 
            Vector3 dir = target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
            partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

            if (!manualFiringMode) {
                if (fireCountdown <= 0f) {
                    Shoot();
                    fireCountdown = 1f / fireRate;
                }
                fireCountdown -= Time.deltaTime;
            }
            else {
                if (Input.GetKeyDown("space")) {
                    Debug.Log("Manual shoot!");
                    Shoot();
                }
            }
        }
    }

    void Shoot() {
        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bulletObject.transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.setAttributes(explosionForce, explosionKillRadius, explosionPushRadius);
        if (bullet != null) bullet.Seek(target);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
