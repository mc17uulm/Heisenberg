using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Circle
{

    private int Id;
    private int Round;
    private int Amplitude;
    private int Size;
    private List<Target> Targets;

    public Circle(int Id, int Amplitude, int Size, List<Target> Targets)
    {
        this.Id = Id;
        this.Round = 0;
        this.Amplitude = Amplitude;
        this.Size = Size;
        this.Targets = Targets;
    }

    public int GetId()
    {
        return this.Id;
    }

    public Target GetTarget()
    {
        return this.Targets[this.Round];
    }

    public bool HasNewRound()
    {
        if(this.Round == this.Targets.Count - 1)
        {
            return false;
        }
        else
        {
            this.Round++;
            return true;
        }
    }

    public List<Target> GetTargets()
    {
        return this.Targets;
    }

    public int GetSize()
    {
        return this.Size;
    }

    public int GetAmplitude()
    {
        return this.Amplitude;
    }

    public string CalculateTroughput()
    {
        // calculate index of difficulty
        float ID = Mathf.Log((float) this.Amplitude / this.Size + 1) / Mathf.Log(2);

        List<float> movementTimes = this.Targets.Select((el) => el.CalculateMovementTime()).ToList();
        List<float> actualDistances = this.Targets.Select(el => el.CalculateDistance()).ToList();
        List<float> deviations = this.Targets.Select(el => el.ClaculateDeviation()).ToList();

        // calculate mean movement time
        float sum = 0;
        foreach(float mt in movementTimes)
        {
            sum += mt;
        }
        float meanMT = (float) sum / movementTimes.Count;

        // calculate regularTP
        float tpRegular = (float) ID / meanMT;

        // calculate effective width
        float sumOfDeviations = 0;
        foreach(float deviation in deviations)
        {
            sumOfDeviations += deviation;
        }
        float effectiveWidth = 4.133f * ((float) sumOfDeviations / Mathf.Sqrt(deviations.Count - 1));

        // calculate effective distance
        float sumOfDitances = 0;
        foreach(float distance in actualDistances)
        {
            sumOfDitances += distance;
        }
        float effectiveDistance = (float) sumOfDitances / actualDistances.Count;

        // calculate effective ID
        float IDEffective = Mathf.Log((float) effectiveDistance / effectiveWidth + 1) / Mathf.Log(2);

        // calculate effective troughput
        float tpEffective = (float) IDEffective / meanMT;

        return string.Format("{0};{1};{2};{3};{4};{5};{6};{7}",
            this.Amplitude,
            this.Size,
            meanMT,
            tpRegular,
            effectiveWidth,
            effectiveDistance,
            IDEffective,
            tpEffective
        );

    }

}
