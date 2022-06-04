using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using UnityEngine.VR.WSA.WebCam;


/**
 * @class AgentPlayer
 */
public class AgentPlayer : Agent
{
    
    public float velocity = 8f;

    // Update is called once per frame
    public new void Update()
    {
        base.Update();
        // Mientras que no definas las propiedades en Bodi esto seguirá dando error.

        Velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        Velocity *= MaxSpeed;
        Vector3 translation = Velocity * Time.deltaTime;
        transform.Translate(translation, Space.World);


        transform.LookAt(transform.position + Velocity);
        Orientation = transform.rotation.eulerAngles.y;
    }

}
