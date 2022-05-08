using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PatrolStateMachine :StateMachineBehaviour
{
    private AudioSource audio;
    public AudioClip clip;
    



    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {


        audio = animator.GetComponentInParent<AudioSource>();


        audio.clip = clip;
        audio.Play();





    }



    // Start is called before the first frame update


    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
          audio.Stop();
    }
}
