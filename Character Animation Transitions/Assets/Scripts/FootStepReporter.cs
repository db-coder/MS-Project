using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepReporter : MonoBehaviour
{

    void OnCollisionEnter(Collision c)
    {
        if (c.impulse.magnitude > 0.5f)
        {
            //we'll just use the first contact point for simplicity
            EventManager.TriggerEvent<FootStepEvent, Vector3, int>(c.contacts[0].point, 0);
        }
    }
}
