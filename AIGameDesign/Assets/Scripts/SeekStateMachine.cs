using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekStateMachine : StateMachineBehaviour
{
    private AudioSource audio;
    public AudioClip clip;




    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {


        audio = animator.GetComponentInParent<AudioSource>();


        audio.clip = clip;
        audio.PlayDelayed(5);





    }



    // Start is called before the first frame update


    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
          audio.Stop();
    }
}
