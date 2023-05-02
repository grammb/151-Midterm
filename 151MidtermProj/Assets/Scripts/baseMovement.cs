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

    private int count;
    public Text countText;

    public float jumpForce = 15;
    public float gravityScale = 5;
    //************* Need to setup this server dictionary...
    Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog>();
    //*************


    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true; //allows unity to update when not in focus

        //************* Instantiate the OSC Handler...
        OSCHandler.Instance.Init();
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/trigger", 650);
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/playseq", 1);
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/pitching", 4);
        //get the rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);

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
            //OSCHandler.Instance.SendMessageToClient("pd", "/unity/trigger", 300);
        }

        //END OF FIXED UPDATE

        
    }


    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("-------- COLLISION!!! ----------");

        if (other.gameObject.CompareTag("Items"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            setCountText ();


            // change the tempo of the sequence based on how many obejcts we have picked up.
            if (count < 2)
            {
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/trigger", 500);
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/pitching", 8);
            }
            if (count < 4)
            {
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/trigger", 400);
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/pitching", 12);
            }
            else if (count < 6)
            {
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/trigger", 300);
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/pitching", 20);
            }
            else if (count < 8)
            {
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/trigger", 150);
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/pitching", 40);
            }
            else
            {
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/trigger", 110);
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/pitching", 70);

            }

        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            //Debug.Log("-------- HIT THE WALL ----------");
            // trigger noise burst whe hitting a wall.
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/colwall", 1);
        }

    }

    void setCountText()
    {
        countText.text = "Count: " + count.ToString();

        //************* Send the message to the client...
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/trigger", count);
        //*************
    }
}
