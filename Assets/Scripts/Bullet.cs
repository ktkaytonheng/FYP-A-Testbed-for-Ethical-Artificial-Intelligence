
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private Vector3 dir;
    public float bulletSpeed = 80f;
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

    void OnTriggerExit(Collider col) {
        if (collided) return;
        collided = true;
        Debug.Log("Hit " + col.name);
        if (col.tag == "Enemy" || col.tag == "Civilian") col.GetComponent<WanderAI>().ShotByBullet();
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        Destroy(gameObject);
    }
}
