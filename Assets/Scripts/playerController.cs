using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{

    public CharacterController controller;
    public Animator anim;

    [SerializeField]
    float speed;
    [SerializeField]
    float turnSmoothTime;
    [SerializeField]
    float throwStrength;
    [SerializeField]
    Transform cam;
    [SerializeField]
    Rigidbody axe;
    [SerializeField]
    GameObject rHand;
    [SerializeField]
    float axeReturnTime;

    float turnSmoothVelocity;
    Vector3 axeLerpStartPos;
    float elapsedTime;
    Quaternion axeHoldRot;
    Vector3 axeHoldPos;
    bool returning;
    BoxCollider handColl;
    handHandler handHandler;


    // Start is called before the first frame update
    void Start()
    {
        //Lock the cursor into the game window so when movng the camera the mouse doesnt escape
        Cursor.lockState = CursorLockMode.Locked;

        //set the default local transform for the axe position and rotation so that later these can be restored upon axe return
        axeHoldPos = axe.transform.localPosition;
        axeHoldRot = axe.transform.localRotation;

        //get the hand handler script
        handHandler = rHand.GetComponent<handHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        //get movement direction from the input axes
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        float directionMagnitude = Mathf.Clamp01(direction.magnitude) / 2;

        //adjust magnitude for sprint
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton11))
        {
            directionMagnitude = 1;
        }

        //Direction magnitude is passed to the animation blend tree to decide whether to play the idle, walk or run animation based on its value
        anim.SetFloat("movementBlend", directionMagnitude, 0.05f, Time.deltaTime);

        //change player rotation relative to camera forward
        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //allow for movement animation
            anim.SetBool("isMoving", true);
        }
        else
        {
            //disallow movement animation
            anim.SetBool("isMoving", false);
        }

        //Prepare for axe to return on keypress by changing it's state and getting the required variables
        if (Input.GetKeyDown(KeyCode.R) && !axe.transform.parent)
        {
            axe.detectCollisions = true;
            axe.isKinematic = false;
            axe.useGravity = false;
            axeLerpStartPos = axe.transform.position; //used to inform the lerp function of the axe's start position
            returning = true;

            //enable hand collider to allow for checking when axe reaches hand
            rHand.GetComponent<BoxCollider>().enabled = true;
        }

        //when returning and colliding with the player's hand, reset the axe to the holding state
        if (returning && handHandler.axeInRange)
        {
            rHand.GetComponent<BoxCollider>().enabled = false;
            axe.transform.SetParent(rHand.transform);
            
            axe.isKinematic = true;
            axe.useGravity = true;
            axe.detectCollisions = true;
            axe.transform.localPosition = axeHoldPos;
            axe.transform.localRotation = axeHoldRot;
            
            elapsedTime = 0;
            
            handHandler.axeInRange = false;
            returning = false;
            
        }

        

        //camHandler.aiming = Input.GetMouseButton(1) ? true : false;
        
        //if(camHandler.aiming)
        //{

        //    transform.rotation = camHandler.transform.rotation;
        //}

    }

    private void FixedUpdate()
    {
        //while the axe is returning use lerp to transition the axe back towards the player's hand over time
        if (returning)
        {

            elapsedTime += Time.deltaTime;
            axe.transform.position = Vector3.Lerp(axeLerpStartPos, rHand.transform.position, elapsedTime / axeReturnTime);

        }

        //if left click and holding the axe, begin preparing to throw the axe
        if ((Input.GetKey(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.JoystickButton11)) && axe.transform.parent)
        {
            throwBegin();
        }

    }

    private void OnAnimatorMove()
    {
        //derive the player velocity from the rootmotion of the playing animation and apply it
        Vector3 velocity = anim.deltaPosition;
        controller.Move(velocity);
    }

    //prime the animation event that will call the axe, this event is in the anim timeline at the apex of the player's attack animation
    private void throwBegin()
    {
        if (!anim.GetCurrentAnimatorStateInfo(1).IsTag("attacking"))
        {
            rHand.GetComponent<BoxCollider>().enabled = false; //disable the collider so that the axe does not instantly collide and become collison stuck on the player hand instantly after being thrown
            anim.SetTrigger("attacking");
        }
    }

    //called by the animator event trigger that was primed in throwBegin()
    //add forward force to the axe and deparent it from the player model
    private void throwAxe()
    {       

        axe.isKinematic = false;
        axe.transform.parent = null;
        axe.AddForce(transform.forward * throwStrength, ForceMode.Impulse);
        axe.transform.rotation = Quaternion.Euler(0, 90 + transform.eulerAngles.y, 0); //fix for the default transform of the axe being rotated 90 degrees in the wrong direction, hence plus 90 TODO: fix default axe rot

        anim.ResetTrigger("attacking");//reset trigger to pre activated state for next throw

    }

}
