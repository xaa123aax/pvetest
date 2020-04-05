using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    EnemyAI AI;
    SphereCollider Col;

    void Start()
    {
        AI = GetComponentInParent<EnemyAI>();
        Col = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        AI.EnemyStatus = EnemyAI.Enemy.Alert;
    }
    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Col.radius = 9; //Trigger範圍變大
            AI.Player = other.transform;
            AI.IsFindPlayer = true;
            if (AI.Second)
            {

                Col.radius = 50; //Trigger範圍變大
                AI.Player = other.transform;
                AI.IsFindPlayer = true;
                if(AI.EnemyStatus == EnemyAI.Enemy.Alert)
                {
                    AI.EnemyStatus = EnemyAI.Enemy.Chase;
                }

            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Col.radius = 3;//Trigger範圍回到原本
            AI.IsFindPlayer = false;
            AI.EnemyStatus = EnemyAI.Enemy.Alert;
        }
    }
}
