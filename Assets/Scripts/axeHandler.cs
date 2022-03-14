using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class axeHandler : MonoBehaviour
{

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //rotate the axe on its' forward axis whilst being thrown, using its' velocity to control the rot speed
        transform.localEulerAngles += Vector3.forward * rb.velocity.z;
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;

        //transform.LookAt(collision.gameObject.transform.position, transform.up);
        //print(transform.forward);
        //gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x, gameObject.transform.rotation.y, gameObject.transform.rotation.z);

    }

}
