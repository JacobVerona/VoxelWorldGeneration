using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float rotSpeed = 10f;
    public float speed = 1f;



    private void FixedUpdate ()
    {
        float x = Input.GetAxis("Horizontal") * speed;
        float z = Input.GetAxis("Vertical") * speed;
        float y = 0;
        Vector3 mov;
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            y = Input.GetAxis("Jump");
            mov = new Vector3(x, y, z);
        }
        else
        {
            y = Input.GetAxis("Jump");
            mov = new Vector3(x, -y, z);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.eulerAngles += new Vector3(0,10 ,0) * Time.deltaTime * rotSpeed;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.eulerAngles += new Vector3(0, -10, 0) * Time.deltaTime * rotSpeed;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.eulerAngles += new Vector3(10, 0, 0) * Time.deltaTime * rotSpeed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.eulerAngles += new Vector3(-10, 0, 0) * Time.deltaTime * rotSpeed;
        }


        transform.Translate(mov);
    }

}