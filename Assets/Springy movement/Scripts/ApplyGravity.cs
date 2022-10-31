using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyGravity : MonoBehaviour
{
    private Rigidbody body;
    public void SetRigidbody (Rigidbody rigidbody) => body = rigidbody; 
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
