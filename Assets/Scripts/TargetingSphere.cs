using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingSphere : MonoBehaviour
{
    Turret turret;
    TurretAgent turretAgent;
    GameVariables gameVariables;
    int noOfCiviliansTargeted;
    int noOfEnemiesTargeted;
    int noOfAnimalsTargeted;
    int civilianWorthValue;
    int enemyWorthValue;
    int animalWorthValue;

    void Start() {
        turret = transform.GetComponentInParent<Turret>();
        turretAgent = transform.GetComponentInParent<TurretAgent>();
        gameVariables = transform.parent.Find("GameVariables").GetComponent<GameVariables>();
    }

    void Update() {
        noOfCiviliansTargeted = 0;
        noOfEnemiesTargeted = 0;
        noOfAnimalsTargeted = 0;
        civilianWorthValue = 0;
        enemyWorthValue = 0;
        animalWorthValue = 0;
        Collider[] colliders = Physics.OverlapSphere(transform.position, transform.GetComponent<SphereCollider>().radius * transform.lossyScale.x);
        foreach (Collider collateral in colliders) {
            if (collateral.tag == "Civilian") {
                noOfCiviliansTargeted += 1;
                civilianWorthValue += collateral.GetComponent<WanderAI>().worthValue;
            }
            else if (collateral.tag == "Enemy") {
                noOfEnemiesTargeted += 1;
                enemyWorthValue += collateral.GetComponent<WanderAI>().worthValue;
            }
            else if (collateral.tag == "Animal") {
                noOfAnimalsTargeted += 1;
                animalWorthValue += collateral.GetComponent<WanderAI>().worthValue;
            }
        }
        turretAgent.worthValueCivilian = civilianWorthValue;
        turretAgent.worthValueEnemy = enemyWorthValue;
        turretAgent.worthValueAnimal = animalWorthValue;
        turretAgent.noOfCiviliansTargeted = noOfCiviliansTargeted;
        turretAgent.noOfEnemiesTargeted = noOfEnemiesTargeted;
        turretAgent.noOfAnimalsTargeted = noOfAnimalsTargeted;
    }

    // void OnTriggerEnter(Collider col) {
    //     if (col.tag == "Enemy") {
    //         turretAgent.noOfEnemiesTargeted += 1;
    //         turretAgent.worthValueEnemy += col.GetComponent<WanderAI>().worthValue;
    //     }
    //     if (col.tag == "Civilian") {
    //         turretAgent.noOfCiviliansTargeted += 1;
    //         turretAgent.worthValueCivilian += col.GetComponent<WanderAI>().worthValue;
    //     }
    //     if (col.tag == "Animal") {
    //         turretAgent.noOfAnimalsTargeted += 1;
    //         turretAgent.worthValueAnimal += col.GetComponent<WanderAI>().worthValue;
    //     }
    // }

    // void OnTriggerExit(Collider col) {
    //     if (col.tag == "Enemy") {
    //         turretAgent.noOfEnemiesTargeted -= 1;
    //         turretAgent.worthValueEnemy -= col.GetComponent<WanderAI>().worthValue;
    //     }
    //     if (col.tag == "Civilian") {
    //         turretAgent.noOfCiviliansTargeted -= 1;
    //         turretAgent.worthValueCivilian -= col.GetComponent<WanderAI>().worthValue;
    //     }
    //     if (col.tag == "Animal") {
    //         turretAgent.noOfAnimalsTargeted -= 1;
    //         turretAgent.worthValueAnimal -= col.GetComponent<WanderAI>().worthValue;
    //     }
    // }
    
}
