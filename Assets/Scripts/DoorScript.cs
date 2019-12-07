using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class DoorScript : MonoBehaviour
{

    public Equippable[] objectsNeeded;

    /** The field for if we should be loweing this or not. */
    private bool shouldLower;

    /** This is what the lowest distance for a door should be. */
    public float lowestDistance;

    /** This is the audio source for when we lower the door. */
    private AudioSource audio;

    /** A variable for if we started loweing the door already. */
    private bool startedLowering;

    /** A variable to say if we are opened already. */
    public bool opened;

    public PlayerController player;


    private void Start()
    {
        audio = GetComponent<AudioSource>();
        shouldLower = false;
        startedLowering = false;
        opened = false;
        player = GameObject.FindObjectOfType<PlayerController>();
    }


    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {

            bool doIt = false;
            int numFound = 0;
            string objectsNeededNames = "";

            foreach (Equippable v in player.equipped)
            {
                if (v != null)
                {
                    foreach (Equippable e in objectsNeeded)
                    {
                        if (e.name == v.name)
                        {
                            numFound++;
                        }
                        objectsNeededNames += e.name + ", ";
                    }
                }
            }

            if (numFound == objectsNeeded.Length)
            {
                doIt = true;
            }

            if (doIt && transform.position.y >= lowestDistance)
            {
                transform.Translate(Vector3.down * Time.deltaTime, Space.World);
                startedLowering = true;
                opened = true;
                if (!audio.isPlaying)
                {
                    audio.Play();
                }
                Debug.Log("Trying to drop door");
            }
            else
            {
                if (transform.position.y < lowestDistance)
                {
                    audio.Stop();
                    startedLowering = false;
                }
                else
                {
                    
                    player.errorText.text = "Sorry, you must first obtain the " + objectsNeededNames;
                    player.errorText.enabled = true;
                }
                //Debug.Log("Shouod not lower");
            }
            //Debug.Log("Getting click");
        }
        //Debug.Log("In mouse over");
    }

    private void FixedUpdate()
    {
        if (startedLowering)
        {
            if (transform.position.y >= lowestDistance)
            {
                transform.Translate(Vector3.down * Time.deltaTime, Space.World);
                startedLowering = true;
                opened = true;
                if (!audio.isPlaying)
                {
                    audio.Play();
                }
                Debug.Log("Trying to drop door");
            }
            else
            {
                if (transform.position.y < lowestDistance)
                {
                    audio.Stop();
                    startedLowering = false;
                }
                //Debug.Log("Shouod not lower");
            }
        }
    }



    /**
     * This method is to lower the door when you get the key.
     * And it will hopefully be done slowly...
     */
    //public void AllowOpen()
    //{
    //    if (transform.position.y >= lowestDistance)
    //    {
    //        shouldLower = true;
    //        //Debug.Log("Setting should lower true");
    //    }
    //    else
    //    {
    //        shouldLower = false;
    //    }
    //}


}
