using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThroughputController : MonoBehaviour {

    // Track user ID
    private int userID;

    // Track current condition (posture, arm, DoF)
    private int currentCondition;

    // Width of the current condition
    private float width;

    // distance of the current condition
    private float distance;

    // List to hold all deviations from intended target (for the current condition and width/distance combination)
    private ArrayList deviations;

    // list of movement times for current D-W combination
    private ArrayList movementTimes;

    // List of all actual movement vectors for later effective distance calculation
    private ArrayList actualDistances;

	// Use this for initialization
	void Start () {
        deviations = new ArrayList();
        actualDistances = new ArrayList();
        movementTimes = new ArrayList();

        // Initialize Logger here!
        // Write header line to logger: "UserID; Condition; Width; Distance; MeanMT; ThroughputRegular; EffectiveWidth; EffectiveDistance; EffectiveThroughput"
	}
	
	// Update is called once per frame
	void Update () {
		

	}

    // Call this upon each new condition
    public void SetCondition(int condition)
    {
        currentCondition = condition;
    }

    // call this for each new W-D combination
    public void SetTargetParameter(float width, float distance)
    {

        if (deviations.Count > 0)
        {
            CalculateTroughput();
        }


        this.width = width;
        this.distance = distance;

    }

    // Call this after each ballistic selection. Remember that the start of a ballistic selection is the end of the last non-ballistic selection :)
    public void AddSelection(float targetX, float targetY, float startX, float startY, float endX, float endY, long movementTime)
    {
        // log movement time
        // optional: convert to seconds
        movementTimes.Add(movementTime);

        // calculate actual movement
        Vector2 actualMovement = new Vector2(endX - startX, endY - startY);

        // calculate actual distance
        actualDistances.Add(actualMovement.magnitude);
        // intended vector
        Vector2 intendedVector = new Vector2(targetX - startX, targetY - startY);

        // calculate deviation
        Vector2 projectedActualMovement = Vector3.Project(actualMovement, intendedVector);
        float deviation = projectedActualMovement.magnitude - intendedVector.magnitude;
        deviations.Add(deviation);
    }

    private void CalculateTroughput()
    {
        // calculate index of difficulty
        float ID = Mathf.Log(distance / width + 1) / Mathf.Log(2);

        // calculate mean movement time
        float sum = 0;
        foreach(float mt in movementTimes)
        {
            sum += mt;
        }
        float meanMT = sum / movementTimes.Count;

        // calculate regularTP
        float tpRegular = ID / meanMT;

        // calculate effective width
        float sumOfDeviations = 0;
        foreach(float deviation in deviations)
        {
            sumOfDeviations += deviation;
        }
        float effectiveWidth = 4.133f * (sumOfDeviations / Mathf.Sqrt(deviations.Count - 1));

        // calculate effective distance
        float sumOfDistances = 0;
        foreach(float distance in actualDistances)
        {
            sumOfDistances += distance;
        }
        float effectiveDistance = sumOfDistances / actualDistances.Count;

        // calculate efective ID
        float IDEffective = Mathf.Log(effectiveDistance / effectiveWidth + 1) / Mathf.Log(2);

        // calculate effective throughput
        float tpEffective = IDEffective / meanMT;

        // Log data
        string logData = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}", userID, currentCondition,width,distance,meanMT,tpRegular,effectiveWidth,effectiveDistance,tpEffective);
        // log.write(logdata);
    }
}
