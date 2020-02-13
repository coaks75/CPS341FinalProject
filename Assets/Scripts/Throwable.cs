using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{

    /** The amount of force we want to throw this with. */
    public int power;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            rb.AddForce(Vector3.forward * power);
            rb.AddForce(Vector3.up * power);
            Debug.Log("Throwing");
        }
    }
}
