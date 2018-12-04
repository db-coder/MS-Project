using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigJoint : MonoBehaviour 
{
	public ConfigurableJoint cj;
	public Rigidbody rb;

	public Vector3 V1;
	private Quaternion QVec;

	// Update is called once per frame
	void Update ()
	{
		if(rb.IsSleeping())
			rb.WakeUp();
		if(Input.GetMouseButtonDown(0))
		{
			float h = Input.GetAxis("Mouse X");
			float v = Input.GetAxis("Mouse Y");
			V1[0] += 10*h;
			V1[2] += 10*v;
		}
		QVec = Quaternion.Euler(V1);
		cj.targetRotation = QVec;
	}
}
