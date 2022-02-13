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


    // Start is called before the first frame update
    void Start()
    {
        axeHoldPos = axe.transform.localPosition;
        axeHoldRot = axe.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        float directionMagnitude = Mathf.Clamp01(direction.magnitude) / 2;

        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton11))
        {
            directionMagnitude = 1;
        }

        anim.SetFloat("movementBlend", directionMagnitude, 0.05f, Time.deltaTime);

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }

        if ((Input.GetKey(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.JoystickButton11)) && axe.transform.parent)
        {
            throwBegin();
        }

        if (Input.GetKeyDown(KeyCode.R) && !axe.transform.parent)
        {
            axe.detectCollisions = false;
            axe.isKinematic = false;
            axe.useGravity = false;
            axeLerpStartPos = axe.transform.position;
            returning = true;
            
            

        }

        if (returning && (Vector3.Dot(axe.transform.position, rHand.transform.position) > .9999f))
        {
            returning = false;
            axe.isKinematic = true;
            axe.useGravity = true;
            axe.detectCollisions = true;
            axe.transform.SetParent(rHand.transform);
            axe.transform.localPosition = axeHoldPos;
            axe.transform.localRotation = axeHoldRot;
            elapsedTime = 0;
            
        }

        if (returning)
        {
            elapsedTime += Time.deltaTime;

            axe.transform.position = Vector3.Lerp(axeLerpStartPos, rHand.transform.position, (elapsedTime / axeReturnTime));
        }
    }

    private void OnAnimatorMove()
    {
        Vector3 velocity = anim.deltaPosition;
        controller.Move(velocity);
    }

    private void throwBegin()
    {
        if (!anim.GetCurrentAnimatorStateInfo(1).IsTag("attacking"))
        {
            anim.SetTrigger("attacking");
        }
    }

    private void throwAxe()
    {       

        axe.isKinematic = false;
        axe.transform.parent = null;
        axe.AddForce(transform.forward * throwStrength);
        axe.transform.rotation = Quaternion.Euler(0, 90 + transform.eulerAngles.y, 0);

        anim.ResetTrigger("attacking");

    }
}