using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fingerController : MonoBehaviour
{
    ArticulationBody articulation;
    public float jointTranslation = 0.0f;
    public float speed = 100.0f;
    public bool invert = false;
    // Start is called before the first frame update
    void Start()
    {
        articulation = GetComponent<ArticulationBody>();
    }

    // Update is called once per frame
    void Update()
    {
        float translation = Input.GetAxis("Vertical");

        if (invert)
            translation = -1 * translation;

        if (translation != 0.0f)
        {
            float translationChange = (float)translation * speed * Time.fixedDeltaTime;
            float translationGoal = CurrentPrimaryAxisTranslation() + translationChange;
            translateTo(translationGoal);
        }
    }
    public float CurrentPrimaryAxisTranslation()
    {
        float currentTranslation = articulation.jointPosition[0];
        return currentTranslation;
    }
    void translateTo(float primaryAxisTranslation)
    {
        var drive = articulation.yDrive;
        drive.target = primaryAxisTranslation;
        articulation.yDrive = drive;
    }

}
