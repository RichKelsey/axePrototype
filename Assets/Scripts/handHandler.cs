using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handHandler : MonoBehaviour
{
    public bool axeInRange;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //check for axe colliding upon return of axe
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "axe")
        {
            axeInRange = true;
        }
    }
}
