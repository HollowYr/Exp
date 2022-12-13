using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallrun : MonoBehaviour
{
    [SerializeField] private float distanceToWall = .5f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.right);
        if (Physics.Raycast(ray, out RaycastHit hit, distanceToWall))
        {
            Debug.DrawLine(hit.transform.position, hit.transform.position + hit.normal, Color.green, Time.deltaTime);
        }

        ray = new Ray(transform.position, -transform.right);
        if (Physics.Raycast(ray, out hit, distanceToWall))
        {
            Debug.DrawLine(hit.transform.position, hit.transform.position + hit.normal, Color.green, Time.deltaTime);
        }
    }
}
