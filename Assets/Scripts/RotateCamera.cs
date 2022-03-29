using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Get horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");

        // Rotate the focal point(this object)
        transform.Rotate(Vector3.up * -horizontalInput * rotationSpeed * Time.deltaTime);
    }
}
