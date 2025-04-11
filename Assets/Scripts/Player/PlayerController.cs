using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Movement Components
    private CharacterController controller;
    private Animator animator;

    private float moveSpeed = 4f;

    [Header("Movement System")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;

    private float gravity = 9.81f;


    //Interaction components
    PlayerInteraction playerInteraction;

    private CameraController cameraController;

    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 10f;
    private float currentZoom = 5f;

    // Start is called before the first frame update
    void Start()
    {
        //Get movement components
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        //Get interaction component
        playerInteraction = GetComponentInChildren<PlayerInteraction>();

        cameraController = FindObjectOfType<CameraController>();
        currentZoom = cameraController != null ? cameraController.defaultOffset : 5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.GameIsPaused) return;
        //Runs the function that handles all movement
        Move();

        //Runs the function that handles all interaction
        Interact();


        //Debugging purposes only
        //Skip the time when the right square bracket is press
        /*if (Input.GetKey(KeyCode.P))
        {
            if (Input.GetKey(KeyCode.LeftShift)) {
                //Advance the entire day
                for(int i =0; i< 60*24;  i++)
                {
                    TimeManager.Instance.Tick();
                }
            } else
            {
                TimeManager.Instance.Tick();
            }


        }*/
        /*
        //Toggle relationship panel
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (UIManager.Instance.IsUIActive() && UIManager.Instance.IsUIActive("PlayerMenu") || UIManager.Instance.IsUIActive() == false)
            {
                UIManager.Instance.ToggleRelationshipPanel();
            }
        }
        /*

        /*
        if (Input.GetKeyDown(KeyCode.N))
        {
            SceneTransitionManager.Location location = LocationManager.GetNextLocation(SceneTransitionManager.Location.PlayerHome, SceneTransitionManager.Location.Farm);
            Debug.Log(location);
        }
        */

        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.E))
        {
            if (UIManager.Instance.IsUIActive() && UIManager.Instance.IsUIActive("PlayerMenu") || UIManager.Instance.IsUIActive() == false)
            {
                UIManager.Instance.DisplayItemInfo(null);
                UIManager.Instance.ToggleMenuPanel();
            }
        }

        if (UIManager.Instance.IsUIActive()) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            HandleCameraZoom(scroll);
        }
    }

    public void Interact() 
    {
        if (UIManager.Instance.IsUIActive()) return;

        //Tool interaction
        if (Input.GetButtonDown("Fire1"))
        {
            /*
            Skill skill = SkillManager.Instance.GetSkill(SkillType.Farming);

            SkillManager.Instance.AddExperience(skill, 100);*/

            //Interact
            playerInteraction.Interact();
        }

        //Item interaction
        if (Input.GetButtonDown("Fire2"))
        {
            
            Skill skill = SkillManager.Instance.GetSkill(SkillType.Cooking);

            SkillManager.Instance.AddExperience(skill, 100);

            playerInteraction.ItemInteract();
        }

        //Keep items
        if (Input.GetButtonDown("Fire3"))
        {
            playerInteraction.ItemKeep();
        }
    }


    public void Move()
    {
        bool isTooTired = UIManager.Instance.staminaCount <= UIManager.Instance.staminaCountTooTired;

        //Get the horizontal and vertical inputs as a number
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //Direction in a normalised vector
        Vector3 dir = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 velocity = moveSpeed * Time.deltaTime * dir;

        if (controller.isGrounded)
        {
            velocity.y = 0;
        }
        velocity.y -= Time.deltaTime * gravity;

        if (isTooTired)
        {
            moveSpeed = walkSpeed / 2;
            animator.SetBool("Running", false);
        }
        else
        {
            //Is the sprint key pressed down?
            if (Input.GetButton("Sprint"))
            {
                //Set the animation to run and increase our movespeed
                moveSpeed = runSpeed;
                animator.SetBool("Running", true);
            } else
            {
                //Set the animation to walk and decrease our movespeed
                moveSpeed = walkSpeed;
                animator.SetBool("Running", false);
            }
        }


        //Check if there is movement
        if(dir.magnitude >= 0.1f)
        {
            //Look towards the direction
            transform.rotation = Quaternion.LookRotation(dir);

            //Move if allowed
            if (controller.enabled)
            {
                controller.Move(velocity);
            }


        }

        //Animation speed parameter
        animator.SetFloat("Speed", dir.magnitude);



    }

    void HandleCameraZoom(float scroll)
    {
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        if (cameraController != null)
        {
            cameraController.SetZoomOffset(currentZoom);
        }
    }
}
