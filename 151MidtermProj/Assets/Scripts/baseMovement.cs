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
    public Text countText;
    public float jumpForce = 15;
    public float gravityScale = 5;
    Vector3 oldPosition;
    //************* Need to setup this server dictionary...
    Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog>();
    //*************


    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true; //allows unity to update when not in focus

        //************* Instantiate the OSC Handler...
        OSCHandler.Instance.Init();
        //OSCHandler.Instance.SendMessageToClient("pd", "/unity/trigger", 150);
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/playseq", 1);
        //get the rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);

        //Debug.Log(rb.position.x);

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        rb.AddForce(movement * speed);
        //************* Routine for receiving the OSC...
        OSCHandler.Instance.UpdateLogs();
        Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog>();
        servers = OSCHandler.Instance.Servers;

        foreach (KeyValuePair<string, ServerLog> item in servers)
        {
            // If we have received at least one packet,
            // show the last received from the log in the Debug console
            if (item.Value.log.Count > 0)
            {
                int lastPacketIndex = item.Value.packets.Count - 1;

                //get address and data packet
                countText.text = item.Value.packets[lastPacketIndex].Address.ToString();
                countText.text += item.Value.packets[lastPacketIndex].Data[0].ToString();

            }
        }
        //*************


        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
            //WANT TO ADD PURE DATA SOUND STUFF HERE
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/trigger", 300);
        }

 

        //OSCHandler.Instance.SendMessageToClient("pd", "/unity/trigger", speed2);
    }
}
