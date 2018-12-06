using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
A helper component that enables blending from Mecanim animation to ragdolling and back. 

To use, do the following:

Add "GetUpFromBelly" and "GetUpFromBack" bool inputs to the Animator controller
and corresponding transitions from any state to the get up animations. When the ragdoll mode
is turned on, Mecanim stops where it was and it needs to transition to the get up state immediately
when it is resumed. Therefore, make sure that the blend times of the transitions to the get up animations are set to zero.

TODO:

Make matching the ragdolled and animated root rotation and position more elegant. Now the transition works only if the ragdoll has stopped, as
the blending code will first wait for mecanim to start playing the get up animation to get the animated hip position and rotation. 
Unfortunately Mecanim doesn't (presently) allow one to force an immediate transition in the same frame. 
Perhaps there could be an editor script that precomputes the needed information.

*/

public class RagdollHelper : MonoBehaviour {
	//public property that can be set to toggle between ragdolled and animated character
	public bool ragdolled
	{
		get
        {
			return state!=RagdollState.animated;
		}
		set
        {
			if (value==true)
            {
				if (state==RagdollState.animated)
                {
                    Vector3 startingVelocity = rb.velocity;
					//Transition from animated to ragdolled
					setKinematic(false); //allow the ragdoll RigidBodies to react to the environment
					anim.enabled = false; //disable animation
					state=RagdollState.ragdolled;
				} 
			}
			else
            {
				if (state==RagdollState.ragdolled)
                {
					//Transition from ragdolled to animated through the blendToAnim state
					setKinematic(true); //disable gravity etc.
					ragdollingEndTime=Time.time; //store the state change time
					anim.enabled = true; //enable animation
					state=RagdollState.blendToAnim;  
					
					//Store the ragdolled position for blending
					foreach (BodyPart b in bodyParts)
					{
						b.storedRotation=b.transform.rotation;
						b.storedPosition=b.transform.position;
					}
					
					//Remember some key positions
					ragdolledFeetPosition=0.5f*(anim.GetBoneTransform(HumanBodyBones.LeftFoot).position + anim.GetBoneTransform(HumanBodyBones.RightFoot).position);
					ragdolledHeadPosition=anim.GetBoneTransform(HumanBodyBones.Head).position;
					ragdolledHipPosition=anim.GetBoneTransform(HumanBodyBones.Hips).position;


                    //int i = -1;
                    //int bone_count = ((HumanBodyBones[])System.Enum.GetValues(typeof(HumanBodyBones))).Length;
                    //bone_count -= 1; //Don't count "LastBone"

                    Quaternion[] bones = new Quaternion[80];

                    //foreach (var b in (HumanBodyBones[])System.Enum.GetValues(typeof(HumanBodyBones)))
                    //{
                    //    ++i;

                    //    if (b == HumanBodyBones.LastBone)
                    //        break;

                    //    var t = anim.GetBoneTransform(b);

                    //    if (t != null)
                    //    {
                    //        bones[i] = t.rotation;

                    //    }
                    //}

                    //Debug.Log("bone count " + bone_count);
                    Transform[] transformsRagdoll = anim.GetComponentsInChildren<Transform>();
                    //Vector3 average = Vector3.zero;
                    int count = 0;
                    foreach (Transform child in transformsRagdoll)
                    {
                        if (!child.name.StartsWith("mixamorig"))
                            continue;
                        if (child != null)
                        {
                            bones[count] = child.rotation;
                            count++;
                        }
                    }
                    //average /= count;
                    Debug.Log("ragdoll joint " + count);

                    //Initiate the get up animation
                    if (anim.GetBoneTransform(HumanBodyBones.Hips).forward.y>0) //hip hips forward vector pointing upwards, initiate the get up from back animation
					{
						anim.SetBool("GetUpFromBack",true);
					}
					else
                    {
						anim.SetBool("GetUpFromBelly",true);
					}

                    //Anim.a
                    //float minAvg = int.MaxValue;
                    int index = 0;
                    float min_error = int.MaxValue;

                    if (anim.GetBoneTransform(HumanBodyBones.Hips).forward.y > 0) //hip hips forward vector pointing upwards, initiate the get up from back animation
                    {
                        for (int j = 0; j < pose_script.animHips.Length; j += 2)
                        {
                            GameObject hipsClone = Instantiate(pose_script.animHips[j]);

                            hipsClone.transform.Translate(anim.GetBoneTransform(HumanBodyBones.Hips).position - hipsClone.transform.position);
                            Vector3 ragdollComp = Vector3.ProjectOnPlane(anim.GetBoneTransform(HumanBodyBones.Hips).up, Vector3.up);
                            Vector3 animComp = Vector3.ProjectOnPlane(hipsClone.transform.up, Vector3.up);

                            hipsClone.transform.Rotate(Vector3.up, Vector3.SignedAngle(animComp, ragdollComp, Vector3.up));

                            Transform[] cloneTransforms = hipsClone.GetComponentsInChildren<Transform>();
                            count = 0;
                            Quaternion[] animBones = new Quaternion[80];
                            foreach (Transform child in cloneTransforms)
                            {
                                if(child != null)
                                {
                                    //Debug.Log(child.name);
                                    animBones[count] = child.rotation;
                                    count++;
                                }
                            }

                            Debug.Log("anim bone count " + count);

                            float error = 0;
                            for (int k = 0; k < count; k++)
                            {
                                error += 1 - Mathf.Pow(Quaternion.Dot(bones[k], animBones[k]), 2);        //1 - <q1,q2>^2
                            }
                            Debug.Log("error for index " + j + " is " + error);
                            if (min_error > error)
                            {
                                min_error = error;
                                index = j;
                            }
                        }
                    }
                    else
                    {
                        for (int j = 1; j < 6; j += 2)
                        {
                            GameObject hipsClone = Instantiate(pose_script.animHips[j]);

                            hipsClone.transform.Translate(anim.GetBoneTransform(HumanBodyBones.Hips).position-hipsClone.transform.position);
                            Vector3 ragdollComp = Vector3.ProjectOnPlane(anim.GetBoneTransform(HumanBodyBones.Hips).up, Vector3.up);
                            Vector3 animComp = Vector3.ProjectOnPlane(hipsClone.transform.up, Vector3.up);

                            hipsClone.transform.Rotate(Vector3.up, Vector3.SignedAngle(animComp, ragdollComp, Vector3.up));

                            Transform[] cloneTransforms = hipsClone.GetComponentsInChildren<Transform>();
                            count = 0;
                            Quaternion[] animBones = new Quaternion[80];
                            foreach (Transform child in cloneTransforms)
                            {
                                if (child != null)
                                {
                                    //Debug.Log(child.name);
                                    animBones[count] = child.rotation;
                                    count++;
                                }
                            }

                            Debug.Log("anim bone count " + count);

                            float error = 0;
                            for (int k = 0; k < count; k++)
                            {
                                error += 1 - Mathf.Pow(Quaternion.Dot(bones[k], animBones[k]), 2);        //1 - <q1,q2>^2
                            }
                            Debug.Log("error for index " + j + " is " + error);
                            if (min_error > error)
                            {
                                min_error = error;
                                index = j;
                            }
                        }
                    }

                    //Quaternion[][] animBones = new Quaternion[6][];
                    //for (int j = 0; j < 6; ++j)
                    //{
                    //    animBones[j] = new Quaternion[bone_count];
                    //}

                    //animBones[0] = pose_script.bones_0;
                    //animBones[1] = pose_script.bones_1;
                    //animBones[2] = pose_script.bones_2;
                    //animBones[3] = pose_script.bones_3;
                    //animBones[4] = pose_script.bones_4;
                    //animBones[5] = pose_script.bones_5;
                    //Debug.Log("bone count: " + bone_count);
                    //float min_error = int.MaxValue;
                    //if (anim.GetBoneTransform(HumanBodyBones.Hips).forward.y > 0) //hip hips forward vector pointing upwards, initiate the get up from back animation
                    //{
                    //    for (int j = 0; j < 6; j += 2)
                    //    {
                    //        float error = 0;
                    //        for (int k = 0; k < bone_count; k++)
                    //        {
                    //            error += 1 - Mathf.Pow(Quaternion.Dot(bones[k], animBones[j][k]), 2);        //1 - <q1,q2>^2
                    //        }
                    //        Debug.Log("error for index " + j + " is " + error);
                    //        if (min_error > error)
                    //        {
                    //            min_error = error;
                    //            index = j;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    for (int j = 1; j < 6; j += 2)
                    //    {
                    //        float error = 0;
                    //        for (int k = 0; k < bone_count; k++)
                    //        {
                    //            error += 1 - Mathf.Pow(Quaternion.Dot(bones[k], animBones[j][k]), 2);        //1 - <q1,q2>^2
                    //        }
                    //        Debug.Log("error for index " + j + " is " + error);
                    //        if (min_error > error)
                    //        {
                    //            min_error = error;
                    //            index = j;
                    //        }
                    //    }
                    //}
                    Debug.Log("index " + index);


                    //AnimationClip c = Anim.GetClip("Getting_Up_fight");
                    //AnimationState stateA = Anim["stand_up_from_back_3"];
                    //foreach (AnimationState stateA in Anim)
                    //{
                    //    stateA.enabled = true;
                    //    stateA.normalizedTime = (1.0f / stateA.length) * 1;
                    //    Anim.Sample();

                    //    Transform[] transforms = Anim.GetComponentsInChildren<Transform>();
                    //    Vector3 averageAnim = Vector3.zero;
                    //    int count1 = 0;
                    //    float error = 0;
                    //    foreach (Transform child in transforms)
                    //    {
                    //        if (!child.name.StartsWith("mixamorig"))
                    //            continue;
                    //        foreach(Transform child_ragdoll in transformsRagdoll)
                    //        {
                    //            if(child_ragdoll.name.CompareTo(child.name) == 0)
                    //            {
                    //                error = (child.position - child_ragdoll.position).sqrMagnitude;
                    //                count1++;
                    //                break;
                    //            }
                    //        }
                    //        if(child.gameObject != null)
                    //            Debug.Log(child.gameObject.name);
                    //        averageAnim += child.position;

                    //    }
                    //    averageAnim /= count1;
                    //    Debug.Log("anim joint " + count1);
                    //    //if (minAvg > Mathf.Abs((averageAnim - average).magnitude))
                    //    if(minAvg > error)
                    //            index = 0;
                    //    //minAvg = Mathf.Min(minAvg, Mathf.Abs((averageAnim - average).magnitude));
                    //    minAvg = Mathf.Min(minAvg, error);
                    //}
                    if (index == 4 || index == 5)
                        anim.SetBool("isDrunk", true);
                    else if(index == 0 || index == 1)
                        anim.SetBool("isFighting", true);
                } //if (state==RagdollState.ragdolled)
			}	//if value==false	
		} //set
	} 

