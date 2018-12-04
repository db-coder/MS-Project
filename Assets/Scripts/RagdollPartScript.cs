using UnityEngine;
using System.Collections;

public class RagdollPartScript : MonoBehaviour {
    //Declare a reference to the main script (of type StairDismount).
    //This will be set by the code that adds this script to all ragdoll parts

    //Declare an Animator member variable, initialized in Start to point to this gameobject's Animator component.
    //Animator anim;

    public StairDismount mainScript;
	// Use this for initialization
	void Start () {
        //anim = this.GetComponentInParent<Animator>();
    }
	void OnCollisionEnter(Collision collision)
	{
		//Increase score if this ragdoll part collides with something else
		//than another ragdoll part with sufficient velocity. 
		//If the colliding object is another ragdoll part, it will have the same root, hence the inequality check.
		if (transform.root != collision.transform.root)
		{			
			//Check that we are colliding with sufficient velocity
			if (collision.relativeVelocity.magnitude > 4.0f){
				//compute score
				//int score=100*Mathf.RoundToInt(collision.relativeVelocity.magnitude);
				//print (gameObject.name + " collided with " + collision.gameObject.name + ", giving score ");
				
				//increase the main script's score variable (see StairDismount.cs)
				//mainScript.score += score;
				
				//Instantiate a text object
				//GameObject scoreText=Instantiate(mainScript.scoreTextTemplate) as GameObject;
				
				//Update the text to show the score
				//scoreText.GetComponent<TextMesh>().text=score.ToString();
				
				//position the text 1m above this ragdoll part
				//scoreText.transform.position=transform.position;
				//scoreText.transform.Translate(0,1,0);
			}
		}
        
        foreach (ContactPoint contact in collision.contacts)
        {
            //bool set = anim.GetBool("isSet");
            //print(set);
            ////if (set != true)
            //{
            //    print(contact.thisCollider.name + " hit " + contact.otherCollider.name);
            //    if (contact.otherCollider.name == "_collision_main_001")
            //    {
            //        if (contact.thisCollider.name == "mixamorig:Neck" || contact.thisCollider.name == "mixamorig:Head")
            //        {
            //            print(contact.thisCollider.name);
            //            anim.SetBool("isFighting", true);
            //            anim.SetBool("isDrunk", false);
            //            anim.SetBool("isSet", true);
            //            break;
            //        }
            //        else if (contact.thisCollider.name == "mixamorig:LeftUpLeg" || contact.thisCollider.name == "mixamorig:LeftLeg" || contact.thisCollider.name == "mixamorig:LeftFoot" || contact.thisCollider.name == "mixamorig:RightUpLeg" || contact.thisCollider.name == "mixamorig:RightLeg" || contact.thisCollider.name == "mixamorig:RightFoot")
            //        {
            //            print(contact.thisCollider.name);
            //            anim.SetBool("isDrunk", true);
            //            anim.SetBool("isFighting", false);
            //            anim.SetBool("isSet", true);
            //            break;
            //        }
            //    }
            //}
        }
    }
    //void OnCollisionStay(Collision collision)
    //{
    //    foreach (ContactPoint contact in collision.contacts)
    //    {
    //        print(contact.thisCollider.name + " hit " + contact.otherCollider.name);
            
    //    }
    //}
    // Update is called once per frame
    void Update () {
	
	}
}
