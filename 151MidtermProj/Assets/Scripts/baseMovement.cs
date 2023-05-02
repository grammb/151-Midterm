using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//************** use UnityOSC namespace...
using UnityOSC;
//*************

public class baseMovement : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;


    //************* Need to setup this server dictionary...
    Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog>();
    //*************


    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true; //allows unity to update when not in focus

        //************* Instantiate the OSC Handler...
        OSCHandler.Instance.Init();
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/trigger", 1);
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/playseq", 1);
        //get the rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //Debug.Log(rb.position.x);

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        rb.AddForce(movement * speed);
    }
}
