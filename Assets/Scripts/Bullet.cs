
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private Vector3 dir;
    public float bulletSpeed;
    public float explosionKillRadius;
    public float explosionPushRadius;
    public float explosionForce;
    public bool explosionEnabled;
    public GameObject explosionEffect;
    private float timeInstantiated;
    private bool collided = false;

    public void Seek(Transform _target) {
        target = _target;
    }

    void Start() {
        timeInstantiated = Time.time;
        dir = target.position - transform.position;
        Turret turretAttributes = transform.parent.GetComponent<Turret>();
        bulletSpeed = turretAttributes.bulletSpeed;
        explosionKillRadius = turretAttributes.explosionKillRadius;
        explosionPushRadius = turretAttributes.explosionPushRadius;
        explosionForce = turretAttributes.explosionForce;
        explosionEnabled = turretAttributes.explosionEnabled;
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
        if (timeElapsed > 3f) {
            Destroy(gameObject);
            return;
        }
    }

    void OnTriggerEnter(Collider col) {
        // Debug.Log("Hit " + col.name);
        if (explosionEnabled) {
            GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(explosion, 3f);
            
            // To kill/destroy
            Collider[] collidersHit = Physics.OverlapSphere(transform.position, explosionKillRadius);
            foreach (Collider collateral in collidersHit) {
                if (collateral.tag == "Enemy" || collateral.tag == "Civilian") collateral.GetComponent<WanderAI>().Hit();
            }   

            // To push
            Collider[] collidersPushed = Physics.OverlapSphere(transform.position, explosionPushRadius);
            foreach (Collider collateral in collidersPushed) {
                Rigidbody rb =collateral.GetComponent<Rigidbody>();
                if (rb != null) {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionPushRadius);
                }
            }  
        }
        else {
            if (col.tag == "Enemy" || col.tag == "Civilian") col.GetComponent<WanderAI>().Hit();
        }    
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        Destroy(gameObject);
    }

    public void setAttributes(float explosionForce, float explosionKillRadius, float explosionPushRadius) {
        this.explosionForce = explosionForce;
        this.explosionKillRadius = explosionKillRadius;
        this.explosionPushRadius = explosionPushRadius;
    }
}