    //Possible states of the ragdoll
	public enum RagdollState
	{
		animated,	 //Mecanim is fully in control
		ragdolled,   //Mecanim turned off, physics controls the ragdoll
		blendToAnim  //Mecanim in control, but LateUpdate() is used to partially blend in the last ragdolled pose
	}
	
	//The current state
	public RagdollState state=RagdollState.animated;
	
	//How long do we blend when transitioning from ragdolled to animated
	public float ragdollToMecanimBlendTime=0.5f;
	float mecanimToGetUpTransitionTime=0.15f;
	
	//A helper variable to store the time when we transitioned from ragdolled to blendToAnim state
	float ragdollingEndTime=-100;
	
	//Declare a class that will hold useful information for each body part
	public class BodyPart
	{
		public Transform transform;
		public Vector3 storedPosition;
		public Quaternion storedRotation;
	}
	//Additional vectores for storing the pose the ragdoll ended up in.
	Vector3 ragdolledHipPosition,ragdolledHeadPosition,ragdolledFeetPosition;
	
	//Declare a list of body parts, initialized in Start()
	List<BodyPart> bodyParts=new List<BodyPart>();
	
	//Declare an Animator member variable, initialized in Start to point to this gameobject's Animator component.
	Animator anim;

    Rigidbody rb;

