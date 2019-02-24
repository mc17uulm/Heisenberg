using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Valve.VR;

public enum State
{
    START,
    SHOW_TASK,
    SHOW_TARGET,
    WAIT_FOR_BALLISTIC,
    WAIT_FOR_IN_TARGET,
    ACITVATE_TIMER,
    OUT_OF_TARGET,
    WAIT_FOR_TIMER,
    FINISHED_TIMER,
    FINISHED_TARGET,
    TERMINATED,
    SAVED
}

[RequireComponent(typeof(SteamVR_LaserPointer))]
public class Processing : MonoBehaviour
{

    private static SteamVR_LaserPointer LaserPointer;
    public GameObject NewController;

    // indicates how long user is in target
    public Image progressIndicator;
    
    public Text debugText;
    public Text commandText;
    
    public GameObject targetSphere;
    public GameObject canvas;

    private Timer Timer;

    private static State State;
    private static List<Position> stack;
    private static int Index;
    private Session session;

    private int Rounds;
    private LatinSquare LTC;
    private LatinSquare LTT;
    
    private static List<Task> Tasks;
    private static bool Initalized = false;
    private static bool Ballistic = false;
    private static EventLog.Type EventType = EventLog.Type.Position;
    private static Task ActualTask;

    void OnEnable()
    {
        Index = 0;

        Config.init();
        Session.Initalize();

        LaserPointer = GetComponent<SteamVR_LaserPointer>();

        Rounds = 0;

        LTC = new LatinSquare(8);
        LTT = new LatinSquare(Config.TargetAmplitudes.Length * Config.TargetWidths.Length);

        Tasks = CreateTasks(LTC, LTT);

        UpdateTask();
        HideTimer();

        if(!Config.Debug)
        {
            debugText.enabled = false;
        }

        State = State.START;

        ShowCommand("Start");
        Tunnel.IsInitalized();
    }

    // Update is called once per frame
    void Update()
    {

        ProcessButtons();

        if(!State.Equals(State.SAVED) && !State.Equals(State.START))
        {
            ProcessHits();
            ExecuteState();
        }

    }

    public void ShowTask(int i)
    {
        Task task = Tasks[i];
        ShowCommand(task.PrintCommand(i) + "Zum Fortfahren clicken");
    }

    public void ShowTarget()
    {
        targetSphere.SetActive(true);
        Circle circle = Tasks[Index].GetCircle();
        Vector3 pos = circle.GetTarget().GetPosition();
        targetSphere.transform.localPosition = pos;
        int dimension = circle.GetSize();
        targetSphere.transform.localScale = new Vector3(dimension, dimension, 1);
        progressIndicator.transform.localPosition = pos;
        progressIndicator.transform.localScale = new Vector3(dimension * 4, dimension * 4, 1);
        Vector3 now = targetSphere.transform.localPosition;
    }

    public void HideTarget()
    {
        targetSphere.SetActive(false);
    }

    private static void UpdateTask()
    {
        Task s = ActualTask;
        if(Index > 0)
        {
            Tasks[Index - 1] = s;
        }
        ActualTask = Tasks[Index];
    }

    public static void SetState(State state)
    {
        State = state;
    }

    public static State GetState()
    {
        return State;
    }

    public void ExecuteState()
    {
        switch(State)
        {
            case State.SHOW_TASK:
                HideCommand();
                HideTarget();
                ShowTask(Index);
                break;

            case State.SHOW_TARGET:
                HideCommand();
                ShowTarget();
                State = State.WAIT_FOR_BALLISTIC;
                break;

            case State.ACITVATE_TIMER:
                ShowTimer();
                State = State.WAIT_FOR_TIMER;
                break;

            case State.WAIT_FOR_TIMER:
                UpdateTimer();
                break;

            case State.OUT_OF_TARGET:
                HideTimer();
                State = State.WAIT_FOR_IN_TARGET;
                break;

            case State.FINISHED_TARGET:
                HideTimer();
                HandleNewRound();
                break;

            case State.TERMINATED:
                State = State.SAVED;
                ShowCommand("Finished");
                Session.Save();
                break;

            default:
                break;

        }
    }

    void HandleNewRound()
    {
        if (Tasks[Index].GetCircle().HasNewRound())
        {
            State = State.SHOW_TARGET;
            Ballistic = true;
        }
        else
        {
            if (Tasks[Index].HasNewRound())
            {
                State = State.SHOW_TARGET;
                Ballistic = true;
            }
            else
            {
                if (Index == Tasks.Count - 1)
                {
                    State = State.TERMINATED;
                }
                else
                {
                    Index++;
                    UpdateTask();
                    State = State.SHOW_TASK;
                }
            }
        }
    }

    void HideTimer()
    {
        progressIndicator.enabled = false;
        Timer = null;
    }

    void UpdateTimer()
    {
        if (Timer.Finished())
        {
            State = State.FINISHED_TIMER;
            progressIndicator.color = new Color(0, 255, 0);
        }
        else
        {
            float progress = Timer.GetProgress(Config.Timespan);
            progressIndicator.fillAmount = progress;
        }
        //UpdateTask();
    }

