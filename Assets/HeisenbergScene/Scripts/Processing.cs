﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;

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
    
    // public Text debugText;
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
    private static Vector3 prev = new Vector3(-1,-1,-1);
    private static EventLog.Type EventType = EventLog.Type.Position;
    private static Task ActualTask;

    void OnEnable()
    {
        Index = 0;

        Config.init();
        Debug.Log("UserID: " + Config.UserId);
        Session.Initalize();

        LaserPointer = GetComponent<SteamVR_LaserPointer>();

        Rounds = 0;

        LTC = new LatinSquare(8);
        LTT = new LatinSquare(Config.TargetAmplitudes.Length * Config.TargetWidths.Length);

        Tasks = CreateTasks(LTC, LTT);

        UpdateTask();
        HideTimer();

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
            if(!State.Equals(State.SHOW_TASK))
            {
                ProcessHits();
            }
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
        if(prev.z != -1)
        {
            RectTransform rt = (RectTransform) targetSphere.transform;
            circle.SetDistance(Vector3.Distance(prev, targetSphere.transform.position));
            circle.SetWidth();
            prev = new Vector3(-1, -1, -1);
        }
        targetSphere.transform.localScale = new Vector3(dimension, dimension, 1);
        progressIndicator.transform.localPosition = pos;
        progressIndicator.transform.localScale = new Vector3(dimension * 4, dimension * 4, 1);
        circle.GetTarget().SetWorldPosition(targetSphere.transform.position);
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
                // Calculate
                HandleNewRound();
                break;

            case State.TERMINATED:
                State = State.SAVED;
                ShowCommand("Finished");
                Session.Save(Tasks);
                break;

            default:
                break;

        }
    }

    void HandleNewRound()
    {
        if (Tasks[Index].GetCircle().HasNewRound())
        {
            prev = targetSphere.transform.position;
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
                Session.SaveTaskToFile(Tasks[Index]);
                if (Index == Tasks.Count - 1)
                {

                    State = State.TERMINATED;
                }
                else
                {
                    Index++;
                    UpdateTask();
                    State = State.SHOW_TASK;
                    Debug.Log(Tasks[Index].GetArmPosition());
                    Debug.Log(Tasks[Index].GetBodyPosition());
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
                    Debug.Log(Tasks[Index].GetArmPosition());
                    Debug.Log(Tasks[Index].GetBodyPosition());
                    break;

                default:
                    break;
            }
        } else if(Input.GetKeyDown(KeyCode.Escape))
        {
            State = State.TERMINATED;
        } else if(Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Tunnel.UpdateStatic();
        }
        else if(Input.GetKeyDown(KeyCode.Keypad0)) { Reset(0); }
        else if (Input.GetKeyDown(KeyCode.Keypad1)) { Reset(1); }
        else if (Input.GetKeyDown(KeyCode.Keypad2)) { Reset(2); }
        else if (Input.GetKeyDown(KeyCode.Keypad3)) { Reset(3); }
        else if (Input.GetKeyDown(KeyCode.Keypad4)) { Reset(4); }
        else if (Input.GetKeyDown(KeyCode.Keypad5)) { Reset(5); }
        else if (Input.GetKeyDown(KeyCode.Keypad6)) { Reset(6); }
        else if (Input.GetKeyDown(KeyCode.Keypad7)) { Reset(7); }
    }

    private static void Reset(int Round) 
    {
        if (Round <= 7) {
            for (int i = Round; i < Tasks.Count; i++) {
                Tasks[i].Reset();
            }
            Index = Round;
            UpdateTask();
            State = State.SHOW_TASK;
            Debug.Log(Tasks[Index].GetArmPosition());
            Debug.Log(Tasks[Index].GetBodyPosition());
        }
    }

    public static void addEvent(PointerEvent e)
    {
        if (stack.Count > 0)
        {
            stack[stack.Count - 1].SetEvent(PointerEvent.Released);
        }
    }

    public static List<Task> CreateTasks(LatinSquare LTC, LatinSquare LTT)
    {
        /**string file = Path.Combine(Application.streamingAssetsPath, "latinsquares.txt");
        for (int r = 0; r <= 12; r++) {*/
            int[] column = LTC.GetColumn(Config.UserId % 8);
            int start = (Config.UserId * LTC.GetSize()) % LTT.GetSize();
            //int[] column = LTC.GetColumn(r % 8);
            //int start = (r * LTC.GetSize()) % LTT.GetSize();
            List<Task> Tasks = new List<Task>();
            for (int i = 0; i < column.Length; i++) {
                // Get states from LatinSquare for Tasks
                int state = column[i];
                BodyPosition Body = state <= 4 ? BodyPosition.SITTING : BodyPosition.STANDING;
                ArmPosition Arm = new List<int>() { 1, 2, 5, 6 }.IndexOf(state) != -1 ? ArmPosition.STRECHED : ArmPosition.APPLIED;
                DOF dof = state % 2 == 0 ? DOF.THREE : DOF.SIX;

                Task tmp = new Task(i, Body, Arm, dof);
                tmp.CreateCircles(LTT, (i * column.Length) % LTT.GetSize());
                Debug.Log(tmp.PrintCommand(i));
               /**if (!File.Exists(file)) {
                    using (StreamWriter w = File.CreateText(file)) {
                        w.WriteLine("UserId: " + r + " | TaskNo. " + i + " | Command: " + tmp.PrintCommand(i));
                    }
                } else {
                    using (StreamWriter w = File.AppendText(file)) {
                        w.WriteLine("UserId: " + r + " | TaskNo. " + i + " | Command: " + tmp.PrintCommand(i));
                    }
                }*/
                
                Tasks.Add(tmp);

                start = start < LTC.GetSize() ? start + 1 : 0;
            //}
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

        if(Hits.Length > 0)
        {
            EventLog now = new EventLog(
                PressValues[0],
                Ballistic,
                NewController.transform.position,
                NewController.transform.rotation.eulerAngles,
                Hits[0].point,
                State
            );

            now.SetType(EventType);

            EventType = ActualTask.GetCircle().GetTarget().AddEvent(now);

            if (Contains(Hits, "Sphere", out Obj, out Hit))
            {
                HandleSphereHit();
            }
            else if (Contains(Hits, "TargetPanel", out Obj, out Hit))
            {
                HandlePanelHit();
            }
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
