using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class footstep_script : MonoBehaviour
{

    private Animator anim;
    public static GameObject floor;
    private string currentFoot;

    private float currentFrameFootstepLeft;
    private float currentFrameFootstepRight;
    private float lastFrameFootstepLeft;
    private float lastFrameFootstepRight;

    [Space(5.0f)]
    private float currentVolume;
    [Range(0.0f, 1.0f)]
    public float volume = 1.0f;
    [Space(5.0f)]
    public GameObject leftFoot ;         //Drag your player's RIG/MESH/BIP/BONE for the left foot here, in the inspector.
    public GameObject rightFoot;        //Drag your player's RIG/MESH/BIP/BONE for the right foot here, in the inspector.
    [Space(5.0f)]
    //public AudioClip default_sound = new AudioClip();
    //public AudioClip dirt = new AudioClip();

    [Space(5.0f)]
    public ParticleSystem dirtFX;
    private Quaternion dirtRotation;
    private Vector3 dirtPos;

    // Use this for initialization
    void Start ()
    {
        anim = GetComponent<Animator>();
        if(dirtFX.isPlaying)
        dirtFX.Stop();
    }

	// Update is called once per frame
	void Update ()
    {
        if (dirtFX != null) {
            dirtFX.transform.rotation = dirtRotation;
            dirtFX.transform.position = dirtPos;
        }
        currentFrameFootstepLeft = anim.GetFloat("FootstepLeft");
        if(currentFrameFootstepLeft > 0 && lastFrameFootstepLeft < 0)
        {
            RaycastHit surfaceHitLeft;
            Ray aboveLeftFoot = new Ray(leftFoot.transform.position + new Vector3(0, 1.5f, 0), Vector3.down);
            LayerMask layerMask = 1 << LayerMask.NameToLayer("Default");
            if(Physics.Raycast(aboveLeftFoot, out surfaceHitLeft, 3f, layerMask))
            {
                floor = surfaceHitLeft.transform.gameObject;
                currentFoot = "Left";
                if (floor != null)
                    Invoke("CheckTexture", 0);
            }
        }
        lastFrameFootstepLeft = anim.GetFloat("FootstepLeft");

        currentFrameFootstepRight = anim.GetFloat("FootstepRight");
        if (currentFrameFootstepRight > 0 && lastFrameFootstepRight < 0)
        {
            RaycastHit surfaceHitRight;
            Ray aboveRightFoot = new Ray(rightFoot.transform.position + new Vector3(0, 1.5f, 0), Vector3.down);
            LayerMask layerMask = 1 << LayerMask.NameToLayer("Default");
            if (Physics.Raycast(aboveRightFoot, out surfaceHitRight, 3f, layerMask))
            {
                floor = surfaceHitRight.transform.gameObject;
                currentFoot = "Right";
                if (floor != null)
                    Invoke("CheckTexture", 0);
            }
        }
        lastFrameFootstepRight = anim.GetFloat("FootstepRight");
    }

    void CheckTexture()
    {
        if(dirtFX.isPlaying)
            dirtFX.Stop();
        if (floor.tag == ("Surface_Dirt"))
            Invoke ("PlayDirt", 0); 
        else if (floor.tag == ("Surface_Stone"))
            Invoke ("PlayStone", 0);
        else if (floor.tag == ("Surface_Wood"))
            Invoke ("PlayWood", 0);
        else if (floor.tag == ("Surface_Metal"))
            Invoke ("PlayMetal", 0);
        else if (floor.tag == ("Surface_Snow"))
            Invoke ("PlaySnow", 0);
        else if (floor.tag == ("Untagged"))
            Invoke ("PlayDefault", 0);
        else if (floor.tag == ("ground"))
            Invoke ("PlayDefault", 0);
    }

    void PlayDirt()
    {
        if(currentFoot == "Left")
            EventManager.TriggerEvent<FootStepEvent, Vector3, int>(leftFoot.transform.position, 1);
        else
            EventManager.TriggerEvent<FootStepEvent, Vector3, int>(rightFoot.transform.position, 1);
        if (dirtFX != null)
        {
            if (currentFoot == ("Left"))
            {
                if(dirtFX.isPlaying)
                    dirtFX.Stop();
                if(!dirtFX.isPlaying)
                    dirtFX.Play();
                dirtFX.transform.position = leftFoot.transform.position + new Vector3 (0, 0.05f, 0);
                dirtRotation = dirtFX.transform.rotation;
                dirtPos = dirtFX.transform.position;
            }
            else if (currentFoot == ("Right"))
            {
                if(dirtFX.isPlaying)
                    dirtFX.Stop();
                if(!dirtFX.isPlaying)
                    dirtFX.Play();
                dirtFX.transform.position = rightFoot.transform.position + new Vector3 (0, 0.05f, 0);
                dirtRotation = dirtFX.transform.rotation;
                dirtPos = dirtFX.transform.position;
            }
        }
    }

    void PlayDefault()
    {
        if(currentFoot == "Left")
            EventManager.TriggerEvent<FootStepEvent, Vector3, int>(leftFoot.transform.position, 0);
        else
            EventManager.TriggerEvent<FootStepEvent, Vector3, int>(rightFoot.transform.position, 0);
    }
}