    /**
     * Initalizes a new Timer to stop the 
     * 
     */
    void ShowTimer()
    {
        Timer = new Timer();
        progressIndicator.enabled = true;
        progressIndicator.fillAmount = 0;
        progressIndicator.color = new Color(255, 0, 0);
    }

    private void ProcessButtons()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            switch(State)
            {
                case State.START:
                    State = State.SHOW_TASK;
                    break;

                default:
                    break;
            }
        }
    }

    /**private void SetTargets(bool first = false)
    {
        if(first)
        {
            targetSphere.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(1, 0, 0.8135681f));
            targetSphere.transform.localPosition = Config.Start;
            progressIndicator.transform.localPosition = Config.Start;
            progressIndicator.enabled = false;
        }
        else
        {
            targetSphere.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(1, 0, 0.8135681f));
            targetSphere.transform.localPosition = TargetPositions[Index];
            progressIndicator.transform.localPosition = TargetPositions[Index];
        }
    }

    public void DebugLog(string log = "")
    {
        string o = "";
        if(Tries == 1)
        {
            o += "Start!\r\n";
        } else
        {
            o += "New Round!\r\n";
        }
        o += "Id: " + Config.UserId + "\r\nRound " + Tries + "/8" + log;

        debugText.text = o;
    }*/

    public static List<Task> CreateTasks(LatinSquare LTC, LatinSquare LTT)
    {
        int[] column = LTC.GetColumn(Config.UserId % 8);
        int start = (Config.UserId * LTC.GetSize()) % LTT.GetSize();
        List<Task> Tasks = new List<Task>();
        for (int i = 0; i < column.Length; i++)
        {
            // Get states from LatinSquare for Tasks
            int state = column[i];
            BodyPosition Body = state <= 4 ? BodyPosition.SITTING : BodyPosition.STANDING;
            ArmPosition Arm = new List<int>() { 1, 2, 5, 6 }.IndexOf(state) != -1 ? ArmPosition.STRECHED : ArmPosition.APPLIED;
            DOF dof = state % 2 == 0 ? DOF.THREE : DOF.SIX;

            // Is PAD option available?
            if (Config.Pad)
            {
                Task first = new Task((i * 2), Body, Arm, dof);
                Task second = new Task((i * 2) + 1, Body, Arm, dof, InputType.PAD);
                first.CreateCircles(LTT, (((i*2)+1)*2*column.Length)%LTT.GetSize());
                second.CreateCircles(LTT, (((i * 2) + 1) * 2 * column.Length) % LTT.GetSize());
                Tasks.Add(first);
                Tasks.Add(second);
            }
            else
            {
                Task tmp = new Task(i, Body, Arm, dof);
                tmp.CreateCircles(LTT, (i*column.Length)%LTT.GetSize());
                Tasks.Add(tmp);
            }
            start = start < LTC.GetSize() ? start + 1 : 0;
        }
        
        return Tasks;
    }

    public void ShowCommand(string cmd)
    {
        this.commandText.text = cmd;
        this.commandText.enabled = true;
    }

    public void HideCommand()
    {
        this.commandText.enabled = false;
    }

    public void ProcessHits()
    {
        float[] PressValues = Tunnel.GetPressValues();
        RaycastHit[] Hits;
        Hits = Physics.RaycastAll(LaserPointer.transform.position, transform.forward, 100.0f);

        GameObject Obj;
        RaycastHit Hit;

        if (Hits.Length > 0)
        {
            if (Contains(Hits, "Sphere", out Obj, out Hit))
            {
                HandleSphereHit();
            }
            else if (Contains(Hits, "TargetPanel", out Obj, out Hit))
            {
                HandlePanelHit();
            }

            EventLogger.AddHit(Ballistic, NewController.transform.position, NewController.transform.rotation.eulerAngles, Hits[0].point, ActualTask);
        }
    }

    public static void HandleClick()
    {
        switch (State)
        {
            case State.SHOW_TASK:
                State = State.SHOW_TARGET;
                Ballistic = true;
                break;
            case State.WAIT_FOR_BALLISTIC:
                Ballistic = false;
                // Static starts
                State = State.WAIT_FOR_IN_TARGET;
                break;
            case State.FINISHED_TIMER:
                // Static ends
                State = State.FINISHED_TARGET;
                break;

            default:
                break;
        }
    }

    private void HandleSphereHit()
    {
        switch (State)
        {
            case State.WAIT_FOR_IN_TARGET:
                State = State.ACITVATE_TIMER;
                break;

            default:
                break;
        }
    }

    private void HandlePanelHit()
    {
        switch (State)
        {
            case State.ACITVATE_TIMER:
            case State.WAIT_FOR_TIMER:
                State = State.OUT_OF_TARGET;
                break;

            default:
                break;
        }
    }

    private Boolean Contains(RaycastHit[] hits, string name, out GameObject x, out RaycastHit hit)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.name.Equals(name))
            {
                x = hits[i].collider.gameObject;
                hit = hits[i];
                return true;
            }
        }

        x = null;
        hit = new RaycastHit();
        return false;
    }

    public static void SetEventType(EventLog.Type Type)
    {
        EventType = Type;
    }

    public static Task GetActualTask()
    {
        return ActualTask;
    }
}
