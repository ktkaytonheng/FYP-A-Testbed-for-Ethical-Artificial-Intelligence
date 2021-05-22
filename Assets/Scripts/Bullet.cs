
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private Vector3 dir;
    public float bulletSpeed = 80f;
    public float explosionRadius = 3f;
    public float explosionForce = 700f;
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
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collateral in colliders) {
            Rigidbody rb =collateral.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }   

        if (col.tag == "Enemy" || col.tag == "Civilian") col.GetComponent<WanderAI>().ShotByBullet();
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        Destroy(gameObject);
    }
}
