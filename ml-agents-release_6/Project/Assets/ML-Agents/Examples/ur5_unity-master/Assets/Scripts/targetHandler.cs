using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetHandler : MonoBehaviour
{
    public float innerDiameter = 5.0f;
    public float outerDiameter = 8.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateTargetPos()
    {
        do
        {
            transform.position = Random.onUnitSphere * (Random.value * (outerDiameter-innerDiameter) + innerDiameter);
            transform.position += new Vector3(0,1,0);
        } while (transform.position.y < 1);
    }
}
