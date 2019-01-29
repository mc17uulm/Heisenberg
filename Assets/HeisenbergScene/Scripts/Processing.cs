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
    START_IN,
    FIRST,
    FIRST_IN,
    FIRST_CLICK,
    SECOND,
    SECOND_IN,
    ACTIVATED,
    CLICKED,
    FINISHED
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

    private Timer timer;

    private State state;
    private static List<Position> stack;
    private List<Vector3> TargetPositions;
    private List<GameObject> missedPositions;
    public static System.Random rand = new System.Random();
    private int Index;
    private int Tries;
    private int h;
    private Session session;
    private Try t;

    // Latin square steps 
    private int[] stage;

    private float triggerPress;
    private float triggerPressBefore;

    private int Rounds;
    private int Column;
    private LatinSquare ParticipantSquare;
    private LatinSquare TargetSquare;

    private List<Target> Targets;

    void OnEnable()
    {
        Index = 0;

        Config.init();

        session = new Session();

        Rounds = 0;

        ParticipantSquare = new LatinSquare(Config.Pad ? 16 : 8);
        TargetSquare = new LatinSquare(Config.TargetAmplitudes.Length * Config.TargetWidths.Length);

        Column = Config.UserId % (Config.Pad ? 16 : 8);
        Targets = CreateTargets();

        laserPointer = GetComponent<SteamVR_LaserPointer>();

        if(!Config.Debug)
        {
            debugText.enabled = false;
        }
       
        Reset();

        LoadPositions();

        SetTargets(true);
        Tries = 1;
        h = 1;

        t = new Try(Tries, SwitchState(Tries));
        DebugLog();

        triggerPress = 0;
        triggerPressBefore = 0;
    }

    // Update is called once per frame
    void Update()
    {

        ProcessHits();

        ProcessButtons();

        triggerPressBefore = triggerPress;
        triggerPress = Tunnel.GetPressedValue();

        if (triggerPressBefore >= 1 && triggerPress < 1)
        {
            switch (state)
            {

                case State.START_IN:
                    state = State.FIRST;
                    targetSphere.transform.localPosition = TargetPositions[0];
                    progressIndicator.transform.localPosition = TargetPositions[0];
                    commandText.enabled = false;
                    break;

                case State.FIRST_IN:
                    state = State.FIRST_CLICK;
                    t.AddHit(new Hit(h, targetSphere.transform.position, stack, true));
                    h++;
                    stack = new List<Position>();

                    targetSphere.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(0.02197933f, 0, 1));
                    state = State.SECOND;
                    break;

                case State.ACTIVATED:
                    state = State.CLICKED;
                    timer = null;
                    progressIndicator.enabled = false;
                    t.AddHit(new Hit(h, targetSphere.transform.position, stack));
                    h++;
                    if (Index >= TargetPositions.Count - 1)
                    {
                        state = State.FINISHED;
                        session.AddTry(t);
                        Tries++;

                        if (Tries > Config.Tries)
                        {
                            state = State.FINISHED;
                            targetSphere.SetActive(false);
                            int ind = 0;
                            int tes = 0;
                            int fir = 0;
                            foreach (Try tr in session.GetTries())
                            {
                                foreach (Hit h in tr.GetHits())
                                {

                                    // Erster hit auf target um position von target zu ermitteln
                                    if (h.GetFirst() && tr.GetIndex() == 1 && ind == 0)
                                    {
                                        GameObject target = Instantiate(targetSphere, h.GetTarget(), Quaternion.identity) as GameObject;
                                        target.transform.parent = canvas.transform;
                                        target.transform.localScale = new Vector3(Config.Dimension, Config.Dimension, 1);
                                        target.SetActive(true);
                                        missedPositions.Add(target);
                                        fir++;
                                    }
                                    GameObject clicked;
                                    if (h.GetFirst())
                                    {
                                        clicked = Instantiate(indicatorButton, h.GetLastPosition(), Quaternion.identity) as GameObject;
                                        clicked.transform.parent = canvas.transform;
                                        clicked.transform.localScale = new Vector3(1, 1, 1);
                                        clicked.GetComponent<Image>().color = new Color(0.03671634f, 1, 0, 1);
                                    }
                                    else
                                    {
                                        clicked = Instantiate(indicatorButton, h.GetAverage(), Quaternion.identity) as GameObject;
                                        clicked.transform.parent = canvas.transform;
                                        clicked.transform.localScale = new Vector3(1, 1, 1);
                                        clicked.GetComponent<Image>().color = new Color(1, 0, 0, 1);
                                    }
                                    clicked.SetActive(true);
                                    tes++;
                                    missedPositions.Add(clicked);
                                }
                            }
                            ind++;

                            session.Save(targetSphere.transform);
                        }
                        else
                        {
                            commandText.enabled = true;

                            LoadPositions();

                            Index = 0;
                            h = 1;

                            SetTargets(true);

                            t = new Try(Tries, SwitchState(Tries));
                            DebugLog();

                            state = State.START;
                        }
                    }
                    else
                    {
                        Index++;
                        SetTargets();
                        state = State.FIRST;
                    }
                    stack = new List<Position>();
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

    private void ProcessHits()
    {
        if (!state.Equals(State.FINISHED))
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(laserPointer.transform.position, transform.forward, 100.0f);

            GameObject obj;
            RaycastHit hit;

            if (Contains(hits, "Sphere", out obj, out hit))
            {
                
                if (hits.Length > 0 && !state.Equals(State.START) && !state.Equals(State.START_IN))
                {
                    stack.Add(
                        new Position(
                            GetNow(),
                            GetEvent(),
                            triggerPress,
                            controller.transform.position,
                            controller.transform.rotation.eulerAngles,
                            targetSphere.transform.position,
                            targetPositions[Index].GetId(),
                            hits[0].point
                        )
                    );
                }

                switch (state)
                {
                    case State.START:
                        state = State.START_IN;
                        break;

                    case State.FIRST:
                        state = State.FIRST_IN;
                        break;

                    case State.SECOND:
                        timer = new Timer();
                        progressIndicator.enabled = true;
                        progressIndicator.fillAmount = 0;
                        progressIndicator.color = new Color(255, 0, 0);
                        state = State.SECOND_IN;
                        break;

                    case State.SECOND_IN:
                        if (timer.Finished())
                        {
                            state = State.ACTIVATED;
                            progressIndicator.color = new Color(0, 255, 0);
                        }
                        else
                        {
                            float progress = timer.GetProgress(Config.Timespan);
                            progressIndicator.fillAmount = progress;
                        }
                        break;

                    default:
                        break;
                }

            }
            else if (Contains(hits, "TargetPanel", out obj, out hit))
            {
                if (state.Equals(State.ACTIVATED) || state.Equals(State.FIRST_IN))
                {
                    stack.Add(
                        new Position(
                            GetNow(), 
                            GetEvent(), 
                            triggerPress, 
                            controller.transform.position, 
                            controller.transform.rotation.eulerAngles, 
                            targetSphere.transform.position, 
                            targetPositions[Index].GetId(), 
                            hits[0].point
                        )
                    );
                }
                else if(state.Equals(State.FIRST))
                {
                    state = State.FIRST_IN;
                }
                else
                {
                    progressIndicator.enabled = false;
                    timer = null;
                    if(state.Equals(State.SECOND_IN))
                    {
                        state = State.SECOND;
                    }
                }
            }
            else
            {
                if(state.Equals(State.FIRST_IN))
                {
                    state = State.FIRST;
                }
            }
        }
    }

    private void SetTargets(bool first = false)
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

    private void Reset()
    {
        timer = null;
        state = State.START;
        progressIndicator.enabled = false;
        stack = new List<Position>();
        missedPositions = new List<GameObject>();
    }

    private void LoadPositions()
    {

        targetPositions = new List<Target>();
        List<Vector3> p;
        switch (Config.grid)
        {

            case GridMode.GRID:
                p = Config.Positions.ToList<Vector3>();
                break;

            case GridMode.CIRCLE:
                p = Config.Circle.ToList<Vector3>();
                break;

            default:
                p = Config.Positions.ToList<Vector3>();
                break;

        }

        List<Target> targets = new List<Target>();
        int index = 0;
        foreach(Vector3 vec in p)
        {
            targets.Add(new Target(vec, index));
            index++;
        }

        if (Config.Random)
        {
            Target first = targets[0];
            targets.RemoveAt(0);
            targets = targets.OrderBy(x => rand.Next()).ToList();
            targets.Insert(0, first);
        }+
        targetPositions = targets;

        Vector3 panel = canvas.transform.localPosition;
        panel.z = Config.Distance;
        canvas.transform.localPosition = panel;
        targetSphere.transform.localScale = new Vector3(Config.Dimension, Config.Dimension, 1);
        float dimension = Config.Dimension * 4;
        progressIndicator.transform.localScale = new Vector3(dimension, dimension, 1);
    }

    private void ProcessButtons()
    {

        if (Input.GetKeyDown(KeyCode.Space) && state.Equals(State.FINISHED))
        {
            Reset();
            Index = 0;
            t = new Try(Tries, SwitchState(Tries));
            stack = new List<Position>();
            state = State.START;
            targetSphere.SetActive(true);
            SetTargets(true);
        }

    }

    private long GetNow()
    {
        return (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
    }

    private PointerEvent GetEvent()
    {
        if(stack.Count <= 0)
        {
            return PointerEvent.None;
        }
        if (triggerPress > 0.1f && triggerPress < 1.0f)
        {
            switch(stack[stack.Count-1].GetEvent())
            {
                case PointerEvent.TriggerPressed:
                    return PointerEvent.TriggerPressed;
                case PointerEvent.TriggerPressedFirst:
                    return PointerEvent.TriggerPressed;
                default: return PointerEvent.TriggerPressedFirst;
            }
        }
        else if (triggerPress >= 1.0f)
        {
            switch(stack[stack.Count-1].GetEvent())
            {
                case PointerEvent.Clicked:
                    return PointerEvent.Clicked;
                case PointerEvent.ClickedFirst:
                    return PointerEvent.Clicked;
                default:
                    return PointerEvent.ClickedFirst;
            }
        }
        else
        {
            return PointerEvent.None;
        }
    }

    public List<Target> CreateTargets()
    {
        List<Target> targets = new List<Target>();
        foreach(int amp in Config.TargetAmplitudes)
        {
            foreach(int size in Config.TargetWidths)
            {
                List<Vector3> t = new List<Vector3>();
                for(int i = 0; i < 13; i++)
                {
                    t.Add(new Vector3(Convert.ToSingle((amp/2)*Math.Sin((i)*(i/360))), Convert.ToSingle((amp / 2) * Math.Sin((i) * (i/360))), 0));
                }
                targets.Add(new Target(t, amp, size ));
            }
        }

        return targets;
    }

    public bool[] SwitchState(int TriesNo)
    {
        int remainder = TriesNo % (Config.TargetAmplitudes.Length * Config.TargetWidths.Length);
        if (remainder == 0)
        {
            // Change PState
        } else
        {
            targetPositions = Targets[remainder - 1].GetCircle();
            // Change TState
        }
        

        Debug.Log("Switch state: " + state + " | " + stage[state-1]);
        bool[] a = Config.LatinSquare.GetStates(stage[state-1]);
        string o = a[0] ? "Position: sitzend\r\n" : "Position: stehend\r\n";
        o += a[1] ? "Arm: ausgestreckt\r\n" : "Arm: angelegt\r\n";

        if (a[2])
        {
            o += "Möglichkeiten: 6DOF\r\n";
            Tunnel.ChangeMode(TunnelState.SIX);
        }
        else
        {
            o += "Möglichkeiten: 3DOF\r\n";
            Tunnel.ChangeMode(TunnelState.THREE);
        }

        commandText.text = o;

        return a;
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
        o += "Id: " + Config.Id + "\r\nRound " + Tries + "/8" + log;

        debugText.text = o;
    }
}
