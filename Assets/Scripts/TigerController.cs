using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerController : MonoBehaviour {

    [Range(1, 10)]
    public float walkSpeed;

    [Range(1, 10)]
    public float runSpeed;
    private float jumpForce = 100;
    public List<ParticleSystem> particleSystems = new List<ParticleSystem>();

    public bool enable;

    private string[] parameters = new string[]
    {
        "Speed", "Shift", "Scream", "Hit", "Jump"
    };

    private string[] states = new string[]
    {
        "Scream", "Hit", "Jump", "Walk", "Run"
    };

    private Dictionary<string, int> paramHash = new Dictionary<string, int>();
    private Dictionary<string, int> stateHashes = new Dictionary<string, int>();

    private Animator animator;
    private AudioSource audioSource;
    private Rigidbody rigidBody;
    private Camera playerCamera;
    private Collider terrainCollider;
    private Collider collider;
    private float groundDist;

    public AudioClips audioClips;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rigidBody = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();
        terrainCollider = FindObjectOfType<Terrain>().GetComponent<Collider>();
        collider = GetComponent<Collider>();


        groundDist = collider.bounds.extents.y;
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
        float mouseHorAxis = Input.GetAxis("Mouse X");
        float mouseVertAxis = Input.GetAxis("Mouse Y");


        playerCamera.transform.RotateAround(transform.position, Vector3.up, mouseHorAxis);

        transform.Rotate(new Vector3(0, horAxis, 0));

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
                PlayParticles();
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                animator.SetTrigger(paramHash["Jump"]);
                rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }         
        }
        if (IsGrounded())
        {
            if (animStateInfo.fullPathHash == stateHashes["Walk"])
            {
                rigidBody.velocity = transform.forward * vertAxis * walkSpeed;
            }
            else if (animStateInfo.fullPathHash == stateHashes["Run"])
            {
                rigidBody.velocity = transform.forward * vertAxis * runSpeed;
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

    private bool IsGrounded()
    {
        RaycastHit hit = new RaycastHit();
        Physics.Raycast(new Ray(collider.bounds.center, -transform.up), out hit, groundDist + 0.2f);
        return hit.collider == terrainCollider;
    }


    private int lastParticlesIndex = 0;
    private void PlayParticles()
    {
        if (lastParticlesIndex == particleSystems.Count)
        {
            lastParticlesIndex = 0;
        }
        if (!particleSystems[lastParticlesIndex].isPlaying)
        {
            particleSystems[lastParticlesIndex].Play();
            particleSystems[lastParticlesIndex++].Emit(1);
        }
    }
}

