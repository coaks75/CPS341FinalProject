using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equippable : MonoBehaviour
{

    /** This is the default points for an equippable object. */
    public int points = 1;

    /** This is the default distance needed to equip the item. */
    public float distanceNeeded = 2;

    /** This is the distance to the player. */
    private float distanceToPlayer;

    /** This is to know if we are equipped. */
    public bool equipped;

    /** The message to display when we get this item. */
    public string message;


    private void Start()
    {
        distanceToPlayer = 0.0f;
        equipped = false;
    }


    private void FixedUpdate()
    {
        distanceToPlayer = CalculateDistanceToPlayer();
    }


    private float CalculateDistanceToPlayer()
    {
        return Vector3.Distance(GameObject.Find("PlayerCamera").transform.position, this.transform.position);
    }

    public float getDistanceToPlayer()
    {
        return distanceToPlayer;
    }

}
