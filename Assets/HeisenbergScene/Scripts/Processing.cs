﻿using UnityEngine;
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
    TERMINATED
}

[RequireComponent(typeof(SteamVR_LaserPointer))]
public class Processing : MonoBehaviour
{

    // indicates how long user is in target
    public Image progressIndicator;
    
    public Text debugText;
    public Text commandText;
    
    public GameObject targetSphere;
    public GameObject indicatorButton;
    public GameObject canvas;
    public GameObject controller;

    private SteamVR_LaserPointer laserPointer;

    private Timer Timer;

    private static State State;
    private static List<Position> stack;
    private static int Index;
    private Session session;

    private int Rounds;
    private LatinSquare LTC;
    private LatinSquare LTT;
    
    private static List<Task> Tasks;

    void OnEnable()
    {
        Index = 0;

        Config.init();

        session = new Session();

        Rounds = 0;

        LTC = new LatinSquare(8);
        LTT = new LatinSquare(Config.TargetAmplitudes.Length * Config.TargetWidths.Length);

        Tasks = CreateTasks(LTC, LTT);

        laserPointer = GetComponent<SteamVR_LaserPointer>();

        if(!Config.Debug)
        {
            debugText.enabled = false;
        }

        State = State.START;

        ShowCommand("Start");
    }

    public static void AddData(Position pos)
    {
        Tasks[Index].AddToStack(pos);
    }

    /**public void ShowCircle(int i)
    {
        Task task = Tasks[i];
        Circle circle = task.GetCircle();

    }*/

    public void ShowTask(int i)
    {
        Task task = Tasks[i];
        ShowCommand(task.PrintCommand(i) + "Zum Fortfahren clicken");
    }

    public void ShowTarget()
    {
        Vector3 pos = Tasks[Index].GetCircle().GetTarget().GetPosition();
        targetSphere.transform.localPosition = pos;
        progressIndicator.transform.localPosition = pos;
    }

    private static void UpdateTask()
    {
        Tunnel.UpdateTask(Tasks[Index]);
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
                ShowCommand("Finished");
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
        }
        else
        {
            if (Tasks[Index].HasNewRound())
            {
                State = State.SHOW_TARGET;
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
        UpdateTask();
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

    // Update is called once per frame
    void Update()
    {

        ProcessButtons();

        ExecuteState();

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

    public static void addEvent(PointerEvent e)
    {
        if (stack.Count > 0)
        {
            stack[stack.Count - 1].SetEvent(PointerEvent.Released);
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
            int state = column[i];
            BodyPosition Body = state <= 4 ? BodyPosition.SITTING : BodyPosition.STANDING;
            ArmPosition Arm = new List<int>() { 1, 2, 5, 6 }.IndexOf(state) != -1 ? ArmPosition.STRECHED : ArmPosition.APPLIED;
            DOF dof = state % 2 == 0 ? DOF.THREE : DOF.SIX;

            if (Config.Pad)
            {
                Task first = new Task((i * 2), Body, Arm, dof);
                Task second = new Task((i * 2) + 1, Body, Arm, dof, InputType.PAD);
                first.CreateCircles(LTT);
                second.CreateCircles(LTT);
                Tasks.Add(first);
                Tasks.Add(second);
            }
            else
            {
                Task tmp = new Task(i, Body, Arm, dof);
                tmp.CreateCircles(LTT);
                Tasks.Add(tmp);
            }
            start = start < LTC.GetSize() ? start + 1 : 0;
        }

        UpdateTask();

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
}
