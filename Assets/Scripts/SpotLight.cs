using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLight : MonoBehaviour
{

    Light light;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        light.enabled = !light.enabled;
        Debug.Log("Turned light off");
        Debug.Log("Jeff");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ToggleLight();
    }

    void ToggleLight()
    {
        FlashLight fl = gameObject.GetComponentInParent(typeof(FlashLight)) as FlashLight;

        if (Input.GetKeyDown(KeyCode.F) && fl.getDistanceToPlayer() < fl.distanceNeeded)
        {
            light.enabled = !light.enabled;
            Debug.Log("Light Toggled");
        }
    }

}
