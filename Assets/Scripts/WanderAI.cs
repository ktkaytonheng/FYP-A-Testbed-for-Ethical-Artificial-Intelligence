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
    public float moveSpeed = 3f;
    public float rotateSpeed = 100f;
    
    private bool isWandering = false;
    private bool isWalking = false;
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;

    // Start is called before the first frame update
    void Start()
    {
        boundary = GameObject.Find("Boundary").transform;
        minX = boundary.position.x - boundary.localScale.x/2;
        maxX = boundary.position.x + boundary.localScale.x/2;
        minZ = boundary.position.z - boundary.localScale.z/2;
        maxZ = boundary.position.z + boundary.localScale.z/2;
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
        int rotateTime = Random.Range(1, 3); // Time takes to rotate
        int rotateWait = Random.Range(1, 4); // Time between each rotate
        int rotateLorR = Random.Range(1, 2); // Determines whether to rotate L or R
        int walkWait = Random.Range(1, 4); // Time between walking
        int walkTime = Random.Range(1, 5); // Time takes to walk

        isWandering = true;

        yield return new WaitForSeconds(walkWait);
        isWalking = true;
        Debug.Log("Walking");
        yield return new WaitForSeconds(walkTime);
        isWalking = false;
        yield return new WaitForSeconds(rotateWait);
        switch(rotateLorR) {
            case 1:
                isRotatingRight = true;
                Debug.Log("Rotating right");
                yield return new WaitForSeconds(rotateTime);
                isRotatingRight = false;
                break;
            case 2:
                isRotatingLeft = true;
                Debug.Log("Rotating left");
                yield return new WaitForSeconds(rotateTime);
                isRotatingLeft = false;
                break;
        }
        isWandering = false;
    }
}
