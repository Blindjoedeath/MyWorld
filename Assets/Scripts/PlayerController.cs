using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private float xMouse;
    private float yMouse;

    private string[] parameters = new string[]
    {
        "Speed", "Shift", "Scream", "Hit", "Jump"
    };

    private string[] states = new string[]
    {
        "Scream", "Hit", "Jump"
    };

    private Dictionary<string, int> paramHash = new Dictionary<string, int>();
    private Dictionary<string, int> stateHashes = new Dictionary<string, int>();

    private Animator animator;
    private AudioSource audioSource;
    private Rigidbody rigidBody;

    public AudioClips audioClips;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rigidBody = GetComponent<Rigidbody>();

        audioClips.ResetLastClipIndex();

        foreach(var param in parameters)
        {
            paramHash[param] = Animator.StringToHash(param);
        }

        foreach(var state in states)
        {
            stateHashes[state] = Animator.StringToHash("Base Layer." + state);
        }

    }

    private void Update()
    {
        float vertAxis = Input.GetAxis("Vertical");
        float horAxis = Input.GetAxis("Horizontal");
        AnimatorStateInfo animStateInfo = animator.GetCurrentAnimatorStateInfo(0); 



        animator.SetFloat(paramHash["Speed"], vertAxis);


        animator.SetBool(paramHash["Shift"], Input.GetKey(KeyCode.LeftShift));
        if (animStateInfo.fullPathHash != stateHashes["Scream"] && animStateInfo.fullPathHash != stateHashes["Hit"]
            && animStateInfo.fullPathHash != stateHashes["Jump"])
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayClip();
                animator.SetTrigger(paramHash["Scream"]);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger(paramHash["Hit"]);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                animator.SetTrigger(paramHash["Jump"]);
                rigidBody.AddForce(Vector3.up * 100, ForceMode.Impulse);
            }
            
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

