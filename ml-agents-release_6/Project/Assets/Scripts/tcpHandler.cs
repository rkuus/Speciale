using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class tcpHandler : MonoBehaviour
{
    public Vector3 TCPpos = new Vector3(0.0f, 1.484f, 0.291f);
    public Vector3 TCPforward;
    public Vector3 eulerAngles;
    public GameObject ground;
    //public quaternion TCPRotation;

    private ArticulationBody articulation;
    // Start is called before the first frame update
    void Start()
    {
        articulation = GetComponent<ArticulationBody>();      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateParams()
    {
        eulerAngles = transform.rotation.eulerAngles / 360f;
        TCPpos = articulation.worldCenterOfMass - ground.transform.position;
        TCPforward = transform.forward;
    }
}
