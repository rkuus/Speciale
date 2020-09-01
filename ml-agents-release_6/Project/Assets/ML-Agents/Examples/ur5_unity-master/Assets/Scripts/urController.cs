using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class urController : MonoBehaviour
{
    public GameObject[] urJoints;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float translation = Input.GetAxis("Vertical");
        for (int jointIndex = 0; jointIndex < urJoints.Length;jointIndex++)
        {
            jointController joint = urJoints[jointIndex].GetComponent<jointController>();
            joint.jointRotation = translation;
        }
    }


}
