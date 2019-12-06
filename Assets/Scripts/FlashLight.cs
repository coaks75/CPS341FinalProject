using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Does nothing
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        EquipLight();
    }

    /**
     * This method is to attach the flashlight to the player object.
     */
     void EquipLight()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            this.transform.parent = GameObject.Find("PlayerCamera").transform;
            this.transform.position = GameObject.Find("PlayerCamera").transform.position;
            this.transform.rotation = GameObject.Find("PlayerCamera").transform.rotation;
            Debug.Log("Flashlight equipped");
        }
    }
}
