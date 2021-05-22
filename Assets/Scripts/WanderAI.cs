using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderAI : MonoBehaviour
{
    public Transform boundary;
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    public float moveSpeed;
    public float minRotateSpeed;
    public float maxRotateSpeed;
    public int maxWalkTime;
    public int maxRotateTime;
    
    private bool isWandering = false;
    private bool isWalking = false;
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;
    private float rotateSpeed;
    private Spawner spawner;

    // Start is called before the first frame update
    void Start()
    {
        boundary = GameObject.Find("Boundary").transform;
        spawner = gameObject.transform.parent.GetComponent<Spawner>();
        minX = boundary.position.x - boundary.localScale.x/2;
        maxX = boundary.position.x + boundary.localScale.x/2;
        minZ = boundary.position.z - boundary.localScale.z/2;
        maxZ = boundary.position.z + boundary.localScale.z/2;
        moveSpeed = spawner.moveSpeed;
        minRotateSpeed = spawner.minRotateSpeed;
        maxRotateSpeed = spawner.maxRotateSpeed;
        maxWalkTime = spawner.maxWalkTime;
        maxRotateTime = spawner.maxRotateTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWandering) StartCoroutine(Wander());
        if (isRotatingRight) transform.Rotate(transform.up * Time.deltaTime * rotateSpeed);
        if (isRotatingLeft) transform.Rotate(transform.up * Time.deltaTime * -rotateSpeed);
        if (isWalking) {
            Vector3 AIpos = transform.position;
            AIpos += transform.forward * Time.deltaTime * moveSpeed;   
            AIpos.x = Mathf.Clamp(AIpos.x, minX, maxX);
            AIpos.z = Mathf.Clamp(AIpos.z, minZ, maxZ);
            transform.position = AIpos;
        }
    }

    IEnumerator Wander() {
        rotateSpeed = Random.Range(minRotateSpeed, maxRotateSpeed);
        int rotateTime = Random.Range(1, maxRotateTime); // Time takes to rotate
        int rotateWait = Random.Range(1, 1); // Time between each rotate
        int rotateLorR = Random.Range(1, 2); // Determines whether to rotate L or R
        int walkWait = Random.Range(1, 1); // Time between walking
        int walkTime = Random.Range(1, maxWalkTime); // Time takes to walk

        isWandering = true;

        yield return new WaitForSeconds(walkWait);
        isWalking = true;
        // Debug.Log("Walking");
        yield return new WaitForSeconds(walkTime);
        isWalking = false;
        yield return new WaitForSeconds(rotateWait);
        switch(rotateLorR) {
            case 1:
                isRotatingRight = true;
                // Debug.Log("Rotating right");
                yield return new WaitForSeconds(rotateTime);
                isRotatingRight = false;
                break;
            case 2:
                isRotatingLeft = true;
                // Debug.Log("Rotating left");
                yield return new WaitForSeconds(rotateTime);
                isRotatingLeft = false;
                break;
        }
        isWandering = false;
    }

    public void Hit() {
        gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
        Destroy(gameObject);
        Debug.Log(gameObject.name + " killed");
        spawner.DecrementNPCCount();
    }

    public void setAttributes(float moveSpeed, float minRotSpeed, float maxRotSpeed, int maxWalkTime, int maxRotTime) {
        this.moveSpeed = moveSpeed;
        this.minRotateSpeed = minRotSpeed;
        this.maxRotateSpeed = maxRotSpeed;
        this.maxWalkTime = maxWalkTime;
        this.maxRotateTime = maxRotTime;
    }
}
