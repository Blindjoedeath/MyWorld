using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private float xMouse;
    private float yMouse;

    private int speedHash;
    private int shiftHash;
    private int screamHash;

    private int idleStateSash;

    private Animator animator;
    private AudioSource audioSource;
    public AudioClips audioClips;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioClips.ResetLastClipIndex();

        speedHash = Animator.StringToHash("Speed");
        shiftHash = Animator.StringToHash("Shift");
        screamHash = Animator.StringToHash("Scream");

        idleStateSash = Animator.StringToHash("Base Layer.Idle");
    }

    private void Update()
    {
        float vertAxis = Input.GetAxis("Vertical");
        float horAxis = Input.GetAxis("Horizontal");
        AnimatorStateInfo animStateInfo = animator.GetCurrentAnimatorStateInfo(0); 



        animator.SetFloat(speedHash, vertAxis);
        animator.SetBool(shiftHash, Input.GetKey(KeyCode.LeftShift));
        if (Input.GetKeyDown(KeyCode.E) && animStateInfo.fullPathHash == idleStateSash)
        {
            PlayClip();
            animator.SetTrigger(screamHash);
            audioSource.Play();
        }

    }

    private void PlayClip()
    {
        var clip = audioClips.NextClip();
        audioSource.volume = clip.volume;
        audioSource.clip = clip.audioClip;

        audioSource.Play();
    }
}

