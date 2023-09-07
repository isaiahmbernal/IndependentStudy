using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform enemyObj;
    [SerializeField] private Transform playerPos;

    void FixedUpdate()
    {

        enemyObj.LookAt(playerPos);
        
    }
}
