using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class axeHandler : MonoBehaviour
{

    Rigidbody rb;
    Ray directionCheck;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localEulerAngles += Vector3.forward * rb.velocity.z;
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
        rb.isKinematic = true;
        print(transform.parent);

        //transform.LookAt(collision.gameObject.transform.position, transform.up);
        //print(transform.forward);
        //gameObject.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x, gameObject.transform.rotation.y, gameObject.transform.rotation.z);

    }
}
