using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingSphere : MonoBehaviour
{
    Turret turret;
    NewTurretAgent turretAgent;

    void Start() {
        turret = transform.GetComponentInParent<Turret>();
        turretAgent = transform.GetComponentInParent<NewTurretAgent>();
    }

    void Update() {

    }

    void OnTriggerEnter(Collider col) {
        if (col.tag == "Enemy") {
            turretAgent.noOfEnemiesTargeted += 1;
            turretAgent.worthValueEnemy += col.GetComponent<WanderAI>().worthValue;
        }
        if (col.tag == "Civilian") {
            turretAgent.noOfCiviliansTargeted += 1;
            turretAgent.worthValueCivilian += col.GetComponent<WanderAI>().worthValue;
        }
        if (col.tag == "Animal") {
            turretAgent.noOfAnimalsTargeted += 1;
            turretAgent.worthValueAnimal += col.GetComponent<WanderAI>().worthValue;
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.tag == "Enemy") {
            turretAgent.noOfEnemiesTargeted -= 1;
            turretAgent.worthValueEnemy -= col.GetComponent<WanderAI>().worthValue;
        }
        if (col.tag == "Civilian") {
            turretAgent.noOfCiviliansTargeted -= 1;
            turretAgent.worthValueCivilian -= col.GetComponent<WanderAI>().worthValue;
        }
        if (col.tag == "Animal") {
            turretAgent.noOfAnimalsTargeted -= 1;
            turretAgent.worthValueAnimal -= col.GetComponent<WanderAI>().worthValue;
        }
    }
    
}
