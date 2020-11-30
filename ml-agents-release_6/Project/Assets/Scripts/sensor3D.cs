using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sensor3D : MonoBehaviour
{
    public bool showRays = false;
    public float sphereRadius = 0.15f;
    public float rayDistance = 4.0f;
    //private float halfRayDist = 2.5f;
    public float rays = 10;

    // Start is called before the first frame update
    void Start()
    {
        //halfRayDist = 0.5f * rayDistance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public float[] getSensorData()
    {
        // transform.right , -transform.up , transform.forward
        RaycastHit hit;
        Vector3 directionI, directionJ = transform.forward, moveTo = transform.right;
        float rayInc = 1.571f / rays;
        List<float> output = new List<float>();
        for (int j = 0;j < rays - 1; j++)
        {
            directionI = directionJ;
            for (int i = 0; i < rays + 1; i++)
            {
                Ray ray = new Ray(transform.position, directionI);

                if (Physics.SphereCast(ray, sphereRadius, out hit, rayDistance))
                {
                    output.Add((hit.distance + sphereRadius) / rayDistance);
                    //Debug.Log((hit.distance + sphereRadius - halfRayDist) / halfRayDist);
                    if (showRays && hit.collider.ToString() != "Ground (UnityEngine.BoxCollider)") // 
                        Debug.DrawRay(transform.position, directionI * (hit.distance + sphereRadius), Color.red);
                }
                    
                else
                {
                    output.Add(0.0f);
                    if (showRays)
                        Debug.DrawRay(transform.position, directionI * rayDistance, Color.green);
                }

                if (directionI == moveTo)
                    break;
                directionI = Vector3.MoveTowards(directionI, moveTo, rayInc).normalized;
            }
            moveTo = Vector3.MoveTowards(moveTo, -transform.up, rayInc).normalized;
            directionJ = Vector3.MoveTowards(directionJ, -transform.up, rayInc).normalized;
        }
        
        return output.ToArray();
    }
}
