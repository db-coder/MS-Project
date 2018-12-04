using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipData : MonoBehaviour {

    //public Animation anim;
    //private List<AnimationClip> Data;
    // Use this for initialization
    void Start () {
        //anim = GetComponent<Animation>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void getClipData(){
        //Data = new List<AnimationClip>();
        //#if UNITY_EDITOR
        //Animator anim = GetComponent<Animator>();
        //if (anim != null)
        //{
        //    UnityEditor.Animations.AnimatorController ac = anim.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

        //    for (int x = 0; x < anim.layerCount; x++)
        //    {
        //        UnityEditor.Animations.AnimatorStateMachine sm = ac.layers[x].stateMachine;
        //        if (x == 0)
        //        {
        //            string DefaultStateName = sm.defaultState.name;
        //            //string DefaultStateHash = sm.defaultState.uniqueNameHash;

        //            if (true) Debug.Log("AnimatorInfo[" + name + "].Gather: DefaultStateName:" + DefaultStateName);
        //        }

        //        for (int i = 0; i < sm.states.Length; i++)
        //        {
        //            UnityEditor.Animations.AnimatorState state = sm.states[i].state;
        //            Motion m = state.motion;

        //            if (m != null)
        //            {
        //                AnimationClip clip = m as AnimationClip;
        //                Data.Add(clip);
        //            }
        //            else
        //            {
        //                Data.Add(null);
        //            }
        //        }
        //    }
        //}
        //#endif
        //AnimationState state = anim;
        //foreach (AnimationState state in anim)
        //{
        //    state.enabled = true;
        //    state.normalizedTime = (1.0f / state.length) * 1;
        //    anim.Sample();

        //    Transform[] transforms = anim.GetComponentsInChildren<Transform>();
        //}
    }
}
