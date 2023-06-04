using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityTurnOn : MonoBehaviour
{
    public float startDelayTimeSec;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Invoke("TurnOnGravity", startDelayTimeSec);
    }

    private void TurnOnGravity() {
        if (rb == null) {
            Debug.Log("No RigidBody attached, cannot turn on gravity");
            return;
        }

        rb.useGravity = true;
    }
}
