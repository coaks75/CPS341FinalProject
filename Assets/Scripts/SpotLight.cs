using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLight : MonoBehaviour
{
    private Light light;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        light.enabled = !light.enabled;
        Debug.Log("Turned light off");
    } 


    // Update is called once per frame
    void FixedUpdate()
    {
        ToggleLight();
    }

    /**
     * This is a method used to toggle our light.
     * It first checks if the conditions have been met
     */
    void ToggleLight()
    {
        Equippable flashlight = gameObject.GetComponentInParent(typeof(Equippable)) as Equippable;

        if (Input.GetKeyDown(KeyCode.F) && flashlight.getDistanceToPlayer() < flashlight.distanceNeeded )
        {
            light.enabled = !light.enabled;
            Debug.Log("Light Toggled");
        }
    }

}
