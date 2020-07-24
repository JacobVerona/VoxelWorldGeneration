using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public bool isGrounded;
    public bool isSprinting;

    private Transform camTrasform;
    private World world;


    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpForce = 5f;
    public float gravity = -9.8f;

    public float playerWidth = 0.15f;
    public float playerHeight = 2f;

    private float horizontal;
    private float vertical;
    private float up;

    private float mouseHorizontal;
    private float mouseVertical;

    private Vector3 velocity;
    private float verticalMomentum = 0;
    public bool jumpRequest;

    public Transform highLightBlock;
    public Transform place_highLightBlock;

    public float checkIncrement = 0.1f;
    public float reach = 8f;

    public Text selectedBlockText;
    public byte selectedBlockIndex = 1;

    private void Start ()
    {
        camTrasform = GameObject.Find("Main Camera").transform;
        world = GameObject.Find("World").GetComponent<World>();

        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate ()
    {


        CalculateVelocity();
        if (jumpRequest)
        {
            Jump();
        }
        transform.Rotate(Vector3.up * mouseHorizontal);
        camTrasform.Rotate(Vector3.right * -mouseVertical);

        transform.Translate(velocity, Space.World);
    }


    private void Update ()
    {
        GetPlayerInputs();
        placeCursorBlock();
    }

    void Jump ()
    {
        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;

    }


    private void CalculateVelocity ()
    {

        velocity = ((transform.forward * vertical) + (transform.right * horizontal) + (transform.up * up))  * Time.fixedDeltaTime * walkSpeed;


        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        
    }

    private void GetPlayerInputs ()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");

        /*if (Input.GetButtonDown("Sprint"))
        {
            isSprinting = true;
        }
        if (Input.GetButtonUp("Sprint"))
        {
            isSprinting = false;
        }*/

        up = Input.GetAxis("Jump");
        


        if (highLightBlock.gameObject.activeSelf)
        {
            //Destroy
            if (Input.GetMouseButtonDown(0))
            {
                world.GetChunkFromV3(highLightBlock.position).SetBlock(World.GlobalCoordToLocalCoord(highLightBlock.position), BlockDatabase.blockDatas["voxel:air"]);
            }
            if (Input.GetMouseButtonDown(1))
            {
                world.GetChunkFromV3(place_highLightBlock.position).SetBlock(World.GlobalCoordToLocalCoord(place_highLightBlock.position), BlockDatabase.blockDatas["voxel:stone"]);
            }
        }

    }

    private void placeCursorBlock ()
    {
        float step = checkIncrement;
        Vector3 lastPos = new Vector3();

        while (step < reach)
        {
            Vector3 pos = (camTrasform.position + (camTrasform.forward * step)+new Vector3(0.5f,0.5f,0.5f));

            if (world.CheckForVoxel(pos))
            {
                highLightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                place_highLightBlock.position = lastPos;

                highLightBlock.gameObject.SetActive(true);
                place_highLightBlock.gameObject.SetActive(true);
                return;
            }

            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

            step += checkIncrement;
        }
        highLightBlock.gameObject.SetActive(false);
        place_highLightBlock.gameObject.SetActive(false);

    }


  
}
