using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{

    /** This variable is for the distance to the player. */
    private double distanceToPlayer;

    ///** This variable is the distance you need to be to equip the flashlight. */
    //public double distanceNeeded;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    distanceToPlayer = CalculateDistanceToPlayer();
    //}

    //// Update is called once per frame
    //void FixedUpdate()
    //{
    //    distanceToPlayer = CalculateDistanceToPlayer();
    //    EquipLight();
    //}

    /**
     * This method is to attach the flashlight to the player object.
     */
    // void EquipLight()
    //{
    //    // This if statement checks if the player presses the correct key and also if they are within 2 of the player
    //    if (Input.GetKeyDown(KeyCode.C) && distanceToPlayer < distanceNeeded)
    //    {
    //        this.transform.parent = GameObject.Find("PlayerCamera").transform;
    //        this.transform.position = GameObject.Find("PlayerCamera").transform.position;
    //        this.transform.rotation = GameObject.Find("PlayerCamera").transform.rotation;
    //        Debug.Log("Flashlight equipped");
    //    }
    //}

    

    ///**
    // * A getter for the distance to player variable, so we can use it in our flashlight script.
    // * 
    // * @returns This method returns the distance of this flashlight to the player
    // */
    //public double getDistanceToPlayer()
    //{
    //    return distanceToPlayer;
    //}

}
