using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

//require some things the bot control needs
[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
public class YBotSimpleControlScript : MonoBehaviour
{
    private Animator anim;	
    private Rigidbody rbody;
    private Rigidbody rbody_ball;

    private Transform leftFoot;
    private Transform rightFoot;
    private Transform leftHand;

    public int groundContacts = 0;
   
    private float filteredForwardInput = 0f;
    private float filteredTurnInput = 0f;

    public float forwardInputFilter = 5f;
    public float turnInputFilter = 5f;

    private float forwardSpeedLimit = 5f;
    private Vector3 lastVelocity;
    private float force = 10f;

    public GameObject ballPrefab;
    private GameObject heldBall = null;
    private float timeOfDeath;
    private bool death;

    public bool IsGrounded
    {
        get { return groundContacts > 0; }
    }

    void Awake()
    {
        anim = GetComponent<Animator>();

        if (anim == null)
            Debug.Log("Animator could not be found");

        rbody = GetComponent<Rigidbody>();

        if (rbody == null)
            Debug.Log("Rigid body could not be found");
    }


    // Use this for initialization
    void Start()
    {
		//example of how to get access to certain limbs
        leftFoot = this.transform.Find("mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg/mixamorig:LeftFoot");
        rightFoot = this.transform.Find("mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot");
        leftHand = this.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand");

        if (leftFoot == null || rightFoot == null)
            Debug.Log("One of the feet could not be found");
    }

    //Update whenever physics updates with FixedUpdate()
    //Updating the animator here should coincide with "Animate Physics"
    //setting in Animator component under the Inspector
    void FixedUpdate()
    {
        //GetAxisRaw() so we can do filtering here instead of the InputManager
        float h = Input.GetAxisRaw("Horizontal");// setup h variable as our horizontal input axis
        float v = Input.GetAxisRaw("Vertical");	// setup v variables as our vertical input axis

        //enforce circular joystick mapping which should coincide with circular blendtree positions
        Vector2 vec = Vector2.ClampMagnitude(new Vector2(h, v), 1.0f);

        h = vec.x;
        v = vec.y;

        //BEGIN ANALOG ON KEYBOARD DEMO CODE
        if (Input.GetKey(KeyCode.Q))
            h = -0.5f;
        else if (Input.GetKey(KeyCode.E))
            h = 0.5f;
        else if (Input.GetKey(KeyCode.G))
        {
            bool drunk = anim.GetBool("isDrunk");
            anim.SetBool("isDrunk", !drunk);
        }
        else if (Input.GetKey(KeyCode.F))
        {
            bool fight = anim.GetBool("isFighting");
            anim.SetBool("isFighting", !fight);
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
            forwardSpeedLimit = 0.1f;
        else if (Input.GetKeyUp(KeyCode.Alpha2))
            forwardSpeedLimit = 0.2f;
        else if (Input.GetKeyUp(KeyCode.Alpha3))
            forwardSpeedLimit = 0.3f;
        else if (Input.GetKeyUp(KeyCode.Alpha4))
            forwardSpeedLimit = 0.4f;
        else if (Input.GetKeyUp(KeyCode.Alpha5))
            forwardSpeedLimit = 0.5f;
        else if (Input.GetKeyUp(KeyCode.Alpha6))
            forwardSpeedLimit = 0.6f;
        else if (Input.GetKeyUp(KeyCode.Alpha7))
            forwardSpeedLimit = 0.7f;
        else if (Input.GetKeyUp(KeyCode.Alpha8))
            forwardSpeedLimit = 0.8f;
        else if (Input.GetKeyUp(KeyCode.Alpha9))
            forwardSpeedLimit = 0.9f;
        else if (Input.GetKeyUp(KeyCode.Alpha0))
            forwardSpeedLimit = 1.0f;
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            anim.SetTrigger("Throw");
            if (ballPrefab != null)
            {
              heldBall = Instantiate(ballPrefab, leftHand.position + new Vector3(0.05f, -0.1f, 0), Quaternion.identity);
              rbody_ball = heldBall.GetComponent<Rigidbody>();
              heldBall.transform.parent = leftHand;
              rbody_ball.isKinematic = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            anim.SetTrigger("Jumping");
            lastVelocity = anim.velocity;
            force = 10f;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
        	death = true;
            timeOfDeath = Time.timeSinceLevelLoad;
            ragdollDeath();
        }
        //END ANALOG ON KEYBOARD DEMO CODE  

        //do some filtering of our input as well as clamp to a speed limit
        filteredForwardInput = Mathf.Clamp(Mathf.Lerp(filteredForwardInput, v, 
                Time.deltaTime * forwardInputFilter), -forwardSpeedLimit, forwardSpeedLimit);
        
        filteredTurnInput = Mathf.Lerp(filteredTurnInput, h, 
            Time.deltaTime * turnInputFilter);
                                                    
        //finally pass the processed input values to the animator
        anim.SetFloat("velx", filteredTurnInput);	// set our animator's float parameter 'Speed' equal to the vertical input axis				
        anim.SetFloat("vely", filteredForwardInput); // set our animator's float parameter 'Direction' equal to the horizontal input axis		


        //if (Input.GetButtonDown("Fire1")) //normally left-ctrl on keyboard
        //    anim.SetTrigger("throw"); 
        if(death && Time.timeSinceLevelLoad - timeOfDeath >= 5)
        {
        	death = false;
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void ExecuteJumpCallback()
    {
        anim.applyRootMotion = false;
        // capsule.material = noFrictionPhysicsMaterial;
        Vector3 launchV = lastVelocity.magnitude*transform.forward + force*Vector3.up;
        rbody.AddForce(launchV, ForceMode.VelocityChange);
        EventManager.TriggerEvent<JumpEvent, Vector3>(transform.position);
    }

    void ExecuteThrowCallback()
    {
        heldBall.transform.parent = null;
        rbody_ball.isKinematic = false;
        Vector3 launchV = force*transform.forward;
        rbody_ball.AddForce(launchV, ForceMode.VelocityChange);
        // EventManager.TriggerEvent<JumpEvent, Vector3>(transform.position);
    }

    void EndJumpCallback()
    {
        anim.applyRootMotion = true;
    }

    //This is a physics callback
    void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.gameObject.tag == "ground" || collision.transform.gameObject.tag == "Surface_Dirt")
        {
            ++groundContacts;

            //Debug.Log("Player hit the ground at: " + collision.impulse.magnitude);

            if (collision.impulse.magnitude > 100f)
            {               
                EventManager.TriggerEvent<PlayerLandsEvent, Vector3>(collision.contacts[0].point);
            }
        }		
    }

    //This is a physics callback
    void OnCollisionExit(Collision collision)
    {
        if (collision.transform.gameObject.tag == "ground" || collision.transform.gameObject.tag == "Surface_Dirt")
            --groundContacts;
    }

    void OnAnimatorMove()
    {
        bool isFalling = !IsGrounded;

        if (isFalling)
        {
            // EventManager.TriggerEvent<FallEvent, Vector3>(this.transform.position);
            const float rayOriginOffset = 1f; //origin near bottom of collider, so need a fudge factor up away from there
            const float rayDepth = 1f; //how far down will we look for ground?
            const float totalRayLen = rayOriginOffset + rayDepth;

            Ray ray = new Ray(this.transform.position + Vector3.up * rayOriginOffset, Vector3.down);

            //visualize ray in the editor
            //I'm using DrawLine because Debug.DrawRay() doesn't allow setting ray length past a certain size
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * totalRayLen, Color.green);

            RaycastHit hit;

            //Cast ray and look for ground. If ground is close, then transition out of falling animation
            if (Physics.Raycast(ray, out hit, totalRayLen))
            {
                if (hit.collider.gameObject.CompareTag("ground") || hit.collider.gameObject.CompareTag("Surface_Dirt"))
                {
                    isFalling = false; //turning falling back off because we are close to the ground

                    //draw an X that denotes where ray hit
                    const float ZBufFix = 0.01f;
                    const float edgeSize = 0.2f;
                    Color col = Color.red;

                    Debug.DrawRay(hit.point + Vector3.up * ZBufFix, Vector3.forward * edgeSize, col);
                    Debug.DrawRay(hit.point + Vector3.up * ZBufFix, Vector3.left * edgeSize, col);
                    Debug.DrawRay(hit.point + Vector3.up * ZBufFix, Vector3.right * edgeSize, col);
                    Debug.DrawRay(hit.point + Vector3.up * ZBufFix, Vector3.back * edgeSize, col);
                }
            }
        }

        anim.SetBool("isFalling", isFalling);

        if (IsGrounded)
        {
         	//use root motion as is if on the ground		
            this.transform.position = anim.rootPosition;
        }
        else
        {
            //Simple trick to keep model from climbing other rigidbodies that aren't the ground
            this.transform.position = new Vector3(anim.rootPosition.x, this.transform.position.y, anim.rootPosition.z);
        }

        //use rotational root motion as is
        this.transform.rotation = anim.rootRotation;
    }

    public void ragdollDeath()
    {
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            rb.isKinematic = false;
            rb.velocity = new Vector3(0,0,0);
            rb.angularVelocity = new Vector3(0,0,0);
        }
        anim.enabled = false;
        EventManager.TriggerEvent<PlayerDeathEvent, Vector3>(this.transform.position);
    }
}