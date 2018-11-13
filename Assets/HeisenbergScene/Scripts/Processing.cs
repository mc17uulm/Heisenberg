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

    // roter Ring um Ziel um Countdown anzuzeigen
    public Image progressIndicator;

    // Gibt Anzahl der Durchgänge an
    public Text scoreText;

    // Sphere welche als Ziel und als Referenzpunkt genutzt werden
    public GameObject targetSphere;
    // Zeigt Treffer an
    public GameObject indicatorButton;
    public GameObject canvas;

    private SteamVR_LaserPointer laserPointer;
    private SteamVR_TrackedController trackedController;

    private Timer timer;
    // Angabe der ms für welche der Nutzer im Ziel sein muss um klicken zu können
    private int timespan = 500;

    private State state;
    private List<Position> stack;
    private List<Target> targetPositions;
    private List<GameObject> missedPositions;
    public static System.Random rand = new System.Random();
    private int Index;
    private IDictionary<string, object> config;
    private int Tries;
    private int h;
    private Session session;
    private Try t;

    private SteamVR_Controller.Device device = null;
    private float triggerPress; 

    void OnEnable()
    {
        Index = 0;

        config = Config.Load();

        session = new Session();

        laserPointer = GetComponent<SteamVR_LaserPointer>();
        trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = GetComponentInParent<SteamVR_TrackedController>();
        }
        if (trackedController == null)
        {
            Debug.Log("TrackedContoroller still null");
            Application.Quit();
        }
        
        trackedController.TriggerUnclicked -= ControllerOnRelease;
        trackedController.TriggerUnclicked += ControllerOnRelease;
        trackedController.TriggerClicked -= ControllerOnClick;
        trackedController.TriggerClicked += ControllerOnClick;
       

        Reset();

        LoadPositions();

        SetTargets(true);
        Tries = 1;
        h = 1;

        t = new Try(Tries);

    }

    // Update is called once per frame
    void Update()
    {

        ProcessHits();

        ProcessButtons();

        if(device == null)
        {
            device = SteamVR_Controller.Input((int)trackedController.controllerIndex);
        }
        else
        {
            triggerPress = device.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger).x;
        }

    }

    private void ControllerOnRelease(object sender, ClickedEventArgs e)
    {
        switch (state)
        {

            case State.START_IN:
                state = State.FIRST;
                targetSphere.transform.localPosition = targetPositions[0].GetPosition();
                progressIndicator.transform.localPosition = targetPositions[0].GetPosition();
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
                if (Index >= targetPositions.Count - 1)
                {
                    state = State.FINISHED;
                    session.AddTry(t);
                    Tries++;

                    if (Tries > (int)config["tries"])
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
                                    target.transform.localScale = new Vector3((int)config["dimension"], (int)config["dimension"], 1);
                                    target.SetActive(true);
                                    missedPositions.Add(target);
                                    fir++;
                                }
                                Vector3 m = (bool)config["last_position"] ? h.GetLastPosition() : h.GetAverage();
                                GameObject clicked = Instantiate(indicatorButton, m, Quaternion.identity) as GameObject;
                                clicked.transform.parent = canvas.transform;
                                clicked.transform.localScale = new Vector3(1, 1, 1);
                                if (h.GetFirst())
                                {
                                    clicked.GetComponent<Image>().color = new Color(0.03671634f, 1, 0, 1);
                                }
                                else
                                {
                                    clicked.GetComponent<Image>().color = new Color(1, 0, 0, 1);
                                }
                                clicked.SetActive(true);
                                tes++;
                                missedPositions.Add(clicked);
                            }
                        }
                        ind++;

                        session.SaveToFile((string)config["savefile"]);
                        session.SaveSum((string)config["savefile_2"]);
                    }
                    else
                    {

                        LoadPositions();

                        Index = 0;
                        h = 1;

                        SetTargets();

                        t = new Try(Tries);

                        state = State.FIRST;
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

    private void ControllerOnClick(object sender, ClickedEventArgs e)
    {
        if(stack.Count > 0)
        {
            stack[stack.Count - 1].SetEvent(PointerEvent.ClickEvent);
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
                    stack.Add(new Position(GetNow(), GetEvent(), triggerPress, trackedController.transform.position, trackedController.transform.rotation.eulerAngles, targetPositions[Index], hits[0].point));
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
                            float progress = timer.GetProgress(timespan);
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
                    stack.Add(new Position(GetNow(), GetEvent(), triggerPress, trackedController.transform.position, trackedController.transform.rotation.eulerAngles, targetPositions[Index], hits[0].point));
                    //stack.Add(hits[0].point);
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
            targetSphere.transform.localPosition = (Vector3)config["start_position"];
            progressIndicator.transform.localPosition = (Vector3)config["start_position"];
            progressIndicator.enabled = false;
        }
        else
        {
            Debug.Log("Index: " + Index);
            //int o = t.GetHits().Count / (int)config["repeat"];
            targetSphere.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(1, 0, 0.8135681f));
            targetSphere.transform.localPosition = targetPositions[Index].GetPosition();
            progressIndicator.transform.localPosition = targetPositions[Index].GetPosition();
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
        List<Vector3> p = ((Vector3[])config["positions"]).ToList<Vector3>();

        List<Target> targets = new List<Target>();
        int index = 0;
        foreach(Vector3 vec in p)
        {
            targets.Add(new Target(vec, index));
            index++;
        }

        if ((bool)config["random"])
        {
            Target first = targets[0];
            targets.RemoveAt(0);
            targets = targets.OrderBy(x => rand.Next()).ToList();
            targets.Insert(0, first);
        }
        targetPositions = targets;

        Vector3 panel = canvas.transform.localPosition;
        panel.z = (int)config["distance"];
        canvas.transform.localPosition = panel;
        targetSphere.transform.localScale = new Vector3((int)config["dimension"], (int)config["dimension"], 1);
        float dimension = (int)config["dimension"] * 4;
        progressIndicator.transform.localScale = new Vector3(dimension, dimension, 1);
    }

    private void ProcessButtons()
    {

        if (Input.GetKeyDown(KeyCode.Space) && state.Equals(State.FINISHED))
        {
            Reset();
            Index = 0;
            t = new Try(Tries);
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
            //TODO: change for first event
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
}
