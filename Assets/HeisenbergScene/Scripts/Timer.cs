using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer{

    private System.DateTime epch;
    private long StartTime;
    private bool finished;

	public Timer()
    {
        this.epch = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        this.StartTime = (long)(System.DateTime.UtcNow - this.epch).TotalMilliseconds;
        this.finished = false;
    }

    public bool Finished()
    {
        return this.finished;
    }

    public float GetProgress(int timespan)
    {
        long now = (long)(System.DateTime.UtcNow - this.epch).TotalMilliseconds;
        float result = ((float)(now - this.StartTime)) / timespan;
        this.finished = result >= 1.0;

        return result;
    }


}
