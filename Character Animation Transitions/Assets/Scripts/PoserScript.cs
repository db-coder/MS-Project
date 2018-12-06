using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class PoserScript : MonoBehaviour {

    //This script will only work if a mesh renderer is enabled!

    //Which anim states are we attempting to capture pose for?
    //NOTE: script currently can't handle changing animStates during play in editor
    public string[] animStates = {"Getting Up_fight", "Getting Up_front_fight", "stand_up_from_back_3", "standing_up_from_belly_2", "Getting Up_drunk", "Getting Up_front_drunk" };

    public bool captureComplete = false;

    //List of bone orientations per anim states (from above)
    public Quaternion[][] bones;

    //Inspector viewable bone arrays for first two states (due to inspector limitations on jagged arrays not supported)
    public Quaternion[] bones_0;
    public Quaternion[] bones_1;
    public Quaternion[] bones_2;
    public Quaternion[] bones_3;
    public Quaternion[] bones_4;
    public Quaternion[] bones_5;

    public GameObject[] animHips;

    private Animator anim;

    private Animator animChar;

    private int currState = 0;

    public GameObject character;

    RagdollHelper ragdollScript;


    private void Awake()
    {
        anim = GetComponent<Animator>();

        if (anim == null)
            Debug.LogError("Animator not found");

        captureComplete = false;

        int bone_count = ((HumanBodyBones[])System.Enum.GetValues(typeof(HumanBodyBones))).Length;

        bone_count -= 1; //Don't count "LastBone"

        bones = new Quaternion[animStates.Length][];

        for (int i=0; i < bones.Length; ++i)
        {
            bones[i] = new Quaternion[bone_count];
        }

        bones_0 = new Quaternion[bone_count];
        bones_1 = new Quaternion[bone_count];
        bones_2 = new Quaternion[bone_count];
        bones_3 = new Quaternion[bone_count];
        bones_4 = new Quaternion[bone_count];
        bones_5 = new Quaternion[bone_count];

        animHips = new GameObject[6];

        //ragdollScript = character.GetComponent<RagdollHelper>();
        //animChar = character.GetComponent<Animator>();
    }

    void Start () {

        //Force animations to stay on first frame
        anim.speed = 0;
	}


    void Update()
    {

        var animStateInfo = anim.GetCurrentAnimatorStateInfo(0);


        if (currState >= animStates.Length)
        {
            captureComplete = true;
            return;
        }


        if (!animStateInfo.IsName(animStates[currState]))
        {
            Debug.Log("Playing " + animStates[currState] + " for pose capture");
            anim.Play(animStates[currState]);

        }
        else
        {
            //if(ragdollScript.state == RagdollHelper.RagdollState.ragdolled)
            //{
            //    Debug.Log("translate done");
            //    anim.GetBoneTransform(HumanBodyBones.Hips).Translate(animChar.GetBoneTransform(HumanBodyBones.Hips).position);
            //    Vector3 ragdollComp = Vector3.ProjectOnPlane(anim.GetBoneTransform(HumanBodyBones.Hips).up, Vector3.up);
            //    Vector3 animComp = Vector3.ProjectOnPlane(animChar.GetBoneTransform(HumanBodyBones.Hips).up, Vector3.up);

            //    anim.GetBoneTransform(HumanBodyBones.Hips).Rotate(Vector3.up, Vector3.SignedAngle(animComp, ragdollComp, Vector3.up));
            //}

            int i = -1;

            animHips[currState] = character;

            //foreach (var b in (HumanBodyBones[])System.Enum.GetValues(typeof(HumanBodyBones)))
            //{
            //    ++i;

            //    if (b == HumanBodyBones.LastBone)
            //        break;

            //    var t = anim.GetBoneTransform(b);

            //    if (t != null)
            //    {
            //        bones[currState][i] = t.rotation;

            //    }
            //}

            Debug.Log("Capture complete for state " + animStates[currState]);


            //Inspector limitation workaround to see bone rotations for first two states
            //if(currState == 0)
            //{
            //    for (int j = 0; j < bones[currState].Length; ++j)
            //    {
            //        bones_0[j] = bones[currState][j];
            //    }
            //}
            //else if (currState == 1)
            //{
            //    for (int j = 0; j < bones[currState].Length; ++j)
            //    {
            //        bones_1[j] = bones[currState][j];
            //    }
            //}
            //else if (currState == 2)
            //{
            //    for (int j = 0; j < bones[currState].Length; ++j)
            //    {
            //        bones_2[j] = bones[currState][j];
            //    }
            //}
            //else if (currState == 3)
            //{
            //    for (int j = 0; j < bones[currState].Length; ++j)
            //    {
            //        bones_3[j] = bones[currState][j];
            //    }
            //}
            //else if (currState == 4)
            //{
            //    for (int j = 0; j < bones[currState].Length; ++j)
            //    {
            //        bones_4[j] = bones[currState][j];
            //    }
            //}
            //else if (currState == 5)
            //{
            //    for (int j = 0; j < bones[currState].Length; ++j)
            //    {
            //        bones_5[j] = bones[currState][j];
            //    }
            //}

            ++currState;

        }
    }

	


    //Note this will only draw the currState animation, so you will probably only see the last state
    //  as all the other states will very quickly iterate through in a fraction of a second
    private void OnDrawGizmos()
    {
        if (anim == null)
            return;

        int i = -1;

        foreach (var b in (HumanBodyBones[])System.Enum.GetValues(typeof(HumanBodyBones)))
        {
            ++i;

            if (b == HumanBodyBones.LastBone)
                break;
            
            var t = anim.GetBoneTransform(b);

            if (t != null)
            {
                Gizmos.matrix = t.localToWorldMatrix;
                Gizmos.DrawWireSphere(Vector3.zero, 0.1f);


            }
        }
        
    }
}
