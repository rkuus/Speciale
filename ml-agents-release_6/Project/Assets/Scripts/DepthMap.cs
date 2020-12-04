using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthMap : MonoBehaviour
{
    public rayCaster rayCaster;
    public GameObject ground;

    public float circleRadius = 1.5f;
    public float lesserCircleRadius = 0.15f;

    public float[] totalOutput;
    // Start is called before the first frame update
    void Start()
    {
        rayCaster.sphereRadius = lesserCircleRadius;
        GameObject rayCastObj = rayCaster.gameObject;
        float sqredRadius = circleRadius * circleRadius;
        for (float x = (lesserCircleRadius - circleRadius); x < circleRadius;x += (lesserCircleRadius * 1.5f))
        {
            for (float y = (lesserCircleRadius - circleRadius); y < circleRadius; y += (lesserCircleRadius * 1.5f))
            {
                if (Vector2.SqrMagnitude(new Vector2(x,y)) < sqredRadius)
                {
                    GameObject newCaster = Instantiate(rayCastObj, new Vector3(x, 2.01f, y) + ground.transform.position, Quaternion.identity);
                    newCaster.transform.SetParent(this.transform);
                }
            }
        }
        totalOutput = new float[transform.childCount];
        Debug.Log(totalOutput.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float[] getRayCasts()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            rayCaster caster = transform.GetChild(i).GetComponent<rayCaster>();
            totalOutput[i] = caster.castRay();
        }

        return totalOutput;
    }
}
