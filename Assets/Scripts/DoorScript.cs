using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{

    public Equippable objectNeeded;

    /** The field for if we should be loweing this or not. */
    private bool shouldLower;

    /** This is what the lowest distance for a door should be. */
    public float lowestDistance;

    /** This is the audio source for when we lower the door. */
    private AudioSource audio;

    /** A variable for if we started loweing the door already. */
    private bool startedLowering;


    private void Start()
    {
        audio = GetComponent<AudioSource>();
        shouldLower = false;
        startedLowering = false;
    }


    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) || startedLowering == true)
        {
            if (shouldLower)
            {
                transform.Translate(Vector3.down * Time.deltaTime, Space.World);
                startedLowering = true;
                if (!audio.isPlaying)
                {
                    //audio.Play();
                }
                Debug.Log("Trying to drop door");
            }
            else
            {
                if (audio.isPlaying)
                {
                    audio.Stop();
                    startedLowering = false;
                }
               //Debug.Log("Shouod not lower");
            }
            //Debug.Log("Getting click");
        }
        //Debug.Log("In mouse over");
    }



    /**
     * This method is to lower the door when you get the key.
     * And it will hopefully be done slowly...
     */
    public void AllowOpen()
    {
        if (transform.position.y >= lowestDistance)
        {
            shouldLower = true;
            //Debug.Log("Setting should lower true");
        }
        else
        {
            shouldLower = false;
        }
    }


}
