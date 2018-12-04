using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpReporter : MonoBehaviour
{

    void OnCollisionEnter(Collision c)
    {
        if (c.impulse.magnitude > 0.5f)
        {
            //we'll just use the first contact point for simplicity
            EventManager.TriggerEvent<JumpEvent, Vector3>(c.contacts[0].point);
        }
    }
}
