using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rayCaster : MonoBehaviour
{
    public float rayDistance = 2.0f;
    public float sphereRadius = 0.25f;
    public bool showRay = false;
    private RaycastHit hit;
    private Ray ray;
    // Start is called before the first frame update
    void Start()
    {
        ray = new Ray(transform.position, -transform.up);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float castRay()
    {
        float output = 0.0f;
        if (Physics.SphereCast(ray, sphereRadius, out hit, rayDistance - sphereRadius))
        {
            output = 1 - ((hit.distance + sphereRadius) / rayDistance);
            //Debug.Log((hit.distance + sphereRadius - halfRayDist) / halfRayDist);
            if (showRay) // 
                Debug.DrawRay(transform.position, -transform.up * (hit.distance + sphereRadius), Color.red);
        }

        else
        {
            //output.Add(0.0f);
            if (showRay)
                Debug.DrawRay(transform.position, -transform.up * (rayDistance + sphereRadius), Color.green);
        }
        return output;
    }
}
