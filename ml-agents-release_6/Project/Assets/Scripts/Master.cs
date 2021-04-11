using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour
{
    public float sphereRadius = 1.2f;
    public float sphereResolution = 0.2f;
    public float noiseOffset = 0.1f;
    public float cylinderRadius = 0.4f;
    // Start is called before the first frame update
    private float totalValue = 0;
    struct spherePoint
    {
        public Vector3 position;
        public float probability;
        public spherePoint(Vector3 v3, float p)
        {
            this.position = v3;
            this.probability = p;
        }
    }

    private spherePoint[] pointInSphere;

    void Start()
    {
        createSphere();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    showSphere();
        //}
    }

    void createSphere()
    {
        List<spherePoint> listOfPoints = new List<spherePoint>();

        for (float y = -0.1f + noiseOffset; y < sphereRadius; y += sphereResolution)
        {
            for (float z = cylinderRadius + noiseOffset; z < sphereRadius; z += sphereResolution)
            {
                Vector3 v3 = new Vector3(0, y, z);

                if (v3.magnitude < (sphereRadius - noiseOffset))
                {
                    v3 += new Vector3(0, 0.1f, 0);
                    listOfPoints.Add(new spherePoint(v3, 1024));
                }
            }
        }

        pointInSphere = listOfPoints.ToArray();
        totalValue = pointInSphere.Length * 1024;
    }

    void showSphere()
    {
        for (int i = 0; i < pointInSphere.Length; i++)
        {
            //Debug.DrawLine(pointInSphere[i].position, pointInSphere[i].position + new Vector3(0, 0.05f, 0), Color.red, 10f);
            for (int j = 0; j < 360;j+= 5)
            {
                Vector3 point = Quaternion.Euler(0, j, 0) * pointInSphere[i].position;
                Debug.DrawLine(point, point + new Vector3(0, 0.01f, 0), Color.red, 10f);
            }
        }
            
    }

    public int pickPoint()
    {
        float value = Random.Range(0, totalValue);
        float counter = 0;
        int index = pointInSphere.Length - 1;

        for (int i = 0; i < pointInSphere.Length; i++)
        {
            counter += pointInSphere[i].probability;

            if (counter > value)
            {
                index = i;
                break;
            }
                
        }

        return index;
    }

    public void pointPicked()
    {
        //for (int i = 0; i < pointInSphere.Length; i++)
        //{
        //    if (pointInSphere[i].probability < 100)
        //    {
        //        pointInSphere[i].probability++;
        //        totalValue++;
        //    }
        //}
        //Debug.Log(totalValue);
    }

    public void winnerPoint(int oldIndex)
    {
        //if (oldIndex != -1)
        //{
        //    totalValue -= pointInSphere[oldIndex].probability - (pointInSphere[oldIndex].probability / 2f);
        //    pointInSphere[oldIndex].probability = pointInSphere[oldIndex].probability / 2f;
        //}

    }
    public Vector3 getPoint(int index)
    {
        Vector3 point = pointInSphere[index].position + new Vector3(Random.Range(-1f * noiseOffset, noiseOffset), 0, Random.Range(-1f * noiseOffset, noiseOffset));
        point = Quaternion.Euler(0, Random.Range(0f, 360f), 0) * point;
        return point;
    }

}