    //Animation Anim;

    public GameObject character;
    PoserScript pose_script;

    //A helper function to set the isKinematc property of all RigidBodies in the children of the 
    //game object that this script is attached to
    void setKinematic(bool newValue)
	{
		//Get an array of components that are of type Rigidbody
		Component[] components=GetComponentsInChildren(typeof(Rigidbody));

		//For each of the components in the array, treat the component as a Rigidbody and set its isKinematic property
		foreach (Component c in components)
		{
			(c as Rigidbody).isKinematic=newValue;
		}
	}
	
	// Initialization, first frame of game
	void Start ()
	{
		//Set all RigidBodies to kinematic so that they can be controlled with Mecanim
		//and there will be no glitches when transitioning to a ragdoll
		setKinematic(true);
		
		//Find all the transforms in the character, assuming that this script is attached to the root
		Component[] components=GetComponentsInChildren(typeof(Transform));
		
		//For each of the transforms, create a BodyPart instance and store the transform 
		foreach (Component c in components)
		{
			BodyPart bodyPart=new BodyPart();
			bodyPart.transform=c as Transform;
			bodyParts.Add(bodyPart);
		}
		
		//Store the Animator component
		anim=GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        //Anim = GetComponent<Animation>();
        pose_script = character.GetComponent<PoserScript>();
    }
	
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	void LateUpdate()
	{
		//Clear the get up animation controls so that we don't end up repeating the animations indefinitely
		anim.SetBool("GetUpFromBelly",false);
		anim.SetBool("GetUpFromBack",false);
        anim.SetBool("isDrunk", false);
        anim.SetBool("isFighting", false);

        //Blending from ragdoll back to animated
        if (state==RagdollState.blendToAnim)
		{
			if (Time.time<=ragdollingEndTime+mecanimToGetUpTransitionTime)
			{
				//If we are waiting for Mecanim to start playing the get up animations, update the root of the mecanim
				//character to the best match with the ragdoll
				Vector3 animatedToRagdolled=ragdolledHipPosition-anim.GetBoneTransform(HumanBodyBones.Hips).position;
				Vector3 newRootPosition=transform.position + animatedToRagdolled;
					
				//Now cast a ray from the computed position downwards and find the highest hit that does not belong to the character 
				RaycastHit[] hits=Physics.RaycastAll(new Ray(newRootPosition,Vector3.down)); 
				newRootPosition.y=0;
				foreach(RaycastHit hit in hits)
				{
					if (!hit.transform.IsChildOf(transform))
					{
						newRootPosition.y=Mathf.Max(newRootPosition.y, hit.point.y);
					}
				}
				transform.position=newRootPosition;
				
				//Get body orientation in ground plane for both the ragdolled pose and the animated get up pose
				Vector3 ragdolledDirection=ragdolledHeadPosition-ragdolledFeetPosition;
				ragdolledDirection.y=0;

				Vector3 meanFeetPosition=0.5f*(anim.GetBoneTransform(HumanBodyBones.LeftFoot).position + anim.GetBoneTransform(HumanBodyBones.RightFoot).position);
				Vector3 animatedDirection=anim.GetBoneTransform(HumanBodyBones.Head).position - meanFeetPosition;
				animatedDirection.y=0;
										
				//Try to match the rotations. Note that we can only rotate around Y axis, as the animated characted must stay upright,
				//hence setting the y components of the vectors to zero. 
				transform.rotation*=Quaternion.FromToRotation(animatedDirection.normalized,ragdolledDirection.normalized);
			}
			//compute the ragdoll blend amount in the range 0...1
			float ragdollBlendAmount=1.0f-(Time.time-ragdollingEndTime-mecanimToGetUpTransitionTime)/ragdollToMecanimBlendTime;
			ragdollBlendAmount=Mathf.Clamp01(ragdollBlendAmount);
			
			//In LateUpdate(), Mecanim has already updated the body pose according to the animations. 
			//To enable smooth transitioning from a ragdoll to animation, we lerp the position of the hips 
			//and slerp all the rotations towards the ones stored when ending the ragdolling
			foreach (BodyPart b in bodyParts)
			{
				if (b.transform!=transform){ //this if is to prevent us from modifying the root of the character, only the actual body parts
					//position is only interpolated for the hips
					if (b.transform==anim.GetBoneTransform(HumanBodyBones.Hips))
						b.transform.position=Vector3.Lerp(b.transform.position, b.storedPosition, ragdollBlendAmount);
					//rotation is interpolated for all body parts
					b.transform.rotation=Quaternion.Slerp(b.transform.rotation, b.storedRotation, ragdollBlendAmount);
				}
			}
			
			//if the ragdoll blend amount has decreased to zero, move to animated state
			if (ragdollBlendAmount==0)
			{
				state=RagdollState.animated;
                anim.SetBool("isDrunk", false);
                anim.SetBool("isFighting", false);
                anim.SetBool("isSet", false);
                return;
			}
		}
	}
	
}
