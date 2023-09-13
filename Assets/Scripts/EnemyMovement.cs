using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterStateManager myState;
    [SerializeField] private Transform myObj;
    [SerializeField] private Transform playerPos;

    void FixedUpdate()
    {

        if (myState.GetAbleState() == CharacterStateManager.AbleState.Normal) {
            // myObj.rotation = Quaternion.Lerp(
            //     myObj.rotation,
            //     Quaternion.LookRotation(new Vector3(
            //         playerPos.position.x,
            //         myObj.position.y,
            //         playerPos.position.z)),
            //     Time.deltaTime);
            myObj.LookAt(new Vector3(playerPos.position.x, myObj.position.y, playerPos.position.z));
            myObj.localPosition = Vector3.Lerp(myObj.localPosition, new Vector3(0, 0, 0), Time.deltaTime);
            // myObj.localPosition = new Vector3(0, 0, 0);
        }
        
    }
}
