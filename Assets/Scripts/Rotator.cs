﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 45, 0) * Time.deltaTime);
    }
}
