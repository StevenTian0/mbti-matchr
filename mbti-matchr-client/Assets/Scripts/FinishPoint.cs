using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    // void OnCollisionEnter(Collision other)
    // {
    //     if (other.gameObject.tag == "Player") {
    //         Debug.Log("You have arrived!");
    //     }
    // }

       void OnTriggerEnter2D(Collider2D other){  // The player enters a trigger collider (trap or spikes)
        if (other.gameObject.tag == "Moose" || other.gameObject.tag == "Racoon")
        {
            Debug.Log("Finish line!");
        }
    }
}
