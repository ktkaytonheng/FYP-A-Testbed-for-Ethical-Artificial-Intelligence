
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private Vector3 dir;
    public float bulletSpeed = 80f;
    public float explosionKillRadius;
    public float explosionPushRadius;
    public float explosionForce;
    public GameObject explosionEffect;
    private float timeInstantiated;
    private bool collided = false;

    public void Seek(Transform _target) {
        target = _target;
    }

    void Start() {
        timeInstantiated = Time.time;
        dir = target.position - transform.position;
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
        
        GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(explosion, 3f);
        
        // To kill/destroy
        Collider[] collidersHit = Physics.OverlapSphere(transform.position, explosionKillRadius);
        foreach (Collider collateral in collidersHit) {
            if (collateral.tag == "Enemy" || collateral.tag == "Civilian") collateral.GetComponent<WanderAI>().ShotByBullet();
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            Destroy(gameObject);
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

    public void setAttributes(float explosionForce, float explosionKillRadius, float explosionPushRadius) {
        this.explosionForce = explosionForce;
        this.explosionKillRadius = explosionKillRadius;
        this.explosionPushRadius = explosionPushRadius;
    }
}
