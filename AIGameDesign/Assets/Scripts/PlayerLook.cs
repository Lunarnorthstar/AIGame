using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
     private Quaternion cameralook;
        public float looksensitivity = 2;
    
        private float cameraX, cameraY;
    
        // Update is called once per frame
        void Update()
        {
            cameraX += Input.GetAxis("Mouse X"); //Apply mouse movments to a variable.
            cameraY += Input.GetAxis("Mouse Y"); //Apply mouse movments to a variable.
    
            cameralook = Quaternion.Euler(cameraY * -looksensitivity, cameraX * looksensitivity, 0); //Apply said variables to the camera rotation variable.
    
            transform.rotation = cameralook; //Applies the rotation to the camera.
        }
}
