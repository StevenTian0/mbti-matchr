using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    public int arrived = 0;

    void OnTriggerEnter2D(Collider2D other){  // The player enters a trigger collider (trap or spikes)
        if (other.gameObject.tag == "Moose" || other.gameObject.tag == "Racoon")
        {
            arrived++;
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Moose" || other.gameObject.tag == "Racoon")
        {
            arrived--;
        }
    }
}
