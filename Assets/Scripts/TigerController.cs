using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerController : MonoBehaviour {

    [Range(1, 10)]
    public float walkSpeed;

    [Range(1, 10)]
    public float runSpeed;
    public float jumpForce;

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
    private Collider collider;
    private ParticlesController particlesController;
    private Transform environmentBase;

    AnimatorStateInfo animStateInfo;

    private float groundDist;
    private bool isGrounded;

    public AudioClips audioClips;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rigidBody = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();
        collider = GetComponent<Collider>();
        particlesController = GetComponentInChildren<ParticlesController>();
        environmentBase = GameObject.FindGameObjectWithTag("Environment").transform;
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

    private void FixedUpdate()
    {
        float vertAxis = Input.GetAxis("Vertical");
        float horAxis = Input.GetAxis("Horizontal");
        float mouseHorAxis = Input.GetAxis("Mouse X");
        float mouseVertAxis = Input.GetAxis("Mouse Y");

        CheckGround();

        Debug.Log(isGrounded);
        playerCamera.transform.RotateAround(transform.position, Vector3.up, mouseHorAxis);

        transform.Rotate(new Vector3(0, horAxis, 0));

        animStateInfo = animator.GetCurrentAnimatorStateInfo(0); 



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
                PlayParticles("Hit");
            }
            else if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                animator.SetTrigger(paramHash["Jump"]);
                rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
        if (isGrounded)
        {
            Vector3 delta = transform.forward * vertAxis;
            if (animStateInfo.fullPathHash == stateHashes["Walk"])
            {
                delta *= walkSpeed;
            }
            else if (animStateInfo.fullPathHash == stateHashes["Run"])
            {
                delta *= runSpeed;
            }
            rigidBody.velocity = new Vector3(delta.x, rigidBody.velocity.y, delta.z);
        }
    }
    private Color[] colors = new Color[]
    {
        Color.red, Color.gray, Color.blue, Color.green
    };


    private Vector3[] FindOrigins()
    {
        Vector3 center = collider.bounds.center;
        Vector3[] rayOrigins = new Vector3[4];
        rayOrigins[0] = new Vector3(center.x - collider.bounds.extents.x, center.y, center.z - collider.bounds.extents.z);
        rayOrigins[1] = new Vector3(center.x - collider.bounds.extents.x, center.y, center.z + collider.bounds.extents.z);
        rayOrigins[2] = new Vector3(center.x + collider.bounds.extents.x, center.y, center.z - collider.bounds.extents.z);
        rayOrigins[3] = new Vector3(center.x + collider.bounds.extents.x, center.y, center.z + collider.bounds.extents.z);

        return rayOrigins;
    }

    private void CheckGround()
    {
        RaycastHit hit = new RaycastHit();
        Vector3[] rayOrigins = FindOrigins();
        isGrounded = false;
        for (int i = 0; i < rayOrigins.Length && !isGrounded; ++i)
        {
            if (isGrounded = Physics.Raycast(new Ray(rayOrigins[i], -transform.up), out hit, groundDist + 0.3f))
            {
                isGrounded = hit.collider.transform.IsChildOf(environmentBase);
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

    private void PlayParticles(string state)
    {
        particlesController.Emit(1);
    }
}

