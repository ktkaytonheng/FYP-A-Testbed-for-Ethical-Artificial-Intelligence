
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform target;
    public Vector3 targetPosition;
    public float targetDistance;
    private Vector3 dir;
    private Vector3 turretPos;
    public float bulletSpeed;
    public float explosionKillRadius;
    public float explosionPushRadius;
    public float explosionForce;
    public bool explosionEnabled;
    public bool explosionPushEnabled;
    public GameObject explosionEffect;
    private float timeInstantiated;
    private bool collided = false;

    public void Seek(Transform _target) {
        target = _target;
        targetPosition = target.position;
        targetDistance = Vector3.Distance(turretPos, targetPosition);
    }

    void Start() {
        timeInstantiated = Time.time;
        dir = target.position - transform.position;
        turretPos = transform.parent.position;
        Turret turretAttributes = transform.parent.GetComponent<Turret>();
        bulletSpeed = turretAttributes.bulletSpeed;
        explosionKillRadius = turretAttributes.explosionKillRadius;
        explosionPushRadius = turretAttributes.explosionPushRadius;
        explosionForce = turretAttributes.explosionForce;
        explosionEnabled = turretAttributes.explosionEnabled;
        explosionPushEnabled = turretAttributes.explosionPushEnabled;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) {
            Destroy(gameObject);
            return;
        }
        
        float distanceThisFrame = bulletSpeed * Time.deltaTime;
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);

        float timeElapsed = Time.time - timeInstantiated;
        // if (timeElapsed > 5f) {
        //     Destroy(gameObject);
        //     return;
        // }
        if (Vector3.Distance(transform.position, turretPos) >= targetDistance) {
            if (explosionEnabled) {
                Explode();
            }
            Destroy(gameObject);
            return;
        }
    }

    // Triggers explosion on collision, disabled for now.
    // void OnTriggerEnter(Collider col) {
    //     // Debug.Log("Hit " + col.name);
    //     if (explosionEnabled) {
    //         Explode();
    //     }
    //     else {
    //         if (col.tag == "Enemy" || col.tag == "Civilian") col.GetComponent<WanderAI>().Hit();
    //     }    
    //     gameObject.GetComponent<MeshRenderer>().enabled = false;
    //     Destroy(gameObject);
    // }

    public void Explode() {
        GameObject explosion = Instantiate(explosionEffect, target.position, target.rotation);
        Destroy(explosion, 3f);
        
        // To kill/destroy
        Collider[] collidersHit = Physics.OverlapSphere(target.position, explosionKillRadius);
        foreach (Collider collateral in collidersHit) {
            if (collateral.tag == "Enemy" || collateral.tag == "Civilian" || collateral.tag == "Animal") collateral.GetComponent<WanderAI>().Hit();
        }   


        // To push
        // if (explosionPushEnabled) {
        //     Collider[] collidersPushed = Physics.OverlapSphere(transform.position, explosionPushRadius);
        //     foreach (Collider collateral in collidersPushed) {
        //         Rigidbody rb =collateral.GetComponent<Rigidbody>();
        //         if (rb != null) {
        //             rb.AddExplosionForce(explosionForce, transform.position, explosionPushRadius);
        //         }
        //     }  
        // }
    }

    public void setAttributes(float explosionForce, float explosionKillRadius, float explosionPushRadius) {
        this.explosionForce = explosionForce;
        this.explosionKillRadius = explosionKillRadius;
        this.explosionPushRadius = explosionPushRadius;
    }
}
