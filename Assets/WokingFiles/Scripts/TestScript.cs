using UnityEngine;
using UnityEngine.Rendering;

public class TestScript : MonoBehaviour
{
    [SerializeField]
    private GameObject puzzle;
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private Vector2 moveInput;
    [SerializeField]
    private bool gravityFlip;
    [SerializeField]
    private bool gravityRotate;
    [SerializeField]
    private bool gravityReset;
    [SerializeField]
    private bool is_AffectingPlayer = false;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private bool destory;

    private CustomGravityProvider customGravityProvider;
    private float rotationCount = 0;
    Vector3 gravityForce = new Vector3(0, -9.81f, 0);

    void Awake()
    {
        //inputSystem = new InputSystem_Actions();
        //inputSystem.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        Debug.Log($"Found {audioSources.Length} objects with AudioSource:");

        foreach (AudioSource source in audioSources)
        {
            Debug.Log($"- {source.gameObject.name}", source.gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        customGravityProvider = player.GetComponent<CustomGravityProvider>();
    }

    // Update is called once per frame
    void Update()
    {
        //puzzle.transform.Rotate(Vector3.up * rotateSpeed * moveInput.x * Time.deltaTime);

        if (gravityFlip)
        {
            GravityFlip();
            gravityFlip = false;
        }

        if (gravityRotate)
        {
            GravityRotate();
            gravityRotate = false;
        }

        if (gravityReset)
        {
            GravityReset();
            gravityReset = false;
        }
        if (destory)
        {
            Destroy(puzzle);
            destory = false;
        }

    }
    private void GravityFlip()
    {
        //customGravityProvider.InvertGravity();
        gravityForce = gravityForce * -1;
        Physics.gravity = gravityForce;
    }
    private void GravityRotate()
    {
        //customGravityProvider.RotateGravity();
        gravityForce = Quaternion.Euler(0, 0, 90) * gravityForce;
        Physics.gravity = gravityForce;

        Debug.Log("New gravity: " + gravityForce);

    }
    private void GravityReset()
    {
        gravityForce = new Vector3(0, 9.81f, 0);
        Physics.gravity = gravityForce;
    }
}